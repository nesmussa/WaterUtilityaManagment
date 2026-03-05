Imports System.Diagnostics

Public Class frmBackupRestore
    Inherits Form

    Private ReadOnly btnBackup As New Button()
    Private ReadOnly btnRestore As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Backup / Restore"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 380
        Me.Height = 170
        Me.MinimumSize = New Size(380, 170)

        btnBackup.Text = "Backup Database"
        btnBackup.Left = 30
        btnBackup.Top = 40
        btnBackup.Width = 140
        AddHandler btnBackup.Click, AddressOf btnBackup_Click

        btnRestore.Text = "Restore Database"
        btnRestore.Left = 190
        btnRestore.Top = 40
        btnRestore.Width = 140
        AddHandler btnRestore.Click, AddressOf btnRestore_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 110
        btnLogout.Top = 90
        btnLogout.Width = 140
        btnLogout.Visible = False
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnBackup, True)
        UiStyleHelper.StyleButton(btnRestore)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(btnBackup)
        Me.Controls.Add(btnRestore)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmBackupRestore_Load
    End Sub

    Private Sub frmBackupRestore_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can access backup and restore.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
        End If
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs)
        Dim sfd As New SaveFileDialog() With {.Filter = "SQL File|*.sql", .FileName = "water_utility_backup.sql"}
        If sfd.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim connInfo = ParseConnectionInfo(DatabaseHelper.GetConnectionString())
        Dim args As String = $"-h {connInfo.Host} -P {connInfo.Port} -u {connInfo.User} -p{connInfo.Password} {connInfo.Database} --result-file=""{sfd.FileName}"""
        ExecuteExternalCommand("mysqldump", args, "Backup completed.")
    End Sub

    Private Sub btnRestore_Click(sender As Object, e As EventArgs)
        Dim ofd As New OpenFileDialog() With {.Filter = "SQL File|*.sql"}
        If ofd.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim connInfo = ParseConnectionInfo(DatabaseHelper.GetConnectionString())
        Dim args As String = $"-h {connInfo.Host} -P {connInfo.Port} -u {connInfo.User} -p{connInfo.Password} {connInfo.Database} -e ""source {ofd.FileName.Replace("\", "/")}"""
        ExecuteExternalCommand("mysql", args, "Restore completed.")
    End Sub

    Private Shared Sub ExecuteExternalCommand(fileName As String, arguments As String, successMessage As String)
        Try
            Dim psi As New ProcessStartInfo(fileName, arguments) With {
                .CreateNoWindow = True,
                .UseShellExecute = False,
                .RedirectStandardError = True,
                .RedirectStandardOutput = True
            }

            Using proc As Process = Process.Start(psi)
                proc.WaitForExit()
                Dim err As String = proc.StandardError.ReadToEnd()
                If proc.ExitCode <> 0 Then
                    Throw New ApplicationException(err)
                End If
            End Using

            MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Operation failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function ParseConnectionInfo(connectionString As String) As (Host As String, Port As String, User As String, Password As String, Database As String)
        Dim builder As New MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString)
        Return (builder.Server, builder.Port.ToString(), builder.UserID, builder.Password, builder.Database)
    End Function
End Class
