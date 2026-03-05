Public Class frmBills
    Inherits Form

    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        Me.Text = "Bills"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 800
        Me.Height = 500

        Dim lblInfo As New Label() With {
            .Text = "Unpaid bills view will be implemented next.",
            .AutoSize = True,
            .Left = 20,
            .Top = 20
        }

        btnLogout.Text = "Logout"
        btnLogout.Left = 650
        btnLogout.Top = 20
        btnLogout.Width = 120
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Me.Controls.Add(lblInfo)
        Me.Controls.Add(btnLogout)
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
