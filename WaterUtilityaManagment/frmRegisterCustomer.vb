Imports MySql.Data.MySqlClient

Public Class frmRegisterCustomer
    Inherits Form

    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtAddress As New TextBox()
    Private ReadOnly txtPhone As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtMeterNumber As New TextBox()
    Private ReadOnly dtpInstallationDate As New DateTimePicker()
    Private ReadOnly txtInitialMeterReading As New TextBox()
    Private ReadOnly btnRegister As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Register Customer"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 550
        Me.Height = 480
        Me.MinimumSize = New Size(550, 480)

        Dim lblFullName As New Label() With {.Text = "Full Name", .Left = 30, .Top = 30, .AutoSize = True}
        txtFullName.Left = 220
        txtFullName.Top = 25
        txtFullName.Width = 260

        Dim lblAddress As New Label() With {.Text = "Address", .Left = 30, .Top = 70, .AutoSize = True}
        txtAddress.Left = 220
        txtAddress.Top = 65
        txtAddress.Width = 260

        Dim lblPhone As New Label() With {.Text = "Phone", .Left = 30, .Top = 110, .AutoSize = True}
        txtPhone.Left = 220
        txtPhone.Top = 105
        txtPhone.Width = 260

        Dim lblEmail As New Label() With {.Text = "Email", .Left = 30, .Top = 150, .AutoSize = True}
        txtEmail.Left = 220
        txtEmail.Top = 145
        txtEmail.Width = 260

        Dim lblMeter As New Label() With {.Text = "Meter Number", .Left = 30, .Top = 190, .AutoSize = True}
        txtMeterNumber.Left = 220
        txtMeterNumber.Top = 185
        txtMeterNumber.Width = 260

        Dim lblInstallationDate As New Label() With {.Text = "Installation Date", .Left = 30, .Top = 230, .AutoSize = True}
        dtpInstallationDate.Left = 220
        dtpInstallationDate.Top = 225
        dtpInstallationDate.Width = 260
        dtpInstallationDate.Format = DateTimePickerFormat.[Short]

        Dim lblInitialReading As New Label() With {.Text = "Initial Meter Reading", .Left = 30, .Top = 270, .AutoSize = True}
        txtInitialMeterReading.Left = 220
        txtInitialMeterReading.Top = 265
        txtInitialMeterReading.Width = 260

        btnRegister.Text = "Register"
        btnRegister.Left = 220
        btnRegister.Top = 320
        btnRegister.Width = 120
        AddHandler btnRegister.Click, AddressOf btnRegister_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 360
        btnLogout.Top = 320
        btnLogout.Width = 120
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleButton(btnRegister, True)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(lblFullName)
        Me.Controls.Add(txtFullName)
        Me.Controls.Add(lblAddress)
        Me.Controls.Add(txtAddress)
        Me.Controls.Add(lblPhone)
        Me.Controls.Add(txtPhone)
        Me.Controls.Add(lblEmail)
        Me.Controls.Add(txtEmail)
        Me.Controls.Add(lblMeter)
        Me.Controls.Add(txtMeterNumber)
        Me.Controls.Add(lblInstallationDate)
        Me.Controls.Add(dtpInstallationDate)
        Me.Controls.Add(lblInitialReading)
        Me.Controls.Add(txtInitialMeterReading)
        Me.Controls.Add(btnRegister)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmRegisterCustomer_Load
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub

    Private Sub frmRegisterCustomer_Load(sender As Object, e As EventArgs)
        DatabaseHelper.EnsureCoreSchema()
        If Not String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only staff users can register customers.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
        End If
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As EventArgs)
        Try
            Dim fullName As String = txtFullName.Text.Trim()
            Dim address As String = txtAddress.Text.Trim()
            Dim phone As String = txtPhone.Text.Trim()
            Dim email As String = txtEmail.Text.Trim()
            Dim meterNumber As String = txtMeterNumber.Text.Trim()
            Dim installationDate As Date = dtpInstallationDate.Value.Date
            Dim initialReading As Decimal

            If String.IsNullOrWhiteSpace(fullName) OrElse
               String.IsNullOrWhiteSpace(address) OrElse
               String.IsNullOrWhiteSpace(phone) OrElse
               String.IsNullOrWhiteSpace(email) OrElse
               String.IsNullOrWhiteSpace(meterNumber) OrElse
               String.IsNullOrWhiteSpace(txtInitialMeterReading.Text) Then
                MessageBox.Show("Please complete all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            If Not Decimal.TryParse(txtInitialMeterReading.Text.Trim(), initialReading) OrElse initialReading < 0D Then
                MessageBox.Show("Initial meter reading must be a valid non-negative number.",
                                "Validation",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            End If

            Dim defaultPassword As String = "password123"

            Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                Using tx As MySqlTransaction = conn.BeginTransaction()
                    Try
                        If MeterNumberExists(conn, tx, meterNumber) Then
                            MessageBox.Show("Meter number already exists. Please enter a unique meter number.",
                                            "Validation",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning)
                            Return
                        End If

                        Dim generatedUsername As String = GenerateUniqueUsername(conn, tx, email, meterNumber)
                        Dim passwordHash As String = PasswordHelper.ComputeSha256Hash(defaultPassword)

                        Dim userId As Integer = InsertUser(conn, tx, generatedUsername, passwordHash, fullName, email, phone)
                        InsertCustomer(conn, tx, userId, address, meterNumber, installationDate, initialReading)
                        InsertInitialReading(conn, tx, userId, initialReading, CurrentUser.UserId)

                        tx.Commit()

                        AuditLogger.LogAction(CurrentUser.UserId, "RegisterCustomer", $"Customer '{generatedUsername}' created with meter '{meterNumber}'")

                        MessageBox.Show($"Customer registered successfully.{Environment.NewLine}Username: {generatedUsername}{Environment.NewLine}Password: {defaultPassword}",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information)

                        ClearFields()
                    Catch ex As Exception
                        tx.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Registration failed: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function MeterNumberExists(conn As MySqlConnection, tx As MySqlTransaction, meterNumber As String) As Boolean
        Const sql As String = "SELECT COUNT(1) FROM customers WHERE meter_number = @meter_number;"
        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@meter_number", meterNumber)
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Shared Function GenerateUniqueUsername(conn As MySqlConnection,
                                                   tx As MySqlTransaction,
                                                   email As String,
                                                   meterNumber As String) As String
        Dim baseUsername As String = meterNumber
        If Not String.IsNullOrWhiteSpace(email) AndAlso email.Contains("@") Then
            baseUsername = email.Substring(0, email.IndexOf("@"c)).Trim()
        End If

        If String.IsNullOrWhiteSpace(baseUsername) Then
            baseUsername = "customer"
        End If

        Dim candidate As String = baseUsername
        Dim suffix As Integer = 1

        While UsernameExists(conn, tx, candidate)
            candidate = $"{baseUsername}{suffix}"
            suffix += 1
        End While

        Return candidate
    End Function

    Private Shared Function UsernameExists(conn As MySqlConnection, tx As MySqlTransaction, username As String) As Boolean
        Const sql As String = "SELECT COUNT(1) FROM users WHERE username = @username;"
        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@username", username)
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Shared Function InsertUser(conn As MySqlConnection,
                                       tx As MySqlTransaction,
                                       username As String,
                                       passwordHash As String,
                                       fullName As String,
                                       email As String,
                                       phone As String) As Integer
        Const sql As String = "INSERT INTO users (username, password_hash, role, full_name, email, phone, force_password_change, is_active) VALUES (@username, @password_hash, 'customer', @full_name, @email, @phone, 1, 1);"

        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@username", username)
            cmd.Parameters.AddWithValue("@password_hash", passwordHash)
            cmd.Parameters.AddWithValue("@full_name", fullName)
            cmd.Parameters.AddWithValue("@email", email)
            cmd.Parameters.AddWithValue("@phone", phone)
            cmd.ExecuteNonQuery()
            Return Convert.ToInt32(cmd.LastInsertedId)
        End Using
    End Function

    Private Shared Sub InsertCustomer(conn As MySqlConnection,
                                      tx As MySqlTransaction,
                                      userId As Integer,
                                      address As String,
                                      meterNumber As String,
                                      installationDate As Date,
                                      initialReading As Decimal)
        Const sql As String = "INSERT INTO customers (id, address, meter_number, installation_date, last_reading, last_reading_date) VALUES (@customer_id, @address, @meter_number, @installation_date, @last_reading, @last_reading_date);"

        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@customer_id", userId)
            cmd.Parameters.AddWithValue("@address", address)
            cmd.Parameters.AddWithValue("@meter_number", meterNumber)
            cmd.Parameters.AddWithValue("@installation_date", installationDate)
            cmd.Parameters.AddWithValue("@last_reading", initialReading)
            cmd.Parameters.AddWithValue("@last_reading_date", Date.Today)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Shared Sub InsertInitialReading(conn As MySqlConnection,
                                            tx As MySqlTransaction,
                                            customerId As Integer,
                                            initialReading As Decimal,
                                            enteredBy As Integer)
        Const sql As String = "INSERT INTO meter_readings (customer_id, reading_value, reading_date, entered_by, consumption) VALUES (@customer_id, @reading_value, @reading_date, @entered_by, @consumption);"

        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@customer_id", customerId)
            cmd.Parameters.AddWithValue("@reading_value", initialReading)
            cmd.Parameters.AddWithValue("@reading_date", Date.Today)
            cmd.Parameters.AddWithValue("@entered_by", enteredBy)
            cmd.Parameters.AddWithValue("@consumption", 0D)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub ClearFields()
        txtFullName.Clear()
        txtAddress.Clear()
        txtPhone.Clear()
        txtEmail.Clear()
        txtMeterNumber.Clear()
        txtInitialMeterReading.Clear()
        dtpInstallationDate.Value = Date.Today
    End Sub
End Class
