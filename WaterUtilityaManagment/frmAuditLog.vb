Public Class frmAuditLog
    Inherits Form

    Private ReadOnly pnlFilter As New Panel()
    Private ReadOnly dtpFrom As New DateTimePicker()
    Private ReadOnly dtpTo As New DateTimePicker()
    Private ReadOnly txtUser As New TextBox()
    Private ReadOnly cboAction As New ComboBox()
    Private ReadOnly btnLoad As New Button()
    Private ReadOnly btnExport As New Button()
    Private ReadOnly dgvAudit As New DataGridView()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Audit Log"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(1200, 800)
        Me.MinimumSize = New Size(1000, 700)
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.ShowInTaskbar = True
        Me.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        pnlFilter.Dock = DockStyle.Top
        pnlFilter.Height = 78
        pnlFilter.BackColor = Color.White
        pnlFilter.Padding = New Padding(16, 12, 16, 10)

        Dim lblFrom As New Label() With {.Text = "From", .Left = 16, .Top = 10, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        dtpFrom.Left = 16
        dtpFrom.Top = 30
        dtpFrom.Width = 130
        dtpFrom.Format = DateTimePickerFormat.[Short]
        dtpFrom.Value = Date.Today.AddDays(-30)

        Dim lblTo As New Label() With {.Text = "To", .Left = 160, .Top = 10, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        dtpTo.Left = 160
        dtpTo.Top = 30
        dtpTo.Width = 130
        dtpTo.Format = DateTimePickerFormat.[Short]

        Dim lblUser As New Label() With {.Text = "User", .Left = 304, .Top = 10, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtUser.Left = 304
        txtUser.Top = 30
        txtUser.Width = 180
        txtUser.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)

        Dim lblAction As New Label() With {.Text = "Action", .Left = 498, .Top = 10, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        cboAction.Left = 498
        cboAction.Top = 30
        cboAction.Width = 180
        cboAction.DropDownStyle = ComboBoxStyle.DropDown
        cboAction.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cboAction.AutoCompleteSource = AutoCompleteSource.ListItems

        btnLoad.Text = "Load"
        btnLoad.Left = 694
        btnLoad.Top = 29
        btnLoad.Width = 82
        btnLoad.Height = 32
        btnLoad.FlatStyle = FlatStyle.Flat
        btnLoad.FlatAppearance.BorderSize = 0
        btnLoad.BackColor = ColorTranslator.FromHtml("#3498db")
        btnLoad.ForeColor = Color.White
        btnLoad.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLoad.Click, AddressOf btnLoad_Click

        btnExport.Text = "Export CSV"
        btnExport.Left = 786
        btnExport.Top = 29
        btnExport.Width = 100
        btnExport.Height = 32
        btnExport.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnExport.FlatStyle = FlatStyle.Flat
        btnExport.FlatAppearance.BorderSize = 0
        btnExport.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnExport.ForeColor = Color.White
        btnExport.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnExport.Click, AddressOf btnExport_Click

        dgvAudit.Dock = DockStyle.Fill
        dgvAudit.AllowUserToAddRows = False
        dgvAudit.AllowUserToDeleteRows = False
        dgvAudit.AllowUserToResizeRows = False
        dgvAudit.ReadOnly = True
        dgvAudit.RowHeadersVisible = False
        dgvAudit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvAudit.BackgroundColor = Color.White
        dgvAudit.BorderStyle = BorderStyle.None
        dgvAudit.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvAudit.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        AddHandler dgvAudit.CellDoubleClick, AddressOf dgvAudit_CellDoubleClick

        pnlFilter.Controls.Add(lblFrom)
        pnlFilter.Controls.Add(dtpFrom)
        pnlFilter.Controls.Add(lblTo)
        pnlFilter.Controls.Add(dtpTo)
        pnlFilter.Controls.Add(lblUser)
        pnlFilter.Controls.Add(txtUser)
        pnlFilter.Controls.Add(lblAction)
        pnlFilter.Controls.Add(cboAction)
        pnlFilter.Controls.Add(btnLoad)
        pnlFilter.Controls.Add(btnExport)

        Dim gridHost As New Panel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16, 12, 16, 10),
            .BackColor = ColorTranslator.FromHtml("#ecf0f1")
        }
        gridHost.Controls.Add(dgvAudit)

        Me.Controls.Add(gridHost)
        Me.Controls.Add(pnlFilter)

        AddHandler Me.Load, AddressOf frmAuditLog_Load
        AddHandler Me.Resize, AddressOf frmAuditLog_Resize
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

        frmAuditLog_Resize(Me, EventArgs.Empty)
        LoadFilterData()
        UiStyleHelper.AddDialogCloseButton(Me)
        LoadAuditLog()
    End Sub

    Private Sub frmAuditLog_Resize(sender As Object, e As EventArgs)
        btnExport.Left = pnlFilter.ClientSize.Width - btnExport.Width - 16
        btnLoad.Left = btnExport.Left - btnLoad.Width - 8
    End Sub

    Private Sub LoadFilterData()
        Try
            Dim userSource As New AutoCompleteStringCollection()
            Dim users As DataTable = DatabaseHelper.ExecuteDataTable("SELECT DISTINCT username FROM users WHERE username IS NOT NULL ORDER BY username;")
            For Each row As DataRow In users.Rows
                userSource.Add(Convert.ToString(row("username")))
            Next
            txtUser.AutoCompleteMode = AutoCompleteMode.SuggestAppend
            txtUser.AutoCompleteSource = AutoCompleteSource.CustomSource
            txtUser.AutoCompleteCustomSource = userSource

            cboAction.Items.Clear()
            cboAction.Items.Add(String.Empty)
            Dim actions As DataTable = DatabaseHelper.ExecuteDataTable("SELECT DISTINCT action_name FROM audit_log WHERE action_name IS NOT NULL AND action_name <> '' ORDER BY action_name;")
            For Each row As DataRow In actions.Rows
                cboAction.Items.Add(Convert.ToString(row("action_name")))
            Next
            cboAction.SelectedIndex = 0
        Catch
        End Try
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

            Const sql As String = "SELECT a.audit_id, a.user_id, u.username, a.action_name, a.action, a.table_name, a.record_id, a.details, a.ip_address, a.action_time, a.action_date FROM audit_log a LEFT JOIN users u ON u.id = a.user_id WHERE a.action_time >= @from_date AND a.action_time < @to_date_exclusive AND (@username = '' OR u.username LIKE @username_like) AND (@action_name = '' OR a.action_name LIKE @action_like) ORDER BY a.action_time DESC;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@from_date", dtpFrom.Value.Date},
                {"@to_date_exclusive", dtpTo.Value.Date.AddDays(1)},
                {"@username", txtUser.Text.Trim()},
                {"@username_like", "%" & txtUser.Text.Trim() & "%"},
                {"@action_name", cboAction.Text.Trim()},
                {"@action_like", "%" & cboAction.Text.Trim() & "%"}
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

    Private Sub dgvAudit_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        Dim row As DataGridViewRow = dgvAudit.Rows(e.RowIndex)
        Dim formatted As String =
$"<Log Entry #{row.Cells("audit_id").Value}>{Environment.NewLine}" &
$"------------------------------------------------{Environment.NewLine}" &
$"User: {row.Cells("username").Value}{Environment.NewLine}" &
$"Action: {row.Cells("action_name").Value}{Environment.NewLine}" &
$"Table: {row.Cells("table_name").Value}{Environment.NewLine}" &
$"Record ID: {row.Cells("record_id").Value}{Environment.NewLine}" &
$"IP: {row.Cells("ip_address").Value}{Environment.NewLine}" &
$"Time: {row.Cells("action_time").Value}{Environment.NewLine}{Environment.NewLine}" &
$"Details:{Environment.NewLine}<p>{row.Cells("details").Value}</p>"

        MessageBox.Show(formatted, "Audit Details", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
