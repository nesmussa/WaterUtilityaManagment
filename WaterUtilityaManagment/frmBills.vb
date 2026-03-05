Public Class frmBills
    Inherits Form

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
        Me.Width = 960
        Me.Height = 640
        Me.MinimumSize = New Size(900, 600)

        Dim lblSearch As New Label() With {
            .Text = "Search (name/meter)",
            .AutoSize = True,
            .Left = 20,
            .Top = 20
        }

        txtSearch.Left = 160
        txtSearch.Top = 16
        txtSearch.Width = 260

        btnSearch.Text = "Search"
        btnSearch.Left = 430
        btnSearch.Top = 14
        btnSearch.Width = 90
        AddHandler btnSearch.Click, AddressOf btnSearch_Click

        lblTotalOutstanding.Left = 540
        lblTotalOutstanding.Top = 20
        lblTotalOutstanding.AutoSize = True
        lblTotalOutstanding.Text = "Total Outstanding: 0.00"

        dgvBills.Left = 20
        dgvBills.Top = 55
        dgvBills.Width = 900
        dgvBills.Height = 450
        dgvBills.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.ReadOnly = True
        dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBills.MultiSelect = False
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        AddHandler dgvBills.CellDoubleClick, AddressOf dgvBills_CellDoubleClick
        AddHandler dgvBills.KeyDown, AddressOf dgvBills_KeyDown

        btnLogout.Visible = False

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Left = 790
        btnProcessPayment.Top = 560
        btnProcessPayment.Width = 130
        btnProcessPayment.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvBills)
        UiStyleHelper.StyleButton(btnSearch, True)
        UiStyleHelper.StyleButton(btnProcessPayment, True)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(lblSearch)
        Me.Controls.Add(txtSearch)
        Me.Controls.Add(btnSearch)
        Me.Controls.Add(lblTotalOutstanding)
        Me.Controls.Add(dgvBills)
        Me.Controls.Add(btnProcessPayment)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmBills_Load
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

        LoadBills()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadBills(txtSearch.Text.Trim())
    End Sub

    Private Sub LoadBills(Optional search As String = "")
        Try
            Const sql As String = "SELECT b.id AS bill_id, b.customer_id, u.full_name, c.meter_number, b.bill_date, b.due_date, b.total_amount, b.status, COALESCE(pa.paid_total, 0) AS paid_amount, (b.total_amount - COALESCE(pa.paid_total, 0)) AS outstanding FROM bills b INNER JOIN customers c ON c.id = b.customer_id INNER JOIN users u ON u.id = c.id LEFT JOIN (SELECT bill_id, SUM(amount_applied) AS paid_total FROM payment_allocations GROUP BY bill_id) pa ON pa.bill_id = b.id WHERE b.status IN ('Unpaid','Partial') AND (@search = '' OR u.full_name LIKE @keyword OR c.meter_number LIKE @keyword) ORDER BY b.due_date, b.bill_date;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@search", search},
                {"@keyword", "%" & search & "%"}
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
            dgvBills.DataSource = dt

            If dgvBills.Columns.Contains("customer_id") Then
                dgvBills.Columns("customer_id").Visible = False
            End If

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

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
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
