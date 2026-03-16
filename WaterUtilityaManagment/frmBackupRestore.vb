Imports System.Diagnostics
Imports System.IO
Imports System.ComponentModel

Public Class frmBackupRestore
    Inherits Form

    Private ReadOnly pnlHeader As New Panel()
    Private ReadOnly lblHeader As New Label()
    Private ReadOnly btnBackup As New Button()
    Private ReadOnly btnRestore As New Button()
    Private ReadOnly lblLastBackup As New Label()
    Private ReadOnly progressOperation As New ProgressBar()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Backup / Restore"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.ClientSize = New Size(500, 300)
        Me.MinimumSize = New Size(500, 300)
        Me.MaximumSize = New Size(500, 300)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.BackColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 54
        pnlHeader.BackColor = ColorTranslator.FromHtml("#3498db")

        lblHeader.Dock = DockStyle.Fill
        lblHeader.Text = "Backup & Restore Utility"
        lblHeader.TextAlign = ContentAlignment.MiddleCenter
        lblHeader.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        pnlHeader.Controls.Add(lblHeader)

        btnBackup.Text = "💾 Backup Database"
        btnBackup.Left = 38
        btnBackup.Top = 86
        btnBackup.Width = 200
        btnBackup.Height = 72
        btnBackup.FlatStyle = FlatStyle.Flat
        btnBackup.FlatAppearance.BorderSize = 0
        btnBackup.BackColor = ColorTranslator.FromHtml("#3498db")
        btnBackup.ForeColor = Color.White
        btnBackup.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnBackup.Click, AddressOf btnBackup_Click

        btnRestore.Text = "📂 Restore Database"
        btnRestore.Left = 258
        btnRestore.Top = 86
        btnRestore.Width = 200
        btnRestore.Height = 72
        btnRestore.FlatStyle = FlatStyle.Flat
        btnRestore.FlatAppearance.BorderSize = 0
        btnRestore.BackColor = ColorTranslator.FromHtml("#e67e22")
        btnRestore.ForeColor = Color.White
        btnRestore.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnRestore.Click, AddressOf btnRestore_Click

        lblLastBackup.Left = 40
        lblLastBackup.Top = 176
        lblLastBackup.Width = 420
        lblLastBackup.Height = 38
        lblLastBackup.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblLastBackup.Text = "Last Backup: Not available"

        progressOperation.Left = 40
        progressOperation.Top = 230
        progressOperation.Width = 420
        progressOperation.Height = 16
        progressOperation.Style = ProgressBarStyle.Marquee
        progressOperation.MarqueeAnimationSpeed = 25
        progressOperation.Visible = False

        Me.Controls.Add(pnlHeader)
        Me.Controls.Add(btnBackup)
        Me.Controls.Add(btnRestore)
        Me.Controls.Add(lblLastBackup)
        Me.Controls.Add(progressOperation)

        AddHandler Me.Load, AddressOf frmBackupRestore_Load
    End Sub

    Private Sub frmBackupRestore_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can access backup and restore.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        UiStyleHelper.AddDialogCloseButton(Me)
    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs)
        Dim sfd As New SaveFileDialog() With {.Filter = "SQL File|*.sql", .FileName = "water_utility_backup.sql"}
        If sfd.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim connInfo = ParseConnectionInfo(DatabaseHelper.GetConnectionString())
        Dim args As String = $"-h {connInfo.Host} -P {connInfo.Port} -u {connInfo.User} -p{connInfo.Password} {connInfo.Database} --result-file=""{sfd.FileName}"""
        Dim dumpPath As String = ResolveExecutablePath("mysqldump")
        If String.IsNullOrWhiteSpace(dumpPath) Then
            ShowExecutableNotFoundMessage("mysqldump.exe")
            Return
        End If

        ExecuteExternalCommand(dumpPath, args, "Backup completed.", sfd.FileName)
    End Sub

    Private Sub btnRestore_Click(sender As Object, e As EventArgs)
        Dim ofd As New OpenFileDialog() With {.Filter = "SQL File|*.sql"}
        If ofd.ShowDialog() <> DialogResult.OK Then
            Return
        End If

        Dim connInfo = ParseConnectionInfo(DatabaseHelper.GetConnectionString())
        Dim args As String = $"-h {connInfo.Host} -P {connInfo.Port} -u {connInfo.User} -p{connInfo.Password} {connInfo.Database} -e ""source {ofd.FileName.Replace("\", "/")}"""
        Dim mysqlPath As String = ResolveExecutablePath("mysql")
        If String.IsNullOrWhiteSpace(mysqlPath) Then
            ShowExecutableNotFoundMessage("mysql.exe")
            Return
        End If

        ExecuteExternalCommand(mysqlPath, args, "Restore completed.")
    End Sub

    Private Sub ExecuteExternalCommand(fileName As String,
                                       arguments As String,
                                       successMessage As String,
                                       Optional backupPath As String = Nothing)
        Try
            progressOperation.Visible = True
            btnBackup.Enabled = False
            btnRestore.Enabled = False

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

            If Not String.IsNullOrWhiteSpace(backupPath) Then
                lblLastBackup.Text = $"Last Backup: {Date.Now:yyyy-MM-dd HH:mm}  |  {backupPath}"
            End If

            MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Win32Exception When ex.NativeErrorCode = 2
            MessageBox.Show($"Required executable was not found: {Path.GetFileName(fileName)}.{Environment.NewLine}{Environment.NewLine}Please install MySQL client tools or provide the executable in a standard installation path.",
                            "Executable Not Found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show("Operation failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            progressOperation.Visible = False
            btnBackup.Enabled = True
            btnRestore.Enabled = True
        End Try
    End Sub

    Private Shared Function ResolveExecutablePath(executableName As String) As String
        Dim exeName As String = If(executableName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase), executableName, executableName & ".exe")
        Dim candidates As New List(Of String) From {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exeName),
            Path.Combine("C:\xampp\mysql\bin", exeName),
            Path.Combine("C:\Program Files\MySQL\MySQL Server 8.0\bin", exeName),
            Path.Combine("C:\Program Files\MySQL\MySQL Server 8.4\bin", exeName),
            Path.Combine("C:\Program Files\MySQL\MySQL Server 5.7\bin", exeName),
            Path.Combine("C:\Program Files (x86)\MySQL\MySQL Server 8.0\bin", exeName),
            Path.Combine("C:\Program Files (x86)\MySQL\MySQL Server 5.7\bin", exeName)
        }

        AddExecutableCandidatesFromDirectory(candidates, "C:\Program Files\MySQL", "MySQL Server *", "bin", exeName)
        AddExecutableCandidatesFromDirectory(candidates, "C:\Program Files (x86)\MySQL", "MySQL Server *", "bin", exeName)
        AddExecutableCandidatesFromDirectory(candidates, "C:\wamp64\bin\mysql", "mysql*", "bin", exeName)
        AddExecutableCandidatesFromDirectory(candidates, "C:\wamp\bin\mysql", "mysql*", "bin", exeName)

        Dim pathValue As String = Environment.GetEnvironmentVariable("PATH")
        If Not String.IsNullOrWhiteSpace(pathValue) Then
            For Each folder As String In pathValue.Split(";"c)
                If String.IsNullOrWhiteSpace(folder) Then
                    Continue For
                End If

                candidates.Add(Path.Combine(folder.Trim(), exeName))
            Next
        End If

        For Each candidate As String In candidates
            If File.Exists(candidate) Then
                Return candidate
            End If
        Next

        Return Nothing
    End Function

    Private Shared Sub AddExecutableCandidatesFromDirectory(candidates As List(Of String),
                                                            rootPath As String,
                                                            directoryPattern As String,
                                                            binFolder As String,
                                                            exeName As String)
        If Not Directory.Exists(rootPath) Then
            Return
        End If

        For Each dir As String In Directory.GetDirectories(rootPath, directoryPattern)
            candidates.Add(Path.Combine(dir, binFolder, exeName))
        Next
    End Sub

    Private Shared Sub ShowExecutableNotFoundMessage(executableName As String)
        MessageBox.Show($"Could not find {executableName}.{Environment.NewLine}{Environment.NewLine}Please locate or install MySQL client tools (MySQL Server/XAMPP/WAMP), then try again.",
                        "Executable Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
    End Sub

    Private Shared Function ParseConnectionInfo(connectionString As String) As (Host As String, Port As String, User As String, Password As String, Database As String)
        Dim builder As New MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString)
        Return (builder.Server, builder.Port.ToString(), builder.UserID, builder.Password, builder.Database)
    End Function
End Class
