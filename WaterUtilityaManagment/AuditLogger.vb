Public NotInheritable Class AuditLogger
    Private Sub New()
    End Sub

    Public Shared Sub LogAction(userId As Integer?, actionName As String, details As String)
        Try
            Const sql As String = "INSERT INTO audit_log (user_id, action_name, details, action_time) VALUES (@user_id, @action_name, @details, @action_time);"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@user_id", If(userId.HasValue, CType(userId.Value, Object), Nothing)},
                {"@action_name", actionName},
                {"@details", details},
                {"@action_time", Date.Now}
            }
            DatabaseHelper.ExecuteNonQuery(sql, parameters)
        Catch
        End Try
    End Sub
End Class
