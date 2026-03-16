Public Class frmProfileSettings
    Inherits Form

    Private ReadOnly lblUsername As New Label()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly lblFullName As New Label()
    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly lblNewPassword As New Label()
    Private ReadOnly txtNewPassword As New TextBox()
    Private ReadOnly lblConfirmPassword As New Label()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "My Profile"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(520, 360)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.ControlBox = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        lblUsername.Text = "Username"
        lblUsername.Left = 28
        lblUsername.Top = 28
        lblUsername.AutoSize = True

        txtUsername.Left = 28
        txtUsername.Top = 48
        txtUsername.Width = 460

        lblFullName.Text = "Full Name"
        lblFullName.Left = 28
        lblFullName.Top = 88
        lblFullName.AutoSize = True

        txtFullName.Left = 28
        txtFullName.Top = 108
        txtFullName.Width = 460

        lblNewPassword.Text = "New Password (optional)"
        lblNewPassword.Left = 28
        lblNewPassword.Top = 148
        lblNewPassword.AutoSize = True

        txtNewPassword.Left = 28
        txtNewPassword.Top = 168
        txtNewPassword.Width = 460
        txtNewPassword.UseSystemPasswordChar = True

        lblConfirmPassword.Text = "Confirm New Password"
        lblConfirmPassword.Left = 28
        lblConfirmPassword.Top = 208
        lblConfirmPassword.AutoSize = True

        txtConfirmPassword.Left = 28
        txtConfirmPassword.Top = 228
        txtConfirmPassword.Width = 460
        txtConfirmPassword.UseSystemPasswordChar = True

        btnSave.Text = "Save"
        btnSave.Left = 308
        btnSave.Top = 286
        btnSave.Width = 88
        btnSave.Height = 36
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnSave.ForeColor = Color.White

        btnCancel.Text = "Cancel"
        btnCancel.Left = 400
        btnCancel.Top = 286
        btnCancel.Width = 88
        btnCancel.Height = 36
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnCancel.ForeColor = Color.White

        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblFullName)
        Me.Controls.Add(txtFullName)
        Me.Controls.Add(lblNewPassword)
        Me.Controls.Add(txtNewPassword)
        Me.Controls.Add(lblConfirmPassword)
        Me.Controls.Add(txtConfirmPassword)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)

        AddHandler Me.Load, AddressOf frmProfileSettings_Load
        AddHandler btnSave.Click, AddressOf btnSave_Click
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
    End Sub

    Private Sub frmProfileSettings_Load(sender As Object, e As EventArgs)
        Try
            Const sql As String = "SELECT username, full_name FROM users WHERE id = @user_id LIMIT 1;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@user_id", CurrentUser.UserId}
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
            If dt.Rows.Count = 0 Then
                txtUsername.Text = CurrentUser.Username
                txtFullName.Text = CurrentUser.FullName
                Return
            End If

            Dim row As DataRow = dt.Rows(0)
            txtUsername.Text = row("username").ToString()
            txtFullName.Text = If(row("full_name") Is DBNull.Value, String.Empty, row("full_name").ToString())
        Catch ex As Exception
            MessageBox.Show("Failed to load profile: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim username As String = txtUsername.Text.Trim()
        Dim fullName As String = txtFullName.Text.Trim()
        Dim newPassword As String = txtNewPassword.Text
        Dim confirmPassword As String = txtConfirmPassword.Text

        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(fullName) Then
            MessageBox.Show("Username and full name are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not String.IsNullOrWhiteSpace(newPassword) Then
            If newPassword.Length < 6 Then
                MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If newPassword <> confirmPassword Then
                MessageBox.Show("New password and confirmation do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        End If

        Try
            Const checkSql As String = "SELECT COUNT(*) FROM users WHERE username = @username AND id <> @user_id;"
            Dim existsCount As Integer = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkSql, New Dictionary(Of String, Object) From {
                {"@username", username},
                {"@user_id", CurrentUser.UserId}
            }))

            If existsCount > 0 Then
                MessageBox.Show("Username already exists. Please choose a different username.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim sql As String = "UPDATE users SET username = @username, full_name = @full_name"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@username", username},
                {"@full_name", fullName},
                {"@user_id", CurrentUser.UserId}
            }

            If Not String.IsNullOrWhiteSpace(newPassword) Then
                sql &= ", password_hash = @password_hash, force_password_change = 0"
                parameters.Add("@password_hash", PasswordHelper.ComputeSha256Hash(newPassword))
            End If

            sql &= " WHERE id = @user_id;"
            DatabaseHelper.ExecuteNonQuery(sql, parameters)

            CurrentUser.Username = username
            CurrentUser.FullName = fullName

            AuditLogger.LogAction(CurrentUser.UserId, "ProfileUpdated", $"Username={username}")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Failed to update profile: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
