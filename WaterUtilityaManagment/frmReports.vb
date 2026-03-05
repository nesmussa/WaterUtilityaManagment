Imports System.Drawing.Printing

Public Class frmReports
    Inherits Form

    Private ReadOnly tabReports As New TabControl()

    Private ReadOnly dgvBillSummary As New DataGridView()
    Private ReadOnly lblPaidSummary As New Label()
    Private ReadOnly lblUnpaidSummary As New Label()

    Private ReadOnly dtpRevenueStart As New DateTimePicker()
    Private ReadOnly dtpRevenueEnd As New DateTimePicker()
    Private ReadOnly btnLoadRevenue As New Button()
    Private ReadOnly lblRevenueTotal As New Label()
    Private ReadOnly dgvRevenueDaily As New DataGridView()

    Private ReadOnly dgvOutstanding As New DataGridView()

    Private ReadOnly dgvStaffActivity As New DataGridView()
    Private ReadOnly lblTotalCustomers As New Label()
    Private ReadOnly lblTotalUnpaid As New Label()
    Private ReadOnly lblCurrentMonthRevenue As New Label()
    Private ReadOnly btnManageTariffs As New Button()
    Private ReadOnly btnBackupRestore As New Button()
    Private ReadOnly btnAuditLog As New Button()
    Private ReadOnly btnViewBills As New Button()
    Private ReadOnly btnExportExcel As New Button()
    Private ReadOnly btnExportPdf As New Button()
    Private ReadOnly btnManageStaff As New Button()
    Private ReadOnly btnLogout As New Button()

    Private _printRows As List(Of String)
    Private _printIndex As Integer

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Manager Reports"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 1100
        Me.Height = 700
        Me.MinimumSize = New Size(1000, 620)

        tabReports.Left = 10
        tabReports.Top = 125
        tabReports.Width = Me.ClientSize.Width - 20
        tabReports.Height = Me.ClientSize.Height - 135
        tabReports.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

        Dim tabSummary As New TabPage("Paid vs Unpaid Summary")
        Dim tabRevenue As New TabPage("Revenue by Period")
        Dim tabOutstanding As New TabPage("Outstanding Balances")
        Dim tabStaff As New TabPage("Staff Activity")

        SetupSummaryTab(tabSummary)
        SetupRevenueTab(tabRevenue)
        SetupOutstandingTab(tabOutstanding)
        SetupStaffActivityTab(tabStaff)

        tabReports.TabPages.Add(tabSummary)
        tabReports.TabPages.Add(tabRevenue)
        tabReports.TabPages.Add(tabOutstanding)
        tabReports.TabPages.Add(tabStaff)

        lblTotalCustomers.Margin = New Padding(0, 0, 18, 0)
        lblTotalCustomers.AutoSize = True
        lblTotalCustomers.Text = "Customers: 0"

        lblTotalUnpaid.Margin = New Padding(0, 0, 18, 0)
        lblTotalUnpaid.AutoSize = True
        lblTotalUnpaid.Text = "Unpaid Amount: 0.00"

        lblCurrentMonthRevenue.Margin = New Padding(0)
        lblCurrentMonthRevenue.AutoSize = True
        lblCurrentMonthRevenue.Text = "Current Month Revenue: 0.00"

        Dim metricsPanel As New FlowLayoutPanel()
        metricsPanel.Left = 10
        metricsPanel.Top = 10
        metricsPanel.Width = Me.ClientSize.Width - 20
        metricsPanel.Height = 24
        metricsPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        metricsPanel.FlowDirection = FlowDirection.LeftToRight
        metricsPanel.WrapContents = False
        metricsPanel.Controls.Add(lblTotalCustomers)
        metricsPanel.Controls.Add(lblTotalUnpaid)
        metricsPanel.Controls.Add(lblCurrentMonthRevenue)

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Left = 10
        actionPanel.Top = 40
        actionPanel.Width = Me.ClientSize.Width - 20
        actionPanel.Height = 70
        actionPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = True
        actionPanel.AutoScroll = False

        btnManageTariffs.Text = "Manage Tariffs"
        btnManageTariffs.Width = 110
        btnManageTariffs.Height = 30
        AddHandler btnManageTariffs.Click, AddressOf btnManageTariffs_Click

        btnBackupRestore.Text = "Backup / Restore"
        btnBackupRestore.Width = 110
        btnBackupRestore.Height = 30
        AddHandler btnBackupRestore.Click, AddressOf btnBackupRestore_Click

        btnAuditLog.Text = "Audit Log"
        btnAuditLog.Width = 110
        btnAuditLog.Height = 30
        AddHandler btnAuditLog.Click, AddressOf btnAuditLog_Click

        btnViewBills.Text = "View Bills"
        btnViewBills.Width = 110
        btnViewBills.Height = 30
        AddHandler btnViewBills.Click, AddressOf btnViewBills_Click

        btnExportExcel.Text = "Export Excel"
        btnExportExcel.Width = 110
        btnExportExcel.Height = 30
        AddHandler btnExportExcel.Click, AddressOf btnExportExcel_Click

        btnExportPdf.Text = "Export PDF"
        btnExportPdf.Width = 110
        btnExportPdf.Height = 30
        AddHandler btnExportPdf.Click, AddressOf btnExportPdf_Click

        btnManageStaff.Text = "Manage Users"
        btnManageStaff.Width = 110
        btnManageStaff.Height = 30
        AddHandler btnManageStaff.Click, AddressOf btnManageStaff_Click

        btnLogout.Text = "Logout"
        btnLogout.Width = 90
        btnLogout.Height = 30
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        actionPanel.Controls.Add(btnManageTariffs)
        actionPanel.Controls.Add(btnBackupRestore)
        actionPanel.Controls.Add(btnAuditLog)
        actionPanel.Controls.Add(btnViewBills)
        actionPanel.Controls.Add(btnExportExcel)
        actionPanel.Controls.Add(btnExportPdf)
        actionPanel.Controls.Add(btnManageStaff)
        actionPanel.Controls.Add(btnLogout)

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvBillSummary)
        UiStyleHelper.StyleDataGrid(dgvRevenueDaily)
        UiStyleHelper.StyleDataGrid(dgvOutstanding)
        UiStyleHelper.StyleDataGrid(dgvStaffActivity)
        UiStyleHelper.StyleButton(btnManageTariffs)
        UiStyleHelper.StyleButton(btnBackupRestore)
        UiStyleHelper.StyleButton(btnAuditLog)
        UiStyleHelper.StyleButton(btnViewBills)
        UiStyleHelper.StyleButton(btnExportExcel, True)
        UiStyleHelper.StyleButton(btnExportPdf, True)
        UiStyleHelper.StyleButton(btnManageStaff)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(metricsPanel)
        Me.Controls.Add(tabReports)
        Me.Controls.Add(actionPanel)

        AddHandler Me.Load, AddressOf frmReports_Load
    End Sub

    Private Sub SetupSummaryTab(tab As TabPage)
        lblPaidSummary.Left = 20
        lblPaidSummary.Top = 20
        lblPaidSummary.AutoSize = True

        lblUnpaidSummary.Left = 20
        lblUnpaidSummary.Top = 50
        lblUnpaidSummary.AutoSize = True

        dgvBillSummary.Left = 20
        dgvBillSummary.Top = 90
        dgvBillSummary.Width = 600
        dgvBillSummary.Height = 180
        dgvBillSummary.AllowUserToAddRows = False
        dgvBillSummary.AllowUserToDeleteRows = False
        dgvBillSummary.ReadOnly = True
        dgvBillSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        tab.Controls.Add(lblPaidSummary)
        tab.Controls.Add(lblUnpaidSummary)
        tab.Controls.Add(dgvBillSummary)
    End Sub

    Private Sub SetupRevenueTab(tab As TabPage)
        Dim lblStart As New Label() With {.Text = "Start Date", .Left = 20, .Top = 20, .AutoSize = True}
        dtpRevenueStart.Left = 90
        dtpRevenueStart.Top = 15
        dtpRevenueStart.Width = 130
        dtpRevenueStart.Format = DateTimePickerFormat.[Short]
        dtpRevenueStart.Value = Date.Today.AddDays(-30)

        Dim lblEnd As New Label() With {.Text = "End Date", .Left = 250, .Top = 20, .AutoSize = True}
        dtpRevenueEnd.Left = 315
        dtpRevenueEnd.Top = 15
        dtpRevenueEnd.Width = 130
        dtpRevenueEnd.Format = DateTimePickerFormat.[Short]
        dtpRevenueEnd.Value = Date.Today

        btnLoadRevenue.Text = "Load"
        btnLoadRevenue.Left = 470
        btnLoadRevenue.Top = 15
        btnLoadRevenue.Width = 80
        AddHandler btnLoadRevenue.Click, AddressOf btnLoadRevenue_Click

        lblRevenueTotal.Left = 20
        lblRevenueTotal.Top = 55
        lblRevenueTotal.AutoSize = True
        lblRevenueTotal.Text = "Total Collected: 0.00"

        dgvRevenueDaily.Left = 20
        dgvRevenueDaily.Top = 90
        dgvRevenueDaily.Width = 1020
        dgvRevenueDaily.Height = 500
        dgvRevenueDaily.AllowUserToAddRows = False
        dgvRevenueDaily.AllowUserToDeleteRows = False
        dgvRevenueDaily.ReadOnly = True
        dgvRevenueDaily.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        tab.Controls.Add(lblStart)
        tab.Controls.Add(dtpRevenueStart)
        tab.Controls.Add(lblEnd)
        tab.Controls.Add(dtpRevenueEnd)
        tab.Controls.Add(btnLoadRevenue)
        tab.Controls.Add(lblRevenueTotal)
        tab.Controls.Add(dgvRevenueDaily)
    End Sub

    Private Sub SetupOutstandingTab(tab As TabPage)
        dgvOutstanding.Dock = DockStyle.Fill
        dgvOutstanding.AllowUserToAddRows = False
        dgvOutstanding.AllowUserToDeleteRows = False
        dgvOutstanding.ReadOnly = True
        dgvOutstanding.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        tab.Controls.Add(dgvOutstanding)
    End Sub

    Private Sub SetupStaffActivityTab(tab As TabPage)
        dgvStaffActivity.Dock = DockStyle.Fill
        dgvStaffActivity.AllowUserToAddRows = False
        dgvStaffActivity.AllowUserToDeleteRows = False
        dgvStaffActivity.ReadOnly = True
        dgvStaffActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        tab.Controls.Add(dgvStaffActivity)
    End Sub

    Private Sub frmReports_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can view reports.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        LoadPaidVsUnpaidSummary()
        LoadRevenueByPeriod()
        LoadOutstandingBalances()
        LoadStaffActivity()
        LoadDashboardMetrics()
    End Sub

    Private Sub LoadDashboardMetrics()
        Try
            Const customerSql As String = "SELECT COUNT(*) FROM customers;"
            Const unpaidSql As String = "SELECT COALESCE(SUM(b.total_amount - COALESCE(pa.paid_total, 0)), 0) FROM bills b LEFT JOIN (SELECT bill_id, SUM(amount_applied) AS paid_total FROM payment_allocations GROUP BY bill_id) pa ON pa.bill_id = b.id WHERE b.status IN ('Unpaid', 'Partial');"
            Const monthRevenueSql As String = "SELECT COALESCE(SUM(amount_paid), 0) FROM payments WHERE YEAR(payment_date) = YEAR(CURDATE()) AND MONTH(payment_date) = MONTH(CURDATE());"

            Dim totalCustomers As Integer = Convert.ToInt32(DatabaseHelper.ExecuteScalar(customerSql))
            Dim totalUnpaid As Decimal = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(unpaidSql))
            Dim currentMonthRevenue As Decimal = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(monthRevenueSql))

            lblTotalCustomers.Text = $"Customers: {totalCustomers}"
            lblTotalUnpaid.Text = $"Unpaid Amount: {totalUnpaid:N2}"
            lblCurrentMonthRevenue.Text = $"Current Month Revenue: {currentMonthRevenue:N2}"
        Catch ex As Exception
            MessageBox.Show("Failed to load dashboard metrics: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadPaidVsUnpaidSummary()
        Try
            Const sql As String = "SELECT COALESCE(SUM(CASE WHEN status = 'Paid' THEN total_amount ELSE 0 END), 0) AS paid_total, COALESCE(SUM(CASE WHEN status <> 'Paid' THEN total_amount ELSE 0 END), 0) AS unpaid_total, COALESCE(SUM(CASE WHEN status = 'Paid' THEN 1 ELSE 0 END), 0) AS paid_count, COALESCE(SUM(CASE WHEN status <> 'Paid' THEN 1 ELSE 0 END), 0) AS unpaid_count FROM bills;"
            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql)
            If dt.Rows.Count = 0 Then
                Return
            End If

            Dim row As DataRow = dt.Rows(0)
            Dim paidTotal As Decimal = Convert.ToDecimal(row("paid_total"))
            Dim unpaidTotal As Decimal = Convert.ToDecimal(row("unpaid_total"))
            Dim paidCount As Integer = Convert.ToInt32(row("paid_count"))
            Dim unpaidCount As Integer = Convert.ToInt32(row("unpaid_count"))

            lblPaidSummary.Text = $"Paid Bills: {paidCount} | Amount: {paidTotal:N2}"
            lblUnpaidSummary.Text = $"Unpaid/Partial Bills: {unpaidCount} | Amount: {unpaidTotal:N2}"

            Dim summaryTable As New DataTable()
            summaryTable.Columns.Add("Status")
            summaryTable.Columns.Add("Count")
            summaryTable.Columns.Add("Total Amount")

            summaryTable.Rows.Add("Paid", paidCount, paidTotal.ToString("N2"))
            summaryTable.Rows.Add("Unpaid", unpaidCount, unpaidTotal.ToString("N2"))

            dgvBillSummary.DataSource = summaryTable
        Catch ex As Exception
            MessageBox.Show("Failed to load paid vs unpaid summary: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnLoadRevenue_Click(sender As Object, e As EventArgs)
        LoadRevenueByPeriod()
    End Sub

    Private Sub btnManageStaff_Click(sender As Object, e As EventArgs)
        Using frm As New frmUserManagement()
            frm.ShowDialog(Me)
        End Using
        LoadStaffActivity()
    End Sub

    Private Sub btnManageTariffs_Click(sender As Object, e As EventArgs)
        Using frm As New frmTariffs()
            frm.ShowDialog(Me)
        End Using
        LoadDashboardMetrics()
    End Sub

    Private Sub btnBackupRestore_Click(sender As Object, e As EventArgs)
        Using frm As New frmBackupRestore()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnAuditLog_Click(sender As Object, e As EventArgs)
        Using frm As New frmAuditLog()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnViewBills_Click(sender As Object, e As EventArgs)
        Using frm As New frmBills()
            frm.ShowDialog(Me)
        End Using
        LoadDashboardMetrics()
        LoadOutstandingBalances()
    End Sub

    Private Sub btnExportExcel_Click(sender As Object, e As EventArgs)
        Dim activeGrid As DataGridView = GetActiveReportGrid()
        If activeGrid Is Nothing Then
            MessageBox.Show("No report table available to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        GridExportHelper.ExportDataGridViewToCsv(activeGrid, Me)
    End Sub

    Private Sub btnExportPdf_Click(sender As Object, e As EventArgs)
        Dim activeGrid As DataGridView = GetActiveReportGrid()
        If activeGrid Is Nothing Then
            MessageBox.Show("No report table available to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        PreparePrintRows(activeGrid)
        If _printRows.Count = 0 Then
            MessageBox.Show("No rows to print.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim printDoc As New PrintDocument()
        AddHandler printDoc.PrintPage, AddressOf printDoc_PrintPage

        Using pd As New PrintDialog()
            pd.Document = printDoc
            pd.UseEXDialog = True
            If pd.ShowDialog(Me) = DialogResult.OK Then
                printDoc.Print()
            End If
        End Using
    End Sub

    Private Function GetActiveReportGrid() As DataGridView
        Select Case tabReports.SelectedIndex
            Case 0
                Return dgvBillSummary
            Case 1
                Return dgvRevenueDaily
            Case 2
                Return dgvOutstanding
            Case 3
                Return dgvStaffActivity
            Case Else
                Return Nothing
        End Select
    End Function

    Private Sub PreparePrintRows(grid As DataGridView)
        _printRows = New List(Of String)()
        _printIndex = 0

        Dim header As New List(Of String)()
        For Each col As DataGridViewColumn In grid.Columns
            If col.Visible Then
                header.Add(col.HeaderText)
            End If
        Next
        _printRows.Add(String.Join(" | ", header))

        For Each row As DataGridViewRow In grid.Rows
            If row.IsNewRow Then
                Continue For
            End If

            Dim values As New List(Of String)()
            For Each col As DataGridViewColumn In grid.Columns
                If col.Visible Then
                    values.Add(If(row.Cells(col.Index).Value?.ToString(), String.Empty))
                End If
            Next

            _printRows.Add(String.Join(" | ", values))
        Next
    End Sub

    Private Sub printDoc_PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim y As Single = e.MarginBounds.Top
        Dim lineHeight As Single = e.Graphics.MeasureString("X", Me.Font).Height + 4

        While _printIndex < _printRows.Count
            If y + lineHeight > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                Return
            End If

            e.Graphics.DrawString(_printRows(_printIndex), Me.Font, Brushes.Black, e.MarginBounds.Left, y)
            y += lineHeight
            _printIndex += 1
        End While

        e.HasMorePages = False
    End Sub

    Private Sub LoadRevenueByPeriod()
        Try
            Dim startDate As Date = dtpRevenueStart.Value.Date
            Dim endDate As Date = dtpRevenueEnd.Value.Date

            If endDate < startDate Then
                MessageBox.Show("End date cannot be earlier than start date.",
                                "Validation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            End If

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@start_date", startDate},
                {"@end_date_exclusive", endDate.AddDays(1)}
            }

            Const totalSql As String = "SELECT COALESCE(SUM(amount_paid), 0) AS total_collected FROM payments WHERE payment_date >= @start_date AND payment_date < @end_date_exclusive;"
            Dim totalObj As Object = DatabaseHelper.ExecuteScalar(totalSql, parameters)
            Dim totalCollected As Decimal = If(totalObj Is Nothing OrElse totalObj Is DBNull.Value, 0D, Convert.ToDecimal(totalObj))
            lblRevenueTotal.Text = $"Total Collected: {totalCollected:N2}"

            Const dailySql As String = "SELECT DATE(payment_date) AS payment_day, COALESCE(SUM(amount_paid), 0) AS total_amount, COUNT(*) AS payment_count FROM payments WHERE payment_date >= @start_date AND payment_date < @end_date_exclusive GROUP BY DATE(payment_date) ORDER BY payment_day;"
            dgvRevenueDaily.DataSource = DatabaseHelper.ExecuteDataTable(dailySql, parameters)
        Catch ex As Exception
            MessageBox.Show("Failed to load revenue report: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadOutstandingBalances()
        Try
            Const sql As String = "SELECT u.full_name, c.meter_number, u.phone, COALESCE(SUM(CASE WHEN b.status <> 'Paid' THEN b.total_amount ELSE 0 END), 0) AS total_due FROM customers c INNER JOIN users u ON c.id = u.id LEFT JOIN bills b ON b.customer_id = c.id GROUP BY u.full_name, c.meter_number, u.phone HAVING total_due > 0 ORDER BY total_due DESC;"
            dgvOutstanding.DataSource = DatabaseHelper.ExecuteDataTable(sql)
        Catch ex As Exception
            MessageBox.Show("Failed to load outstanding balances: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadStaffActivity()
        Try
            Const sql As String = "SELECT u.full_name AS staff_name, COALESCE(r.readings_count, 0) AS readings_entered, r.last_reading_date, COALESCE(p.payments_count, 0) AS payments_recorded, COALESCE(p.total_collected, 0) AS total_collected, p.last_payment_date FROM users u LEFT JOIN (SELECT entered_by AS staff_id, COUNT(*) AS readings_count, MAX(reading_date) AS last_reading_date FROM meter_readings GROUP BY entered_by) r ON r.staff_id = u.id LEFT JOIN (SELECT received_by AS staff_id, COUNT(*) AS payments_count, COALESCE(SUM(amount_paid), 0) AS total_collected, MAX(payment_date) AS last_payment_date FROM payments GROUP BY received_by) p ON p.staff_id = u.id WHERE LOWER(u.role) = 'staff' ORDER BY u.full_name;"
            dgvStaffActivity.DataSource = DatabaseHelper.ExecuteDataTable(sql)
        Catch ex As Exception
            MessageBox.Show("Failed to load staff activity report: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
