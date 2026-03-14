Imports System.Linq

Public NotInheritable Class UiStyleHelper
    Private Sub New()
    End Sub

    Public Shared Sub StyleForm(frm As Form)
        frm.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        frm.BackColor = Color.FromArgb(245, 247, 250)
    End Sub

    Public Shared Sub StyleDataGrid(grid As DataGridView)
        grid.BackgroundColor = Color.White
        grid.BorderStyle = BorderStyle.FixedSingle
        grid.RowHeadersVisible = False
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 70, 115)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 9.0F)
        grid.ColumnHeadersHeight = 34
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 233, 255)
        grid.DefaultCellStyle.SelectionForeColor = Color.Black
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 255)
    End Sub

    Public Shared Sub StyleButton(btn As Button, Optional primary As Boolean = False)
        btn.FlatStyle = FlatStyle.Flat
        btn.FlatAppearance.BorderSize = 0
        btn.Cursor = Cursors.Hand
        btn.AutoSize = False
        btn.TextAlign = ContentAlignment.MiddleCenter
        btn.Padding = New Padding(8, 0, 8, 0)
        If btn.Height < 34 Then
            btn.Height = 34
        End If

        If primary Then
            btn.BackColor = Color.FromArgb(24, 119, 242)
            btn.ForeColor = Color.White
        Else
            btn.BackColor = Color.FromArgb(230, 236, 245)
            btn.ForeColor = Color.FromArgb(40, 40, 40)
        End If
    End Sub

    Public Shared Sub AddDialogCloseButton(frm As Form)
        If frm.Controls.OfType(Of Button)().Any(Function(b) String.Equals(Convert.ToString(b.Tag), "DialogCloseButton", StringComparison.Ordinal)) Then
            Return
        End If

        Dim btnClose As New Button() With {
            .Text = "Close",
            .Size = New Size(100, 35),
            .BackColor = Color.FromArgb(231, 76, 60),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right,
            .Tag = "DialogCloseButton"
        }
        btnClose.FlatAppearance.BorderSize = 0

        Dim placeButton As Action =
            Sub()
                btnClose.Location = New Point(Math.Max(0, frm.ClientSize.Width - 120), Math.Max(0, frm.ClientSize.Height - 60))
            End Sub

        AddHandler frm.Resize, Sub() placeButton()
        AddHandler btnClose.Click, Sub(sender, e) frm.Close()

        placeButton()
        frm.Controls.Add(btnClose)
        btnClose.BringToFront()
    End Sub
End Class
