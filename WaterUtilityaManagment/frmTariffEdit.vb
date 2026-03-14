Imports System.ComponentModel

Public Class frmTariffEdit
    Inherits Form

    Private ReadOnly nudRate As New NumericUpDown()
    Private ReadOnly dtpEffectiveFrom As New DateTimePicker()
    Private ReadOnly dtpEffectiveTo As New DateTimePicker()
    Private ReadOnly chkNoEndDate As New CheckBox()
    Private ReadOnly btnSave As New Button()
    Private ReadOnly btnCancel As New Button()

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
        Me.Text = "Add/Edit Tariff"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(1200, 800)
        Me.MinimumSize = New Size(900, 600)
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.ShowInTaskbar = True
        Me.BackColor = Color.White
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        Dim headerPanel As New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 50,
            .BackColor = ColorTranslator.FromHtml("#3498db")
        }
        Dim lblHeader As New Label() With {
            .Dock = DockStyle.Fill,
            .Text = "Add/Edit Tariff",
            .TextAlign = ContentAlignment.MiddleCenter,
            .Font = New Font("Segoe UI", 14.0F, FontStyle.Bold),
            .ForeColor = Color.White
        }
        headerPanel.Controls.Add(lblHeader)

        Dim contentGrid As New TableLayoutPanel() With {
            .Left = 24,
            .Top = 66,
            .Width = 352,
            .Height = 150,
            .ColumnCount = 2,
            .RowCount = 4
        }
        contentGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38.0F))
        contentGrid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 62.0F))
        contentGrid.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        contentGrid.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        contentGrid.RowStyles.Add(New RowStyle(SizeType.Absolute, 34.0F))
        contentGrid.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))

        Dim lblRate As New Label() With {.Text = "Rate", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        nudRate.Dock = DockStyle.Fill
        nudRate.DecimalPlaces = 2
        nudRate.Minimum = 0
        nudRate.Maximum = 1000000
        nudRate.Increment = 0.1D
        nudRate.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)

        Dim lblFrom As New Label() With {.Text = "Effective From", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        dtpEffectiveFrom.Dock = DockStyle.Fill
        dtpEffectiveFrom.Format = DateTimePickerFormat.[Short]
        dtpEffectiveFrom.CalendarMonthBackground = Color.White
        dtpEffectiveFrom.CalendarTitleBackColor = ColorTranslator.FromHtml("#3498db")
        dtpEffectiveFrom.CalendarTitleForeColor = Color.White

        chkNoEndDate.Text = "No End Date (Active)"
        chkNoEndDate.Anchor = AnchorStyles.Left
        chkNoEndDate.AutoSize = True
        chkNoEndDate.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        chkNoEndDate.ForeColor = ColorTranslator.FromHtml("#2c3e50")
        chkNoEndDate.BackColor = Color.White
        AddHandler chkNoEndDate.CheckedChanged, AddressOf chkNoEndDate_CheckedChanged

        Dim lblTo As New Label() With {.Text = "Effective To", .AutoSize = True, .Anchor = AnchorStyles.Right, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
        dtpEffectiveTo.Dock = DockStyle.Fill
        dtpEffectiveTo.Format = DateTimePickerFormat.[Short]
        dtpEffectiveTo.CalendarMonthBackground = Color.White
        dtpEffectiveTo.CalendarTitleBackColor = ColorTranslator.FromHtml("#3498db")
        dtpEffectiveTo.CalendarTitleForeColor = Color.White

        contentGrid.Controls.Add(lblRate, 0, 0)
        contentGrid.Controls.Add(nudRate, 1, 0)
        contentGrid.Controls.Add(lblFrom, 0, 1)
        contentGrid.Controls.Add(dtpEffectiveFrom, 1, 1)
        contentGrid.Controls.Add(New Label() With {.Text = String.Empty}, 0, 2)
        contentGrid.Controls.Add(chkNoEndDate, 1, 2)
        contentGrid.Controls.Add(lblTo, 0, 3)
        contentGrid.Controls.Add(dtpEffectiveTo, 1, 3)

        btnSave.Text = "Save"
        btnSave.Left = 196
        btnSave.Top = 236
        btnSave.Width = 86
        btnSave.Height = 36
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = ColorTranslator.FromHtml("#27ae60")
        btnSave.ForeColor = Color.White
        btnSave.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnSave.Click, AddressOf btnSave_Click

        btnCancel.Text = "Cancel"
        btnCancel.Left = 290
        btnCancel.Top = 236
        btnCancel.Width = 86
        btnCancel.Height = 36
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.FlatAppearance.BorderSize = 0
        btnCancel.BackColor = ColorTranslator.FromHtml("#e74c3c")
        btnCancel.ForeColor = Color.White
        btnCancel.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnCancel.Click, AddressOf btnCancel_Click

        Dim borderPanel As New Panel() With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(1)
        }

        Me.Controls.Add(borderPanel)
        Me.Controls.Add(headerPanel)

        borderPanel.Controls.Add(contentGrid)
        Me.Controls.Add(btnSave)
        Me.Controls.Add(btnCancel)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel

        UiStyleHelper.AddDialogCloseButton(Me)
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

End Class
