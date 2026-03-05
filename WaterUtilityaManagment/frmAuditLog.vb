Public Class frmAuditLog
    Inherits Form

    Private ReadOnly dtpFrom As New DateTimePicker()
    Private ReadOnly dtpTo As New DateTimePicker()
    Private ReadOnly txtUser As New TextBox()
    Private ReadOnly txtAction As New TextBox()
    Private ReadOnly btnLoad As New Button()
    Private ReadOnly btnExport As New Button()
    Private ReadOnly dgvAudit As New DataGridView()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Audit Log"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 1100
        Me.Height = 650
        Me.MinimumSize = New Size(980, 560)

        Dim lblFrom As New Label() With {.Text = "From", .Left = 20, .Top = 20, .AutoSize = True}
        dtpFrom.Left = 60
        dtpFrom.Top = 15
        dtpFrom.Width = 120
        dtpFrom.Format = DateTimePickerFormat.[Short]
        dtpFrom.Value = Date.Today.AddDays(-30)

        Dim lblTo As New Label() With {.Text = "To", .Left = 200, .Top = 20, .AutoSize = True}
        dtpTo.Left = 225
        dtpTo.Top = 15
        dtpTo.Width = 120
        dtpTo.Format = DateTimePickerFormat.[Short]

        Dim lblUser As New Label() With {.Text = "User", .Left = 360, .Top = 20, .AutoSize = True}
        txtUser.Left = 400
        txtUser.Top = 15
        txtUser.Width = 150

        Dim lblAction As New Label() With {.Text = "Action", .Left = 565, .Top = 20, .AutoSize = True}
        txtAction.Left = 615
        txtAction.Top = 15
        txtAction.Width = 180

        btnLoad.Text = "Load"
        btnLoad.Left = 810
        btnLoad.Top = 14
        btnLoad.Width = 80
        AddHandler btnLoad.Click, AddressOf btnLoad_Click

        btnExport.Text = "Export CSV"
        btnExport.Left = 900
        btnExport.Top = 14
        btnExport.Width = 90
        btnExport.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnExport.Click, AddressOf btnExport_Click

        dgvAudit.Left = 20
        dgvAudit.Top = 50
        dgvAudit.Width = 1040
        dgvAudit.Height = 540
        dgvAudit.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvAudit.AllowUserToAddRows = False
        dgvAudit.AllowUserToDeleteRows = False
        dgvAudit.ReadOnly = True
        dgvAudit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvAudit)
        UiStyleHelper.StyleButton(btnLoad, True)
        UiStyleHelper.StyleButton(btnExport)

        Me.Controls.Add(lblFrom)
        Me.Controls.Add(dtpFrom)
        Me.Controls.Add(lblTo)
        Me.Controls.Add(dtpTo)
        Me.Controls.Add(lblUser)
        Me.Controls.Add(txtUser)
        Me.Controls.Add(lblAction)
        Me.Controls.Add(txtAction)
        Me.Controls.Add(btnLoad)
        Me.Controls.Add(btnExport)
        Me.Controls.Add(dgvAudit)

        AddHandler Me.Load, AddressOf frmAuditLog_Load
    End Sub

    Private Sub frmAuditLog_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can view audit logs.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        LoadAuditLog()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs)
        LoadAuditLog()
    End Sub

    Private Sub LoadAuditLog()
        Try
            If dtpTo.Value.Date < dtpFrom.Value.Date Then
                MessageBox.Show("'To' date cannot be earlier than 'From' date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Const sql As String = "SELECT a.audit_id, a.action_time, u.username, a.action_name, a.details FROM audit_log a LEFT JOIN users u ON u.id = a.user_id WHERE a.action_time >= @from_date AND a.action_time < @to_date_exclusive AND (@username = '' OR u.username LIKE @username_like) AND (@action_name = '' OR a.action_name LIKE @action_like) ORDER BY a.action_time DESC;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@from_date", dtpFrom.Value.Date},
                {"@to_date_exclusive", dtpTo.Value.Date.AddDays(1)},
                {"@username", txtUser.Text.Trim()},
                {"@username_like", "%" & txtUser.Text.Trim() & "%"},
                {"@action_name", txtAction.Text.Trim()},
                {"@action_like", "%" & txtAction.Text.Trim() & "%"}
            }

            dgvAudit.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
        Catch ex As Exception
            MessageBox.Show("Failed to load audit log: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs)
        GridExportHelper.ExportDataGridViewToCsv(dgvAudit, Me)
    End Sub
End Class
