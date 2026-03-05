Public Class frmStaffActivity
    Inherits Form

    Private ReadOnly dgvActivity As New DataGridView()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "My Activity Log"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Width = 900
        Me.Height = 520
        Me.MinimumSize = New Size(760, 460)

        dgvActivity.Dock = DockStyle.Fill
        dgvActivity.AllowUserToAddRows = False
        dgvActivity.AllowUserToDeleteRows = False
        dgvActivity.ReadOnly = True
        dgvActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

        UiStyleHelper.StyleForm(Me)
        UiStyleHelper.StyleDataGrid(dgvActivity)

        Me.Controls.Add(dgvActivity)

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

        LoadActivity()
    End Sub

    Private Sub LoadActivity()
        Try
            Const sql As String = "SELECT action_time, action_name, details FROM audit_log WHERE user_id = @user_id ORDER BY action_time DESC LIMIT 200;"
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@user_id", CurrentUser.UserId}
            }
            dgvActivity.DataSource = DatabaseHelper.ExecuteDataTable(sql, parameters)
        Catch ex As Exception
            MessageBox.Show("Failed to load activity log: " & ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub
End Class
