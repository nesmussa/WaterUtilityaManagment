Public Class frmReports
    Inherits Form

    Private ReadOnly tabReports As New TabControl()
    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly lblWelcome As New Label()
    Private ReadOnly pnlStatCustomers As New Panel()
    Private ReadOnly pnlStatUnpaid As New Panel()
    Private ReadOnly pnlStatRevenue As New Panel()
    Private ReadOnly lblStatCustomers As New Label()
    Private ReadOnly lblStatUnpaid As New Label()
    Private ReadOnly lblStatRevenue As New Label()
    Private ReadOnly chartSummary As New Panel()
    Private ReadOnly pnlChartHost As New Panel()
    Private ReadOnly pnlPaidCard As New Panel()
    Private ReadOnly pnlUnpaidCard As New Panel()
    Private ReadOnly lblPaidCardTitle As New Label()
    Private ReadOnly lblPaidCardValue As New Label()
    Private ReadOnly lblUnpaidCardTitle As New Label()
    Private ReadOnly lblUnpaidCardValue As New Label()

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
    Private ReadOnly btnProfile As New Button()
    Private ReadOnly btnLogout As New Button()

    Private _paidTotalSummary As Decimal
    Private _unpaidTotalSummary As Decimal
    Private _partialTotalSummary As Decimal

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Manager Reports"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        pnlTopBar.Dock = DockStyle.Top
        pnlTopBar.Height = 110
        pnlTopBar.BackColor = Color.White
        pnlTopBar.Padding = New Padding(16, 10, 16, 10)

        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
        lblWelcome.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblWelcome.Left = 16
        lblWelcome.Top = 12

        btnLogout.Text = "Logout"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 20
        btnLogout.Top = 10
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        btnProfile.Text = "My Profile"
        btnProfile.Width = 110
        btnProfile.Height = 34
        btnProfile.Left = btnLogout.Left - btnProfile.Width - 8
        btnProfile.Top = 10
        btnProfile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnProfile.FlatStyle = FlatStyle.Flat
        btnProfile.FlatAppearance.BorderSize = 0
        btnProfile.BackColor = ColorTranslator.FromHtml("#3498db")
        btnProfile.ForeColor = Color.White
        btnProfile.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnProfile.Click, AddressOf btnProfile_Click

        pnlStatCustomers.BackColor = ColorTranslator.FromHtml("#3498db")
        pnlStatCustomers.Width = 280
        pnlStatCustomers.Height = 54
        pnlStatCustomers.Left = 16
        pnlStatCustomers.Top = 46
        lblStatCustomers.ForeColor = Color.White
        lblStatCustomers.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblStatCustomers.AutoSize = True
        lblStatCustomers.Left = 12
        lblStatCustomers.Top = 16
        lblStatCustomers.Text = "👥 Total Customers: 0"
        pnlStatCustomers.Controls.Add(lblStatCustomers)

        pnlStatUnpaid.BackColor = ColorTranslator.FromHtml("#e67e22")
        pnlStatUnpaid.Width = 300
        pnlStatUnpaid.Height = 54
        pnlStatUnpaid.Left = 308
        pnlStatUnpaid.Top = 46
        lblStatUnpaid.ForeColor = Color.White
        lblStatUnpaid.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblStatUnpaid.AutoSize = True
        lblStatUnpaid.Left = 12
        lblStatUnpaid.Top = 16
        lblStatUnpaid.Text = "💸 Unpaid Amount: 0.00"
        pnlStatUnpaid.Controls.Add(lblStatUnpaid)

        pnlStatRevenue.BackColor = ColorTranslator.FromHtml("#27ae60")
        pnlStatRevenue.Width = 320
        pnlStatRevenue.Height = 54
        pnlStatRevenue.Left = 620
        pnlStatRevenue.Top = 46
        lblStatRevenue.ForeColor = Color.White
        lblStatRevenue.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblStatRevenue.AutoSize = True
        lblStatRevenue.Left = 12
        lblStatRevenue.Top = 16
        lblStatRevenue.Text = "📈 Current Month Revenue: 0.00"
        pnlStatRevenue.Controls.Add(lblStatRevenue)

        pnlTopBar.Controls.Add(lblWelcome)
        pnlTopBar.Controls.Add(btnProfile)
        pnlTopBar.Controls.Add(btnLogout)
        pnlTopBar.Controls.Add(pnlStatCustomers)
        pnlTopBar.Controls.Add(pnlStatUnpaid)
        pnlTopBar.Controls.Add(pnlStatRevenue)

        tabReports.Dock = DockStyle.Fill
        tabReports.Appearance = TabAppearance.FlatButtons
        tabReports.DrawMode = TabDrawMode.OwnerDrawFixed
        tabReports.ItemSize = New Size(160, 36)
        AddHandler tabReports.DrawItem, AddressOf tabReports_DrawItem

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

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Dock = DockStyle.Bottom
        actionPanel.Height = 86
        actionPanel.Padding = New Padding(16, 10, 16, 10)
        actionPanel.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = True
        actionPanel.AutoScroll = True

        btnManageTariffs.Text = "⚙ Manage Tariffs"
        btnManageTariffs.Width = 180
        btnManageTariffs.Height = 58
        AddHandler btnManageTariffs.Click, AddressOf btnManageTariffs_Click

        btnBackupRestore.Text = "💾 Backup / Restore"
        btnBackupRestore.Width = 180
        btnBackupRestore.Height = 58
        AddHandler btnBackupRestore.Click, AddressOf btnBackupRestore_Click

        btnAuditLog.Text = "🧾 Audit Log"
        btnAuditLog.Width = 180
        btnAuditLog.Height = 58
        AddHandler btnAuditLog.Click, AddressOf btnAuditLog_Click

        btnManageStaff.Text = "👤 Manage User"
        btnManageStaff.Width = 180
        btnManageStaff.Height = 58
        AddHandler btnManageStaff.Click, AddressOf btnManageStaff_Click

        btnExportExcel.Text = "📊 Export Excel"
        btnExportExcel.Width = 180
        btnExportExcel.Height = 58
        AddHandler btnExportExcel.Click, AddressOf btnExportExcel_Click

        btnExportPdf.Text = "📄 Export PDF"
        btnExportPdf.Width = 180
        btnExportPdf.Height = 58
        AddHandler btnExportPdf.Click, AddressOf btnExportPdf_Click

        actionPanel.Controls.Add(btnManageTariffs)
        actionPanel.Controls.Add(btnBackupRestore)
        actionPanel.Controls.Add(btnAuditLog)
        actionPanel.Controls.Add(btnManageStaff)
        actionPanel.Controls.Add(btnExportExcel)
        actionPanel.Controls.Add(btnExportPdf)

        StyleActionTile(btnManageTariffs, ColorTranslator.FromHtml("#34495e"))
        StyleActionTile(btnBackupRestore, ColorTranslator.FromHtml("#34495e"))
        StyleActionTile(btnAuditLog, ColorTranslator.FromHtml("#34495e"))
        StyleActionTile(btnManageStaff, ColorTranslator.FromHtml("#34495e"))
        StyleActionTile(btnExportExcel, ColorTranslator.FromHtml("#2d89ef"))
        StyleActionTile(btnExportPdf, ColorTranslator.FromHtml("#8e44ad"))

        StyleGrid(dgvBillSummary)
        StyleGrid(dgvRevenueDaily)
        StyleGrid(dgvOutstanding)
        StyleGrid(dgvStaffActivity)

        Me.Controls.Add(tabReports)
        Me.Controls.Add(actionPanel)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmReports_Load
        AddHandler Me.Resize, AddressOf frmReports_Resize
    End Sub

    Private Sub SetupSummaryTab(tab As TabPage)
        tab.BackColor = ColorTranslator.FromHtml("#ecf0f1")

        pnlChartHost.Left = 16
        pnlChartHost.Top = 16
        pnlChartHost.Width = 420
        pnlChartHost.Height = 320
        pnlChartHost.BackColor = Color.White

        chartSummary.Left = 10
        chartSummary.Top = 10
        chartSummary.Width = 400
        chartSummary.Height = 300
        chartSummary.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        chartSummary.BackColor = Color.White
        AddHandler chartSummary.Paint, AddressOf chartSummary_Paint
        pnlChartHost.Controls.Add(chartSummary)

        dgvBillSummary.Left = 450
        dgvBillSummary.Top = 16
        dgvBillSummary.Width = 400
        dgvBillSummary.Height = 300
        dgvBillSummary.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvBillSummary.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvBillSummary.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")

        pnlPaidCard.Left = 16
        pnlPaidCard.Top = 350
        pnlPaidCard.Width = 200
        pnlPaidCard.Height = 72
        pnlPaidCard.BackColor = ColorTranslator.FromHtml("#27ae60")
        lblPaidCardTitle.Text = "Paid"
        lblPaidCardTitle.Left = 12
        lblPaidCardTitle.Top = 10
        lblPaidCardTitle.ForeColor = Color.White
        lblPaidCardTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblPaidCardValue.Left = 12
        lblPaidCardValue.Top = 34
        lblPaidCardValue.ForeColor = Color.White
        lblPaidCardValue.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        pnlPaidCard.Controls.Add(lblPaidCardTitle)
        pnlPaidCard.Controls.Add(lblPaidCardValue)

        pnlUnpaidCard.Left = 226
        pnlUnpaidCard.Top = 350
        pnlUnpaidCard.Width = 210
        pnlUnpaidCard.Height = 72
        pnlUnpaidCard.BackColor = ColorTranslator.FromHtml("#e74c3c")
        lblUnpaidCardTitle.Text = "Unpaid"
        lblUnpaidCardTitle.Left = 12
        lblUnpaidCardTitle.Top = 10
        lblUnpaidCardTitle.ForeColor = Color.White
        lblUnpaidCardTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblUnpaidCardValue.Left = 12
        lblUnpaidCardValue.Top = 34
        lblUnpaidCardValue.ForeColor = Color.White
        lblUnpaidCardValue.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        pnlUnpaidCard.Controls.Add(lblUnpaidCardTitle)
        pnlUnpaidCard.Controls.Add(lblUnpaidCardValue)

        tab.Controls.Add(pnlChartHost)
        tab.Controls.Add(dgvBillSummary)
        tab.Controls.Add(pnlPaidCard)
        tab.Controls.Add(pnlUnpaidCard)
    End Sub

    Private Sub SetupRevenueTab(tab As TabPage)
        Dim topPanel As New Panel() With {.Dock = DockStyle.Top, .Height = 74}
        Dim lblStart As New Label() With {.Text = "📅 Start Date", .Left = 16, .Top = 14, .AutoSize = True}
        dtpRevenueStart.Left = 16
        dtpRevenueStart.Top = 34
        dtpRevenueStart.Width = 150
        dtpRevenueStart.Format = DateTimePickerFormat.[Short]
        dtpRevenueStart.Value = Date.Today.AddDays(-30)

        Dim lblEnd As New Label() With {.Text = "📅 End Date", .Left = 186, .Top = 14, .AutoSize = True}
        dtpRevenueEnd.Left = 186
        dtpRevenueEnd.Top = 34
        dtpRevenueEnd.Width = 150
        dtpRevenueEnd.Format = DateTimePickerFormat.[Short]
        dtpRevenueEnd.Value = Date.Today

        btnLoadRevenue.Text = "Load"
        btnLoadRevenue.Left = 356
        btnLoadRevenue.Top = 32
        btnLoadRevenue.Width = 92
        btnLoadRevenue.Height = 30
        btnLoadRevenue.FlatStyle = FlatStyle.Flat
        btnLoadRevenue.FlatAppearance.BorderSize = 0
        btnLoadRevenue.BackColor = ColorTranslator.FromHtml("#3498db")
        btnLoadRevenue.ForeColor = Color.White
        AddHandler btnLoadRevenue.Click, AddressOf btnLoadRevenue_Click

        lblRevenueTotal.Left = 472
        lblRevenueTotal.Top = 37
        lblRevenueTotal.AutoSize = True
        lblRevenueTotal.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblRevenueTotal.Text = "Total Collected: 0.00"

        topPanel.Controls.Add(lblStart)
        topPanel.Controls.Add(dtpRevenueStart)
        topPanel.Controls.Add(lblEnd)
        topPanel.Controls.Add(dtpRevenueEnd)
        topPanel.Controls.Add(btnLoadRevenue)
        topPanel.Controls.Add(lblRevenueTotal)

        dgvRevenueDaily.Dock = DockStyle.Fill
        tab.Controls.Add(dgvRevenueDaily)
        tab.Controls.Add(topPanel)
    End Sub

    Private Sub SetupOutstandingTab(tab As TabPage)
        dgvOutstanding.Dock = DockStyle.Fill
        AddHandler dgvOutstanding.CellFormatting, AddressOf dgvOutstanding_CellFormatting
        tab.Controls.Add(dgvOutstanding)
    End Sub

    Private Sub SetupStaffActivityTab(tab As TabPage)
        dgvStaffActivity.Dock = DockStyle.Fill
        AddHandler dgvStaffActivity.CellPainting, AddressOf dgvStaffActivity_CellPainting
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

        lblWelcome.Text = GetWelcomeText()
        frmReports_Resize(Me, EventArgs.Empty)

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

            lblStatCustomers.Text = $"👥 Total Customers: {totalCustomers}"
            lblStatUnpaid.Text = $"💸 Unpaid Amount: {totalUnpaid:N2}"
            lblStatRevenue.Text = $"📈 Current Month Revenue: {currentMonthRevenue:N2}"
        Catch ex As Exception
            MessageBox.Show("Failed to load dashboard metrics: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadPaidVsUnpaidSummary()
        Try
            Const sql As String = "SELECT COALESCE(SUM(CASE WHEN status = 'Paid' THEN total_amount ELSE 0 END), 0) AS paid_total, COALESCE(SUM(CASE WHEN status = 'Unpaid' THEN total_amount ELSE 0 END), 0) AS unpaid_total, COALESCE(SUM(CASE WHEN status = 'Partial' THEN total_amount ELSE 0 END), 0) AS partial_total, COALESCE(SUM(CASE WHEN status = 'Paid' THEN 1 ELSE 0 END), 0) AS paid_count, COALESCE(SUM(CASE WHEN status = 'Unpaid' THEN 1 ELSE 0 END), 0) AS unpaid_count, COALESCE(SUM(CASE WHEN status = 'Partial' THEN 1 ELSE 0 END), 0) AS partial_count FROM bills;"
            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql)
            If dt.Rows.Count = 0 Then
                Return
            End If

            Dim row As DataRow = dt.Rows(0)
            Dim paidTotal As Decimal = Convert.ToDecimal(row("paid_total"))
            Dim unpaidTotal As Decimal = Convert.ToDecimal(row("unpaid_total"))
            Dim partialTotal As Decimal = Convert.ToDecimal(row("partial_total"))
            Dim paidCount As Integer = Convert.ToInt32(row("paid_count"))
            Dim unpaidCount As Integer = Convert.ToInt32(row("unpaid_count"))
            Dim partialCount As Integer = Convert.ToInt32(row("partial_count"))

            lblPaidSummary.Text = $"Paid Bills: {paidCount} | Amount: {paidTotal:N2}"
            lblUnpaidSummary.Text = $"Unpaid Bills: {unpaidCount} | Amount: {unpaidTotal:N2}"
            lblPaidCardValue.Text = $"{paidCount} | {paidTotal:N2}"
            lblUnpaidCardValue.Text = $"{unpaidCount} | {unpaidTotal:N2}"

            _paidTotalSummary = paidTotal
            _unpaidTotalSummary = unpaidTotal
            _partialTotalSummary = partialTotal
            chartSummary.Invalidate()

            Dim summaryTable As New DataTable()
            summaryTable.Columns.Add("Status")
            summaryTable.Columns.Add("Count")
            summaryTable.Columns.Add("Total Amount")

            summaryTable.Rows.Add("Paid", paidCount, paidTotal.ToString("N2"))
            summaryTable.Rows.Add("Unpaid", unpaidCount, unpaidTotal.ToString("N2"))
            summaryTable.Rows.Add("Partial", partialCount, partialTotal.ToString("N2"))

            dgvBillSummary.DataSource = summaryTable
        Catch ex As Exception
            MessageBox.Show("Failed to load paid vs unpaid summary: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub tabReports_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim tab As TabPage = tabReports.TabPages(e.Index)
        Dim tabRect As Rectangle = tabReports.GetTabRect(e.Index)
        Dim isSelected As Boolean = (e.Index = tabReports.SelectedIndex)

        Using backBrush As New SolidBrush(If(isSelected, Color.White, ColorTranslator.FromHtml("#dfe6e9")))
            e.Graphics.FillRectangle(backBrush, tabRect)
        End Using

        TextRenderer.DrawText(e.Graphics,
                              tab.Text,
                              New Font("Segoe UI", 9.0F, FontStyle.Bold),
                              tabRect,
                              ColorTranslator.FromHtml("#2c3e50"),
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        If isSelected Then
            Using pen As New Pen(ColorTranslator.FromHtml("#3498db"), 3)
                e.Graphics.DrawLine(pen, tabRect.Left + 6, tabRect.Bottom - 2, tabRect.Right - 6, tabRect.Bottom - 2)
            End Using
        End If
    End Sub

    Private Sub frmReports_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 16
        btnProfile.Left = btnLogout.Left - btnProfile.Width - 8
        pnlStatRevenue.Left = btnProfile.Left - pnlStatRevenue.Width - 14
        pnlStatUnpaid.Left = pnlStatRevenue.Left - pnlStatUnpaid.Width - 12
        pnlStatCustomers.Left = pnlStatUnpaid.Left - pnlStatCustomers.Width - 12
    End Sub

    Private Function GetWelcomeText() As String
        Dim displayName As String = If(String.IsNullOrWhiteSpace(CurrentUser.FullName), CurrentUser.Username, CurrentUser.FullName)
        Return $"Welcome, {displayName}"
    End Function

    Private Shared Sub StyleActionTile(button As Button, backColor As Color)
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 0
        button.BackColor = backColor
        button.ForeColor = Color.White
        button.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
    End Sub

    Private Shared Sub StyleGrid(grid As DataGridView)
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.AllowUserToResizeRows = False
        grid.ReadOnly = True
        grid.RowHeadersVisible = False
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grid.BackgroundColor = Color.White
        grid.BorderStyle = BorderStyle.None
        grid.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
    End Sub

    Private Sub dgvOutstanding_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not dgvOutstanding.Columns.Contains("total_due") OrElse e.ColumnIndex <> dgvOutstanding.Columns("total_due").Index Then
            Return
        End If

        Dim amount As Decimal
        If Decimal.TryParse(Convert.ToString(e.Value), amount) AndAlso amount >= 1000D Then
            e.CellStyle.ForeColor = ColorTranslator.FromHtml("#c0392b")
            e.CellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        End If
    End Sub

    Private Sub dgvStaffActivity_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse Not dgvStaffActivity.Columns.Contains("readings_entered") OrElse e.ColumnIndex <> dgvStaffActivity.Columns("readings_entered").Index Then
            Return
        End If

        e.Handled = True
        e.PaintBackground(e.CellBounds, True)

        Dim value As Integer = 0
        Integer.TryParse(Convert.ToString(e.Value), value)
        Dim maxValue As Integer = 50
        Dim width As Integer = CInt((Math.Min(value, maxValue) / maxValue) * (e.CellBounds.Width - 8))
        Dim barRect As New Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 6, width, e.CellBounds.Height - 12)

        Using barBrush As New SolidBrush(ColorTranslator.FromHtml("#3498db"))
            e.Graphics.FillRectangle(barBrush, barRect)
        End Using

        TextRenderer.DrawText(e.Graphics,
                              value.ToString(),
                              e.CellStyle.Font,
                              e.CellBounds,
                              ColorTranslator.FromHtml("#2c3e50"),
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        e.Paint(e.CellBounds, DataGridViewPaintParts.Border)
    End Sub

    Private Sub chartSummary_Paint(sender As Object, e As PaintEventArgs)
        Dim total As Decimal = _paidTotalSummary + _unpaidTotalSummary + _partialTotalSummary
        If total <= 0D Then
            TextRenderer.DrawText(e.Graphics,
                                  "No data",
                                  New Font("Segoe UI", 10.0F, FontStyle.Bold),
                                  chartSummary.ClientRectangle,
                                  ColorTranslator.FromHtml("#7f8c8d"),
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If

        Dim pieRect As New Rectangle(40, 36, 210, 210)
        Dim paidSweep As Single = CSng((_paidTotalSummary / total) * 360D)
        Dim unpaidSweep As Single = CSng((_unpaidTotalSummary / total) * 360D)
        Dim partialSweep As Single = 360.0F - paidSweep - unpaidSweep

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        TextRenderer.DrawText(e.Graphics,
                              "Bills by Status",
                              New Font("Segoe UI", 12.0F, FontStyle.Bold),
                              New Rectangle(0, 8, chartSummary.Width, 24),
                              ColorTranslator.FromHtml("#2c3e50"),
                              TextFormatFlags.HorizontalCenter)

        Dim startAngle As Single = -90
        Using paidBrush As New SolidBrush(ColorTranslator.FromHtml("#27ae60"))
            e.Graphics.FillPie(paidBrush, pieRect, startAngle, paidSweep)
        End Using
        startAngle += paidSweep
        Using unpaidBrush As New SolidBrush(ColorTranslator.FromHtml("#e74c3c"))
            e.Graphics.FillPie(unpaidBrush, pieRect, startAngle, unpaidSweep)
        End Using
        startAngle += unpaidSweep
        Using partialBrush As New SolidBrush(ColorTranslator.FromHtml("#f39c12"))
            e.Graphics.FillPie(partialBrush, pieRect, startAngle, partialSweep)
        End Using

        DrawPieLabel(e.Graphics, pieRect, -90 + (paidSweep / 2), paidSweep, total, _paidTotalSummary)
        DrawPieLabel(e.Graphics, pieRect, -90 + paidSweep + (unpaidSweep / 2), unpaidSweep, total, _unpaidTotalSummary)
        DrawPieLabel(e.Graphics, pieRect, -90 + paidSweep + unpaidSweep + (partialSweep / 2), partialSweep, total, _partialTotalSummary)

        Dim legendTop As Integer = 260
        DrawLegendItem(e.Graphics, 58, legendTop, ColorTranslator.FromHtml("#27ae60"), "Paid")
        DrawLegendItem(e.Graphics, 148, legendTop, ColorTranslator.FromHtml("#e74c3c"), "Unpaid")
        DrawLegendItem(e.Graphics, 245, legendTop, ColorTranslator.FromHtml("#f39c12"), "Partial")
    End Sub

    Private Shared Sub DrawLegendItem(g As Graphics, x As Integer, y As Integer, color As Color, text As String)
        Using brush As New SolidBrush(color)
            g.FillRectangle(brush, x, y, 14, 14)
        End Using
        TextRenderer.DrawText(g,
                              text,
                              New Font("Segoe UI", 9.0F, FontStyle.Regular),
                              New Point(x + 18, y - 1),
                              ColorTranslator.FromHtml("#2c3e50"))
    End Sub

    Private Shared Sub DrawPieLabel(g As Graphics,
                                    pieRect As Rectangle,
                                    angle As Single,
                                    sweep As Single,
                                    total As Decimal,
                                    value As Decimal)
        If sweep <= 0.1F Then
            Return
        End If

        Dim rad As Double = angle * Math.PI / 180.0
        Dim cx As Single = pieRect.X + pieRect.Width / 2
        Dim cy As Single = pieRect.Y + pieRect.Height / 2
        Dim radius As Single = pieRect.Width / 3.2F
        Dim x As Single = cx + CSng(Math.Cos(rad) * radius)
        Dim y As Single = cy + CSng(Math.Sin(rad) * radius)
        Dim percent As Decimal = If(total <= 0D, 0D, (value / total) * 100D)

        TextRenderer.DrawText(g,
                              $"{percent:0.#}%",
                              New Font("Segoe UI", 8.0F, FontStyle.Bold),
                              New Point(CInt(x - 14), CInt(y - 8)),
                              Color.White)
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

        Try
            GridExportHelper.ExportDataGridViewToPdf(activeGrid, Me, GetActiveReportTitle())
        Catch ex As Exception
            Dim details As String = ex.Message
            If ex.InnerException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(ex.InnerException.Message) Then
                details &= Environment.NewLine & ex.InnerException.Message
            End If

            MessageBox.Show("Failed to export PDF: " & details,
                            "Export",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
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

    Private Function GetActiveReportTitle() As String
        Select Case tabReports.SelectedIndex
            Case 0
                Return "Paid vs Unpaid Summary"
            Case 1
                Return "Revenue by Period"
            Case 2
                Return "Outstanding Balances"
            Case 3
                Return "Staff Activity"
            Case Else
                Return "Report"
        End Select
    End Function

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

    Private Sub btnProfile_Click(sender As Object, e As EventArgs)
        Using frm As New frmProfileSettings()
            If frm.ShowDialog(Me) = DialogResult.OK Then
                lblWelcome.Text = GetWelcomeText()
            End If
        End Using
    End Sub
End Class
