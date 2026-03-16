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
    Private ReadOnly err As New ErrorProvider()
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
        Me.Width = 550
        Me.Height = 500
        Me.MinimumSize = New Size(550, 500)
        Me.FormBorderStyle = FormBorderStyle.Sizable

        Dim lblUsername As New Label() With {.Text = "Username", .Left = 30, .Top = 30, .AutoSize = True}
        txtUsername.Left = 220
        txtUsername.Top = 25
        txtUsername.Width = 260

        Dim lblPassword As New Label() With {.Text = If(_isEditMode, "Password (optional)", "Password"), .Left = 30, .Top = 70, .AutoSize = True}
        Dim passwordPanel As New Panel() With {.Left = 220, .Top = 65, .Width = 260, .Height = 28}
        txtPassword.Left = 0
        txtPassword.Top = 2
        txtPassword.Width = 224
        txtPassword.UseSystemPasswordChar = True

        btnTogglePassword.Text = "👁"
        btnTogglePassword.Width = 32
        btnTogglePassword.Height = 24
        btnTogglePassword.Left = 228
        btnTogglePassword.Top = 2
        btnTogglePassword.FlatStyle = FlatStyle.Flat
        btnTogglePassword.FlatAppearance.BorderSize = 0
        btnTogglePassword.BackColor = ColorTranslator.FromHtml("#ecf0f1")

        passwordPanel.Controls.Add(txtPassword)
        passwordPanel.Controls.Add(btnTogglePassword)

        Dim lblStrength As New Label() With {.Text = "Password Strength", .Left = 30, .Top = 110, .AutoSize = True}
        progressPasswordStrength.Left = 220
        progressPasswordStrength.Top = 105
        progressPasswordStrength.Width = 260
        progressPasswordStrength.Minimum = 0
        progressPasswordStrength.Maximum = 100

        Dim lblRole As New Label() With {.Text = "Role", .Left = 30, .Top = 150, .AutoSize = True}
        cboRole.Left = 220
        cboRole.Top = 145
        cboRole.Width = 260
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboRole.Items.AddRange(New Object() {"Manager", "Staff"})
        cboRole.SelectedIndex = 1
        If _staffOnly Then
            cboRole.SelectedItem = "Staff"
            cboRole.Enabled = False
        End If

        Dim lblFullName As New Label() With {.Text = "Full Name", .Left = 30, .Top = 190, .AutoSize = True}
        txtFullName.Left = 220
        txtFullName.Top = 185
        txtFullName.Width = 260

        Dim lblEmail As New Label() With {.Text = "Email", .Left = 30, .Top = 230, .AutoSize = True}
        txtEmail.Left = 220
        txtEmail.Top = 225
        txtEmail.Width = 260

        Dim lblPhone As New Label() With {.Text = "Phone", .Left = 30, .Top = 270, .AutoSize = True}
        txtPhone.Left = 220
        txtPhone.Top = 265
        txtPhone.Width = 260

        btnSave.Text = If(_isEditMode, "Update", "Save")
        btnSave.Left = 220
        btnSave.Top = 330
        btnSave.Width = 120

        btnCancel.Text = "Cancel"
        btnCancel.Left = 360
        btnCancel.Top = 330
        btnCancel.Width = 120

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnSave, True)
        UiStyleHelper.StyleButton(btnCancel)

        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblPassword)
        Me.Controls.Add(passwordPanel)
        Me.Controls.Add(lblStrength)
        Me.Controls.Add(progressPasswordStrength)
        Me.Controls.Add(lblRole)
        Me.Controls.Add(cboRole)
        Me.Controls.Add(lblFullName)
        Me.Controls.Add(txtFullName)
        Me.Controls.Add(lblEmail)
        Me.Controls.Add(txtEmail)
        Me.Controls.Add(lblPhone)
        Me.Controls.Add(txtPhone)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)

        AddHandler btnTogglePassword.Click, AddressOf btnTogglePassword_Click
        AddHandler txtPassword.TextChanged, AddressOf txtPassword_TextChanged
        AddHandler txtEmail.Leave, AddressOf txtEmail_Leave
        AddHandler txtPhone.Leave, AddressOf txtPhone_Leave
        AddHandler btnSave.Click, AddressOf btnSave_Click
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel

        err.BlinkStyle = ErrorBlinkStyle.NeverBlink
        err.ContainerControl = Me
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
        err.Clear()

        UsernameValue = txtUsername.Text.Trim()
        PasswordValue = txtPassword.Text
        RoleValue = cboRole.Text
        FullNameValue = txtFullName.Text.Trim()
        EmailValue = txtEmail.Text.Trim()
        PhoneValue = ValidationHelper.NormalizePhone(txtPhone.Text)

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

        Dim emailError As String = ValidateEmailField()
        If Not String.IsNullOrWhiteSpace(emailError) Then
            err.SetError(txtEmail, emailError)
            MessageBox.Show(emailError, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtEmail.Focus()
            Return
        End If

        Dim phoneError As String = ValidatePhoneField()
        If Not String.IsNullOrWhiteSpace(phoneError) Then
            err.SetError(txtPhone, phoneError)
            MessageBox.Show(phoneError, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(PhoneValue) Then
            err.SetError(txtPhone, "Phone number is required.")
            MessageBox.Show("Phone number is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPhone.Focus()
            Return
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub txtEmail_Leave(sender As Object, e As EventArgs)
        Dim emailError As String = ValidateEmailField()
        err.SetError(txtEmail, emailError)
    End Sub

    Private Sub txtPhone_Leave(sender As Object, e As EventArgs)
        Dim phoneError As String = ValidatePhoneField()
        err.SetError(txtPhone, phoneError)
    End Sub

    Private Function ValidateEmailField() As String
        Dim email As String = txtEmail.Text.Trim()
        If String.IsNullOrWhiteSpace(email) Then
            Return String.Empty
        End If

        If ValidationHelper.IsValidEmail(email) Then
            Return String.Empty
        End If

        Return "Invalid email format. Example: user@example.com"
    End Function

    Private Function ValidatePhoneField() As String
        Dim phone As String = txtPhone.Text.Trim()
        If String.IsNullOrWhiteSpace(phone) Then
            Return "Phone number is required."
        End If

        If ValidationHelper.IsValidEthiopianPhone(phone) Then
            Return String.Empty
        End If

        Return "Invalid phone format. Use 09XXXXXXXX, 07XXXXXXXX, +2519XXXXXXXX, or +2517XXXXXXXX."
    End Function

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
