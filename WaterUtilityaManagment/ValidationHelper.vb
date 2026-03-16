Imports System.Linq

Public Module ValidationHelper
    Public Function IsValidEmail(email As String) As Boolean
        If String.IsNullOrWhiteSpace(email) Then
            Return False
        End If

        Dim value As String = email.Trim()
        If value.Contains(" "c) Then
            Return False
        End If

        Dim atIndex As Integer = value.IndexOf("@"c)
        If atIndex <= 0 OrElse atIndex <> value.LastIndexOf("@"c) Then
            Return False
        End If

        Dim localPart As String = value.Substring(0, atIndex)
        Dim domainPart As String = value.Substring(atIndex + 1)
        If String.IsNullOrWhiteSpace(localPart) OrElse String.IsNullOrWhiteSpace(domainPart) Then
            Return False
        End If

        If domainPart.StartsWith("."c) OrElse domainPart.EndsWith("."c) Then
            Return False
        End If

        Return domainPart.Contains("."c)
    End Function

    Public Function NormalizePhone(phone As String) As String
        If String.IsNullOrWhiteSpace(phone) Then
            Return String.Empty
        End If

        Return phone.Trim().Replace(" ", String.Empty).
                            Replace("-", String.Empty).
                            Replace("(", String.Empty).
                            Replace(")", String.Empty)
    End Function

    Public Function IsValidEthiopianPhone(phone As String) As Boolean
        Dim clean As String = NormalizePhone(phone)
        If String.IsNullOrWhiteSpace(clean) Then
            Return False
        End If

        For i As Integer = 0 To clean.Length - 1
            Dim ch As Char = clean(i)
            If ch = "+"c Then
                If i <> 0 Then
                    Return False
                End If
            ElseIf Not Char.IsDigit(ch) Then
                Return False
            End If
        Next

        If clean.StartsWith("09", StringComparison.Ordinal) OrElse clean.StartsWith("07", StringComparison.Ordinal) Then
            Return clean.Length = 10 AndAlso clean.All(AddressOf Char.IsDigit)
        End If

        If clean.StartsWith("+2519", StringComparison.Ordinal) OrElse clean.StartsWith("+2517", StringComparison.Ordinal) Then
            If clean.Length <> 13 Then
                Return False
            End If

            Return clean.Substring(1).All(AddressOf Char.IsDigit)
        End If

        Return False
    End Function
End Module
