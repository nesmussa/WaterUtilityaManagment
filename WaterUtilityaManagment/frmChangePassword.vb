Public Class frmChangePassword
    Inherits Form

    Private ReadOnly txtNewPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Change Password"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 420
        Me.Height = 220
        Me.MinimumSize = New Size(420, 220)

        Dim lblNew As New Label() With {.Text = "New Password", .Left = 20, .Top = 30, .AutoSize = True}
        txtNewPassword.Left = 140
        txtNewPassword.Top = 25
        txtNewPassword.Width = 240
        txtNewPassword.UseSystemPasswordChar = True

        Dim lblConfirm As New Label() With {.Text = "Confirm", .Left = 20, .Top = 70, .AutoSize = True}
        txtConfirmPassword.Left = 140
        txtConfirmPassword.Top = 65
        txtConfirmPassword.Width = 240
        txtConfirmPassword.UseSystemPasswordChar = True

        btnSave.Text = "Save"
        btnSave.Left = 140
        btnSave.Top = 110
        btnSave.Width = 120
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 270
        btnLogout.Top = 110
        btnLogout.Width = 110
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnSave, True)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(lblNew)
        Me.Controls.Add(txtNewPassword)
        Me.Controls.Add(lblConfirm)
        Me.Controls.Add(txtConfirmPassword)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnLogout)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim newPassword As String = txtNewPassword.Text
        Dim confirmPassword As String = txtConfirmPassword.Text

        If String.IsNullOrWhiteSpace(newPassword) OrElse newPassword.Length < 6 Then
            MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If newPassword <> confirmPassword Then
            MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sql As String = "UPDATE users SET password_hash = @password_hash, force_password_change = 0 WHERE id = @user_id;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@password_hash", PasswordHelper.ComputeSha256Hash(newPassword)},
            {"@user_id", CurrentUser.UserId}
        }
        DatabaseHelper.ExecuteNonQuery(sql, parameters)

        AuditLogger.LogAction(CurrentUser.UserId, "PasswordChanged", "Password changed on first login")

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
