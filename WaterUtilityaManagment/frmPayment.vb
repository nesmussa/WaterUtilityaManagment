Imports MySql.Data.MySqlClient

Public Class frmPayment
    Inherits Form

    Private ReadOnly _initialCustomerId As Integer?
    Private ReadOnly txtCustomerId As New TextBox()
    Private ReadOnly cboCustomer As New ComboBox()
    Private ReadOnly lblCustomerName As New Label()
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
        Me.Width = 950
        Me.Height = 620
        Me.MinimumSize = New Size(900, 580)

        Dim lblCustomerId As New Label() With {.Text = "Customer ID", .Left = 20, .Top = 20, .AutoSize = True}
        txtCustomerId.Left = 120
        txtCustomerId.Top = 15
        txtCustomerId.Width = 120
        txtCustomerId.Anchor = AnchorStyles.Top Or AnchorStyles.Left
        AddHandler txtCustomerId.Leave, AddressOf txtCustomerId_Leave

        Dim lblCustomer As New Label() With {.Text = "Select Customer", .Left = 270, .Top = 20, .AutoSize = True}
        cboCustomer.Left = 380
        cboCustomer.Top = 15
        cboCustomer.Width = 530
        cboCustomer.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        cboCustomer.DropDownStyle = ComboBoxStyle.DropDown
        cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems
        AddHandler cboCustomer.SelectedIndexChanged, AddressOf cboCustomer_SelectedIndexChanged

        Dim lblNameTitle As New Label() With {.Text = "Customer Name", .Left = 20, .Top = 55, .AutoSize = True}
        lblCustomerName.Left = 120
        lblCustomerName.Top = 55
        lblCustomerName.AutoSize = True
        lblCustomerName.Text = "-"

        dgvBills.Left = 20
        dgvBills.Top = 90
        dgvBills.Width = 890
        dgvBills.Height = 350
        dgvBills.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvBills.MultiSelect = False
        dgvBills.AutoGenerateColumns = False

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

        Dim lblAmountPaid As New Label() With {.Text = "Amount Paid", .Left = 20, .Top = 465, .AutoSize = True}
        txtAmountPaid.Left = 120
        txtAmountPaid.Top = 460
        txtAmountPaid.Width = 150
        txtAmountPaid.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom

        Dim lblPaymentMode As New Label() With {.Text = "Payment Mode", .Left = 300, .Top = 465, .AutoSize = True}
        cboPaymentMode.Left = 400
        cboPaymentMode.Top = 460
        cboPaymentMode.Width = 180
        cboPaymentMode.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        cboPaymentMode.DropDownStyle = ComboBoxStyle.DropDownList
        cboPaymentMode.Items.AddRange(New Object() {"Cash", "Bank Transfer", "Mobile Money", "Online"})
        cboPaymentMode.SelectedIndex = 0

        Dim lblReference As New Label() With {.Text = "Reference", .Left = 610, .Top = 465, .AutoSize = True}
        txtReference.Left = 680
        txtReference.Top = 460
        txtReference.Width = 230
        txtReference.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Left = 120
        btnProcessPayment.Top = 510
        btnProcessPayment.Width = 180
        btnProcessPayment.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 320
        btnLogout.Top = 510
        btnLogout.Width = 120
        btnLogout.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvBills)
        UiStyleHelper.StyleButton(btnProcessPayment, True)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(lblCustomerId)
        Me.Controls.Add(txtCustomerId)
        Me.Controls.Add(lblCustomer)
        Me.Controls.Add(cboCustomer)
        Me.Controls.Add(lblNameTitle)
        Me.Controls.Add(lblCustomerName)
        Me.Controls.Add(dgvBills)
        Me.Controls.Add(lblAmountPaid)
        Me.Controls.Add(txtAmountPaid)
        Me.Controls.Add(lblPaymentMode)
        Me.Controls.Add(cboPaymentMode)
        Me.Controls.Add(lblReference)
        Me.Controls.Add(txtReference)
        Me.Controls.Add(btnProcessPayment)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmPayment_Load
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

            Const sql As String = "SELECT c.id AS customer_id, c.meter_number, u.full_name FROM customers c INNER JOIN users u ON c.id = u.id WHERE u.role = 'customer' ORDER BY u.full_name;"
            Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql)

            For Each row As DataRow In data.Rows
                Dim item As New CustomerItem With {
                    .CustomerId = Convert.ToInt32(row("customer_id")),
                    .FullName = row("full_name").ToString(),
                    .MeterNumber = row("meter_number").ToString()
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

    Private Sub txtCustomerId_Leave(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtCustomerId.Text) Then
            Return
        End If

        Dim customerId As Integer
        If Not Integer.TryParse(txtCustomerId.Text.Trim(), customerId) Then
            MessageBox.Show("Customer ID must be numeric.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        For i As Integer = 0 To cboCustomer.Items.Count - 1
            Dim item As CustomerItem = TryCast(cboCustomer.Items(i), CustomerItem)
            If item IsNot Nothing AndAlso item.CustomerId = customerId Then
                cboCustomer.SelectedIndex = i
                Return
            End If
        Next

        MessageBox.Show("Customer not found.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

    Private Sub cboCustomer_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim selected As CustomerItem = TryCast(cboCustomer.SelectedItem, CustomerItem)
        If selected Is Nothing Then
            txtCustomerId.Text = String.Empty
            lblCustomerName.Text = "-"
            dgvBills.Rows.Clear()
            txtAmountPaid.Text = "0.00"
            Return
        End If

        txtCustomerId.Text = selected.CustomerId.ToString()
        lblCustomerName.Text = selected.FullName

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
        SessionManager.Logout(Me)
    End Sub

    Private Class CustomerItem
        Public Property CustomerId As Integer
        Public Property FullName As String
        Public Property MeterNumber As String

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
