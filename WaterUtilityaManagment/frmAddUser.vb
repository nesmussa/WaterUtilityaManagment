Imports System.ComponentModel
Imports System.Linq

Public Class frmAddUser
    Inherits Form

    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly btnTogglePassword As New Button()
    Private ReadOnly cboRole As New ComboBox()
    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtPhone As New TextBox()
    Private ReadOnly progressPasswordStrength As New ProgressBar()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly _staffOnly As Boolean
    Private ReadOnly _isEditMode As Boolean

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property UsernameValue As String
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property PasswordValue As String
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property RoleValue As String
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property FullNameValue As String
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EmailValue As String
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property PhoneValue As String

    Public Sub New(Optional staffOnly As Boolean = False, Optional isEditMode As Boolean = False)
        _staffOnly = staffOnly
        _isEditMode = isEditMode
        InitializeComponent()
    End Sub

    Public Sub SetUserData(username As String,
                           role As String,
                           fullName As String,
                           email As String,
                           phone As String)
        txtUsername.Text = username
        txtUsername.Enabled = False
        cboRole.SelectedItem = role
        txtFullName.Text = fullName
        txtEmail.Text = email
        txtPhone.Text = phone
    End Sub

    Private Sub InitializeComponent()
        Me.Text = If(_isEditMode, "Edit User", "Add New User")
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(1200, 800)
        Me.MinimumSize = New Size(900, 600)
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.ShowInTaskbar = True
        Me.BackColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        Dim headerPanel As New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 52,
            .BackColor = ColorTranslator.FromHtml("#3498db")
        }
        Dim lblHeader As New Label() With {
            .Dock = DockStyle.Fill,
            .Text = If(_isEditMode, "Edit User", "Add New User"),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font("Segoe UI", 14.0F, FontStyle.Bold),
            .ForeColor = Color.White
        }
        headerPanel.Controls.Add(lblHeader)

        Dim contentGrid As New TableLayoutPanel() With {
            .Left = 22,
            .Top = 68,
            .Width = 406,
            .Height = 244,
            .ColumnCount = 2,
            .RowCount = 7
        }
        contentGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35.0F))
        contentGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65.0F))
        For i As Integer = 0 To 6
            contentGrid.RowStyles.Add(New RowStyle(SizeType.Absolute, 34.0F))
        Next

        Dim lblUsername As New Label() With {.Text = "Username", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtUsername.Dock = DockStyle.Fill

        Dim lblPassword As New Label() With {.Text = "Password", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        Dim passwordPanel As New Panel() With {.Dock = DockStyle.Fill}
        txtPassword.Left = 0
        txtPassword.Top = 2
        txtPassword.Width = 232
        txtPassword.UseSystemPasswordChar = True
        If _isEditMode Then
            lblPassword.Text = "Password (optional)"
        End If

        btnTogglePassword.Text = "👁"
        btnTogglePassword.Width = 32
        btnTogglePassword.Height = 28
        btnTogglePassword.Left = 236
        btnTogglePassword.Top = 2
        btnTogglePassword.FlatStyle = FlatStyle.Flat
        btnTogglePassword.FlatAppearance.BorderSize = 0
        btnTogglePassword.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        AddHandler btnTogglePassword.Click, AddressOf btnTogglePassword_Click
        AddHandler txtPassword.TextChanged, AddressOf txtPassword_TextChanged
        passwordPanel.Controls.Add(txtPassword)
        passwordPanel.Controls.Add(btnTogglePassword)

        Dim lblRole As New Label() With {.Text = "Role", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        cboRole.Dock = DockStyle.Fill
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboRole.Items.AddRange(New Object() {"Manager", "Staff"})
        cboRole.SelectedIndex = 1
        If _staffOnly Then
            cboRole.SelectedItem = "Staff"
            cboRole.Enabled = False
        End If

        Dim lblFullName As New Label() With {.Text = "Full Name", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtFullName.Dock = DockStyle.Fill

        Dim lblEmail As New Label() With {.Text = "Email", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtEmail.Dock = DockStyle.Fill

        Dim lblPhone As New Label() With {.Text = "Phone", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtPhone.Dock = DockStyle.Fill

        Dim lblStrength As New Label() With {.Text = "Strength", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        progressPasswordStrength.Dock = DockStyle.Fill
        progressPasswordStrength.Minimum = 0
        progressPasswordStrength.Maximum = 100

        contentGrid.Controls.Add(lblUsername, 0, 0)
        contentGrid.Controls.Add(txtUsername, 1, 0)
        contentGrid.Controls.Add(lblPassword, 0, 1)
        contentGrid.Controls.Add(passwordPanel, 1, 1)
        contentGrid.Controls.Add(lblStrength, 0, 2)
        contentGrid.Controls.Add(progressPasswordStrength, 1, 2)
        contentGrid.Controls.Add(lblRole, 0, 3)
        contentGrid.Controls.Add(cboRole, 1, 3)
        contentGrid.Controls.Add(lblFullName, 0, 4)
        contentGrid.Controls.Add(txtFullName, 1, 4)
        contentGrid.Controls.Add(lblEmail, 0, 5)
        contentGrid.Controls.Add(txtEmail, 1, 5)
        contentGrid.Controls.Add(lblPhone, 0, 6)
        contentGrid.Controls.Add(txtPhone, 1, 6)

        btnSave.Text = "Save"
        btnSave.Left = 254
        btnSave.Top = 334
        btnSave.Width = 80
        btnSave.Height = 36
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnSave.ForeColor = Color.White
        btnSave.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 344
        btnCancel.Top = 334
        btnCancel.Width = 80
        btnCancel.Height = 36
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnCancel.ForeColor = Color.White
        btnCancel.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        Me.Controls.Add(headerPanel)
        Me.Controls.Add(contentGrid)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel

        UiStyleHelper.AddDialogCloseButton(Me)
    End Sub

    Private Sub btnTogglePassword_Click(sender As Object, e As EventArgs)
        txtPassword.UseSystemPasswordChar = Not txtPassword.UseSystemPasswordChar
        btnTogglePassword.Text = If(txtPassword.UseSystemPasswordChar, "👁", "🙈")
    End Sub

    Private Sub txtPassword_TextChanged(sender As Object, e As EventArgs)
        progressPasswordStrength.Value = CalculatePasswordStrength(txtPassword.Text)
    End Sub

    Private Shared Function CalculatePasswordStrength(password As String) As Integer
        If String.IsNullOrWhiteSpace(password) Then
            Return 0
        End If

        Dim score As Integer = Math.Min(30, password.Length * 4)
        If password.Any(AddressOf Char.IsDigit) Then score += 20
        If password.Any(AddressOf Char.IsUpper) Then score += 20
        If password.Any(Function(ch) Not Char.IsLetterOrDigit(ch)) Then score += 30
        Return Math.Min(100, score)
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        UsernameValue = txtUsername.Text.Trim()
        PasswordValue = txtPassword.Text
        RoleValue = cboRole.Text
        FullNameValue = txtFullName.Text.Trim()
        EmailValue = txtEmail.Text.Trim()
        PhoneValue = txtPhone.Text.Trim()

        If String.IsNullOrWhiteSpace(UsernameValue) OrElse
           (Not _isEditMode AndAlso String.IsNullOrWhiteSpace(PasswordValue)) OrElse
           String.IsNullOrWhiteSpace(RoleValue) OrElse
           String.IsNullOrWhiteSpace(FullNameValue) Then
            MessageBox.Show("Username, password, role, and full name are required.",
                            "Validation",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
