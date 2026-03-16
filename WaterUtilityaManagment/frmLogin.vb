Imports System.ComponentModel
Imports System.Data
Imports System.Drawing.Drawing2D

Public Class frmLogin
    Inherits Form

    Private ReadOnly pnlCardShadow As New RoundedPanel()
    Private ReadOnly pnlLoginCard As New RoundedPanel()
    Private ReadOnly tblCardContent As New TableLayoutPanel()
    Private ReadOnly pnlHeader As New Panel()
    Private ReadOnly lblIcon As New Label()
    Private ReadOnly lblTitle As New Label()
    Private ReadOnly lblSubtitle As New Label()
    Private ReadOnly pnlUsernameBox As New RoundedPanel()
    Private ReadOnly lblUsernameIcon As New Label()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly pnlPasswordBox As New RoundedPanel()
    Private ReadOnly lblPasswordIcon As New Label()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly btnTogglePassword As New Button()
    Private ReadOnly btnLogin As New Button()
    Private ReadOnly pnlFooter As New Panel()
    Private ReadOnly lblDemoHint As New Label()
    Private ReadOnly btnClose As New Button()
    Private ReadOnly err As New ErrorProvider()
    Private ReadOnly tips As New ToolTip()

    Private ReadOnly _inputBackground As Color = ColorTranslator.FromHtml("#f2f4f6")
    Private ReadOnly _inputBorderDefault As Color = ColorTranslator.FromHtml("#d9dee5")
    Private ReadOnly _inputBorderFocus As Color = ColorTranslator.FromHtml("#3498db")
    Private ReadOnly _inputBorderError As Color = ColorTranslator.FromHtml("#e74c3c")
    Private ReadOnly _loginBaseColor As Color = ColorTranslator.FromHtml("#3498db")
    Private ReadOnly _loginHoverColor As Color = ColorTranslator.FromHtml("#2980b9")
    Private ReadOnly _toggleHoverColor As Color = ColorTranslator.FromHtml("#dfe6eb")
    Private ReadOnly _backgroundColor As Color = ColorTranslator.FromHtml("#2c3e50")

    Private Const UsernamePlaceholder As String = "Username"
    Private Const PasswordPlaceholder As String = "Password"
    Private _isUsernamePlaceholder As Boolean = True
    Private _isPasswordPlaceholder As Boolean = True

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Text = "Login"
        WindowState = FormWindowState.Maximized
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        MinimizeBox = False
        StartPosition = FormStartPosition.CenterScreen
        BackColor = _backgroundColor
        Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        DoubleBuffered = True
        KeyPreview = True

        pnlCardShadow.Size = New Size(420, 500)
        pnlCardShadow.BackColor = Color.FromArgb(65, 0, 0, 0)
        pnlCardShadow.CornerRadius = 20
        pnlCardShadow.BorderColor = Color.Transparent

        pnlLoginCard.Size = New Size(420, 500)
        pnlLoginCard.BackColor = Color.White
        pnlLoginCard.CornerRadius = 20
        pnlLoginCard.BorderColor = ColorTranslator.FromHtml("#e5e8eb")
        pnlLoginCard.Padding = New Padding(24)

        tblCardContent.Dock = DockStyle.Fill
        tblCardContent.ColumnCount = 1
        tblCardContent.RowCount = 6
        tblCardContent.Margin = New Padding(0)
        tblCardContent.Padding = New Padding(0)
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 140.0F))
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        tblCardContent.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        pnlHeader.Dock = DockStyle.Fill
        pnlHeader.Padding = New Padding(0)

        lblIcon.Dock = DockStyle.Top
        lblIcon.Height = 64
        lblIcon.Text = "💧"
        lblIcon.Font = New Font("Segoe UI Emoji", 48.0F, FontStyle.Regular)
        lblIcon.ForeColor = ColorTranslator.FromHtml("#3498db")
        lblIcon.TextAlign = ContentAlignment.MiddleCenter

        lblTitle.Dock = DockStyle.Top
        lblTitle.Height = 50
        lblTitle.Text = "Welcome Back"
        lblTitle.Font = New Font("Segoe UI", 24.0F, FontStyle.Bold)
        lblTitle.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblTitle.TextAlign = ContentAlignment.MiddleCenter

        pnlHeader.Controls.Add(lblTitle)
        pnlHeader.Controls.Add(lblIcon)

        lblSubtitle.Dock = DockStyle.Fill
        lblSubtitle.Text = "Sign in to your account"
        lblSubtitle.Font = New Font("Segoe UI", 10.0F)
        lblSubtitle.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        lblSubtitle.TextAlign = ContentAlignment.MiddleCenter

        ConfigureInputContainer(pnlUsernameBox)
        lblUsernameIcon.Dock = DockStyle.Left
        lblUsernameIcon.Width = 36
        lblUsernameIcon.Text = "👤"
        lblUsernameIcon.Font = New Font("Segoe UI Emoji", 11.0F)
        lblUsernameIcon.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        lblUsernameIcon.TextAlign = ContentAlignment.MiddleCenter

        txtUsername.BorderStyle = BorderStyle.None
        txtUsername.Font = New Font("Segoe UI", 11.0F)
        txtUsername.BackColor = _inputBackground
        txtUsername.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        txtUsername.Dock = DockStyle.Fill
        txtUsername.Margin = New Padding(0)

        pnlUsernameBox.Controls.Add(txtUsername)
        pnlUsernameBox.Controls.Add(lblUsernameIcon)

        ConfigureInputContainer(pnlPasswordBox)
        lblPasswordIcon.Dock = DockStyle.Left
        lblPasswordIcon.Width = 36
        lblPasswordIcon.Text = "🔒"
        lblPasswordIcon.Font = New Font("Segoe UI Emoji", 11.0F)
        lblPasswordIcon.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        lblPasswordIcon.TextAlign = ContentAlignment.MiddleCenter

        txtPassword.BorderStyle = BorderStyle.None
        txtPassword.Font = New Font("Segoe UI", 11.0F)
        txtPassword.BackColor = _inputBackground
        txtPassword.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        txtPassword.Dock = DockStyle.Fill
        txtPassword.Margin = New Padding(0)
        txtPassword.UseSystemPasswordChar = False

        btnTogglePassword.Text = "👁"
        btnTogglePassword.Font = New Font("Segoe UI Emoji", 10.0F)
        btnTogglePassword.Dock = DockStyle.Right
        btnTogglePassword.Size = New Size(42, 42)
        btnTogglePassword.FlatStyle = FlatStyle.Flat
        btnTogglePassword.FlatAppearance.BorderSize = 0
        btnTogglePassword.BackColor = Color.Transparent
        btnTogglePassword.ForeColor = ColorTranslator.FromHtml("#7f8c8d")
        btnTogglePassword.Cursor = Cursors.Hand
        btnTogglePassword.TabStop = False

        pnlPasswordBox.Controls.Add(txtPassword)
        pnlPasswordBox.Controls.Add(btnTogglePassword)
        pnlPasswordBox.Controls.Add(lblPasswordIcon)

        btnLogin.Text = "LOGIN"
        btnLogin.Dock = DockStyle.Fill
        btnLogin.Height = 44
        btnLogin.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.ForeColor = Color.White
        btnLogin.BackColor = _loginBaseColor
        btnLogin.Cursor = Cursors.Hand
        btnLogin.Margin = New Padding(0)
        btnLogin.TabIndex = 2

        pnlFooter.Dock = DockStyle.Fill
        pnlFooter.Padding = New Padding(0, 10, 0, 0)

        lblDemoHint.AutoSize = False
        lblDemoHint.Dock = DockStyle.Left
        lblDemoHint.Width = 210
        lblDemoHint.Text = "Demo: admin/admin123"
        lblDemoHint.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        lblDemoHint.ForeColor = ColorTranslator.FromHtml("#95a5a6")
        lblDemoHint.TextAlign = ContentAlignment.BottomLeft

        btnClose.Text = "Close"
        btnClose.AutoSize = False
        btnClose.Dock = DockStyle.Right
        btnClose.Width = 62
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.FlatAppearance.BorderSize = 0
        btnClose.BackColor = Color.Transparent
        btnClose.ForeColor = ColorTranslator.FromHtml("#e74c3c")
        btnClose.Font = New Font("Segoe UI", 9.0F, FontStyle.Underline)
        btnClose.Cursor = Cursors.Hand
        btnClose.TabStop = False

        pnlFooter.Controls.Add(btnClose)
        pnlFooter.Controls.Add(lblDemoHint)

        tblCardContent.Controls.Add(pnlHeader, 0, 0)
        tblCardContent.Controls.Add(lblSubtitle, 0, 1)
        tblCardContent.Controls.Add(pnlUsernameBox, 0, 2)
        tblCardContent.Controls.Add(pnlPasswordBox, 0, 3)
        tblCardContent.Controls.Add(btnLogin, 0, 4)
        tblCardContent.Controls.Add(pnlFooter, 0, 5)

        pnlLoginCard.Controls.Add(tblCardContent)
        Controls.Add(pnlCardShadow)
        Controls.Add(pnlLoginCard)

        AcceptButton = btnLogin

        AddHandler Load, AddressOf frmLogin_Load
        AddHandler Resize, AddressOf frmLogin_Resize
        AddHandler Paint, AddressOf frmLogin_Paint
        AddHandler KeyDown, AddressOf frmLogin_KeyDown
        AddHandler btnLogin.Click, AddressOf btnLogin_Click
        AddHandler btnClose.Click, AddressOf btnClose_Click
        AddHandler btnTogglePassword.Click, AddressOf btnTogglePassword_Click
        AddHandler btnTogglePassword.MouseEnter, Sub() btnTogglePassword.BackColor = _toggleHoverColor
        AddHandler btnTogglePassword.MouseLeave, Sub() btnTogglePassword.BackColor = Color.Transparent
        AddHandler btnLogin.MouseEnter, Sub() btnLogin.BackColor = _loginHoverColor
        AddHandler btnLogin.MouseLeave, Sub() btnLogin.BackColor = _loginBaseColor
        AddHandler txtUsername.Enter, AddressOf txtUsername_Enter
        AddHandler txtUsername.Leave, AddressOf txtUsername_Leave
        AddHandler txtPassword.Enter, AddressOf txtPassword_Enter
        AddHandler txtPassword.Leave, AddressOf txtPassword_Leave
        AddHandler txtUsername.TextChanged, AddressOf txtUsername_TextChanged
        AddHandler txtPassword.TextChanged, AddressOf txtPassword_TextChanged

        err.BlinkStyle = ErrorBlinkStyle.NeverBlink
        err.ContainerControl = Me
        err.SetIconAlignment(txtUsername, ErrorIconAlignment.MiddleRight)
        err.SetIconAlignment(txtPassword, ErrorIconAlignment.MiddleRight)

        tips.SetToolTip(txtUsername, "Enter your username")
        tips.SetToolTip(txtPassword, "Enter your password")
        tips.SetToolTip(btnTogglePassword, "Show password")
        tips.SetToolTip(btnLogin, "Log in to your account")
        tips.SetToolTip(btnClose, "Close application")

        txtUsername.TabIndex = 0
        txtPassword.TabIndex = 1
        ApplyRoundedRegion(btnLogin, 8)
    End Sub

    Private Sub ConfigureInputContainer(container As RoundedPanel)
        container.Dock = DockStyle.Fill
        container.BackColor = _inputBackground
        container.CornerRadius = 10
        container.BorderColor = _inputBorderDefault
        container.Padding = New Padding(8, 0, 8, 0)
        container.Margin = New Padding(0, 6, 0, 0)
    End Sub

    Private Sub frmLogin_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()
        ShowUsernamePlaceholder()
        ShowPasswordPlaceholder()
        CenterLoginPanel()
    End Sub

    Private Sub frmLogin_Resize(sender As Object, e As EventArgs)
        CenterLoginPanel()
    End Sub

    Private Sub frmLogin_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    Private Sub frmLogin_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using brush As New LinearGradientBrush(ClientRectangle,
                                               ColorTranslator.FromHtml("#3498db"),
                                               _backgroundColor,
                                               LinearGradientMode.ForwardDiagonal)
            e.Graphics.FillRectangle(brush, ClientRectangle)
        End Using

        Using overlayBrush As New SolidBrush(Color.FromArgb(55, Color.Black))
            e.Graphics.FillRectangle(overlayBrush, ClientRectangle)
        End Using
    End Sub

    Private Sub CenterLoginPanel()
        pnlLoginCard.Left = (ClientSize.Width - pnlLoginCard.Width) \ 2
        pnlLoginCard.Top = (ClientSize.Height - pnlLoginCard.Height) \ 2
        pnlCardShadow.Left = pnlLoginCard.Left + 10
        pnlCardShadow.Top = pnlLoginCard.Top + 12
        pnlLoginCard.BringToFront()
    End Sub

    Private Sub txtUsername_Enter(sender As Object, e As EventArgs)
        If _isUsernamePlaceholder Then
            txtUsername.Text = String.Empty
            txtUsername.ForeColor = ColorTranslator.FromHtml("#2c3e50")
            _isUsernamePlaceholder = False
        End If
        SetInputBorder(pnlUsernameBox, _inputBorderFocus)
    End Sub

    Private Sub txtUsername_Leave(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtUsername.Text) Then
            ShowUsernamePlaceholder()
        End If
        If String.IsNullOrWhiteSpace(GetUsernameValue()) Then
            SetInputBorder(pnlUsernameBox, _inputBorderDefault)
        End If
    End Sub

    Private Sub txtPassword_Enter(sender As Object, e As EventArgs)
        If _isPasswordPlaceholder Then
            txtPassword.Text = String.Empty
            txtPassword.ForeColor = ColorTranslator.FromHtml("#2c3e50")
            txtPassword.UseSystemPasswordChar = True
            _isPasswordPlaceholder = False
        End If
        SetInputBorder(pnlPasswordBox, _inputBorderFocus)
    End Sub

    Private Sub txtPassword_Leave(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtPassword.Text) Then
            ShowPasswordPlaceholder()
        End If
        If String.IsNullOrWhiteSpace(GetPasswordValue()) Then
            SetInputBorder(pnlPasswordBox, _inputBorderDefault)
        End If
    End Sub

    Private Sub txtUsername_TextChanged(sender As Object, e As EventArgs)
        If Not String.IsNullOrWhiteSpace(GetUsernameValue()) Then
            err.SetError(txtUsername, String.Empty)
            SetInputBorder(pnlUsernameBox, _inputBorderDefault)
        End If
    End Sub

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs)
        If Not String.IsNullOrWhiteSpace(GetPasswordValue()) Then
            err.SetError(txtPassword, String.Empty)
            SetInputBorder(pnlPasswordBox, _inputBorderDefault)
        End If
    End Sub

    Private Sub ShowUsernamePlaceholder()
        txtUsername.Text = UsernamePlaceholder
        txtUsername.ForeColor = ColorTranslator.FromHtml("#95a5a6")
        _isUsernamePlaceholder = True
    End Sub

    Private Sub ShowPasswordPlaceholder()
        txtPassword.UseSystemPasswordChar = False
        txtPassword.Text = PasswordPlaceholder
        txtPassword.ForeColor = ColorTranslator.FromHtml("#95a5a6")
        _isPasswordPlaceholder = True
        btnTogglePassword.Text = "👁"
    End Sub

    Private Function GetUsernameValue() As String
        Return If(_isUsernamePlaceholder, String.Empty, txtUsername.Text.Trim())
    End Function

    Private Function GetPasswordValue() As String
        Return If(_isPasswordPlaceholder, String.Empty, txtPassword.Text)
    End Function

    Private Sub SetInputBorder(targetPanel As RoundedPanel, color As Color)
        targetPanel.BorderColor = color
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs)
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub btnTogglePassword_Click(sender As Object, e As EventArgs)
        If _isPasswordPlaceholder Then
            Return
        End If

        txtPassword.UseSystemPasswordChar = Not txtPassword.UseSystemPasswordChar
        Dim isHidden As Boolean = txtPassword.UseSystemPasswordChar
        btnTogglePassword.Text = If(isHidden, "👁", "🙈")
        tips.SetToolTip(btnTogglePassword, If(isHidden, "Show password", "Hide password"))
    End Sub

    Private Shared Sub ApplyRoundedRegion(ctrl As Control, radius As Integer)
        Using path As GraphicsPath = CreateRoundedPath(New Rectangle(0, 0, ctrl.Width, ctrl.Height), radius)
            ctrl.Region = New Region(path)
        End Using
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs)
        Try
            Dim username As String = GetUsernameValue()
            Dim password As String = GetPasswordValue()

            err.Clear()

            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
                If String.IsNullOrWhiteSpace(username) Then
                    err.SetError(txtUsername, "Username is required")
                    SetInputBorder(pnlUsernameBox, _inputBorderError)
                End If
                If String.IsNullOrWhiteSpace(password) Then
                    err.SetError(txtPassword, "Password is required")
                    SetInputBorder(pnlPasswordBox, _inputBorderError)
                End If

                MessageBox.Show("Please enter username and password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Const sql As String = "SELECT id AS user_id, username, full_name, password_hash, role, is_active, force_password_change FROM users WHERE username = @username LIMIT 1;"
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
            CurrentUser.FullName = If(row("full_name") Is DBNull.Value, String.Empty, row("full_name").ToString())
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
                nextForm = New frmStaffDashboard()
            Case "customer"
                nextForm = New frmCustomerDashboard()
            Case Else
                MessageBox.Show("Unknown user role.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
        End Select

        nextForm.Show()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private NotInheritable Class RoundedPanel
        Inherits Panel

        Private _cornerRadius As Integer = 12
        Private _borderColor As Color = ColorTranslator.FromHtml("#d5d8dc")

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <DefaultValue(12)>
        Public Property CornerRadius As Integer
            Get
                Return _cornerRadius
            End Get
            Set(value As Integer)
                _cornerRadius = Math.Max(1, value)
                UpdatePanelRegion()
                Invalidate()
            End Set
        End Property

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property BorderColor As Color
            Get
                Return _borderColor
            End Get
            Set(value As Color)
                _borderColor = value
                Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnSizeChanged(e As EventArgs)
            MyBase.OnSizeChanged(e)
            UpdatePanelRegion()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
            Using path As GraphicsPath = CreateRoundedPath(New Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius)
                Using borderPen As New Pen(_borderColor, 1)
                    e.Graphics.DrawPath(borderPen, path)
                End Using
            End Using
        End Sub

        Private Sub UpdatePanelRegion()
            If Width <= 0 OrElse Height <= 0 Then
                Return
            End If

            Using path As GraphicsPath = CreateRoundedPath(New Rectangle(0, 0, Width, Height), _cornerRadius)
                Region = New Region(path)
            End Using
        End Sub
    End Class

    Private Shared Function CreateRoundedPath(bounds As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim diameter As Integer = radius * 2
        Dim arc As New Rectangle(bounds.X, bounds.Y, diameter, diameter)

        path.AddArc(arc, 180, 90)
        arc.X = bounds.Right - diameter
        path.AddArc(arc, 270, 90)
        arc.Y = bounds.Bottom - diameter
        path.AddArc(arc, 0, 90)
        arc.X = bounds.X
        path.AddArc(arc, 90, 90)
        path.CloseFigure()

        Return path
    End Function
End Class
