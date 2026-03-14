Public Class frmUserManagement
    Inherits Form

    Private ReadOnly pnlTopBar As New Panel()
    Private ReadOnly txtSearch As New TextBox()
    Private ReadOnly cboRoleFilter As New ComboBox()
    Private ReadOnly btnSearch As New Button()
    Private ReadOnly dgvUsers As New DataGridView()
    Private ReadOnly btnAddUser As New Button()
    Private ReadOnly btnResetPassword As New Button()
    Private ReadOnly btnToggleActive As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "User Management"
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
        pnlTopBar.Height = 70
        pnlTopBar.BackColor = Color.White
        pnlTopBar.Padding = New Padding(16, 12, 16, 12)

        Dim lblSearch As New Label() With {.Text = "Search", .Left = 16, .Top = 8, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        txtSearch.Left = 16
        txtSearch.Top = 28
        txtSearch.Width = 280
        txtSearch.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        AddHandler txtSearch.KeyDown, AddressOf txtSearch_KeyDown

        Dim lblRole As New Label() With {.Text = "Role", .Left = 310, .Top = 8, .AutoSize = True, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        cboRoleFilter.Left = 310
        cboRoleFilter.Top = 28
        cboRoleFilter.Width = 150
        cboRoleFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleFilter.Items.AddRange(New Object() {"All", "Manager", "Staff", "Customer"})
        cboRoleFilter.SelectedIndex = 0
        AddHandler cboRoleFilter.SelectedIndexChanged, AddressOf cboRoleFilter_SelectedIndexChanged

        btnSearch.Text = "🔍"
        btnSearch.Left = 470
        btnSearch.Top = 27
        btnSearch.Width = 48
        btnSearch.Height = 32
        btnSearch.FlatStyle = FlatStyle.Flat
        btnSearch.FlatAppearance.BorderSize = 0
        btnSearch.BackColor = ColorTranslator.FromHtml("#3498db")
        btnSearch.ForeColor = Color.White
        btnSearch.Font = New Font("Segoe UI Emoji", 10.0F, FontStyle.Bold)
        AddHandler btnSearch.Click, AddressOf btnSearch_Click

        btnLogout.Text = "Logout"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 20
        btnLogout.Top = 18
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        pnlTopBar.Controls.Add(lblSearch)
        pnlTopBar.Controls.Add(txtSearch)
        pnlTopBar.Controls.Add(lblRole)
        pnlTopBar.Controls.Add(cboRoleFilter)
        pnlTopBar.Controls.Add(btnSearch)
        pnlTopBar.Controls.Add(btnLogout)

        dgvUsers.Dock = DockStyle.Fill
        dgvUsers.AllowUserToAddRows = False
        dgvUsers.AllowUserToDeleteRows = False
        dgvUsers.AllowUserToResizeRows = False
        dgvUsers.ReadOnly = True
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUsers.MultiSelect = False
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvUsers.RowHeadersVisible = False
        dgvUsers.BackgroundColor = Color.White
        dgvUsers.BorderStyle = BorderStyle.None
        dgvUsers.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        AddHandler dgvUsers.CellFormatting, AddressOf dgvUsers_CellFormatting
        AddHandler dgvUsers.DataError, AddressOf dgvUsers_DataError
        AddHandler dgvUsers.CellDoubleClick, AddressOf dgvUsers_CellDoubleClick

        btnAddUser.Text = "➕ Add User"
        btnAddUser.Width = 170
        btnAddUser.Height = 44
        btnAddUser.FlatStyle = FlatStyle.Flat
        btnAddUser.FlatAppearance.BorderSize = 0
        btnAddUser.BackColor = ColorTranslator.FromHtml("#3498db")
        btnAddUser.ForeColor = Color.White
        btnAddUser.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnAddUser.Click, AddressOf btnAddUser_Click

        btnResetPassword.Text = "🔑 Reset Password"
        btnResetPassword.Width = 190
        btnResetPassword.Height = 44
        btnResetPassword.FlatStyle = FlatStyle.Flat
        btnResetPassword.FlatAppearance.BorderSize = 0
        btnResetPassword.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnResetPassword.ForeColor = Color.White
        btnResetPassword.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnResetPassword.Click, AddressOf btnResetPassword_Click

        btnToggleActive.Text = "🔁 Toggle Active"
        btnToggleActive.Width = 180
        btnToggleActive.Height = 44
        btnToggleActive.FlatStyle = FlatStyle.Flat
        btnToggleActive.FlatAppearance.BorderSize = 0
        btnToggleActive.BackColor = ColorTranslator.FromHtml("#95a5a6")
        btnToggleActive.ForeColor = Color.White
        btnToggleActive.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnToggleActive.Click, AddressOf btnToggleActive_Click

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Dock = DockStyle.Bottom
        actionPanel.Height = 68
        actionPanel.Padding = New Padding(16, 10, 16, 10)
        actionPanel.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = False
        actionPanel.Controls.Add(btnAddUser)
        actionPanel.Controls.Add(btnResetPassword)
        actionPanel.Controls.Add(btnToggleActive)

        Dim gridHost As New Panel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16, 12, 16, 8),
            .BackColor = ColorTranslator.FromHtml("#ecf0f1")
        }
        gridHost.Controls.Add(dgvUsers)

        Me.Controls.Add(gridHost)
        Me.Controls.Add(actionPanel)
        Me.Controls.Add(pnlTopBar)

        AddHandler Me.Load, AddressOf frmUserManagement_Load
        AddHandler Me.Resize, AddressOf frmUserManagement_Resize
    End Sub

    Private Sub frmUserManagement_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can manage users.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        DatabaseHelper.EnsureCoreSchema()
        frmUserManagement_Resize(Me, EventArgs.Empty)
        UiStyleHelper.AddDialogCloseButton(Me)
        LoadUsers()
    End Sub

    Private Sub frmUserManagement_Resize(sender As Object, e As EventArgs)
        btnLogout.Left = pnlTopBar.ClientSize.Width - btnLogout.Width - 16
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadUsers(txtSearch.Text.Trim(), cboRoleFilter.Text)
    End Sub

    Private Sub cboRoleFilter_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadUsers(txtSearch.Text.Trim(), cboRoleFilter.Text)
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            LoadUsers(txtSearch.Text.Trim(), cboRoleFilter.Text)
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub LoadUsers(Optional search As String = "", Optional roleFilter As String = "All")
        Try
            Const sql As String = "SELECT id AS user_id, username, full_name, role, email, phone, is_active FROM users WHERE (@role_filter = '' OR LOWER(role) = LOWER(@role_filter)) AND (@search = '' OR username LIKE @keyword OR full_name LIKE @keyword OR email LIKE @keyword OR phone LIKE @keyword) ORDER BY id DESC;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@role_filter", If(String.Equals(roleFilter, "All", StringComparison.OrdinalIgnoreCase), String.Empty, roleFilter)},
                {"@search", search},
                {"@keyword", "%" & search & "%"}
            }

            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql, parameters)

            If dt.Columns.Contains("is_active") AndAlso Not dt.Columns.Contains("status_text") Then
                dt.Columns.Add("status_text", GetType(String))
                For Each row As DataRow In dt.Rows
                    Dim isActiveValue As Boolean = False
                    If row("is_active") IsNot DBNull.Value Then
                        isActiveValue = Convert.ToBoolean(row("is_active"))
                    End If

                    row("status_text") = If(isActiveValue, "Active", "Inactive")
                Next
            End If

            dgvUsers.DataSource = dt

            If dgvUsers.Columns.Contains("user_id") Then
                dgvUsers.Columns("user_id").Visible = False
            End If

            If dgvUsers.Columns.Contains("username") Then dgvUsers.Columns("username").HeaderText = "Username"
            If dgvUsers.Columns.Contains("full_name") Then dgvUsers.Columns("full_name").HeaderText = "Full Name"
            If dgvUsers.Columns.Contains("role") Then dgvUsers.Columns("role").HeaderText = "Role"
            If dgvUsers.Columns.Contains("email") Then dgvUsers.Columns("email").HeaderText = "Email"
            If dgvUsers.Columns.Contains("phone") Then dgvUsers.Columns("phone").HeaderText = "Phone"
            If dgvUsers.Columns.Contains("is_active") Then
                dgvUsers.Columns("is_active").Visible = False
            End If

            If dgvUsers.Columns.Contains("status_text") Then
                dgvUsers.Columns("status_text").HeaderText = "Is Active"
            End If
        Catch ex As Exception
            MessageBox.Show("Failed to load users: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvUsers_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        If dgvUsers.Columns.Contains("status_text") AndAlso e.ColumnIndex = dgvUsers.Columns("status_text").Index Then
            Dim isActive As Boolean = False
            Dim statusRow As DataGridViewRow = dgvUsers.Rows(e.RowIndex)
            If dgvUsers.Columns.Contains("is_active") Then
                Boolean.TryParse(Convert.ToString(statusRow.Cells("is_active").Value), isActive)
            End If

            e.Value = If(isActive, "Active", "Inactive")
            e.CellStyle.ForeColor = Color.White
            e.CellStyle.BackColor = If(isActive, ColorTranslator.FromHtml("#27ae60"), ColorTranslator.FromHtml("#7f8c8d"))
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor
            e.CellStyle.SelectionForeColor = Color.White
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            e.FormattingApplied = True
        End If

        Dim row As DataGridViewRow = dgvUsers.Rows(e.RowIndex)
        Dim rowActive As Boolean = True
        If dgvUsers.Columns.Contains("is_active") Then
            rowActive = Convert.ToString(row.Cells("is_active").Value).ToLowerInvariant().Contains("true") OrElse
                        Convert.ToString(row.Cells("is_active").Value).Equals("Active", StringComparison.OrdinalIgnoreCase)
        End If

        If Not rowActive Then
            row.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#95a5a6")
            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f2f3f4")
        End If
    End Sub

    Private Sub dgvUsers_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        btnToggleActive_Click(btnToggleActive, EventArgs.Empty)
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs)
        Using frm As New frmAddUser(True)
            If frm.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                Const checkSql As String = "SELECT COUNT(*) FROM users WHERE username = @username;"
                Dim existsObj As Object = DatabaseHelper.ExecuteScalar(checkSql, New Dictionary(Of String, Object) From {
                    {"@username", frm.UsernameValue}
                })
                If Convert.ToInt32(existsObj) > 0 Then
                    MessageBox.Show("Username already exists.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                Const insertSql As String = "INSERT INTO users (username, password_hash, role, full_name, email, phone, is_active) VALUES (@username, @password_hash, @role, @full_name, @email, @phone, @is_active);"
                Dim parameters As New Dictionary(Of String, Object) From {
                    {"@username", frm.UsernameValue},
                    {"@password_hash", PasswordHelper.ComputeSha256Hash(frm.PasswordValue)},
                    {"@role", "Staff"},
                    {"@full_name", frm.FullNameValue},
                    {"@email", If(String.IsNullOrWhiteSpace(frm.EmailValue), Nothing, frm.EmailValue)},
                    {"@phone", If(String.IsNullOrWhiteSpace(frm.PhoneValue), Nothing, frm.PhoneValue)},
                    {"@is_active", True}
                }

                DatabaseHelper.ExecuteNonQuery(insertSql, parameters)
                AuditLogger.LogAction(CurrentUser.UserId, "UserAdded", $"Username={frm.UsernameValue}, Role={frm.RoleValue}")
                MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadUsers(txtSearch.Text.Trim(), cboRoleFilter.Text)
            Catch ex As Exception
                MessageBox.Show("Failed to add user: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnResetPassword_Click(sender As Object, e As EventArgs)
        Dim selected As DataGridViewRow = GetSelectedRow()
        If selected Is Nothing Then
            MessageBox.Show("Please select a user.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim userId As Integer = Convert.ToInt32(selected.Cells("user_id").Value)
        Dim username As String = selected.Cells("username").Value.ToString()
        Dim tempPassword As String = GenerateTemporaryPassword()

        Try
            Const sql As String = "UPDATE users SET password_hash = @password_hash, force_password_change = 1 WHERE id = @user_id;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@password_hash", PasswordHelper.ComputeSha256Hash(tempPassword)},
                {"@user_id", userId}
            }
            DatabaseHelper.ExecuteNonQuery(sql, parameters)
            AuditLogger.LogAction(CurrentUser.UserId, "PasswordReset", $"TargetUser={username}")

            MessageBox.Show($"Password reset successful for '{username}'.{Environment.NewLine}Temporary password: {tempPassword}{Environment.NewLine}Ask user to change it at next login.",
                            "Password Reset",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Failed to reset password: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnToggleActive_Click(sender As Object, e As EventArgs)
        Dim selected As DataGridViewRow = GetSelectedRow()
        If selected Is Nothing Then
            MessageBox.Show("Please select a user.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim userId As Integer = Convert.ToInt32(selected.Cells("user_id").Value)
        Dim username As String = selected.Cells("username").Value.ToString()
        Dim isActive As Boolean = Convert.ToBoolean(selected.Cells("is_active").Value)
        Dim newStatus As Boolean = Not isActive

        Dim prompt As String = If(newStatus,
                                  $"Enable user '{username}'?",
                                  $"Disable user '{username}'?")

        If MessageBox.Show(prompt, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return
        End If

        Try
            Const sql As String = "UPDATE users SET is_active = @is_active WHERE id = @user_id;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@is_active", newStatus},
                {"@user_id", userId}
            }
            DatabaseHelper.ExecuteNonQuery(sql, parameters)
            AuditLogger.LogAction(CurrentUser.UserId, If(newStatus, "UserEnabled", "UserDisabled"), $"TargetUser={username}")
            LoadUsers(txtSearch.Text.Trim(), cboRoleFilter.Text)
        Catch ex As Exception
            MessageBox.Show("Failed to update user status: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSelectedRow() As DataGridViewRow
        If dgvUsers.SelectedRows.Count = 0 Then
            Return Nothing
        End If

        Return dgvUsers.SelectedRows(0)
    End Function

    Private Shared Function GenerateTemporaryPassword() As String
        Dim rng As New Random()
        Return "Temp" & rng.Next(100000, 999999).ToString()
    End Function

    Private Sub dgvUsers_DataError(sender As Object, e As DataGridViewDataErrorEventArgs)
        e.ThrowException = False
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
