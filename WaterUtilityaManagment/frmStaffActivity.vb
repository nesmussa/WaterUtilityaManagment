Public Class frmStaffActivity
    Inherits Form

    Private ReadOnly pnlTop As New Panel()
    Private ReadOnly btnRefresh As New Button()
    Private ReadOnly dgvActivity As New DataGridView()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "My Activity Log"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ClientSize = New Size(1200, 800)
        Me.MinimumSize = New Size(900, 650)
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.ShowInTaskbar = True
        Me.BackColor = ColorTranslator.FromHtml("#ecf0f1")
        Me.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)

        pnlTop.Dock = DockStyle.Top
        pnlTop.Height = 54
        pnlTop.BackColor = Color.White
        pnlTop.Padding = New Padding(12, 10, 12, 10)

        btnRefresh.Text = "Refresh"
        btnRefresh.Width = 95
        btnRefresh.Height = 32
        btnRefresh.Left = 12
        btnRefresh.Top = 10
        btnRefresh.FlatStyle = FlatStyle.Flat
        btnRefresh.FlatAppearance.BorderSize = 0
        btnRefresh.BackColor = ColorTranslator.FromHtml("#3498db")
        btnRefresh.ForeColor = Color.White
        btnRefresh.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        AddHandler btnRefresh.Click, AddressOf btnRefresh_Click
        pnlTop.Controls.Add(btnRefresh)

        dgvActivity.Dock = DockStyle.Fill
        dgvActivity.AllowUserToAddRows = False
        dgvActivity.AllowUserToDeleteRows = False
        dgvActivity.AllowUserToResizeRows = False
        dgvActivity.ReadOnly = True
        dgvActivity.RowHeadersVisible = False
        dgvActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvActivity.BackgroundColor = Color.White
        dgvActivity.BorderStyle = BorderStyle.None
        dgvActivity.GridColor = ColorTranslator.FromHtml("#d5d8dc")
        dgvActivity.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#f8f9f9")
        dgvActivity.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)

        Me.Controls.Add(dgvActivity)
        Me.Controls.Add(pnlTop)

        AddHandler Me.Load, AddressOf frmStaffActivity_Load
    End Sub

    Private Sub frmStaffActivity_Load(sender As Object, e As EventArgs)
        If Not String.Equals(CurrentUser.Role, "Staff", StringComparison.OrdinalIgnoreCase) Then
            MessageBox.Show("Access denied. Only staff can view this log.",
                            "Unauthorized",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        UiStyleHelper.AddDialogCloseButton(Me)
        LoadActivity()
    End Sub

    Private Sub LoadActivity()
        Try
            Const sql As String = "SELECT * FROM view_staff_activity LIMIT 500;"
            dgvActivity.DataSource = DatabaseHelper.ExecuteDataTable(sql)
        Catch ex As Exception
            MessageBox.Show("Failed to load activity log: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs)
        LoadActivity()
    End Sub
End Class
