Public Class frmLogin
    Inherits Form

    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly btnLogin As New Button()
    Private ReadOnly err As New ErrorProvider()
    Private ReadOnly tips As New ToolTip()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Login"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 420
        Me.Height = 250
        Me.MinimumSize = New Size(420, 250)

        Dim lblUsername As New Label() With {.Text = "Username", .Left = 25, .Top = 35, .AutoSize = True}
        txtUsername.Left = 120
        txtUsername.Top = 30
        txtUsername.Width = 250

        Dim lblPassword As New Label() With {.Text = "Password", .Left = 25, .Top = 75, .AutoSize = True}
        txtPassword.Left = 120
        txtPassword.Top = 70
        txtPassword.Width = 250
        txtPassword.UseSystemPasswordChar = True

        btnLogin.Text = "Login"
        btnLogin.Left = 120
        btnLogin.Top = 115
        btnLogin.Width = 100
        btnLogin.Height = 34
        btnLogin.TextAlign = ContentAlignment.MiddleCenter
        AddHandler btnLogin.Click, AddressOf btnLogin_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnLogin, True)

        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblPassword)
        Me.Controls.Add(txtPassword)
        Me.Controls.Add(btnLogin)

        AddHandler Me.Load, AddressOf frmLogin_Load
    End Sub

    Private Sub frmLogin_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()
        tips.SetToolTip(txtUsername, "Enter your username")
        tips.SetToolTip(txtPassword, "Enter your password")
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs)
        Try
            Dim username As String = txtUsername.Text.Trim()
            Dim password As String = txtPassword.Text

            err.Clear()

            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
                If String.IsNullOrWhiteSpace(username) Then
                    err.SetError(txtUsername, "Username is required")
                End If
                If String.IsNullOrWhiteSpace(password) Then
                    err.SetError(txtPassword, "Password is required")
                End If
                MessageBox.Show("Please enter username and password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Const sql As String = "SELECT id AS user_id, username, password_hash, role, is_active, force_password_change FROM users WHERE username = @username LIMIT 1;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@username", username}
            }
            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)

            If dt.Rows.Count = 0 Then
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim row As DataRow = dt.Rows(0)
            Dim isActive As Boolean = Convert.ToBoolean(If(row("is_active") Is DBNull.Value, True, row("is_active")))
            If Not isActive Then
                MessageBox.Show("This account is disabled. Please contact an administrator.",
                                "Account Disabled",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            End If

            Dim storedHash As String = row("password_hash").ToString()
            If Not PasswordHelper.VerifySha256Password(password, storedHash) Then
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            CurrentUser.UserId = Convert.ToInt32(row("user_id"))
            CurrentUser.Username = row("username").ToString()
            CurrentUser.Role = row("role").ToString()

            Dim forcePasswordChange As Boolean = Convert.ToBoolean(If(row("force_password_change") Is DBNull.Value, False, row("force_password_change")))
            If forcePasswordChange AndAlso String.Equals(CurrentUser.Role, "Customer", StringComparison.OrdinalIgnoreCase) Then
                Using frm As New frmChangePassword()
                    If frm.ShowDialog() <> DialogResult.OK Then
                        Return
                    End If
                End Using
            End If

            AuditLogger.LogAction(CurrentUser.UserId, "Login", $"User logged in as {CurrentUser.Role}")

            OpenDashboardByRole(CurrentUser.Role)
        Catch ex As Exception
            MessageBox.Show("Login failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub OpenDashboardByRole(role As String)
        Dim nextForm As Form

        Select Case role.Trim().ToLowerInvariant()
            Case "manager"
                nextForm = New frmReports()
            Case "staff"
                nextForm = New frmEnterReading()
            Case "customer"
                nextForm = New frmCustomerDashboard()
            Case Else
                MessageBox.Show("Unknown user role.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
        End Select

        nextForm.Show()
        Me.Hide()
    End Sub
End Class
