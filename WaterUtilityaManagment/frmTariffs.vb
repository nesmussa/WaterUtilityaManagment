Imports MySql.Data.MySqlClient

Public Class frmTariffs
    Inherits Form

    Private ReadOnly dgvTariffs As New DataGridView()
    Private ReadOnly btnEdit As New Button()
    Private ReadOnly btnDeactivate As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Tariff Management"
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

        Dim topBar As New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 64,
            .BackColor = Color.White,
            .Padding = New Padding(16, 12, 16, 12)
        }

        Dim lblTitle As New Label() With {
            .Text = "Tariff Rates",
            .AutoSize = True,
            .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#2c3e50"),
            .Location = New Point(16, 16)
        }

        btnLogout.Text = "Logout"
        btnLogout.Width = 95
        btnLogout.Height = 34
        btnLogout.Left = Me.ClientSize.Width - btnLogout.Width - 20
        btnLogout.Top = 14
        btnLogout.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.FlatAppearance.BorderSize = 0
        btnLogout.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnLogout.ForeColor = Color.White
        btnLogout.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        topBar.Controls.Add(lblTitle)
        topBar.Controls.Add(btnLogout)

        Dim mainPanel As New Panel() With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(16),
            .BackColor = ColorTranslator.FromHtml("#ecf0f1")
        }

        Dim contentPanel As New Panel() With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(14)
        }

        dgvTariffs.Dock = DockStyle.Fill
        dgvTariffs.AllowUserToAddRows = False
        dgvTariffs.AllowUserToDeleteRows = False
        dgvTariffs.AllowUserToResizeRows = False
        dgvTariffs.ReadOnly = True
        dgvTariffs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvTariffs.MultiSelect = False
        dgvTariffs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvTariffs.RowHeadersVisible = False
        dgvTariffs.BackgroundColor = Color.White
        dgvTariffs.BorderStyle = BorderStyle.None
        dgvTariffs.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvTariffs.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        AddHandler dgvTariffs.CellFormatting, AddressOf dgvTariffs_CellFormatting
        AddHandler dgvTariffs.CellDoubleClick, AddressOf dgvTariffs_CellDoubleClick

        btnEdit.Text = "✏ Edit"
        btnEdit.Width = 130
        btnEdit.Height = 38
        btnEdit.FlatStyle = FlatStyle.Flat
        btnEdit.FlatAppearance.BorderSize = 0
        btnEdit.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnEdit.ForeColor = Color.White
        btnEdit.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnEdit.Click, AddressOf btnEdit_Click

        btnDeactivate.Text = "🛑 Deactivate"
        btnDeactivate.Width = 160
        btnDeactivate.Height = 38
        btnDeactivate.FlatStyle = FlatStyle.Flat
        btnDeactivate.FlatAppearance.BorderSize = 0
        btnDeactivate.BackColor = ColorTranslator.FromHtml("#95a5a6")
        btnDeactivate.ForeColor = Color.White
        btnDeactivate.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnDeactivate.Click, AddressOf btnDeactivate_Click

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Dock = DockStyle.Bottom
        actionPanel.Height = 50
        actionPanel.Padding = New Padding(0, 4, 0, 0)
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = False
        actionPanel.Controls.Add(btnEdit)
        actionPanel.Controls.Add(btnDeactivate)

        contentPanel.Controls.Add(dgvTariffs)
        contentPanel.Controls.Add(actionPanel)
        mainPanel.Controls.Add(contentPanel)

        Me.Controls.Add(mainPanel)
        Me.Controls.Add(topBar)

        AddHandler Me.Load, AddressOf frmTariffs_Load
        AddHandler Me.Resize, Sub() btnLogout.Left = topBar.ClientSize.Width - btnLogout.Width - 16
    End Sub

    Private Sub frmTariffs_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Manager", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only managers can manage tariffs.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        UiStyleHelper.AddDialogCloseButton(Me)
        LoadTariffs()
    End Sub

    Private Sub LoadTariffs()
        Try
            Const sql As String = "SELECT tariff_id, rate_per_unit AS rate, effective_from, effective_to FROM tariffs ORDER BY effective_from DESC;"
            Dim dt As DataTable = DatabaseHelper.ExecuteDataTable(sql)
            dgvTariffs.DataSource = dt

            If dgvTariffs.Columns.Contains("tariff_id") Then
                dgvTariffs.Columns("tariff_id").Visible = False
            End If

            If dgvTariffs.Columns.Contains("rate") Then
                dgvTariffs.Columns("rate").HeaderText = "Rate"
                dgvTariffs.Columns("rate").DefaultCellStyle.Format = "N2"
            End If

            If dgvTariffs.Columns.Contains("effective_from") Then
                dgvTariffs.Columns("effective_from").HeaderText = "Effective From"
                dgvTariffs.Columns("effective_from").DefaultCellStyle.Format = "yyyy-MM-dd"
            End If

            If dgvTariffs.Columns.Contains("effective_to") Then
                dgvTariffs.Columns("effective_to").HeaderText = "Effective To"
                dgvTariffs.Columns("effective_to").DefaultCellStyle.Format = "yyyy-MM-dd"
            End If
        Catch ex As Exception
            MessageBox.Show("Failed to load tariffs: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvTariffs_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.RowIndex < 0 OrElse Not dgvTariffs.Columns.Contains("effective_to") Then
            Return
        End If

        Dim row As DataGridViewRow = dgvTariffs.Rows(e.RowIndex)
        Dim effectiveToValue As Object = row.Cells("effective_to").Value
        Dim isActive As Boolean = effectiveToValue Is Nothing OrElse effectiveToValue Is DBNull.Value

        If isActive Then
            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#eafaf1")
        Else
            row.DefaultCellStyle.BackColor = Color.White
        End If
    End Sub

    Private Sub dgvTariffs_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 Then
            Return
        End If

        btnEdit_Click(btnEdit, EventArgs.Empty)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs)
        Using frm As New frmTariffEdit()
            If frm.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                    Using tx As MySqlTransaction = conn.BeginTransaction()
                        Try
                            If frm.EffectiveTo.HasValue AndAlso IsOverlappingTariff(conn, tx, Nothing, frm.EffectiveFrom, frm.EffectiveTo.Value) Then
                                Throw New ApplicationException("The selected period overlaps with an existing tariff period.")
                            End If

                            If Not frm.EffectiveTo.HasValue Then
                                CloseCurrentActiveTariff(conn, tx, frm.EffectiveFrom)
                                If IsOverlappingTariff(conn, tx, Nothing, frm.EffectiveFrom, Nothing) Then
                                    Throw New ApplicationException("The selected period overlaps with an existing tariff period.")
                                End If
                            End If

                            Const insertSql As String = "INSERT INTO tariffs (rate_per_unit, effective_from, effective_to) VALUES (@rate, @effective_from, @effective_to);"
                            Using cmd As New MySqlCommand(insertSql, conn, tx)
                                cmd.Parameters.AddWithValue("@rate", frm.TariffRate)
                                cmd.Parameters.AddWithValue("@effective_from", frm.EffectiveFrom)
                                cmd.Parameters.AddWithValue("@effective_to", If(frm.EffectiveTo.HasValue, CType(frm.EffectiveTo.Value, Object), DBNull.Value))
                                cmd.ExecuteNonQuery()
                            End Using

                            tx.Commit()
                        Catch
                            tx.Rollback()
                            Throw
                        End Try
                    End Using
                End Using

                LoadTariffs()
                AuditLogger.LogAction(CurrentUser.UserId, "TariffAdded", $"Rate={frm.TariffRate}, From={frm.EffectiveFrom:yyyy-MM-dd}")
                MessageBox.Show("Tariff added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Failed to add tariff: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs)
        Dim selected As DataGridViewRow = GetSelectedRow()
        If selected Is Nothing Then
            MessageBox.Show("Please select a tariff to edit.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim tariffId As Integer = Convert.ToInt32(selected.Cells("tariff_id").Value)
        Dim rate As Decimal = Convert.ToDecimal(selected.Cells("rate").Value)
        Dim effectiveFrom As Date = Convert.ToDateTime(selected.Cells("effective_from").Value)
        Dim effectiveTo As Nullable(Of Date) = Nothing
        If selected.Cells("effective_to").Value IsNot DBNull.Value AndAlso selected.Cells("effective_to").Value IsNot Nothing Then
            effectiveTo = Convert.ToDateTime(selected.Cells("effective_to").Value)
        End If

        Using frm As New frmTariffEdit(rate, effectiveFrom, effectiveTo)
            If frm.ShowDialog() <> DialogResult.OK Then
                Return
            End If

            Try
                Using conn As MySqlConnection = DatabaseHelper.GetOpenConnection()
                    Using tx As MySqlTransaction = conn.BeginTransaction()
                        Try
                            If IsOverlappingTariff(conn, tx, tariffId, frm.EffectiveFrom, frm.EffectiveTo) Then
                                Throw New ApplicationException("The selected period overlaps with another tariff period.")
                            End If

                            Const updateSql As String = "UPDATE tariffs SET rate_per_unit = @rate, effective_from = @effective_from, effective_to = @effective_to WHERE tariff_id = @tariff_id;"
                            Using cmd As New MySqlCommand(updateSql, conn, tx)
                                cmd.Parameters.AddWithValue("@rate", frm.TariffRate)
                                cmd.Parameters.AddWithValue("@effective_from", frm.EffectiveFrom)
                                cmd.Parameters.AddWithValue("@effective_to", If(frm.EffectiveTo.HasValue, CType(frm.EffectiveTo.Value, Object), DBNull.Value))
                                cmd.Parameters.AddWithValue("@tariff_id", tariffId)
                                cmd.ExecuteNonQuery()
                            End Using

                            tx.Commit()
                        Catch
                            tx.Rollback()
                            Throw
                        End Try
                    End Using
                End Using

                LoadTariffs()
                AuditLogger.LogAction(CurrentUser.UserId, "TariffUpdated", $"TariffId={tariffId}, Rate={frm.TariffRate}")
                MessageBox.Show("Tariff updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Failed to update tariff: " & ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Sub btnDeactivate_Click(sender As Object, e As EventArgs)
        Dim selected As DataGridViewRow = GetSelectedRow()
        If selected Is Nothing Then
            MessageBox.Show("Please select a tariff to deactivate.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim tariffId As Integer = Convert.ToInt32(selected.Cells("tariff_id").Value)
        Dim effectiveFrom As Date = Convert.ToDateTime(selected.Cells("effective_from").Value).Date
        Dim deactivateDate As Date = Date.Today

        If deactivateDate < effectiveFrom Then
            deactivateDate = effectiveFrom
        End If

        Dim response = MessageBox.Show($"Deactivate this tariff by setting Effective To = {deactivateDate:yyyy-MM-dd}?",
                                       "Confirm Deactivate",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question)
        If response <> DialogResult.Yes Then
            Return
        End If

        Try
            Const sql As String = "UPDATE tariffs SET effective_to = @effective_to WHERE tariff_id = @tariff_id;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@effective_to", deactivateDate},
                {"@tariff_id", tariffId}
            }
            DatabaseHelper.ExecuteNonQuery(sql, parameters)

            LoadTariffs()
            AuditLogger.LogAction(CurrentUser.UserId, "TariffDeactivated", $"TariffId={tariffId}, EffectiveTo={deactivateDate:yyyy-MM-dd}")
            MessageBox.Show("Tariff deactivated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Failed to deactivate tariff: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function IsOverlappingTariff(conn As MySqlConnection,
                                                tx As MySqlTransaction,
                                                currentTariffId As Nullable(Of Integer),
                                                newFrom As Date,
                                                newTo As Nullable(Of Date)) As Boolean
        Const sql As String = "SELECT COUNT(*) FROM tariffs WHERE (@current_id IS NULL OR tariff_id <> @current_id) AND COALESCE(effective_to, '9999-12-31') >= @new_from AND COALESCE(@new_to, '9999-12-31') >= effective_from;"
        Using cmd As New MySqlCommand(sql, conn, tx)
            cmd.Parameters.AddWithValue("@current_id", If(currentTariffId.HasValue, CType(currentTariffId.Value, Object), DBNull.Value))
            cmd.Parameters.AddWithValue("@new_from", newFrom)
            cmd.Parameters.AddWithValue("@new_to", If(newTo.HasValue, CType(newTo.Value, Object), DBNull.Value))
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    Private Shared Sub CloseCurrentActiveTariff(conn As MySqlConnection, tx As MySqlTransaction, newEffectiveFrom As Date)
        Const activeSql As String = "SELECT tariff_id, effective_from FROM tariffs WHERE effective_to IS NULL ORDER BY effective_from DESC LIMIT 1;"
        Using activeCmd As New MySqlCommand(activeSql, conn, tx)
            Using reader As MySqlDataReader = activeCmd.ExecuteReader()
                If Not reader.Read() Then
                    Return
                End If

                Dim activeId As Integer = Convert.ToInt32(reader("tariff_id"))
                Dim activeFrom As Date = Convert.ToDateTime(reader("effective_from")).Date
                reader.Close()

                If activeFrom >= newEffectiveFrom Then
                    Throw New ApplicationException("New active tariff effective date must be after the current active tariff effective date.")
                End If

                Const closeSql As String = "UPDATE tariffs SET effective_to = @effective_to WHERE tariff_id = @tariff_id;"
                Using closeCmd As New MySqlCommand(closeSql, conn, tx)
                    closeCmd.Parameters.AddWithValue("@effective_to", newEffectiveFrom.AddDays(-1))
                    closeCmd.Parameters.AddWithValue("@tariff_id", activeId)
                    closeCmd.ExecuteNonQuery()
                End Using
            End Using
        End Using
    End Sub

    Private Function GetSelectedRow() As DataGridViewRow
        If dgvTariffs.SelectedRows.Count = 0 Then
            Return Nothing
        End If

        Return dgvTariffs.SelectedRows(0)
    End Function

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
