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
    Private ReadOnly btnManageStaff As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Manager Reports"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 1100
        Me.Height = 700

        tabReports.Dock = DockStyle.Fill

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

        btnManageStaff.Text = "Manage Staff"
        btnManageStaff.Width = 120
        btnManageStaff.Height = 30
        btnManageStaff.Left = Me.ClientSize.Width - 250
        btnManageStaff.Top = 10
        btnManageStaff.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnManageStaff.Click, AddressOf btnManageStaff_Click

        btnLogout.Text = "Logout"
        btnLogout.Width = 100
        btnLogout.Height = 30
        btnLogout.Left = Me.ClientSize.Width - 120
        btnLogout.Top = 10
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Me.Controls.Add(tabReports)
        Me.Controls.Add(btnManageStaff)
        Me.Controls.Add(btnLogout)

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
