Imports MySql.Data.MySqlClient
Imports System.ComponentModel

Public Class frmOnlinePayment
    Inherits Form

    Private ReadOnly _customerId As Integer
    Private ReadOnly cardPanel As New RoundedPanel()
    Private ReadOnly lblHeader As New Label()
    Private ReadOnly dgvUnpaidBills As New DataGridView()
    Private ReadOnly txtAmount As New TextBox()
    Private ReadOnly btnPay As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly tips As New ToolTip()

    Public Sub New(customerId As Integer)
        _customerId = customerId
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Online Payment"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(1200, 800)
        Me.MinimumSize = New Size(900, 650)
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.ShowInTaskbar = True
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        Me.DoubleBuffered = True

        cardPanel.Width = 600
        cardPanel.Height = 640
        cardPanel.BackColor = Color.White
        cardPanel.CornerRadius = 18
        cardPanel.Padding = New Padding(18)

        dgvUnpaidBills.Left = 20
        dgvUnpaidBills.Top = 92
        dgvUnpaidBills.Width = 560
        dgvUnpaidBills.Height = 360
        dgvUnpaidBills.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvUnpaidBills.AllowUserToAddRows = False
        dgvUnpaidBills.AllowUserToDeleteRows = False
        dgvUnpaidBills.AllowUserToResizeRows = False
        dgvUnpaidBills.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUnpaidBills.AutoGenerateColumns = False
        dgvUnpaidBills.BackgroundColor = Color.White
        dgvUnpaidBills.BorderStyle = BorderStyle.None
        dgvUnpaidBills.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvUnpaidBills.RowHeadersVisible = False
        dgvUnpaidBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvUnpaidBills.GridColor = ColorTranslator.FromHtml("#e5e7e9")
        dgvUnpaidBills.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")

        lblHeader.Left = 20
        lblHeader.Top = 18
        lblHeader.Width = 560
        lblHeader.Height = 56
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblHeader.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblHeader.Text = "Pay Your Bill"

        Dim colSelect As New DataGridViewCheckBoxColumn() With {.Name = "colSelect", .HeaderText = "Select", .Width = 60}
        Dim colBillId As New DataGridViewTextBoxColumn() With {.Name = "colBillId", .HeaderText = "Bill ID", .ReadOnly = True, .Width = 90}
        Dim colBillDate As New DataGridViewTextBoxColumn() With {.Name = "colBillDate", .HeaderText = "Bill Date", .ReadOnly = True, .Width = 120}
        Dim colDueDate As New DataGridViewTextBoxColumn() With {.Name = "colDueDate", .HeaderText = "Due Date", .ReadOnly = True, .Width = 120}
        Dim colOutstanding As New DataGridViewTextBoxColumn() With {.Name = "colOutstanding", .HeaderText = "Outstanding", .ReadOnly = True, .Width = 130}

        dgvUnpaidBills.Columns.AddRange(New DataGridViewColumn() {colSelect, colBillId, colBillDate, colDueDate, colOutstanding})
        AddHandler dgvUnpaidBills.CurrentCellDirtyStateChanged, AddressOf dgvUnpaidBills_CurrentCellDirtyStateChanged
        AddHandler dgvUnpaidBills.CellValueChanged, AddressOf dgvUnpaidBills_CellValueChanged

        Dim lblAmount As New Label() With {
            .Text = "Amount",
            .Left = 20,
            .Top = 470,
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        txtAmount.Left = 95
        txtAmount.Top = 464
        txtAmount.Width = 190
        txtAmount.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        txtAmount.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular)

        btnPay.Text = "💳 Pay Now"
        btnPay.Left = 20
        btnPay.Top = 515
        btnPay.Width = 265
        btnPay.Height = 48
        btnPay.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        btnPay.FlatStyle = FlatStyle.Flat
        btnPay.FlatAppearance.BorderSize = 0
        btnPay.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnPay.ForeColor = Color.White
        btnPay.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        AddHandler btnPay.Click, AddressOf btnPay_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 315
        btnCancel.Top = 515
        btnCancel.Width = 265
        btnCancel.Height = 48
        btnCancel.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnCancel.ForeColor = Color.White
        btnCancel.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        cardPanel.Controls.Add(lblHeader)
        cardPanel.Controls.Add(dgvUnpaidBills)
        cardPanel.Controls.Add(lblAmount)
        cardPanel.Controls.Add(txtAmount)
        cardPanel.Controls.Add(btnPay)
        cardPanel.Controls.Add(btnCancel)
        Me.Controls.Add(cardPanel)

        tips.SetToolTip(txtAmount, "Enter amount to pay (can be less than total).")

        AddHandler Me.Load, AddressOf frmOnlinePayment_Load
        AddHandler Me.Resize, AddressOf frmOnlinePayment_Resize
    End Sub

    Private Sub frmOnlinePayment_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()
        LoadCustomerHeader()
        CenterCard()
        UiStyleHelper.AddDialogCloseButton(Me)
        LoadUnpaidBills()
    End Sub

    Private Sub frmOnlinePayment_Resize(sender As Object, e As EventArgs)
        CenterCard()
    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Using brush As New Drawing2D.LinearGradientBrush(Me.ClientRectangle,
                                                          ColorTranslator.FromHtml("#2c3e50"),
                                                          ColorTranslator.FromHtml("#3498db"),
                                                          Drawing2D.LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(brush, Me.ClientRectangle)
        End Using
    End Sub

    Private Sub CenterCard()
        cardPanel.Left = (Me.ClientSize.Width - cardPanel.Width) \ 2
        cardPanel.Top = (Me.ClientSize.Height - cardPanel.Height) \ 2
    End Sub

    Private Sub LoadCustomerHeader()
        Try
            Const sql As String = "SELECT full_name FROM users WHERE id = @customer_id LIMIT 1;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@customer_id", _customerId}
            }
            Dim result As Object = DatabaseHelper.ExecuteScalar(sql, parameters)
            Dim customerName As String = If(result Is Nothing OrElse result Is DBNull.Value, "Customer", result.ToString())
            lblHeader.Text = $"Pay Your Bill - {customerName}"
        Catch
            lblHeader.Text = "Pay Your Bill"
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
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

    Private NotInheritable Class RoundedPanel
        Inherits Panel

        Private _cornerRadius As Integer = 16

        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        <DefaultValue(16)>
        Public Property CornerRadius As Integer
            Get
                Return _cornerRadius
            End Get
            Set(value As Integer)
                _cornerRadius = Math.Max(1, value)
                UpdateRegion()
                Me.Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnSizeChanged(e As EventArgs)
            MyBase.OnSizeChanged(e)
            UpdateRegion()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            Using path As Drawing2D.GraphicsPath = CreateRoundedPath(Me.ClientRectangle, _cornerRadius)
                Using pen As New Pen(Color.FromArgb(220, ColorTranslator.FromHtml("#d5d8dc")), 1)
                    e.Graphics.DrawPath(pen, path)
                End Using
            End Using
        End Sub

        Private Sub UpdateRegion()
            If Me.Width <= 0 OrElse Me.Height <= 0 Then
                Return
            End If

            Using path As Drawing2D.GraphicsPath = CreateRoundedPath(Me.ClientRectangle, _cornerRadius)
                Me.Region = New Region(path)
            End Using
        End Sub

        Private Shared Function CreateRoundedPath(bounds As Rectangle, radius As Integer) As Drawing2D.GraphicsPath
            Dim path As New Drawing2D.GraphicsPath()
            Dim diameter As Integer = radius * 2
            Dim arc As New Rectangle(bounds.X, bounds.Y, diameter, diameter)

            path.AddArc(arc, 180, 90)
            arc.X = bounds.Right - diameter
            path.AddArc(arc, 270, 90)
            arc.Y = bounds.Bottom - diameter
            path.AddArc(arc, 0, 90)
            arc.X = bounds.X
            path.AddArc(arc, 90, 90)
            path.CloseFigure()

            Return path
        End Function
    End Class
End Class
