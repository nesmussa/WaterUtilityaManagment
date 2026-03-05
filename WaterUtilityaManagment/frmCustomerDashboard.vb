Imports MySql.Data.MySqlClient

Public Class frmCustomerDashboard
    Inherits Form

    Private ReadOnly lblWelcome As New Label()
    Private ReadOnly lblCurrentReading As New Label()
    Private ReadOnly lblLastReadingDate As New Label()
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
        Me.Height = 680

        lblWelcome.Left = 20
        lblWelcome.Top = 20
        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font(lblWelcome.Font, FontStyle.Bold)

        lblCurrentReading.Left = 20
        lblCurrentReading.Top = 60
        lblCurrentReading.AutoSize = True

        lblLastReadingDate.Left = 20
        lblLastReadingDate.Top = 90
        lblLastReadingDate.AutoSize = True

        btnChangePassword.Text = "Change Password"
        btnChangePassword.Left = 800
        btnChangePassword.Top = 20
        btnChangePassword.Width = 200
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        btnPayOnline.Text = "Pay Online"
        btnPayOnline.Left = 800
        btnPayOnline.Top = 55
        btnPayOnline.Width = 200
        AddHandler btnPayOnline.Click, AddressOf btnPayOnline_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 800
        btnLogout.Top = 90
        btnLogout.Width = 200
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Dim lblBills As New Label() With {.Text = "Recent Bills", .Left = 20, .Top = 130, .AutoSize = True}
        dgvBills.Left = 20
        dgvBills.Top = 155
        dgvBills.Width = 980
        dgvBills.Height = 220
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.ReadOnly = True
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Dim lblPayments As New Label() With {.Text = "Payments", .Left = 20, .Top = 390, .AutoSize = True}
        dgvPayments.Left = 20
        dgvPayments.Top = 415
        dgvPayments.Width = 980
        dgvPayments.Height = 200
        dgvPayments.AllowUserToAddRows = False
        dgvPayments.AllowUserToDeleteRows = False
        dgvPayments.ReadOnly = True
        dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        Me.Controls.Add(lblWelcome)
        Me.Controls.Add(lblCurrentReading)
        Me.Controls.Add(lblLastReadingDate)
        Me.Controls.Add(btnChangePassword)
        Me.Controls.Add(btnPayOnline)
        Me.Controls.Add(btnLogout)
        Me.Controls.Add(lblBills)
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
        Catch ex As Exception
            MessageBox.Show("Failed to load dashboard: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadWelcomeAndMeterInfo()
        Const sql As String = "SELECT u.full_name, c.last_reading, c.last_reading_date FROM users u LEFT JOIN customers c ON c.id = u.id WHERE u.id = @customer_id LIMIT 1;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }

        Dim data As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
        If data.Rows.Count = 0 Then
            lblWelcome.Text = "Welcome"
            lblCurrentReading.Text = "Current Meter Reading: -"
            lblLastReadingDate.Text = "Last Reading Date: -"
            Return
        End If

        Dim row As DataRow = data.Rows(0)
        Dim fullName As String = row("full_name").ToString()
        lblWelcome.Text = $"Welcome, {fullName}"

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
        Const sql As String = "SELECT bill_date, due_date, total_amount, status FROM bills WHERE customer_id = @customer_id ORDER BY bill_date DESC LIMIT 20;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }
        dgvBills.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
    End Sub

    Private Sub LoadPayments()
        Const sql As String = "SELECT payment_date, amount_paid, mode AS payment_mode FROM payments WHERE customer_id = @customer_id ORDER BY payment_date DESC LIMIT 20;"
        Dim parameters As New Dictionary(Of String, Object) From {
            {"@customer_id", CurrentUser.UserId}
        }
        dgvPayments.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
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
            End If
        End Using
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
