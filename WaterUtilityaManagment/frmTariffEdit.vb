Imports System.ComponentModel

Public Class frmTariffEdit
    Inherits Form

    Private ReadOnly nudRate As New NumericUpDown()
    Private ReadOnly dtpEffectiveFrom As New DateTimePicker()
    Private ReadOnly dtpEffectiveTo As New DateTimePicker()
    Private ReadOnly chkNoEndDate As New CheckBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()
    Private ReadOnly btnLogout As New Button()

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property TariffRate As Decimal
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EffectiveFrom As Date
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property EffectiveTo As Nullable(Of Date)

    Public Sub New(Optional existingRate As Decimal = 0D,
                   Optional existingFrom As Nullable(Of Date) = Nothing,
                   Optional existingTo As Nullable(Of Date) = Nothing)
        InitializeComponent()

        nudRate.Value = Math.Max(0D, existingRate)
        dtpEffectiveFrom.Value = If(existingFrom.HasValue, existingFrom.Value, Date.Today)

        If existingTo.HasValue Then
            dtpEffectiveTo.Value = existingTo.Value
            chkNoEndDate.Checked = False
        Else
            dtpEffectiveTo.Value = Date.Today
            chkNoEndDate.Checked = True
        End If

        ToggleEffectiveTo()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Tariff Details"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 420
        Me.Height = 260

        Dim lblRate As New Label() With {.Text = "Rate", .Left = 20, .Top = 25, .AutoSize = True}
        nudRate.Left = 150
        nudRate.Top = 20
        nudRate.Width = 200
        nudRate.DecimalPlaces = 2
        nudRate.Minimum = 0
        nudRate.Maximum = 1000000
        nudRate.Increment = 0.1D

        Dim lblFrom As New Label() With {.Text = "Effective From", .Left = 20, .Top = 65, .AutoSize = True}
        dtpEffectiveFrom.Left = 150
        dtpEffectiveFrom.Top = 60
        dtpEffectiveFrom.Width = 200
        dtpEffectiveFrom.Format = DateTimePickerFormat.[Short]

        chkNoEndDate.Text = "No End Date (Active)"
        chkNoEndDate.Left = 150
        chkNoEndDate.Top = 95
        chkNoEndDate.AutoSize = True
        AddHandler chkNoEndDate.CheckedChanged, AddressOf chkNoEndDate_CheckedChanged

        Dim lblTo As New Label() With {.Text = "Effective To", .Left = 20, .Top = 130, .AutoSize = True}
        dtpEffectiveTo.Left = 150
        dtpEffectiveTo.Top = 125
        dtpEffectiveTo.Width = 200
        dtpEffectiveTo.Format = DateTimePickerFormat.[Short]

        btnSave.Text = "Save"
        btnSave.Left = 150
        btnSave.Top = 165
        btnSave.Width = 90
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 260
        btnCancel.Top = 165
        btnCancel.Width = 90
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        btnLogout.Text = "Logout"
        btnLogout.Left = 40
        btnLogout.Top = 165
        btnLogout.Width = 90
        AddHandler btnLogout.Click, AddressOf btnLogout_Click

        Me.Controls.Add(lblRate)
        Me.Controls.Add(nudRate)
        Me.Controls.Add(lblFrom)
        Me.Controls.Add(dtpEffectiveFrom)
        Me.Controls.Add(chkNoEndDate)
        Me.Controls.Add(lblTo)
        Me.Controls.Add(dtpEffectiveTo)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)
        Me.Controls.Add(btnLogout)
    End Sub

    Private Sub chkNoEndDate_CheckedChanged(sender As Object, e As EventArgs)
        ToggleEffectiveTo()
    End Sub

    Private Sub ToggleEffectiveTo()
        dtpEffectiveTo.Enabled = Not chkNoEndDate.Checked
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Dim fromDate As Date = dtpEffectiveFrom.Value.Date
        Dim toDate As Nullable(Of Date) = If(chkNoEndDate.Checked, CType(Nothing, Nullable(Of Date)), dtpEffectiveTo.Value.Date)

        If toDate.HasValue AndAlso toDate.Value < fromDate Then
            MessageBox.Show("Effective To date cannot be earlier than Effective From date.",
                            "Validation",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Return
        End If

        TariffRate = nudRate.Value
        EffectiveFrom = fromDate
        EffectiveTo = toDate

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs)
        SessionManager.Logout(Me)
    End Sub
End Class
