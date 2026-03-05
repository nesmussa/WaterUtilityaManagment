Imports MySql.Data.MySqlClient

Public Class frmOnlinePayment
    Inherits Form

    Private ReadOnly _customerId As Integer
    Private ReadOnly dgvUnpaidBills As New DataGridView()
    Private ReadOnly txtAmount As New TextBox()
    Private ReadOnly btnPay As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New(customerId As Integer)
        _customerId = customerId
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Online Payment"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 760
        Me.Height = 480
        Me.MinimumSize = New Size(720, 440)

        dgvUnpaidBills.Left = 20
        dgvUnpaidBills.Top = 20
        dgvUnpaidBills.Width = 700
        dgvUnpaidBills.Height = 300
        dgvUnpaidBills.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvUnpaidBills.AllowUserToAddRows = False
        dgvUnpaidBills.AllowUserToDeleteRows = False
        dgvUnpaidBills.AutoGenerateColumns = False

        Dim colSelect As New DataGridViewCheckBoxColumn() With {.Name = "colSelect", .HeaderText = "Select", .Width = 60}
        Dim colBillId As New DataGridViewTextBoxColumn() With {.Name = "colBillId", .HeaderText = "Bill ID", .ReadOnly = True, .Width = 90}
        Dim colBillDate As New DataGridViewTextBoxColumn() With {.Name = "colBillDate", .HeaderText = "Bill Date", .ReadOnly = True, .Width = 120}
        Dim colDueDate As New DataGridViewTextBoxColumn() With {.Name = "colDueDate", .HeaderText = "Due Date", .ReadOnly = True, .Width = 120}
        Dim colOutstanding As New DataGridViewTextBoxColumn() With {.Name = "colOutstanding", .HeaderText = "Outstanding", .ReadOnly = True, .Width = 130}

        dgvUnpaidBills.Columns.AddRange(New DataGridViewColumn() {colSelect, colBillId, colBillDate, colDueDate, colOutstanding})
        AddHandler dgvUnpaidBills.CurrentCellDirtyStateChanged, AddressOf dgvUnpaidBills_CurrentCellDirtyStateChanged
        AddHandler dgvUnpaidBills.CellValueChanged, AddressOf dgvUnpaidBills_CellValueChanged

        Dim lblAmount As New Label() With {.Text = "Amount", .Left = 20, .Top = 340, .AutoSize = True}
        txtAmount.Left = 80
        txtAmount.Top = 335
        txtAmount.Width = 130
        txtAmount.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom

        btnPay.Text = "Pay Now"
        btnPay.Left = 240
        btnPay.Top = 333
        btnPay.Width = 120
        btnPay.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        AddHandler btnPay.Click, AddressOf btnPay_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 380
        btnLogout.Top = 333
        btnLogout.Width = 120
        btnLogout.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvUnpaidBills)
        UiStyleHelper.StyleButton(btnPay, True)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(dgvUnpaidBills)
        Me.Controls.Add(lblAmount)
        Me.Controls.Add(txtAmount)
        Me.Controls.Add(btnPay)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmOnlinePayment_Load
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub

    Private Sub frmOnlinePayment_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()
        LoadUnpaidBills()
    End Sub

    Private Sub LoadUnpaidBills()
        dgvUnpaidBills.Rows.Clear()

        Const sql As String = "SELECT b.id AS bill_id, b.bill_date, b.due_date, b.total_amount, COALESCE(SUM(pa.amount_applied), 0) AS paid_amount FROM bills b LEFT JOIN payment_allocations pa ON pa.bill_id = b.id WHERE b.customer_id = @customer_id AND b.status IN ('Unpaid', 'Partial') GROUP BY b.id, b.bill_date, b.due_date, b.total_amount ORDER BY b.due_date, b.bill_date;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", _customerId}
        }
        Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)

        For Each row As DataRow In data.Rows
            Dim outstanding As Decimal = Convert.ToDecimal(row("total_amount")) - Convert.ToDecimal(row("paid_amount"))
            If outstanding <= 0D Then
                Continue For
            End If

            dgvUnpaidBills.Rows.Add(True,
                                    Convert.ToInt32(row("bill_id")),
                                    Convert.ToDateTime(row("bill_date")).ToShortDateString(),
                                    Convert.ToDateTime(row("due_date")).ToShortDateString(),
                                    outstanding.ToString("N2"))
        Next

        UpdateDefaultAmount()
    End Sub

    Private Sub dgvUnpaidBills_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs)
        If dgvUnpaidBills.IsCurrentCellDirty Then
            dgvUnpaidBills.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub dgvUnpaidBills_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = dgvUnpaidBills.Columns("colSelect").Index Then
            UpdateDefaultAmount()
        End If
    End Sub

    Private Sub UpdateDefaultAmount()
        Dim total As Decimal = 0D

        For Each row As DataGridViewRow In dgvUnpaidBills.Rows
            Dim isSelected As Boolean = False
            If row.Cells("colSelect").Value IsNot Nothing Then
                Boolean.TryParse(row.Cells("colSelect").Value.ToString(), isSelected)
            End If

            If isSelected Then
                total += Convert.ToDecimal(row.Cells("colOutstanding").Value)
            End If
        Next

        txtAmount.Text = total.ToString("0.00")
    End Sub

    Private Sub btnPay_Click(sender As Object, e As EventArgs)
        Try
            Dim selectedBills As New List(Of BillItem)()
            Dim selectedTotalOutstanding As Decimal = 0D

            For Each row As DataGridViewRow In dgvUnpaidBills.Rows
                Dim isSelected As Boolean = False
                If row.Cells("colSelect").Value IsNot Nothing Then
                    Boolean.TryParse(row.Cells("colSelect").Value.ToString(), isSelected)
                End If

                If Not isSelected Then
                    Continue For
                End If

                Dim bill As New BillItem With {
                    .BillId = Convert.ToInt32(row.Cells("colBillId").Value),
                    .Outstanding = Convert.ToDecimal(row.Cells("colOutstanding").Value)
                }
                selectedBills.Add(bill)
                selectedTotalOutstanding += bill.Outstanding
            Next

            If selectedBills.Count = 0 Then
                MessageBox.Show("Please select at least one bill.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim amount As Decimal
            If Not Decimal.TryParse(txtAmount.Text.Trim(), amount) OrElse amount <= 0D Then
                MessageBox.Show("Enter a valid amount.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If amount > selectedTotalOutstanding Then
                MessageBox.Show("Amount cannot exceed selected outstanding total.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim paymentId As Integer
            Dim remaining As Decimal = amount

            Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    Try
                        Const paymentSql As String = "INSERT INTO payments (customer_id, payment_date, amount_paid, mode, reference, received_by) VALUES (@customer_id, @payment_date, @amount_paid, @mode, @reference, @received_by);"
                        Using paymentCmd As New MySqlCommand(paymentSql, conn, tx)
                            paymentCmd.Parameters.AddWithValue("@customer_id", _customerId)
                            paymentCmd.Parameters.AddWithValue("@payment_date", Date.Now)
                            paymentCmd.Parameters.AddWithValue("@amount_paid", amount)
                            paymentCmd.Parameters.AddWithValue("@mode", "Online")
                            paymentCmd.Parameters.AddWithValue("@reference", "Online Portal")
                            paymentCmd.Parameters.AddWithValue("@received_by", CurrentUser.UserId)
                            paymentCmd.ExecuteNonQuery()
                            paymentId = Convert.ToInt32(paymentCmd.LastInsertedId)
                        End Using

                        For Each bill In selectedBills
                            If remaining <= 0D Then
                                Exit For
                            End If

                            Dim applied As Decimal = Math.Min(remaining, bill.Outstanding)
                            If applied <= 0D Then
                                Continue For
                            End If

                            Const allocSql As String = "INSERT INTO payment_allocations (payment_id, bill_id, amount_applied) VALUES (@payment_id, @bill_id, @amount_applied);"
                            Using allocCmd As New MySqlCommand(allocSql, conn, tx)
                                allocCmd.Parameters.AddWithValue("@payment_id", paymentId)
                                allocCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                allocCmd.Parameters.AddWithValue("@amount_applied", applied)
                                allocCmd.ExecuteNonQuery()
                            End Using

                            Const totalSql As String = "SELECT b.total_amount, COALESCE(SUM(pa.amount_applied), 0) AS allocated FROM bills b LEFT JOIN payment_allocations pa ON pa.bill_id = b.id WHERE b.id = @bill_id GROUP BY b.total_amount;"
                            Using totalCmd As New MySqlCommand(totalSql, conn, tx)
                                totalCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                Using reader As MySqlDataReader = totalCmd.ExecuteReader()
                                    If reader.Read() Then
                                        Dim billTotal As Decimal = Convert.ToDecimal(reader("total_amount"))
                                        Dim allocated As Decimal = Convert.ToDecimal(reader("allocated"))
                                        Dim status As String = If(allocated >= billTotal, "Paid", "Partial")

                                        reader.Close()

                                        Const updateSql As String = "UPDATE bills SET status = @status WHERE id = @bill_id;"
                                        Using updateCmd As New MySqlCommand(updateSql, conn, tx)
                                            updateCmd.Parameters.AddWithValue("@status", status)
                                            updateCmd.Parameters.AddWithValue("@bill_id", bill.BillId)
                                            updateCmd.ExecuteNonQuery()
                                        End Using
                                    Else
                                        reader.Close()
                                    End If
                                End Using
                            End Using

                            remaining -= applied
                        Next

                        tx.Commit()
                    Catch
                        tx.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            MessageBox.Show($"Online payment successful. Payment ID: {paymentId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            AuditLogger.LogAction(CurrentUser.UserId, "OnlinePayment", $"CustomerId={_customerId}, PaymentId={paymentId}, Amount={amount}")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Online payment failed: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Class BillItem
        Public Property BillId As Integer
        Public Property Outstanding As Decimal
    End Class
End Class
