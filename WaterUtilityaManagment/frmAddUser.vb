Imports System.ComponentModel

Public Class frmAddUser
    Inherits Form

    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly cboRole As New ComboBox()
    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtPhone As New TextBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly btnLogout As New Button()
    Private ReadOnly _staffOnly As Boolean

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

    Public Sub New(Optional staffOnly As Boolean = False)
        _staffOnly = staffOnly
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = If(_staffOnly, "Add Staff", "Add User")
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 460
        Me.Height = 360
        Me.MinimumSize = New Size(460, 360)

        Dim lblUsername As New Label() With {.Text = "Username", .Left = 20, .Top = 25, .AutoSize = True}
        txtUsername.Left = 140
        txtUsername.Top = 20
        txtUsername.Width = 280

        Dim lblPassword As New Label() With {.Text = "Password", .Left = 20, .Top = 65, .AutoSize = True}
        txtPassword.Left = 140
        txtPassword.Top = 60
        txtPassword.Width = 280
        txtPassword.UseSystemPasswordChar = True

        Dim lblRole As New Label() With {.Text = "Role", .Left = 20, .Top = 105, .AutoSize = True}
        cboRole.Left = 140
        cboRole.Top = 100
        cboRole.Width = 280
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboRole.Items.AddRange(New Object() {"Manager", "Staff"})
        cboRole.SelectedIndex = 1
        If _staffOnly Then
            cboRole.SelectedItem = "Staff"
            cboRole.Enabled = False
        End If

        Dim lblFullName As New Label() With {.Text = "Full Name", .Left = 20, .Top = 145, .AutoSize = True}
        txtFullName.Left = 140
        txtFullName.Top = 140
        txtFullName.Width = 280

        Dim lblEmail As New Label() With {.Text = "Email", .Left = 20, .Top = 185, .AutoSize = True}
        txtEmail.Left = 140
        txtEmail.Top = 180
        txtEmail.Width = 280

        Dim lblPhone As New Label() With {.Text = "Phone", .Left = 20, .Top = 225, .AutoSize = True}
        txtPhone.Left = 140
        txtPhone.Top = 220
        txtPhone.Width = 280

        btnSave.Text = "Save"
        btnSave.Left = 240
        btnSave.Top = 265
        btnSave.Width = 85
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 335
        btnCancel.Top = 265
        btnCancel.Width = 85
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 145
        btnLogout.Top = 265
        btnLogout.Width = 85
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnSave, True)
        UiStyleHelper.StyleButton(btnCancel)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(lblUsername)
        Me.Controls.Add(txtUsername)
        Me.Controls.Add(lblPassword)
        Me.Controls.Add(txtPassword)
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
        Me.Controls.Add(btnLogout)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        UsernameValue = txtUsername.Text.Trim()
        PasswordValue = txtPassword.Text
        RoleValue = cboRole.Text
        FullNameValue = txtFullName.Text.Trim()
        EmailValue = txtEmail.Text.Trim()
        PhoneValue = txtPhone.Text.Trim()

        If String.IsNullOrWhiteSpace(UsernameValue) OrElse
           String.IsNullOrWhiteSpace(PasswordValue) OrElse
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

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
