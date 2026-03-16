Imports MySql.Data.MySqlClient
Imports System.ComponentModel
Imports System.Linq

Public Class frmCustomerDashboard
    Inherits Form

    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly lblTopMeter As New Label()
    Private ReadOnly pnlMainCard As New RoundedPanel()
    Private ReadOnly pnlSidebar As New Panel()
    Private ReadOnly tabMain As New TabControl()
    Private ReadOnly tabBills As New TabPage("My Bills")
    Private ReadOnly tabPayments As New TabPage("My Payments")
    Private ReadOnly tabConsumption As New TabPage("Consumption")
    Private ReadOnly pnlConsumptionChart As New Panel()
    Private ReadOnly _consumptionPoints As New List(Of Decimal)()
    Private ReadOnly _consumptionLabels As New List(Of String)()

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
    Private ReadOnly btnProfile As New Button()
    Private ReadOnly btnPayOnline As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Customer Dashboard"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        Me.DoubleBuffered = True

        pnlTopBar.Dock = DockStyle.Top
        pnlTopBar.Height = 64
        pnlTopBar.BackColor = Color.FromArgb(30, Color.White)
        pnlTopBar.Padding = New Padding(16, 12, 16, 10)

        lblWelcome.AutoSize = True
        lblWelcome.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblWelcome.ForeColor = Color.White
        lblWelcome.Left = 16
        lblWelcome.Top = 20

        lblTopMeter.AutoSize = True
        lblTopMeter.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        lblTopMeter.ForeColor = Color.White
        lblTopMeter.Left = 350
        lblTopMeter.Top = 23

        btnLogout.Text = "Logout"
        btnLogout.Width = 100
        btnLogout.Height = 34
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 20
        btnLogout.Top = 15
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        pnlTopBar.Controls.Add(lblWelcome)
        pnlTopBar.Controls.Add(lblTopMeter)
        pnlTopBar.Controls.Add(btnLogout)

        pnlMainCard.Dock = DockStyle.Fill
        pnlMainCard.BackColor = Color.White
        pnlMainCard.Padding = New Padding(16)
        pnlMainCard.CornerRadius = 18

        Dim summaryPanel As New FlowLayoutPanel() With {.Dock = DockStyle.Top, .Height = 92, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False}
        lblCurrentReading.AutoSize = True
        lblCurrentReading.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblCurrentReading.ForeColor = Color.White
        Dim boxCurrent As Panel = CreateInfoBox(ColorTranslator.FromHtml("#3498db"), "Current Reading", lblCurrentReading)

        lblLastReadingDate.AutoSize = True
        lblLastReadingDate.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblLastReadingDate.ForeColor = Color.White
        Dim boxLastDate As Panel = CreateInfoBox(ColorTranslator.FromHtml("#16a085"), "Last Reading Date", lblLastReadingDate)

        lblOutstandingBalance.AutoSize = True
        lblOutstandingBalance.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        lblOutstandingBalance.ForeColor = Color.White
        Dim boxOutstanding As Panel = CreateInfoBox(ColorTranslator.FromHtml("#e67e22"), "Outstanding Balance", lblOutstandingBalance)

        summaryPanel.Controls.Add(boxCurrent)
        summaryPanel.Controls.Add(boxLastDate)
        summaryPanel.Controls.Add(boxOutstanding)

        tabMain.Dock = DockStyle.Fill
        tabMain.Appearance = TabAppearance.FlatButtons

        Dim billsTopPanel As New Panel() With {.Dock = DockStyle.Top, .Height = 42}
        Dim lblBillFilter As New Label() With {.Text = "Status", .Left = 8, .Top = 12, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        cboBillStatusFilter.Left = 62
        cboBillStatusFilter.Top = 8
        cboBillStatusFilter.Width = 150
        cboBillStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboBillStatusFilter.Items.AddRange(New Object() {"All", "Unpaid", "Partial", "Paid"})
        cboBillStatusFilter.SelectedIndex = 0
        AddHandler cboBillStatusFilter.SelectedIndexChanged, AddressOf cboBillStatusFilter_SelectedIndexChanged
        billsTopPanel.Controls.Add(lblBillFilter)
        billsTopPanel.Controls.Add(cboBillStatusFilter)

        dgvBills.Dock = DockStyle.Fill
        dgvBills.AllowUserToAddRows = False
        dgvBills.AllowUserToDeleteRows = False
        dgvBills.ReadOnly = True
        dgvBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        AddHandler dgvBills.CellFormatting, AddressOf dgvBills_CellFormatting

        tabBills.Controls.Add(dgvBills)
        tabBills.Controls.Add(billsTopPanel)

        dgvPayments.Dock = DockStyle.Fill
        dgvPayments.AllowUserToAddRows = False
        dgvPayments.AllowUserToDeleteRows = False
        dgvPayments.ReadOnly = True
        dgvPayments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        tabPayments.Controls.Add(dgvPayments)

        pnlConsumptionChart.Dock = DockStyle.Fill
        pnlConsumptionChart.BackColor = Color.White
        AddHandler pnlConsumptionChart.Paint, AddressOf pnlConsumptionChart_Paint
        tabConsumption.Controls.Add(pnlConsumptionChart)

        tabMain.TabPages.Add(tabBills)
        tabMain.TabPages.Add(tabPayments)
        tabMain.TabPages.Add(tabConsumption)

        pnlSidebar.Dock = DockStyle.Right
        pnlSidebar.Width = 290
        pnlSidebar.Padding = New Padding(10, 0, 0, 0)
        pnlSidebar.BackColor = Color.Transparent
        Dim lblNotif As New Label() With {.Text = "Notifications", .AutoSize = True, .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold), .ForeColor = ColorTranslator.FromHtml("#2c3e50"), .Top = 4}
        lstNotifications.Top = 28
        lstNotifications.Width = 270
        lstNotifications.Height = 460
        lstNotifications.DrawMode = DrawMode.OwnerDrawFixed
        lstNotifications.ItemHeight = 26
        AddHandler lstNotifications.DrawItem, AddressOf lstNotifications_DrawItem
        pnlSidebar.Controls.Add(lblNotif)
        pnlSidebar.Controls.Add(lstNotifications)

        Dim actionsPanel As New FlowLayoutPanel() With {.Dock = DockStyle.Bottom, .Height = 86, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False}
        btnChangePassword.Text = "🔐 Change Password"
        btnChangePassword.Width = 240
        btnChangePassword.Height = 56
        btnChangePassword.FlatStyle = FlatStyle.Flat
        btnChangePassword.FlatAppearance.BorderSize = 0
        btnChangePassword.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnChangePassword.ForeColor = Color.White
        btnChangePassword.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        btnProfile.Text = "👤 Profile Settings"
        btnProfile.Width = 240
        btnProfile.Height = 56
        btnProfile.FlatStyle = FlatStyle.Flat
        btnProfile.FlatAppearance.BorderSize = 0
        btnProfile.BackColor = ColorTranslator.FromHtml("#3498db")
        btnProfile.ForeColor = Color.White
        btnProfile.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnProfile.Click, AddressOf btnProfile_Click

        btnPayOnline.Text = "💳 Pay Online"
        btnPayOnline.Width = 240
        btnPayOnline.Height = 56
        btnPayOnline.FlatStyle = FlatStyle.Flat
        btnPayOnline.FlatAppearance.BorderSize = 0
        btnPayOnline.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnPayOnline.ForeColor = Color.White
        btnPayOnline.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        AddHandler btnPayOnline.Click, AddressOf btnPayOnline_Click
        actionsPanel.Controls.Add(btnChangePassword)
        actionsPanel.Controls.Add(btnProfile)
        actionsPanel.Controls.Add(btnPayOnline)

        pnlMainCard.Controls.Add(tabMain)
        pnlMainCard.Controls.Add(actionsPanel)
        pnlMainCard.Controls.Add(pnlSidebar)
        pnlMainCard.Controls.Add(summaryPanel)

        Me.Controls.Add(pnlMainCard)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmCustomerDashboard_Load
        AddHandler Me.Resize, AddressOf frmCustomerDashboard_Resize
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

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Using brush As New Drawing2D.LinearGradientBrush(Me.ClientRectangle,
                                                          ColorTranslator.FromHtml("#2c3e50"),
                                                          ColorTranslator.FromHtml("#3498db"),
                                                          Drawing2D.LinearGradientMode.Vertical)
            e.Graphics.FillRectangle(brush, Me.ClientRectangle)
        End Using
    End Sub

    Private Sub btnProfile_Click(sender As Object, e As EventArgs)
        Using frm As New frmProfileSettings()
            If frm.ShowDialog(Me) = DialogResult.OK Then
                LoadWelcomeAndMeterInfo()
            End If
        End Using
    End Sub

    Private Sub frmCustomerDashboard_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 16
        lblTopMeter.Left = Math.Max(lblWelcome.Right + 30, (pnlTopBar.ClientSize.Width \ 2) - 100)
    End Sub

    Private Shared Function CreateInfoBox(backColor As Color, title As String, valueLabel As Label) As Panel
        Dim box As New Panel() With {.Width = 260, .Height = 78, .BackColor = backColor, .Margin = New Padding(0, 0, 10, 0)}
        Dim lblTitle As New Label() With {.Text = title, .AutoSize = True, .ForeColor = Color.White, .Font = New Font("Segoe UI", 9.0F, FontStyle.Regular), .Left = 10, .Top = 8}
        valueLabel.Left = 10
        valueLabel.Top = 35
        box.Controls.Add(lblTitle)
        box.Controls.Add(valueLabel)
        Return box
    End Function

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
        lblTopMeter.Text = lblMeterNumber.Text

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

        Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)
        _consumptionPoints.Clear()
        _consumptionLabels.Clear()
        For Each row As DataRow In dt.Rows
            _consumptionLabels.Add(Convert.ToString(row("reading_month")))
            _consumptionPoints.Add(Convert.ToDecimal(row("total_consumption")))
        Next
        pnlConsumptionChart.Invalidate()
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

    Private Sub dgvBills_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not dgvBills.Columns.Contains("status") OrElse e.ColumnIndex <> dgvBills.Columns("status").Index Then
            Return
        End If

        Dim statusText As String = Convert.ToString(e.Value)
        Select Case statusText.Trim().ToLowerInvariant()
            Case "paid"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#27ae60")
            Case "unpaid"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#d35400")
            Case "partial"
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#2980b9")
        End Select
        e.CellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
    End Sub

    Private Sub lstNotifications_DrawItem(sender As Object, e As DrawItemEventArgs)
        e.DrawBackground()
        If e.Index < 0 Then
            Return
        End If

        Dim itemText As String = lstNotifications.Items(e.Index).ToString()
        Dim icon As String = If(itemText.StartsWith("Reminder", StringComparison.OrdinalIgnoreCase), "⚠", "ℹ")
        TextRenderer.DrawText(e.Graphics,
                              $"{icon} {itemText}",
                              New Font("Segoe UI", 9.0F, FontStyle.Regular),
                              e.Bounds,
                              ColorTranslator.FromHtml("#2c3e50"),
                              TextFormatFlags.Left Or TextFormatFlags.VerticalCenter)
        e.DrawFocusRectangle()
    End Sub

    Private Sub pnlConsumptionChart_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.Clear(Color.White)

        If _consumptionPoints.Count = 0 Then
            TextRenderer.DrawText(e.Graphics,
                                  "No consumption data",
                                  New Font("Segoe UI", 10.0F, FontStyle.Bold),
                                  pnlConsumptionChart.ClientRectangle,
                                  ColorTranslator.FromHtml("#7f8c8d"),
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            Return
        End If

        Dim maxVal As Decimal = Math.Max(1D, _consumptionPoints.Max())
        Dim points As New List(Of PointF)()
        Dim chartRect As New RectangleF(46, 20, pnlConsumptionChart.Width - 70, pnlConsumptionChart.Height - 56)

        For i As Integer = 0 To _consumptionPoints.Count - 1
            Dim x As Single = chartRect.Left + (i * (chartRect.Width / Math.Max(1, _consumptionPoints.Count - 1)))
            Dim y As Single = chartRect.Bottom - CSng((_consumptionPoints(i) / maxVal) * chartRect.Height)
            points.Add(New PointF(x, y))
        Next

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        Using axisPen As New Pen(ColorTranslator.FromHtml("#bdc3c7"), 1)
            e.Graphics.DrawLine(axisPen, chartRect.Left, chartRect.Bottom, chartRect.Right, chartRect.Bottom)
            e.Graphics.DrawLine(axisPen, chartRect.Left, chartRect.Top, chartRect.Left, chartRect.Bottom)
        End Using

        Using linePen As New Pen(ColorTranslator.FromHtml("#3498db"), 2)
            If points.Count > 1 Then
                e.Graphics.DrawLines(linePen, points.ToArray())
            End If
        End Using

        For i As Integer = 0 To points.Count - 1
            e.Graphics.FillEllipse(Brushes.White, points(i).X - 3, points(i).Y - 3, 6, 6)
            e.Graphics.DrawEllipse(Pens.SteelBlue, points(i).X - 3, points(i).Y - 3, 6, 6)
            TextRenderer.DrawText(e.Graphics,
                                  _consumptionLabels(i),
                                  New Font("Segoe UI", 8.0F, FontStyle.Regular),
                                  New Point(CInt(points(i).X - 24), CInt(chartRect.Bottom + 8)),
                                  ColorTranslator.FromHtml("#2c3e50"))
        Next
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
                UpdatePanelRegion()
                Me.Invalidate()
            End Set
        End Property

        Protected Overrides Sub OnSizeChanged(e As EventArgs)
            MyBase.OnSizeChanged(e)
            UpdatePanelRegion()
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
            e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

            Using path As Drawing2D.GraphicsPath = CreateRoundedPath(Me.ClientRectangle, _cornerRadius)
                Using borderPen As New Pen(Color.FromArgb(220, ColorTranslator.FromHtml("#d5d8dc")), 1)
                    e.Graphics.DrawPath(borderPen, path)
                End Using
            End Using
        End Sub

        Private Sub UpdatePanelRegion()
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
