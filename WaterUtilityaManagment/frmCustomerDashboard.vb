Imports MySql.Data.MySqlClient

Public Class frmCustomerDashboard
    Inherits Form

    Private ReadOnly lblWelcome As New Label()
    Private ReadOnly lblCurrentReading As New Label()
    Private ReadOnly lblLastReadingDate As New Label()
    Private ReadOnly lblMeterNumber As New Label()
    Private ReadOnly lblOutstandingBalance As New Label()
    Private ReadOnly cboBillStatusFilter As New ComboBox()
    Private ReadOnly dgvConsumption As New DataGridView()
    Private ReadOnly lstNotifications As New ListBox()
    Private ReadOnly dgvBills As New DataGridView()
    Private ReadOnly dgvPayments As New DataGridView()
    Private ReadOnly btnChangePassword As New Button()
    Private ReadOnly btnPayOnline As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Customer Dashboard"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 1050
        Me.Height = 820
        Me.MinimumSize = New Size(980, 760)

        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font(lblWelcome.Font, FontStyle.Bold)

        lblCurrentReading.AutoSize = True

        lblLastReadingDate.AutoSize = True

        lblMeterNumber.AutoSize = True
        lblMeterNumber.Text = "Meter Number: -"

        lblOutstandingBalance.AutoSize = True
        lblOutstandingBalance.Font = New Font(lblOutstandingBalance.Font, FontStyle.Bold)
        lblOutstandingBalance.Text = "Outstanding Balance: 0.00"

        dgvConsumption.Left = 0
        dgvConsumption.Top = 0
        dgvConsumption.Width = 450
        dgvConsumption.Height = 180
        dgvConsumption.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvConsumption.AllowUserToAddRows = False
        dgvConsumption.AllowUserToDeleteRows = False
        dgvConsumption.ReadOnly = True
        dgvConsumption.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        lstNotifications.Left = 0
        lstNotifications.Top = 185
        lstNotifications.Width = 450
        lstNotifications.Height = 55
        lstNotifications.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right

        btnChangePassword.Text = "Change Password"
        btnChangePassword.Width = 170
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        btnPayOnline.Text = "Pay Online"
        btnPayOnline.Width = 170
        AddHandler btnPayOnline.Click, AddressOf btnPayOnline_Click

        btnLogout.Text = "Logout"
        btnLogout.Width = 170
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Dim infoPanel As New FlowLayoutPanel()
        infoPanel.Left = 20
        infoPanel.Top = 20
        infoPanel.Width = 260
        infoPanel.Height = 210
        infoPanel.FlowDirection = FlowDirection.TopDown
        infoPanel.WrapContents = False
        infoPanel.Controls.Add(lblWelcome)
        infoPanel.Controls.Add(lblCurrentReading)
        infoPanel.Controls.Add(lblLastReadingDate)
        infoPanel.Controls.Add(lblMeterNumber)
        infoPanel.Controls.Add(lblOutstandingBalance)

        Dim centerPanel As New Panel()
        centerPanel.Left = 290
        centerPanel.Top = 20
        centerPanel.Width = 460
        centerPanel.Height = 245
        centerPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        centerPanel.Controls.Add(dgvConsumption)
        centerPanel.Controls.Add(lstNotifications)

        Dim actionsPanel As New FlowLayoutPanel()
        actionsPanel.Left = 760
        actionsPanel.Top = 20
        actionsPanel.Width = 230
        actionsPanel.Height = 130
        actionsPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        actionsPanel.FlowDirection = FlowDirection.TopDown
        actionsPanel.WrapContents = False
        actionsPanel.Controls.Add(btnChangePassword)
        actionsPanel.Controls.Add(btnPayOnline)
        actionsPanel.Controls.Add(btnLogout)

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvConsumption)
        UiStyleHelper.StyleDataGrid(dgvBills)
        UiStyleHelper.StyleDataGrid(dgvPayments)
        UiStyleHelper.StyleButton(btnChangePassword)
        UiStyleHelper.StyleButton(btnPayOnline, True)
        UiStyleHelper.StyleButton(btnLogout)

        Dim lblBills As New Label() With {.Text = "Bills", .Left = 20, .Top = 280, .AutoSize = True}

        Dim lblBillFilter As New Label() With {.Text = "Status", .Left = 90, .Top = 280, .AutoSize = True}
        cboBillStatusFilter.Left = 140
        cboBillStatusFilter.Top = 276
        cboBillStatusFilter.Width = 150
        cboBillStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboBillStatusFilter.Items.AddRange(New Object() {"All", "Unpaid", "Partial", "Paid"})
        cboBillStatusFilter.SelectedIndex = 0
        AddHandler cboBillStatusFilter.SelectedIndexChanged, AddressOf cboBillStatusFilter_SelectedIndexChanged

        dgvBills.Left = 20
        dgvBills.Top = 305
        dgvBills.Width = 980
        dgvBills.Height = 220
        dgvBills.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.ReadOnly = True
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Dim lblPayments As New Label() With {.Text = "Payments", .Left = 20, .Top = 540, .AutoSize = True}
        dgvPayments.Left = 20
        dgvPayments.Top = 565
        dgvPayments.Width = 980
        dgvPayments.Height = 200
        dgvPayments.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvPayments.AllowUserToAddRows = False
        dgvPayments.AllowUserToDeleteRows = False
        dgvPayments.ReadOnly = True
        dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Me.Controls.Add(infoPanel)
        Me.Controls.Add(centerPanel)
        Me.Controls.Add(actionsPanel)
        Me.Controls.Add(lblBills)
        Me.Controls.Add(lblBillFilter)
        Me.Controls.Add(cboBillStatusFilter)
        Me.Controls.Add(dgvBills)
        Me.Controls.Add(lblPayments)
        Me.Controls.Add(dgvPayments)

        AddHandler Me.Load, AddressOf frmCustomerDashboard_Load
    End Sub

    Private Sub frmCustomerDashboard_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()

        If Not String.Equals(CurrentUser.Role, "Customer", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Customer dashboard is only for customer accounts.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        LoadDashboardData()
    End Sub

    Private Sub LoadDashboardData()
        Try
            LoadWelcomeAndMeterInfo()
            LoadBills()
            LoadPayments()
            LoadOutstandingBalance()
            LoadConsumptionChart()
            LoadNotifications()
        Catch ex As Exception
            MessageBox.Show("Failed to load dashboard: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadWelcomeAndMeterInfo()
        Const sql As String = "SELECT u.full_name, c.meter_number, c.last_reading, c.last_reading_date FROM users u LEFT JOIN customers c ON c.id = u.id WHERE u.id = @customer_id LIMIT 1;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }

        Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
        If data.Rows.Count = 0 Then
            lblWelcome.Text = "Welcome"
            lblCurrentReading.Text = "Current Meter Reading: -"
            lblLastReadingDate.Text = "Last Reading Date: -"
            lblMeterNumber.Text = "Meter Number: -"
            Return
        End If

        Dim row As DataRow = data.Rows(0)
        Dim fullName As String = row("full_name").ToString()
        lblWelcome.Text = $"Welcome, {fullName}"
        lblMeterNumber.Text = $"Meter Number: {If(row("meter_number") Is DBNull.Value, "-", row("meter_number").ToString())}"

        Dim currentReading As Decimal = Convert.ToDecimal(If(row("last_reading") Is DBNull.Value, 0D, row("last_reading")))
        lblCurrentReading.Text = $"Current Meter Reading: {currentReading:N2}"

        If row("last_reading_date") Is DBNull.Value Then
            lblLastReadingDate.Text = "Last Reading Date: -"
        Else
            Dim lastDate As Date = Convert.ToDateTime(row("last_reading_date"))
            lblLastReadingDate.Text = $"Last Reading Date: {lastDate:yyyy-MM-dd}"
        End If
    End Sub

    Private Sub LoadBills()
        Dim statusFilter As String = cboBillStatusFilter.Text
        Const sql As String = "SELECT bill_date, due_date, total_amount, status FROM bills WHERE customer_id = @customer_id AND (@status_filter = 'All' OR status = @status_filter) ORDER BY bill_date DESC LIMIT 20;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId},
            {"@status_filter", statusFilter}
        }
        dgvBills.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
    End Sub

    Private Sub LoadOutstandingBalance()
        Const sql As String = "SELECT COALESCE(SUM(b.total_amount - COALESCE(pa.paid_total, 0)), 0) FROM bills b LEFT JOIN (SELECT bill_id, SUM(amount_applied) AS paid_total FROM payment_allocations GROUP BY bill_id) pa ON pa.bill_id = b.id WHERE b.customer_id = @customer_id AND b.status IN ('Unpaid', 'Partial');"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }
        Dim outstanding As Decimal = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(sql, parameters))
        lblOutstandingBalance.Text = $"Outstanding Balance: {outstanding:N2}"
    End Sub

    Private Sub LoadPayments()
        Const sql As String = "SELECT payment_date, amount_paid, mode AS payment_mode FROM payments WHERE customer_id = @customer_id ORDER BY payment_date DESC LIMIT 20;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }
        dgvPayments.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
    End Sub

    Private Sub LoadConsumptionChart()
        Const sql As String = "SELECT DATE_FORMAT(reading_date, '%Y-%m') AS reading_month, COALESCE(SUM(consumption), 0) AS total_consumption FROM meter_readings WHERE customer_id = @customer_id AND reading_date >= DATE_SUB(CURDATE(), INTERVAL 6 MONTH) GROUP BY DATE_FORMAT(reading_date, '%Y-%m') ORDER BY reading_month;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }

        dgvConsumption.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
    End Sub

    Private Sub LoadNotifications()
        lstNotifications.Items.Clear()

        Const dueSql As String = "SELECT COUNT(*) FROM bills WHERE customer_id = @customer_id AND status IN ('Unpaid', 'Partial') AND due_date <= DATE_ADD(CURDATE(), INTERVAL 7 DAY);"
        Dim dueSoon As Integer = Convert.ToInt32(DatabaseHelper.ExecuteScalar(dueSql, New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }))

        If dueSoon > 0 Then
            lstNotifications.Items.Add($"Reminder: {dueSoon} bill(s) are due within 7 days.")
        End If

        Const paymentSql As String = "SELECT payment_date, amount_paid FROM payments WHERE customer_id = @customer_id ORDER BY payment_date DESC LIMIT 1;"
        Dim paymentDt As DataTable = DatabaseHelper.ExecuteDataTable(paymentSql, New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        })
        If paymentDt.Rows.Count > 0 Then
            Dim row As DataRow = paymentDt.Rows(0)
            lstNotifications.Items.Add($"Last payment: {Convert.ToDateTime(row("payment_date")):yyyy-MM-dd} - {Convert.ToDecimal(row("amount_paid")):N2}")
        End If

        lstNotifications.Items.Add("System announcement: Keep your contact details up to date.")
    End Sub

    Private Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        Try
            Dim currentPassword As String = Microsoft.VisualBasic.Interaction.InputBox("Enter current password:", "Change Password")
            If String.IsNullOrWhiteSpace(currentPassword) Then
                Return
            End If

            Dim newPassword As String = Microsoft.VisualBasic.Interaction.InputBox("Enter new password:", "Change Password")
            If String.IsNullOrWhiteSpace(newPassword) Then
                MessageBox.Show("New password cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim confirmPassword As String = Microsoft.VisualBasic.Interaction.InputBox("Confirm new password:", "Change Password")
            If newPassword <> confirmPassword Then
                MessageBox.Show("New password and confirmation do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Const getSql As String = "SELECT password_hash FROM users WHERE id = @user_id LIMIT 1;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@user_id", CurrentUser.UserId}
            }
            Dim hashObj As Object = DatabaseHelper.ExecuteScalar(getSql, parameters)
            If hashObj Is Nothing OrElse hashObj Is DBNull.Value Then
                MessageBox.Show("Unable to verify current password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If Not PasswordHelper.VerifySha256Password(currentPassword, hashObj.ToString()) Then
                MessageBox.Show("Current password is incorrect.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim newHash As String = PasswordHelper.ComputeSha256Hash(newPassword)
            Const updateSql As String = "UPDATE users SET password_hash = @password_hash, force_password_change = 0 WHERE id = @user_id;"
            Dim updateParams As New Dictionary(Of String, Object) From {
                {"@password_hash", newHash},
                {"@user_id", CurrentUser.UserId}
            }

            DatabaseHelper.ExecuteNonQuery(updateSql, updateParams)
            AuditLogger.LogAction(CurrentUser.UserId, "PasswordChanged", "Customer changed password from dashboard")
            MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Failed to change password: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnPayOnline_Click(sender As Object, e As EventArgs)
        Using frm As New frmOnlinePayment(CurrentUser.UserId)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadBills()
                LoadPayments()
                LoadOutstandingBalance()
                LoadConsumptionChart()
                LoadNotifications()
            End If
        End Using
    End Sub

    Private Sub cboBillStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadBills()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
