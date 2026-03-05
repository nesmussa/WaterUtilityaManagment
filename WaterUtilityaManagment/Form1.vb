Public Class Form1
    Private Sub btnGoToLogin_Click(sender As Object, e As EventArgs) Handles btnGoToLogin.Click
        Dim loginForm As New frmLogin()
        loginForm.Show()
        Me.Hide()
    End Sub

End Class
