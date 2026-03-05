Public NotInheritable Class CurrentUser
    Private Sub New()
    End Sub

    Public Shared Property UserId As Integer
    Public Shared Property Username As String
    Public Shared Property Role As String

    Public Shared Sub Clear()
        UserId = 0
        Username = Nothing
        Role = Nothing
    End Sub
End Class
