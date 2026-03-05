Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports System.Data

Public NotInheritable Class DatabaseHelper
    Private Sub New()
    End Sub

    Private Shared ReadOnly _connectionString As String =
        ConfigurationManager.ConnectionStrings("waterutilitydb").ConnectionString
    Private Shared ReadOnly _schemaEnsureLock As New Object()
    Private Shared _coreSchemaEnsured As Integer = 0

    Public Shared Function GetOpenConnection() As MySqlConnection
        Dim conn As New MySqlConnection(_connectionString)
        conn.Open()
        Return conn
    End Function

    Public Shared Function ExecuteScalar(sql As String,
                                         Optional parameters As Dictionary(Of String, Object) = Nothing) As Object
        Using conn As MySqlConnection = GetOpenConnection()
            Using cmd As New MySqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    Public Shared Function ExecuteDataTable(sql As String,
                                            Optional parameters As Dictionary(Of String, Object) = Nothing) As DataTable
        Using conn As MySqlConnection = GetOpenConnection()
            Using cmd As New MySqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                Using adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    Public Shared Function ExecuteNonQuery(sql As String,
                                           Optional parameters As Dictionary(Of String, Object) = Nothing) As Integer
        Using conn As MySqlConnection = GetOpenConnection()
            Using cmd As New MySqlCommand(sql, conn)
                AddParameters(cmd, parameters)
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    Public Shared Sub EnsureUsersIsActiveColumn()
        Try
            Const sql As String = "ALTER TABLE users ADD COLUMN is_active BOOLEAN DEFAULT TRUE;"
            ExecuteNonQuery(sql)
        Catch ex As MySqlException
            If ex.Number <> 1060 Then
                Throw
            End If
        End Try
    End Sub

    Public Shared Sub EnsureUsersForcePasswordChangeColumn()
        Try
            Const sql As String = "ALTER TABLE users ADD COLUMN force_password_change BOOLEAN DEFAULT FALSE;"
            ExecuteNonQuery(sql)
        Catch ex As MySqlException
            If ex.Number <> 1060 Then
                Throw
            End If
        End Try
    End Sub

    Public Shared Sub EnsureAuditLogTable()
        Const sql As String = "CREATE TABLE IF NOT EXISTS audit_log (audit_id INT AUTO_INCREMENT PRIMARY KEY, user_id INT NULL, action_name VARCHAR(100) NULL, action VARCHAR(255) NULL, table_name VARCHAR(50) NULL, record_id INT NULL, details TEXT NULL, ip_address VARCHAR(45) NULL, action_time DATETIME NULL DEFAULT CURRENT_TIMESTAMP, action_date DATETIME NULL DEFAULT CURRENT_TIMESTAMP);"
        ExecuteNonQuery(sql)
    End Sub

    Public Shared Sub EnsureAuditLogCompatibilityColumns()
        EnsureColumnExists("audit_log", "action_name", "VARCHAR(100) NULL")
        EnsureColumnExists("audit_log", "action", "VARCHAR(255) NULL")
        EnsureColumnExists("audit_log", "table_name", "VARCHAR(50) NULL")
        EnsureColumnExists("audit_log", "record_id", "INT NULL")
        EnsureColumnExists("audit_log", "ip_address", "VARCHAR(45) NULL")
        EnsureColumnExists("audit_log", "action_time", "DATETIME NULL DEFAULT CURRENT_TIMESTAMP")
        EnsureColumnExists("audit_log", "action_date", "DATETIME NULL DEFAULT CURRENT_TIMESTAMP")

        EnsureColumnDefinition("audit_log", "action_name", "VARCHAR(100) NULL DEFAULT NULL")
        EnsureColumnDefinition("audit_log", "action_time", "DATETIME NULL DEFAULT CURRENT_TIMESTAMP")
        EnsureColumnDefinition("audit_log", "action_date", "DATETIME NULL DEFAULT CURRENT_TIMESTAMP")
    End Sub

    Public Shared Sub EnsureCoreSchema()
        EnsureUsersIsActiveColumn()
        EnsureUsersForcePasswordChangeColumn()
        EnsureAuditLogTable()
        EnsureAuditLogCompatibilityColumns()
    End Sub

    Private Shared Sub EnsureColumnExists(tableName As String, columnName As String, columnDefinition As String)
        Try
            Dim sql As String = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};"
            ExecuteNonQuery(sql)
        Catch ex As MySqlException
            If ex.Number <> 1060 Then
                Throw
            End If
        End Try
    End Sub

    Private Shared Sub EnsureColumnDefinition(tableName As String, columnName As String, columnDefinition As String)
        Try
            Dim sql As String = $"ALTER TABLE {tableName} MODIFY COLUMN {columnName} {columnDefinition};"
            ExecuteNonQuery(sql)
        Catch ex As MySqlException
            If ex.Number <> 1054 Then
                Throw
            End If
        End Try
    End Sub

    Public Shared Function GetConnectionString() As String
        Return _connectionString
    End Function

    Private Shared Sub AddParameters(cmd As MySqlCommand, parameters As Dictionary(Of String, Object))
        If parameters Is Nothing Then
            Return
        End If

        For Each param As KeyValuePair(Of String, Object) In parameters
            cmd.Parameters.AddWithValue(param.Key, If(param.Value, DBNull.Value))
        Next
    End Sub
End Class
