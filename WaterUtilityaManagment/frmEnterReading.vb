Imports MySql.Data.MySqlClient

Public Class frmEnterReading
    Inherits Form

    Private ReadOnly cboCustomer As New ComboBox()
    Private ReadOnly txtNewReading As New TextBox()
    Private ReadOnly dtpReadingDate As New DateTimePicker()
    Private ReadOnly lblLastReadingValue As New Label()
    Private ReadOnly lblLastReadingDate As New Label()
    Private ReadOnly btnSaveReading As New Button()
    Private ReadOnly btnRegisterCustomer As New Button()
    Private ReadOnly btnProcessPayment As New Button()
    Private ReadOnly btnViewBills As New Button()
    Private ReadOnly btnMyActivity As New Button()
    Private ReadOnly btnChangePassword As New Button()
    Private ReadOnly lblTodayCollections As New Label()
    Private ReadOnly btnLogout As New Button()
    Private ReadOnly err As New ErrorProvider()
    Private ReadOnly tips As New ToolTip()

    Private _selectedCustomerId As Integer
    Private _lastReading As Decimal

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Enter Meter Reading"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 700
        Me.Height = 455
        Me.MinimumSize = New Size(700, 430)

        Dim labelLeft As Integer = 30
        Dim fieldLeft As Integer = 210
        Dim row1Top As Integer = 28
        Dim rowGap As Integer = 44

        Dim lblCustomer As New Label() With {.Text = "Customer", .Left = labelLeft, .Top = row1Top + 4, .AutoSize = True}
        cboCustomer.Left = fieldLeft
        cboCustomer.Top = row1Top
        cboCustomer.Width = 320
        cboCustomer.DropDownStyle = ComboBoxStyle.DropDown
        cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems
        AddHandler cboCustomer.SelectedIndexChanged, AddressOf cboCustomer_SelectedIndexChanged

        Dim lblNewReading As New Label() With {.Text = "New Reading", .Left = labelLeft, .Top = row1Top + rowGap + 4, .AutoSize = True}
        txtNewReading.Left = fieldLeft
        txtNewReading.Top = row1Top + rowGap
        txtNewReading.Width = 320

        Dim lblReadingDate As New Label() With {.Text = "Reading Date", .Left = labelLeft, .Top = row1Top + (rowGap * 2) + 4, .AutoSize = True}
        dtpReadingDate.Left = fieldLeft
        dtpReadingDate.Top = row1Top + (rowGap * 2)
        dtpReadingDate.Width = 320
        dtpReadingDate.Format = DateTimePickerFormat.[Short]
        dtpReadingDate.Value = Date.Today

        Dim lblLastReadingTitle As New Label() With {.Text = "Last Reading", .Left = labelLeft, .Top = row1Top + (rowGap * 3) + 4, .AutoSize = True}
        lblLastReadingValue.Left = fieldLeft
        lblLastReadingValue.Top = row1Top + (rowGap * 3) + 4
        lblLastReadingValue.AutoSize = True
        lblLastReadingValue.Text = "-"

        Dim lblLastReadingDateTitle As New Label() With {.Text = "Last Reading Date", .Left = labelLeft, .Top = row1Top + (rowGap * 4) - 8, .AutoSize = True}
        lblLastReadingDate.Left = fieldLeft
        lblLastReadingDate.Top = row1Top + (rowGap * 4) - 8
        lblLastReadingDate.AutoSize = True
        lblLastReadingDate.Text = "-"

        btnSaveReading.Text = "Save Reading"
        btnSaveReading.Width = 130
        btnSaveReading.Height = 32
        AddHandler btnSaveReading.Click, AddressOf btnSaveReading_Click

        btnRegisterCustomer.Text = "Register Customer"
        btnRegisterCustomer.Width = 130
        btnRegisterCustomer.Height = 32
        AddHandler btnRegisterCustomer.Click, AddressOf btnRegisterCustomer_Click

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Width = 130
        btnProcessPayment.Height = 32
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        btnViewBills.Text = "View Bills"
        btnViewBills.Width = 130
        btnViewBills.Height = 32
        AddHandler btnViewBills.Click, AddressOf btnViewBills_Click

        btnMyActivity.Text = "My Activity"
        btnMyActivity.Width = 130
        btnMyActivity.Height = 32
        AddHandler btnMyActivity.Click, AddressOf btnMyActivity_Click

        btnChangePassword.Text = "Change Password"
        btnChangePassword.Width = 130
        btnChangePassword.Height = 32
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Left = 190
        actionPanel.Top = 232
        actionPanel.Width = 480
        actionPanel.Height = 110
        actionPanel.Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = True

        actionPanel.Controls.Add(btnSaveReading)
        actionPanel.Controls.Add(btnRegisterCustomer)
        actionPanel.Controls.Add(btnProcessPayment)
        actionPanel.Controls.Add(btnViewBills)
        actionPanel.Controls.Add(btnMyActivity)
        actionPanel.Controls.Add(btnChangePassword)

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnSaveReading, True)
        UiStyleHelper.StyleButton(btnRegisterCustomer)
        UiStyleHelper.StyleButton(btnProcessPayment)
        UiStyleHelper.StyleButton(btnViewBills)
        UiStyleHelper.StyleButton(btnMyActivity)
        UiStyleHelper.StyleButton(btnChangePassword)
        UiStyleHelper.StyleButton(btnLogout)

        lblTodayCollections.Left = 190
        lblTodayCollections.Top = 365
        lblTodayCollections.AutoSize = True
        lblTodayCollections.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        lblTodayCollections.Text = "Today's Collections: 0.00"

        btnLogout.Text = "Logout"
        btnLogout.Left = 590
        btnLogout.Top = 360
        btnLogout.Width = 80
        btnLogout.Height = 32
        btnLogout.Anchor = AnchorStyles.Right Or AnchorStyles.Bottom
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Me.Controls.Add(lblCustomer)
        Me.Controls.Add(cboCustomer)
        Me.Controls.Add(lblNewReading)
        Me.Controls.Add(txtNewReading)
        Me.Controls.Add(lblReadingDate)
        Me.Controls.Add(dtpReadingDate)
        Me.Controls.Add(lblLastReadingTitle)
        Me.Controls.Add(lblLastReadingValue)
        Me.Controls.Add(lblLastReadingDateTitle)
        Me.Controls.Add(lblLastReadingDate)
        Me.Controls.Add(actionPanel)
        Me.Controls.Add(lblTodayCollections)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmEnterReading_Load
    End Sub

    Private Sub btnRegisterCustomer_Click(sender As Object, e As EventArgs)
        Using frm As New frmRegisterCustomer()
            frm.ShowDialog(Me)
        End Using
        LoadCustomers()
    End Sub

    Private Sub frmEnterReading_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()

        If Not String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only staff users can enter meter readings.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        tips.SetToolTip(txtNewReading, "Enter a positive reading")

        LoadCustomers()
        LoadTodayCollections()
    End Sub

    Private Sub LoadTodayCollections()
        Try
            Const sql As String = "SELECT COALESCE(SUM(amount_paid), 0) FROM payments WHERE received_by = @staff_id AND DATE(payment_date) = CURDATE();"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@staff_id", CurrentUser.UserId}
            }
            Dim total As Decimal = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sql, parameters))
            lblTodayCollections.Text = $"Today's Collections: {total:N2}"
        Catch
            lblTodayCollections.Text = "Today's Collections: -"
        End Try
    End Sub

    Private Sub LoadCustomers()
        Try
            cboCustomer.Items.Clear()

            Const sql As String = "SELECT c.id AS customer_id, c.meter_number, u.full_name FROM customers c INNER JOIN users u ON c.id = u.id WHERE u.role = 'customer' ORDER BY c.meter_number;"
            Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql)

            For Each row As DataRow In data.Rows
                Dim item As New CustomerDropdownItem With {
                    .CustomerId = Convert.ToInt32(row("customer_id")),
                    .DisplayText = $"{row("meter_number")} - {row("full_name")}".Trim()
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
        Dim selected As CustomerDropdownItem = TryCast(cboCustomer.SelectedItem, CustomerDropdownItem)
        If selected Is Nothing Then
            _selectedCustomerId = 0
            _lastReading = 0D
            lblLastReadingValue.Text = "-"
            lblLastReadingDate.Text = "-"
            Return
        End If

        _selectedCustomerId = selected.CustomerId
        LoadLastReading(_selectedCustomerId)
    End Sub

    Private Sub LoadLastReading(customerId As Integer)
        Try
            Const sql As String = "SELECT last_reading, last_reading_date FROM customers WHERE id = @customer_id LIMIT 1;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@customer_id", customerId}
            }
            Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)

            If data.Rows.Count = 0 Then
                _lastReading = 0D
                lblLastReadingValue.Text = "0"
                lblLastReadingDate.Text = "-"
                Return
            End If

            Dim row As DataRow = data.Rows(0)
            _lastReading = Convert.ToDecimal(If(row("last_reading") Is DBNull.Value, 0D, row("last_reading")))
            lblLastReadingValue.Text = _lastReading.ToString("N2")

            If row("last_reading_date") Is DBNull.Value Then
                lblLastReadingDate.Text = "-"
            Else
                Dim lastDate As Date = Convert.ToDateTime(row("last_reading_date"))
                lblLastReadingDate.Text = lastDate.ToShortDateString()
            End If
        Catch ex As Exception
            MessageBox.Show("Failed to load last reading: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveReading_Click(sender As Object, e As EventArgs)
        Try
            If _selectedCustomerId <= 0 Then
                MessageBox.Show("Please select a customer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            err.Clear()

            Dim newReading As Decimal
            If Not Decimal.TryParse(txtNewReading.Text.Trim(), newReading) OrElse newReading < 0D Then
                err.SetError(txtNewReading, "Reading must be a non-negative number")
                MessageBox.Show("Please enter a valid non-negative reading value.",
                                "Validation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            End If

            If dtpReadingDate.Value.Date > Date.Today Then
                MessageBox.Show("Reading date cannot be in the future.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If newReading < _lastReading Then
                Dim decision = MessageBox.Show("New reading is lower than the last reading. Do you want to continue?",
                                               "Confirm Override",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Warning)
                If decision <> DialogResult.Yes Then
                    Return
                End If
            End If

            Dim readingDate As Date = dtpReadingDate.Value.Date
            Dim consumption As Decimal = newReading - _lastReading
            Dim readingId As Integer

            Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    Try
                        Const insertReadingSql As String = "INSERT INTO meter_readings (customer_id, reading_value, reading_date, entered_by, consumption) VALUES (@customer_id, @reading_value, @reading_date, @entered_by, @consumption);"
                        Using insertCmd As New MySqlCommand(insertReadingSql, conn, tx)
                            insertCmd.Parameters.AddWithValue("@customer_id", _selectedCustomerId)
                            insertCmd.Parameters.AddWithValue("@reading_value", newReading)
                            insertCmd.Parameters.AddWithValue("@reading_date", readingDate)
                            insertCmd.Parameters.AddWithValue("@entered_by", CurrentUser.UserId)
                            insertCmd.Parameters.AddWithValue("@consumption", consumption)
                            insertCmd.ExecuteNonQuery()
                            readingId = Convert.ToInt32(insertCmd.LastInsertedId)
                        End Using

                        Const updateCustomerSql As String = "UPDATE customers SET last_reading = @last_reading, last_reading_date = @last_reading_date WHERE id = @customer_id;"
                        Using updateCmd As New MySqlCommand(updateCustomerSql, conn, tx)
                            updateCmd.Parameters.AddWithValue("@last_reading", newReading)
                            updateCmd.Parameters.AddWithValue("@last_reading_date", readingDate)
                            updateCmd.Parameters.AddWithValue("@customer_id", _selectedCustomerId)
                            updateCmd.ExecuteNonQuery()
                        End Using

                        GenerateBill(conn, tx, _selectedCustomerId, readingId, readingDate, consumption)

                        tx.Commit()
                    Catch
                        tx.Rollback()
                        Throw
                    End Try
                End Using
            End Using
            AuditLogger.LogAction(CurrentUser.UserId, "MeterReadingAdded", $"CustomerId={_selectedCustomerId}, ReadingId={readingId}, Value={newReading}")

            MessageBox.Show("Meter reading saved successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

            txtNewReading.Clear()
            LoadLastReading(_selectedCustomerId)
        Catch ex As Exception
            MessageBox.Show("Failed to save reading: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub

    Private Sub btnProcessPayment_Click(sender As Object, e As EventArgs)
        Using frm As New frmPayment()
            frm.ShowDialog(Me)
        End Using
        LoadTodayCollections()
    End Sub

    Private Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        Using frm As New frmChangePassword()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnMyActivity_Click(sender As Object, e As EventArgs)
        Using frm As New frmStaffActivity()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnViewBills_Click(sender As Object, e As EventArgs)
        Using frm As New frmBills()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub GenerateBill(conn As MySqlConnection, tx As MySqlTransaction, customerId As Integer, readingId As Integer, readingDate As Date, consumption As Decimal)
        Dim tariffRate As Decimal

        Const tariffSql As String = "SELECT rate_per_unit FROM tariffs WHERE effective_to IS NULL OR effective_to >= @today ORDER BY effective_from DESC LIMIT 1;"
        Using tariffCmd As New MySqlCommand(tariffSql, conn, tx)
            tariffCmd.Parameters.AddWithValue("@today", Date.Today)
            Dim tariffObj As Object = tariffCmd.ExecuteScalar()
            If tariffObj Is Nothing OrElse tariffObj Is DBNull.Value Then
                Throw New ApplicationException("No active tariff found for bill generation.")
            End If
            tariffRate = Convert.ToDecimal(tariffObj)
        End Using

        Dim totalAmount As Decimal = consumption * tariffRate
        Dim dueDate As Date = readingDate.AddDays(30)

        Const billSql As String = "INSERT INTO bills (customer_id, reading_id, bill_date, due_date, total_amount, status) VALUES (@customer_id, @reading_id, @bill_date, @due_date, @total_amount, @status);"
        Using billCmd As New MySqlCommand(billSql, conn, tx)
            billCmd.Parameters.AddWithValue("@customer_id", customerId)
            billCmd.Parameters.AddWithValue("@reading_id", readingId)
            billCmd.Parameters.AddWithValue("@bill_date", readingDate)
            billCmd.Parameters.AddWithValue("@due_date", dueDate)
            billCmd.Parameters.AddWithValue("@total_amount", totalAmount)
            billCmd.Parameters.AddWithValue("@status", "Unpaid")
            billCmd.ExecuteNonQuery()
        End Using

        AuditLogger.LogAction(CurrentUser.UserId, "BillGenerated", $"CustomerId={customerId}, ReadingId={readingId}, Amount={totalAmount}")
    End Sub

    Private Class CustomerDropdownItem
        Public Property CustomerId As Integer
        Public Property DisplayText As String

        Public Overrides Function ToString() As String
            Return DisplayText
        End Function
    End Class
End Class
