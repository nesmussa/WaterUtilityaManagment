Public Class frmChangePassword
    Inherits Form

    Private ReadOnly pnlHeader As New Panel()
    Private ReadOnly lblHeaderTitle As New Label()
    Private ReadOnly lblNew As New Label()
    Private ReadOnly lblConfirm As New Label()
    Private ReadOnly txtNewPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly err As New ErrorProvider()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Change Password"
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

        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 50
        pnlHeader.BackColor = ColorTranslator.FromHtml("#3498db")

        lblHeaderTitle.Dock = DockStyle.Fill
        lblHeaderTitle.Text = "Change Password"
        lblHeaderTitle.TextAlign = ContentAlignment.MiddleCenter
        lblHeaderTitle.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        lblHeaderTitle.ForeColor = Color.White
        pnlHeader.Controls.Add(lblHeaderTitle)

        lblNew.Text = "New Password"
        lblNew.Left = 45
        lblNew.Top = 78
        lblNew.AutoSize = True

        txtNewPassword.Left = 45
        txtNewPassword.Top = 100
        txtNewPassword.Width = 360
        txtNewPassword.Height = 32
        txtNewPassword.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        txtNewPassword.UseSystemPasswordChar = True

        lblConfirm.Text = "Confirm Password"
        lblConfirm.Left = 45
        lblConfirm.Top = 145
        lblConfirm.AutoSize = True

        txtConfirmPassword.Left = 45
        txtConfirmPassword.Top = 167
        txtConfirmPassword.Width = 360
        txtConfirmPassword.Height = 32
        txtConfirmPassword.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        txtConfirmPassword.UseSystemPasswordChar = True

        btnSave.Text = "Save"
        btnSave.Left = 45
        btnSave.Top = 220
        btnSave.Width = 175
        btnSave.Height = 38
        btnSave.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnSave.ForeColor = Color.White
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 230
        btnCancel.Top = 220
        btnCancel.Width = 175
        btnCancel.Height = 38
        btnCancel.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnCancel.ForeColor = Color.White
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        btnSave.Text = "Change Password"

        err.BlinkStyle = ErrorBlinkStyle.NeverBlink
        err.ContainerControl = Me

        Me.Controls.Add(pnlHeader)
        Me.Controls.Add(lblNew)
        Me.Controls.Add(txtNewPassword)
        Me.Controls.Add(lblConfirm)
        Me.Controls.Add(txtConfirmPassword)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel

        UiStyleHelper.AddDialogCloseButton(Me)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim newPassword As String = txtNewPassword.Text
        Dim confirmPassword As String = txtConfirmPassword.Text

        err.Clear()

        If String.IsNullOrWhiteSpace(newPassword) OrElse newPassword.Length < 6 Then
            err.SetError(txtNewPassword, "Password must be at least 6 characters.")
            MessageBox.Show("Password must be at least 6 characters.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If newPassword <> confirmPassword Then
            err.SetError(txtConfirmPassword, "Passwords do not match.")
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

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
