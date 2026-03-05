Imports MySql.Data.MySqlClient

Public Class frmTariffs
    Inherits Form

    Private ReadOnly dgvTariffs As New DataGridView()
    Private ReadOnly btnAdd As New Button()
    Private ReadOnly btnEdit As New Button()
    Private ReadOnly btnDeactivate As New Button()
    Private ReadOnly btnLogout As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Tariff Management"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 850
        Me.Height = 560
        Me.MinimumSize = New Size(760, 520)

        dgvTariffs.Left = 20
        dgvTariffs.Top = 20
        dgvTariffs.Width = 790
        dgvTariffs.Height = 420
        dgvTariffs.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        dgvTariffs.AllowUserToAddRows = False
        dgvTariffs.AllowUserToDeleteRows = False
        dgvTariffs.ReadOnly = True
        dgvTariffs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvTariffs.MultiSelect = False
        dgvTariffs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        btnAdd.Text = "Add New Tariff"
        btnAdd.Width = 150
        btnAdd.Height = 32
        AddHandler btnAdd.Click, AddressOf btnAdd_Click

        btnEdit.Text = "Edit"
        btnEdit.Width = 120
        btnEdit.Height = 32
        AddHandler btnEdit.Click, AddressOf btnEdit_Click

        btnDeactivate.Text = "Deactivate"
        btnDeactivate.Width = 120
        btnDeactivate.Height = 32
        AddHandler btnDeactivate.Click, AddressOf btnDeactivate_Click

        btnLogout.Text = "Logout"
        btnLogout.Width = 120
        btnLogout.Height = 32
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Dim actionPanel As New FlowLayoutPanel()
        actionPanel.Left = 20
        actionPanel.Top = 452
        actionPanel.Width = 540
        actionPanel.Height = 40
        actionPanel.Anchor = AnchorStyles.Left Or AnchorStyles.Bottom
        actionPanel.FlowDirection = FlowDirection.LeftToRight
        actionPanel.WrapContents = False
        actionPanel.Padding = New Padding(0)
        actionPanel.Margin = New Padding(0)
        actionPanel.Controls.Add(btnAdd)
        actionPanel.Controls.Add(btnEdit)
        actionPanel.Controls.Add(btnDeactivate)
        actionPanel.Controls.Add(btnLogout)

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvTariffs)
        UiStyleHelper.StyleButton(btnAdd, True)
        UiStyleHelper.StyleButton(btnEdit, True)
        UiStyleHelper.StyleButton(btnDeactivate)
        UiStyleHelper.StyleButton(btnLogout)

        Me.Controls.Add(dgvTariffs)
        Me.Controls.Add(actionPanel)

        AddHandler Me.Load, AddressOf frmTariffs_Load
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
        Catch ex As Exception
            MessageBox.Show("Failed to load tariffs: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
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
