Imports System.Security.Cryptography
Imports System.Text

Public NotInheritable Class PasswordHelper
    Private Sub New()
    End Sub

    Public Shared Function ComputeSha256Hash(plainText As String) As String
        If plainText Is Nothing Then
            plainText = String.Empty
        End If

        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(plainText)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Dim sb As New StringBuilder()

            For Each b As Byte In hash
                sb.Append(b.ToString("x2"))
            Next

            Return sb.ToString()
        End Using
    End Function

    Public Shared Function VerifySha256Password(plainText As String, storedHash As String) As Boolean
        Dim computedHash As String = ComputeSha256Hash(plainText)
        Return String.Equals(computedHash, storedHash, StringComparison.OrdinalIgnoreCase)
    End Function
End Class
