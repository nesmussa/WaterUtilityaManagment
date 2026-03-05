Public NotInheritable Class SessionManager
    Private Sub New()
    End Sub

    Public Shared Sub Logout(currentForm As Form)
        CurrentUser.Clear()

        Dim openForms As New List(Of Form)()
        Dim homeForm As Form1 = Nothing

        For Each frm As Form In Application.OpenForms
            openForms.Add(frm)
            If TypeOf frm Is Form1 Then
                homeForm = DirectCast(frm, Form1)
            End If
        Next

        If homeForm Is Nothing OrElse homeForm.IsDisposed Then
            homeForm = New Form1()
        End If

        If Not homeForm.Visible Then
            homeForm.Show()
        End If
        homeForm.BringToFront()

        For Each frm As Form In openForms
            If Not Object.ReferenceEquals(frm, homeForm) Then
                frm.Close()
            End If
        Next
    End Sub
End Class
