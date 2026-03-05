Public Class frmUserManagement
    Inherits Form

    Private ReadOnly txtSearch As New TextBox()
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
        Me.Text = "Staff Management"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 1050
        Me.Height = 620

        Dim lblSearch As New Label() With {.Text = "Search", .Left = 20, .Top = 22, .AutoSize = True}
        txtSearch.Left = 75
        txtSearch.Top = 18
        txtSearch.Width = 280
        AddHandler txtSearch.KeyDown, AddressOf txtSearch_KeyDown

        btnSearch.Text = "Search"
        btnSearch.Left = 365
        btnSearch.Top = 17
        btnSearch.Width = 85
        AddHandler btnSearch.Click, AddressOf btnSearch_Click

        dgvUsers.Left = 20
        dgvUsers.Top = 55
        dgvUsers.Width = 990
        dgvUsers.Height = 470
        dgvUsers.AllowUserToAddRows = False
        dgvUsers.AllowUserToDeleteRows = False
        dgvUsers.ReadOnly = True
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvUsers.MultiSelect = False
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        btnAddUser.Text = "Add Staff"
        btnAddUser.Left = 20
        btnAddUser.Top = 540
        btnAddUser.Width = 120
        AddHandler btnAddUser.Click, AddressOf btnAddUser_Click

        btnResetPassword.Text = "Reset Password"
        btnResetPassword.Left = 155
        btnResetPassword.Top = 540
        btnResetPassword.Width = 140
        AddHandler btnResetPassword.Click, AddressOf btnResetPassword_Click

        btnToggleActive.Text = "Disable/Enable"
        btnToggleActive.Left = 310
        btnToggleActive.Top = 540
        btnToggleActive.Width = 140
        AddHandler btnToggleActive.Click, AddressOf btnToggleActive_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 465
        btnLogout.Top = 540
        btnLogout.Width = 120
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Me.Controls.Add(lblSearch)
        Me.Controls.Add(txtSearch)
        Me.Controls.Add(btnSearch)
        Me.Controls.Add(dgvUsers)
        Me.Controls.Add(btnAddUser)
        Me.Controls.Add(btnResetPassword)
        Me.Controls.Add(btnToggleActive)
        Me.Controls.Add(btnLogout)

        AddHandler Me.Load, AddressOf frmUserManagement_Load
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
        LoadUsers()
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs)
        LoadUsers(txtSearch.Text.Trim())
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            LoadUsers(txtSearch.Text.Trim())
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub LoadUsers(Optional search As String = "")
        Try
            Const sql As String = "SELECT id AS user_id, username, role, full_name, email, phone, is_active FROM users WHERE LOWER(role) = 'staff' AND (@search = '' OR username LIKE @keyword OR full_name LIKE @keyword OR email LIKE @keyword OR phone LIKE @keyword) ORDER BY id DESC;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@search", search},
                {"@keyword", "%" & search & "%"}
            }

            dgvUsers.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
        Catch ex As Exception
            MessageBox.Show("Failed to load users: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
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
                LoadUsers(txtSearch.Text.Trim())
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
            LoadUsers(txtSearch.Text.Trim())
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

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
