Imports MySql.Data.MySqlClient

Public Class frmEnterReading
    Inherits Form

    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly lblStaffName As New Label()
    Private ReadOnly pnlBottomActions As New Panel()
    Private ReadOnly actionsFlow As New FlowLayoutPanel()
    Private ReadOnly splitMain As New SplitContainer()
    Private ReadOnly pnlLeftCard As New Panel()
    Private ReadOnly pnlRightCard As New Panel()
    Private ReadOnly lblSearchIcon As New Label()
    Private ReadOnly lblLastReadingHeader As New Label()
    Private ReadOnly pnlLastReadingInfo As New Panel()
    Private ReadOnly lblLastReadingTitle As New Label()
    Private ReadOnly lblLastReadingDateTitle As New Label()
    Private ReadOnly cboCustomer As New ComboBox()
    Private ReadOnly txtNewReading As New TextBox()
    Private ReadOnly dtpReadingDate As New DateTimePicker()
    Private ReadOnly lblLastReadingValue As New Label()
    Private ReadOnly lblLastReadingDate As New Label()
    Private ReadOnly lblCollectionsCaption As New Label()
    Private ReadOnly lblCollectionsAmount As New Label()
    Private ReadOnly dgvRecentActivity As New DataGridView()
    Private ReadOnly btnSaveReading As New Button()
    Private ReadOnly btnRegisterCustomer As New Button()
    Private ReadOnly btnProcessPayment As New Button()
    Private ReadOnly btnViewBills As New Button()
    Private ReadOnly btnMyActivity As New Button()
    Private ReadOnly btnProfile As New Button()
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
        pnlTopBar.Height = 58
        pnlTopBar.BackColor = Color.White
        pnlTopBar.Padding = New Padding(20, 10, 20, 10)

        lblStaffName.AutoSize = True
        lblStaffName.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblStaffName.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblStaffName.Location = New Point(20, 18)

        btnLogout.Text = "Logout"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 24
        btnLogout.Top = 12
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        btnProfile.Text = "My Profile"
        btnProfile.Width = 110
        btnProfile.Height = 34
        btnProfile.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnProfile.Left = btnLogout.Left - btnProfile.Width - 8
        btnProfile.Top = 12
        btnProfile.FlatStyle = FlatStyle.Flat
        btnProfile.FlatAppearance.BorderSize = 0
        btnProfile.BackColor = ColorTranslator.FromHtml("#3498db")
        btnProfile.ForeColor = Color.White
        btnProfile.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnProfile.Click, AddressOf btnProfile_Click

        pnlTopBar.Controls.Add(lblStaffName)
        pnlTopBar.Controls.Add(btnProfile)
        pnlTopBar.Controls.Add(btnLogout)

        splitMain.Dock = DockStyle.Fill
        splitMain.Orientation = Orientation.Vertical
        splitMain.SplitterDistance = 380
        splitMain.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        splitMain.Panel1.Padding = New Padding(16, 14, 8, 10)
        splitMain.Panel2.Padding = New Padding(8, 14, 16, 10)

        pnlLeftCard.Dock = DockStyle.Fill
        pnlLeftCard.BackColor = Color.White
        pnlLeftCard.Padding = New Padding(22)

        Dim leftLayout As New TableLayoutPanel()
        leftLayout.Dock = DockStyle.Fill
        leftLayout.ColumnCount = 1
        leftLayout.RowCount = 9
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 40.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 14.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 80.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 50.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        leftLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 40.0F))

        Dim lblCustomer As New Label() With {
            .Text = "Customer",
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
        cboCustomer.Width = 290
        cboCustomer.DropDownStyle = ComboBoxStyle.DropDown
        cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems
        cboCustomer.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        AddHandler cboCustomer.SelectedIndexChanged, AddressOf cboCustomer_SelectedIndexChanged
        customerPanel.Controls.Add(lblSearchIcon)
        customerPanel.Controls.Add(cboCustomer)

        lblLastReadingHeader.Text = "Last Reading"
        lblLastReadingHeader.AutoSize = True
        lblLastReadingHeader.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblLastReadingHeader.ForeColor = ColorTranslator.FromHtml("#2c3e50")

        pnlLastReadingInfo.Dock = DockStyle.Fill
        pnlLastReadingInfo.BackColor = ColorTranslator.FromHtml("#d6eaf8")
        pnlLastReadingInfo.Padding = New Padding(12, 10, 12, 10)

        lblLastReadingTitle.Text = "Value:"
        lblLastReadingTitle.AutoSize = True
        lblLastReadingTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblLastReadingTitle.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblLastReadingTitle.Left = 8
        lblLastReadingTitle.Top = 10

        lblLastReadingValue.AutoSize = True
        lblLastReadingValue.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblLastReadingValue.ForeColor = ColorTranslator.FromHtml("#1f618d")
        lblLastReadingValue.Left = 70
        lblLastReadingValue.Top = 8
        lblLastReadingValue.Text = "-"

        lblLastReadingDateTitle.Text = "Date:"
        lblLastReadingDateTitle.AutoSize = True
        lblLastReadingDateTitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblLastReadingDateTitle.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblLastReadingDateTitle.Left = 8
        lblLastReadingDateTitle.Top = 42

        lblLastReadingDate.AutoSize = True
        lblLastReadingDate.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        lblLastReadingDate.ForeColor = ColorTranslator.FromHtml("#1f618d")
        lblLastReadingDate.Left = 70
        lblLastReadingDate.Top = 40
        lblLastReadingDate.Text = "-"

        pnlLastReadingInfo.Controls.Add(lblLastReadingTitle)
        pnlLastReadingInfo.Controls.Add(lblLastReadingValue)
        pnlLastReadingInfo.Controls.Add(lblLastReadingDateTitle)
        pnlLastReadingInfo.Controls.Add(lblLastReadingDate)

        Dim lblNewReading As New Label() With {
            .Text = "New Reading",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        txtNewReading.Dock = DockStyle.Fill
        txtNewReading.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)

        Dim lblReadingDate As New Label() With {
            .Text = "Reading Date",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }
        dtpReadingDate.Dock = DockStyle.Fill
        dtpReadingDate.Format = DateTimePickerFormat.[Short]
        dtpReadingDate.Value = Date.Today

        btnSaveReading.Text = "Save Reading"
        btnSaveReading.Dock = DockStyle.Fill
        btnSaveReading.Height = 42
        btnSaveReading.FlatStyle = FlatStyle.Flat
        btnSaveReading.FlatAppearance.BorderSize = 0
        btnSaveReading.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnSaveReading.ForeColor = Color.White
        btnSaveReading.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnSaveReading.Click, AddressOf btnSaveReading_Click

        leftLayout.Controls.Add(lblCustomer, 0, 0)
        leftLayout.Controls.Add(customerPanel, 0, 1)
        leftLayout.Controls.Add(New Label() With {.AutoSize = True}, 0, 2)
        leftLayout.Controls.Add(lblLastReadingHeader, 0, 3)
        leftLayout.Controls.Add(pnlLastReadingInfo, 0, 4)
        leftLayout.Controls.Add(lblNewReading, 0, 5)
        leftLayout.Controls.Add(txtNewReading, 0, 6)
        leftLayout.Controls.Add(lblReadingDate, 0, 7)
        leftLayout.Controls.Add(dtpReadingDate, 0, 8)

        Dim savePanel As New Panel() With {.Dock = DockStyle.Bottom, .Height = 58}
        btnSaveReading.Left = 0
        btnSaveReading.Top = 8
        btnSaveReading.Width = 330
        savePanel.Controls.Add(btnSaveReading)
        pnlLeftCard.Controls.Add(leftLayout)
        pnlLeftCard.Controls.Add(savePanel)

        btnRegisterCustomer.Text = "Register Customer"
        btnRegisterCustomer.Width = 150
        btnRegisterCustomer.Height = 66
        AddHandler btnRegisterCustomer.Click, AddressOf btnRegisterCustomer_Click

        btnProcessPayment.Text = "Process Payment"
        btnProcessPayment.Width = 150
        btnProcessPayment.Height = 66
        AddHandler btnProcessPayment.Click, AddressOf btnProcessPayment_Click

        btnViewBills.Text = "View Bills"
        btnViewBills.Width = 150
        btnViewBills.Height = 66
        AddHandler btnViewBills.Click, AddressOf btnViewBills_Click

        btnMyActivity.Text = "My Activity"
        btnMyActivity.Width = 150
        btnMyActivity.Height = 66
        AddHandler btnMyActivity.Click, AddressOf btnMyActivity_Click

        StyleQuickActionButton(btnRegisterCustomer, "👤")
        StyleQuickActionButton(btnProcessPayment, "💳")
        StyleQuickActionButton(btnViewBills, "📄")
        StyleQuickActionButton(btnMyActivity, "📈")

        pnlRightCard.Dock = DockStyle.Fill
        pnlRightCard.BackColor = Color.White
        pnlRightCard.Padding = New Padding(18)

        lblCollectionsCaption.Text = "Today's Collections"
        lblCollectionsCaption.AutoSize = True
        lblCollectionsCaption.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        lblCollectionsCaption.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        lblCollectionsCaption.Left = 2
        lblCollectionsCaption.Top = 2

        lblCollectionsAmount.Text = "0.00"
        lblCollectionsAmount.AutoSize = True
        lblCollectionsAmount.Font = New Font("Segoe UI", 24.0F, FontStyle.Bold)
        lblCollectionsAmount.ForeColor = ColorTranslator.FromHtml("#27ae60")
        lblCollectionsAmount.Left = 2
        lblCollectionsAmount.Top = 28

        Dim recentHeader As New Label() With {
            .Text = "Recent Activity",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50")
        }

        dgvRecentActivity.Dock = DockStyle.Fill
        dgvRecentActivity.ReadOnly = True
        dgvRecentActivity.AllowUserToAddRows = False
        dgvRecentActivity.AllowUserToDeleteRows = False
        dgvRecentActivity.AllowUserToResizeRows = False
        dgvRecentActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvRecentActivity.MultiSelect = False
        dgvRecentActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvRecentActivity.RowHeadersVisible = False
        dgvRecentActivity.BackgroundColor = Color.White
        dgvRecentActivity.BorderStyle = BorderStyle.None
        dgvRecentActivity.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvRecentActivity.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")

        Dim rightLayout As New TableLayoutPanel()
        rightLayout.Dock = DockStyle.Fill
        rightLayout.ColumnCount = 1
        rightLayout.RowCount = 4
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 70.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 28.0F))
        rightLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        rightLayout.Controls.Add(lblCollectionsCaption, 0, 0)
        rightLayout.Controls.Add(lblCollectionsAmount, 0, 1)
        rightLayout.Controls.Add(recentHeader, 0, 2)
        rightLayout.Controls.Add(dgvRecentActivity, 0, 3)
        pnlRightCard.Controls.Add(rightLayout)

        splitMain.Panel1.Controls.Add(pnlLeftCard)
        splitMain.Panel2.Controls.Add(pnlRightCard)

        pnlBottomActions.Dock = DockStyle.Bottom
        pnlBottomActions.Height = 96
        pnlBottomActions.Padding = New Padding(16, 4, 16, 12)
        pnlBottomActions.BackColor = ColorTranslator.FromHtml("#ecf0f1")

        actionsFlow.Dock = DockStyle.Fill
        actionsFlow.FlowDirection = FlowDirection.LeftToRight
        actionsFlow.WrapContents = False
        actionsFlow.AutoScroll = True
        actionsFlow.Controls.Add(btnRegisterCustomer)
        actionsFlow.Controls.Add(btnProcessPayment)
        actionsFlow.Controls.Add(btnViewBills)
        actionsFlow.Controls.Add(btnMyActivity)
        pnlBottomActions.Controls.Add(actionsFlow)

        Me.Controls.Add(splitMain)
        Me.Controls.Add(pnlBottomActions)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmEnterReading_Load
        AddHandler Me.Resize, AddressOf frmEnterReading_Resize
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

        lblStaffName.Text = GetStaffDisplayText()
        tips.SetToolTip(txtNewReading, "Enter a positive reading")

        LoadCustomers()
        LoadTodayCollections()
        LoadRecentActivity()
    End Sub

    Private Sub frmEnterReading_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 20
        btnProfile.Left = btnLogout.Left - btnProfile.Width - 8
        splitMain.SplitterDistance = Math.Max(340, CInt(Me.ClientSize.Width * 0.3))
    End Sub

    Private Function GetStaffDisplayText() As String
        Dim displayName As String = If(String.IsNullOrWhiteSpace(CurrentUser.FullName), CurrentUser.Username, CurrentUser.FullName)
        Return $"Staff: {displayName}"
    End Function

    Private Shared Sub StyleQuickActionButton(button As Button, icon As String)
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 0
        button.BackColor = Color.White
        button.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        button.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        button.TextAlign = ContentAlignment.MiddleCenter
        button.Text = $"{icon}{Environment.NewLine}{button.Text}"
    End Sub

    Private Sub LoadTodayCollections()
        Try
            Const sql As String = "SELECT COALESCE(SUM(amount_paid), 0) FROM payments WHERE received_by = @staff_id AND DATE(payment_date) = CURDATE();"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@staff_id", CurrentUser.UserId}
            }
            Dim total As Decimal = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sql, parameters))
            lblCollectionsAmount.Text = total.ToString("N2")
        Catch
            lblCollectionsAmount.Text = "-"
        End Try
    End Sub

    Private Sub LoadRecentActivity()
        Try
            Const sql As String = "SELECT mr.reading_date AS `Date`, c.meter_number AS `Meter`, u.full_name AS `Customer`, mr.reading_value AS `Reading`, mr.consumption AS `Consumption` FROM meter_readings mr INNER JOIN customers c ON mr.customer_id = c.id INNER JOIN users u ON c.id = u.id WHERE mr.entered_by = @staff_id ORDER BY mr.reading_date DESC, mr.reading_id DESC LIMIT 40;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@staff_id", CurrentUser.UserId}
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
            dgvRecentActivity.DataSource = dt
        Catch
            dgvRecentActivity.DataSource = Nothing
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
            LoadRecentActivity()
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

    Private Sub btnProfile_Click(sender As Object, e As EventArgs)
        Using frm As New frmProfileSettings()
            If frm.ShowDialog(Me) = DialogResult.OK Then
                lblStaffName.Text = GetStaffDisplayText()
            End If
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
