Imports MySql.Data.MySqlClient

Public Class frmPayment
    Inherits Form

    Private ReadOnly _initialCustomerId As Integer?
    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly lblTitle As New Label()
    Private ReadOnly splitMain As New SplitContainer()
    Private ReadOnly pnlLeftCard As New Panel()
    Private ReadOnly pnlRightCard As New Panel()
    Private ReadOnly lblSearchIcon As New Label()
    Private ReadOnly txtCustomerDetails As New TextBox()
    Private ReadOnly lblTotalAmountValue As New Label()
    Private ReadOnly txtCustomerId As New TextBox()
    Private ReadOnly cboCustomer As New ComboBox()
    Private ReadOnly dgvBills As New DataGridView()
    Private ReadOnly txtAmountPaid As New TextBox()
    Private ReadOnly cboPaymentMode As New ComboBox()
    Private ReadOnly txtReference As New TextBox()
    Private ReadOnly btnProcessPayment As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New(Optional initialCustomerId As Integer? = Nothing)
        _initialCustomerId = initialCustomerId
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Process Payment"
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

        txtCustomerId.Visible = False

        pnlTopBar.Dock = DockStyle.Top
        pnlTopBar.Height = 62
        pnlTopBar.BackColor = Color.White
        pnlTopBar.Padding = New Padding(20, 10, 20, 10)

        lblTitle.Text = "Process Payment"
        lblTitle.AutoSize = True
        lblTitle.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblTitle.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblTitle.Location = New Point(20, 16)

        btnLogout.Text = "Close"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 24
        btnLogout.Top = 14
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        pnlTopBar.Controls.Add(lblTitle)
        pnlTopBar.Controls.Add(btnLogout)

        splitMain.Dock = DockStyle.Fill
        splitMain.Orientation = Orientation.Vertical
        splitMain.SplitterDistance = 380
        splitMain.Panel1.Padding = New Padding(16, 14, 8, 14)
        splitMain.Panel2.Padding = New Padding(8, 14, 16, 14)

        pnlLeftCard.Dock = DockStyle.Fill
        pnlLeftCard.BackColor = Color.White
        pnlLeftCard.Padding = New Padding(20)

        Dim leftLayout As New TableLayoutPanel()
        leftLayout.Dock = DockStyle.Fill
        leftLayout.ColumnCount = 1
        leftLayout.RowCount = 4
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim lblCustomer As New Label() With {
            .Text = "Select Customer",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }

        Dim customerPanel As New Panel() With {.Dock = DockStyle.Fill}
        lblSearchIcon.Text = "🔍"
        lblSearchIcon.AutoSize = True
        lblSearchIcon.Font = New Font("Segoe UI Emoji", 10.0F, FontStyle.Regular)
        lblSearchIcon.Left = 8
        lblSearchIcon.Top = 10

        cboCustomer.Left = 34
        cboCustomer.Top = 6
        cboCustomer.Width = 300
        cboCustomer.DropDownStyle = ComboBoxStyle.DropDown
        cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems
        cboCustomer.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        AddHandler cboCustomer.SelectedIndexChanged, AddressOf cboCustomer_SelectedIndexChanged
        customerPanel.Controls.Add(lblSearchIcon)
        customerPanel.Controls.Add(cboCustomer)

        Dim lblCustomerInfo As New Label() With {
            .Text = "Customer Details",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }

        txtCustomerDetails.Dock = DockStyle.Fill
        txtCustomerDetails.Multiline = True
        txtCustomerDetails.ReadOnly = True
        txtCustomerDetails.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        txtCustomerDetails.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        txtCustomerDetails.BorderStyle = BorderStyle.FixedSingle
        txtCustomerDetails.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)

        leftLayout.Controls.Add(lblCustomer, 0, 0)
        leftLayout.Controls.Add(customerPanel, 0, 1)
        leftLayout.Controls.Add(lblCustomerInfo, 0, 2)
        leftLayout.Controls.Add(txtCustomerDetails, 0, 3)
        pnlLeftCard.Controls.Add(leftLayout)

        pnlRightCard.Dock = DockStyle.Fill
        pnlRightCard.BackColor = Color.White
        pnlRightCard.Padding = New Padding(16)

        Dim rightLayout As New TableLayoutPanel()
        rightLayout.Dock = DockStyle.Fill
        rightLayout.ColumnCount = 4
        rightLayout.RowCount = 6
        rightLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 24.0F))
        rightLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 26.0F))
        rightLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 22.0F))
        rightLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 28.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))

        dgvBills.Dock = DockStyle.Fill
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.AllowUserToResizeRows = False
        dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBills.MultiSelect = False
        dgvBills.AutoGenerateColumns = False
        dgvBills.BackgroundColor = Color.White
        dgvBills.BorderStyle = BorderStyle.None
        dgvBills.RowHeadersVisible = False
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvBills.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")

        Dim colSelect As New DataGridViewCheckBoxColumn() With {
            .Name = "colSelect",
            .HeaderText = "Select",
            .Width = 60
        }
        Dim colBillId As New DataGridViewTextBoxColumn() With {
            .Name = "colBillId",
            .HeaderText = "Bill ID",
            .Width = 90,
            .ReadOnly = True
        }
        Dim colBillDate As New DataGridViewTextBoxColumn() With {
            .Name = "colBillDate",
            .HeaderText = "Bill Date",
            .Width = 130,
            .ReadOnly = True
        }
        Dim colDueDate As New DataGridViewTextBoxColumn() With {
            .Name = "colDueDate",
            .HeaderText = "Due Date",
            .Width = 130,
            .ReadOnly = True
        }
        Dim colTotal As New DataGridViewTextBoxColumn() With {
            .Name = "colTotal",
            .HeaderText = "Total Amount",
            .Width = 140,
            .ReadOnly = True
        }
        Dim colOutstanding As New DataGridViewTextBoxColumn() With {
            .Name = "colOutstanding",
            .HeaderText = "Outstanding",
            .Width = 140,
            .ReadOnly = True
        }

        dgvBills.Columns.AddRange(New DataGridViewColumn() {colSelect, colBillId, colBillDate, colDueDate, colTotal, colOutstanding})
        AddHandler dgvBills.CurrentCellDirtyStateChanged, AddressOf dgvBills_CurrentCellDirtyStateChanged
        AddHandler dgvBills.CellValueChanged, AddressOf dgvBills_CellValueChanged

        Dim lblTotal As New Label() With {
            .Text = "Total Amount",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }

        lblTotalAmountValue.Text = "0.00"
        lblTotalAmountValue.AutoSize = True
        lblTotalAmountValue.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        lblTotalAmountValue.ForeColor = ColorTranslator.FromHtml("#27ae60")

        Dim lblAmountPaid As New Label() With {
            .Text = "Amount Paid",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        txtAmountPaid.Dock = DockStyle.Fill
        txtAmountPaid.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)

        Dim lblPaymentMode As New Label() With {
            .Text = "Payment Mode",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        cboPaymentMode.Dock = DockStyle.Fill
        cboPaymentMode.DropDownStyle = ComboBoxStyle.DropDownList
        cboPaymentMode.Items.AddRange(New Object() {"Cash", "Bank Transfer", "Mobile Money", "Online"})
        cboPaymentMode.SelectedIndex = 0

        Dim lblReference As New Label() With {
            .Text = "Reference",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        txtReference.Dock = DockStyle.Fill

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Dock = DockStyle.Right
        btnProcessPayment.Width = 220
        btnProcessPayment.Height = 44
        btnProcessPayment.FlatStyle = FlatStyle.Flat
        btnProcessPayment.FlatAppearance.BorderSize = 0
        btnProcessPayment.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnProcessPayment.ForeColor = Color.White
        btnProcessPayment.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        rightLayout.Controls.Add(dgvBills, 0, 0)
        rightLayout.SetColumnSpan(dgvBills, 4)
        rightLayout.Controls.Add(lblTotal, 0, 1)
        rightLayout.Controls.Add(lblTotalAmountValue, 1, 1)
        rightLayout.Controls.Add(lblAmountPaid, 0, 2)
        rightLayout.Controls.Add(txtAmountPaid, 1, 2)
        rightLayout.Controls.Add(lblPaymentMode, 2, 2)
        rightLayout.Controls.Add(cboPaymentMode, 3, 2)
        rightLayout.Controls.Add(lblReference, 0, 3)
        rightLayout.Controls.Add(txtReference, 1, 3)
        rightLayout.SetColumnSpan(txtReference, 3)

        Dim buttonHost As New Panel() With {.Dock = DockStyle.Fill}
        buttonHost.Controls.Add(btnProcessPayment)
        btnProcessPayment.Top = 6
        btnProcessPayment.Left = buttonHost.Width - btnProcessPayment.Width
        AddHandler buttonHost.Resize,
            Sub()
                btnProcessPayment.Left = buttonHost.Width - btnProcessPayment.Width
            End Sub
        rightLayout.Controls.Add(buttonHost, 0, 5)
        rightLayout.SetColumnSpan(buttonHost, 4)

        pnlRightCard.Controls.Add(rightLayout)

        splitMain.Panel1.Controls.Add(pnlLeftCard)
        splitMain.Panel2.Controls.Add(pnlRightCard)

        Me.Controls.Add(splitMain)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmPayment_Load
        AddHandler Me.Resize, AddressOf frmPayment_Resize
    End Sub

    Private Sub frmPayment_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()

        If Not String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only staff users can process payments.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        LoadCustomers()

        If _initialCustomerId.HasValue Then
            SelectCustomerById(_initialCustomerId.Value)
        End If
    End Sub

    Private Sub frmPayment_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 20
        splitMain.SplitterDistance = Math.Max(320, CInt(Me.ClientSize.Width * 0.35))
    End Sub

    Private Sub SelectCustomerById(customerId As Integer)
        For i As Integer = 0 To cboCustomer.Items.Count - 1
            Dim item As CustomerItem = TryCast(cboCustomer.Items(i), CustomerItem)
            If item IsNot Nothing AndAlso item.CustomerId = customerId Then
                cboCustomer.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub LoadCustomers()
        Try
            cboCustomer.Items.Clear()

            Const sql As String = "SELECT c.id AS customer_id, c.meter_number, c.address, u.full_name FROM customers c INNER JOIN users u ON c.id = u.id WHERE u.role = 'customer' ORDER BY u.full_name;"
            Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql)

            For Each row As DataRow In data.Rows
                Dim item As New CustomerItem With {
                    .CustomerId = Convert.ToInt32(row("customer_id")),
                    .FullName = row("full_name").ToString(),
                    .MeterNumber = row("meter_number").ToString(),
                    .Address = row("address").ToString()
                }
                cboCustomer.Items.Add(item)
            Next

            If cboCustomer.Items.Count > 0 Then
                cboCustomer.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show("Failed to load customers: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cboCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim selected As CustomerItem = TryCast(cboCustomer.SelectedItem, CustomerItem)
        If selected Is Nothing Then
            txtCustomerId.Text = String.Empty
            txtCustomerDetails.Text = "-"
            dgvBills.Rows.Clear()
            txtAmountPaid.Text = "0.00"
            lblTotalAmountValue.Text = "0.00"
            Return
        End If

        txtCustomerId.Text = selected.CustomerId.ToString()
        txtCustomerDetails.Text = $"Name: {selected.FullName}{Environment.NewLine}Meter: {selected.MeterNumber}{Environment.NewLine}Address: {selected.Address}"

        LoadUnpaidBills(selected.CustomerId)
    End Sub

    Private Sub LoadUnpaidBills(customerId As Integer)
        Try
            dgvBills.Rows.Clear()

            Const sql As String = "SELECT b.id AS bill_id, b.bill_date, b.due_date, b.total_amount, COALESCE(SUM(pa.amount_applied), 0) AS paid_amount FROM bills b LEFT JOIN payment_allocations pa ON b.id = pa.bill_id WHERE b.customer_id = @customer_id AND b.status IN ('Unpaid', 'Partial') GROUP BY b.id, b.bill_date, b.due_date, b.total_amount ORDER BY b.due_date, b.bill_date;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@customer_id", customerId}
            }
            Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)

            For Each row As DataRow In data.Rows
                Dim totalAmount As Decimal = Convert.ToDecimal(row("total_amount"))
                Dim paidAmount As Decimal = Convert.ToDecimal(row("paid_amount"))
                Dim outstanding As Decimal = totalAmount - paidAmount
                If outstanding <= 0D Then
                    Continue For
                End If

                dgvBills.Rows.Add(True,
                                 Convert.ToInt32(row("bill_id")),
                                 Convert.ToDateTime(row("bill_date")).ToShortDateString(),
                                 Convert.ToDateTime(row("due_date")).ToShortDateString(),
                                 totalAmount.ToString("N2"),
                                 outstanding.ToString("N2"))
            Next

            UpdateDefaultAmountPaid()
        Catch ex As Exception
            MessageBox.Show("Failed to load unpaid bills: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvBills_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If dgvBills.IsCurrentCellDirty Then
            dgvBills.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub dgvBills_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvBills.Columns("colSelect").Index Then
            UpdateDefaultAmountPaid()
        End If
    End Sub

    Private Sub UpdateDefaultAmountPaid()
        Dim totalSelected As Decimal = 0D

        For Each row As DataGridViewRow In dgvBills.Rows
            Dim isSelected As Boolean = False
            If row.Cells("colSelect").Value IsNot Nothing Then
                Boolean.TryParse(row.Cells("colSelect").Value.ToString(), isSelected)
            End If

            If isSelected Then
                totalSelected += Convert.ToDecimal(row.Cells("colOutstanding").Value)
            End If
        Next

        lblTotalAmountValue.Text = totalSelected.ToString("N2")
        txtAmountPaid.Text = totalSelected.ToString("0.00")
    End Sub

    Private Sub btnProcessPayment_Click(sender As Object, e As EventArgs)
        Try
            Dim selectedCustomer As CustomerItem = TryCast(cboCustomer.SelectedItem, CustomerItem)
            If selectedCustomer Is Nothing Then
                MessageBox.Show("Please select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim selectedBills As New List(Of BillAllocationInput)()
            Dim selectedOutstandingTotal As Decimal = 0D

            For Each row As DataGridViewRow In dgvBills.Rows
                Dim isSelected As Boolean = False
                If row.Cells("colSelect").Value IsNot Nothing Then
                    Boolean.TryParse(row.Cells("colSelect").Value.ToString(), isSelected)
                End If

                If Not isSelected Then
                    Continue For
                End If

                Dim bill As New BillAllocationInput With {
                    .BillId = Convert.ToInt32(row.Cells("colBillId").Value),
                    .BillTotal = Convert.ToDecimal(row.Cells("colTotal").Value),
                    .Outstanding = Convert.ToDecimal(row.Cells("colOutstanding").Value)
                }

                selectedBills.Add(bill)
                selectedOutstandingTotal += bill.Outstanding
            Next

            If selectedBills.Count = 0 Then
                MessageBox.Show("Please select at least one unpaid bill.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim amountPaid As Decimal
            If Not Decimal.TryParse(txtAmountPaid.Text.Trim(), amountPaid) OrElse amountPaid <= 0D Then
                MessageBox.Show("Please enter a valid payment amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If amountPaid > selectedOutstandingTotal Then
                MessageBox.Show("Payment amount cannot exceed total outstanding of selected bills.",
                                "Validation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            End If

            Dim paymentMode As String = cboPaymentMode.Text
            If String.IsNullOrWhiteSpace(paymentMode) Then
                MessageBox.Show("Please select payment mode.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim paymentId As Integer
            Dim remaining As Decimal = amountPaid
            Dim allocationSummary As New List(Of String)()

            Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    Try
                        Const paymentSql As String = "INSERT INTO payments (customer_id, payment_date, amount_paid, mode, reference, received_by) VALUES (@customer_id, @payment_date, @amount_paid, @mode, @reference, @received_by);"
                        Using paymentCmd As New MySqlCommand(paymentSql, conn, tx)
                            paymentCmd.Parameters.AddWithValue("@customer_id", selectedCustomer.CustomerId)
                            paymentCmd.Parameters.AddWithValue("@payment_date", Date.Now)
                            paymentCmd.Parameters.AddWithValue("@amount_paid", amountPaid)
                            paymentCmd.Parameters.AddWithValue("@mode", paymentMode)
                            paymentCmd.Parameters.AddWithValue("@reference", If(String.IsNullOrWhiteSpace(txtReference.Text), DBNull.Value, txtReference.Text.Trim()))
                            paymentCmd.Parameters.AddWithValue("@received_by", CurrentUser.UserId)
                            paymentCmd.ExecuteNonQuery()
                            paymentId = Convert.ToInt32(paymentCmd.LastInsertedId)
                        End Using

                        For Each bill In selectedBills
                            If remaining <= 0D Then
                                Exit For
                            End If

                            Dim applyAmount As Decimal = Math.Min(remaining, bill.Outstanding)
                            If applyAmount <= 0D Then
                                Continue For
                            End If

                            Const allocSql As String = "INSERT INTO payment_allocations (payment_id, bill_id, amount_applied) VALUES (@payment_id, @bill_id, @amount_applied);"
                            Using allocCmd As New MySqlCommand(allocSql, conn, tx)
                                allocCmd.Parameters.AddWithValue("@payment_id", paymentId)
                                allocCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                allocCmd.Parameters.AddWithValue("@amount_applied", applyAmount)
                                allocCmd.ExecuteNonQuery()
                            End Using

                            Const sumSql As String = "SELECT COALESCE(SUM(amount_applied), 0) FROM payment_allocations WHERE bill_id = @bill_id;"
                            Dim totalAllocated As Decimal
                            Using sumCmd As New MySqlCommand(sumSql, conn, tx)
                                sumCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                totalAllocated = Convert.ToDecimal(sumCmd.ExecuteScalar())
                            End Using

                            Dim newStatus As String = If(totalAllocated >= bill.BillTotal, "Paid", "Partial")
                            Const updateBillSql As String = "UPDATE bills SET status = @status WHERE id = @bill_id;"
                            Using updateCmd As New MySqlCommand(updateBillSql, conn, tx)
                                updateCmd.Parameters.AddWithValue("@status", newStatus)
                                updateCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                updateCmd.ExecuteNonQuery()
                            End Using

                            allocationSummary.Add($"Bill #{bill.BillId}: {applyAmount:N2} ({newStatus})")
                            remaining -= applyAmount
                        Next

                        tx.Commit()
                    Catch
                        tx.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            MessageBox.Show($"Payment processed successfully.{Environment.NewLine}{Environment.NewLine}Receipt:{Environment.NewLine}Payment ID: {paymentId}{Environment.NewLine}Customer: {selectedCustomer.FullName}{Environment.NewLine}Amount Paid: {amountPaid:N2}{Environment.NewLine}Mode: {paymentMode}{Environment.NewLine}{String.Join(Environment.NewLine, allocationSummary)}",
                            "Payment Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            AuditLogger.LogAction(CurrentUser.UserId, "PaymentProcessed", $"CustomerId={selectedCustomer.CustomerId}, PaymentId={paymentId}, Amount={amountPaid}")

            txtReference.Clear()
            LoadUnpaidBills(selectedCustomer.CustomerId)
        Catch ex As Exception
            MessageBox.Show("Payment processing failed: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Class CustomerItem
        Public Property CustomerId As Integer
        Public Property FullName As String
        Public Property MeterNumber As String
        Public Property Address As String

        Public Overrides Function ToString() As String
            Return $"{MeterNumber} - {FullName}"
        End Function
    End Class

    Private Class BillAllocationInput
        Public Property BillId As Integer
        Public Property BillTotal As Decimal
        Public Property Outstanding As Decimal
    End Class
End Class
