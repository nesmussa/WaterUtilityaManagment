Public Class frmBills
    Inherits Form

    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly pnlOutstandingBox As New Panel()
    Private ReadOnly txtSearch As New TextBox()
    Private ReadOnly btnSearch As New Button()
    Private ReadOnly lblTotalOutstanding As New Label()
    Private ReadOnly dgvBills As New DataGridView()
    Private ReadOnly btnProcessPayment As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Bills"
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

        pnlTopBar.Dock = DockStyle.Top
        pnlTopBar.Height = 74
        pnlTopBar.BackColor = Color.White
        pnlTopBar.Padding = New Padding(16, 12, 16, 12)

        Dim lblSearch As New Label() With {
            .Text = "Search (name/meter)",
            .AutoSize = True,
            .Left = 18,
            .Top = 7,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }

        txtSearch.Left = 18
        txtSearch.Top = 30
        txtSearch.Width = 280
        txtSearch.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)

        btnSearch.Text = "🔍"
        btnSearch.Left = 304
        btnSearch.Top = 29
        btnSearch.Width = 46
        btnSearch.Height = 32
        btnSearch.FlatStyle = FlatStyle.Flat
        btnSearch.FlatAppearance.BorderSize = 0
        btnSearch.BackColor = ColorTranslator.FromHtml("#3498db")
        btnSearch.ForeColor = Color.White
        btnSearch.Font = New Font("Segoe UI Emoji", 10.0F, FontStyle.Bold)
        AddHandler btnSearch.Click, AddressOf btnSearch_Click

        pnlOutstandingBox.Left = 370
        pnlOutstandingBox.Top = 14
        pnlOutstandingBox.Width = 270
        pnlOutstandingBox.Height = 46
        pnlOutstandingBox.BackColor = ColorTranslator.FromHtml("#d6eaf8")
        pnlOutstandingBox.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        lblTotalOutstanding.Left = 12
        lblTotalOutstanding.Top = 13
        lblTotalOutstanding.AutoSize = True
        lblTotalOutstanding.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblTotalOutstanding.ForeColor = ColorTranslator.FromHtml("#1f618d")
        lblTotalOutstanding.Text = "Total Outstanding: 0.00"
        pnlOutstandingBox.Controls.Add(lblTotalOutstanding)

        btnLogout.Text = "Close"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 20
        btnLogout.Top = 20
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        pnlTopBar.Controls.Add(lblSearch)
        pnlTopBar.Controls.Add(txtSearch)
        pnlTopBar.Controls.Add(btnSearch)
        pnlTopBar.Controls.Add(pnlOutstandingBox)
        pnlTopBar.Controls.Add(btnLogout)

        dgvBills.Dock = DockStyle.Fill
        dgvBills.Margin = New Padding(16, 10, 16, 8)
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.AllowUserToResizeRows = False
        dgvBills.ReadOnly = True
        dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBills.MultiSelect = False
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvBills.BackgroundColor = Color.White
        dgvBills.BorderStyle = BorderStyle.None
        dgvBills.RowHeadersVisible = False
        dgvBills.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvBills.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        AddHandler dgvBills.CellDoubleClick, AddressOf dgvBills_CellDoubleClick
        AddHandler dgvBills.KeyDown, AddressOf dgvBills_KeyDown
        AddHandler dgvBills.CellFormatting, AddressOf dgvBills_CellFormatting

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Width = 180
        btnProcessPayment.Height = 42
        btnProcessPayment.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom
        btnProcessPayment.FlatStyle = FlatStyle.Flat
        btnProcessPayment.FlatAppearance.BorderSize = 0
        btnProcessPayment.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnProcessPayment.ForeColor = Color.White
        btnProcessPayment.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        Dim bottomPanel As New Panel() With {
            .Dock = DockStyle.Bottom,
            .Height = 64,
            .Padding = New Padding(16, 10, 16, 10),
            .BackColor = ColorTranslator.FromHtml("#ecf0f1")
        }
        bottomPanel.Controls.Add(btnProcessPayment)
        AddHandler bottomPanel.Resize,
            Sub()
                btnProcessPayment.Left = bottomPanel.ClientSize.Width - btnProcessPayment.Width
                btnProcessPayment.Top = 10
            End Sub

        Dim gridHost As New Panel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16, 12, 16, 8),
            .BackColor = ColorTranslator.FromHtml("#ecf0f1")
        }
        gridHost.Controls.Add(dgvBills)

        Me.Controls.Add(gridHost)
        Me.Controls.Add(bottomPanel)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmBills_Load
        AddHandler Me.Resize, AddressOf frmBills_Resize
    End Sub

    Private Sub frmBills_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()

        Dim isAllowed As Boolean = String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) OrElse
                                   String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase)
        If Not isAllowed Then
            MessageBox.Show("Access denied. Bills view is available to staff and managers.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        btnProcessPayment.Visible = String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase)
        frmBills_Resize(Me, EventArgs.Empty)

        LoadBills()
    End Sub

    Private Sub frmBills_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 18
        pnlOutstandingBox.Left = btnLogout.Left - pnlOutstandingBox.Width - 14
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadBills(txtSearch.Text.Trim())
    End Sub

    Private Sub LoadBills(Optional search As String = "")
        Try
            Const sql As String = "SELECT b.id AS bill_id, b.customer_id, u.full_name, c.meter_number, b.bill_date, b.due_date, b.total_amount, b.status, COALESCE(pa.paid_total, 0) AS paid_amount, (b.total_amount - COALESCE(pa.paid_total, 0)) AS outstanding FROM bills b INNER JOIN customers c ON c.id = b.customer_id INNER JOIN users u ON u.id = c.id LEFT JOIN (SELECT bill_id, SUM(amount_applied) AS paid_total FROM payment_allocations GROUP BY bill_id) pa ON pa.bill_id = b.id WHERE (@search = '' OR u.full_name LIKE @keyword OR c.meter_number LIKE @keyword) ORDER BY b.due_date DESC, b.bill_date DESC;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@search", search},
                {"@keyword", "%" & search & "%"}
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
            dgvBills.DataSource = dt

            If dgvBills.Columns.Contains("customer_id") Then
                dgvBills.Columns("customer_id").Visible = False
            End If

            If dgvBills.Columns.Contains("status") Then
                dgvBills.Columns("status").HeaderText = "Status"
            End If

            For Each col As DataGridViewColumn In dgvBills.Columns
                col.SortMode = DataGridViewColumnSortMode.Automatic
                col.HeaderCell.ToolTipText = "Click to sort"
            Next

            Dim totalOutstanding As Decimal = 0D
            For Each row As DataRow In dt.Rows
                totalOutstanding += Convert.ToDecimal(row("outstanding"))
            Next

            lblTotalOutstanding.Text = $"Total Outstanding: {totalOutstanding:N2}"
        Catch ex As Exception
            MessageBox.Show("Failed to load bills: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvBills_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not dgvBills.Columns.Contains("status") OrElse e.ColumnIndex <> dgvBills.Columns("status").Index Then
            Return
        End If

        Dim statusText As String = Convert.ToString(e.Value)
        If String.IsNullOrWhiteSpace(statusText) Then
            Return
        End If

        Select Case statusText.Trim().ToLowerInvariant()
            Case "unpaid"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#d35400")
                e.CellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            Case "paid"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#27ae60")
                e.CellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            Case "partial"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#2980b9")
                e.CellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        End Select
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub btnProcessPayment_Click(sender As Object, e As EventArgs)
        If dgvBills.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a bill row first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selected As DataGridViewRow = dgvBills.SelectedRows(0)
        Dim customerId As Integer = Convert.ToInt32(selected.Cells("customer_id").Value)

        Using frm As New frmPayment(customerId)
            frm.ShowDialog(Me)
        End Using

        LoadBills(txtSearch.Text.Trim())
    End Sub

    Private Sub dgvBills_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse Not btnProcessPayment.Visible Then
            Return
        End If

        btnProcessPayment_Click(btnProcessPayment, EventArgs.Empty)
    End Sub

    Private Sub dgvBills_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter AndAlso btnProcessPayment.Visible Then
            e.SuppressKeyPress = True
            btnProcessPayment_Click(btnProcessPayment, EventArgs.Empty)
        End If
    End Sub
End Class
