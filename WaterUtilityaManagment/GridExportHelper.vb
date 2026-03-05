Imports System.IO
Imports System.Text

Public NotInheritable Class GridExportHelper
    Private Sub New()
    End Sub

    Public Shared Sub ExportDataGridViewToCsv(grid As DataGridView, owner As IWin32Window)
        If grid Is Nothing OrElse grid.Columns.Count = 0 Then
            MessageBox.Show("No data available to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using sfd As New SaveFileDialog()
            sfd.Filter = "CSV files (*.csv)|*.csv"
            sfd.FileName = "report_export.csv"
            If sfd.ShowDialog(owner) <> DialogResult.OK Then
                Return
            End If

            Using writer As New StreamWriter(sfd.FileName, False, Encoding.UTF8)
                Dim header As New List(Of String)()
                For Each col As DataGridViewColumn In grid.Columns
                    If col.Visible Then
                        header.Add(EscapeCsv(col.HeaderText))
                    End If
                Next
                writer.WriteLine(String.Join(",", header))

                For Each row As DataGridViewRow In grid.Rows
                    If row.IsNewRow Then
                        Continue For
                    End If

                    Dim values As New List(Of String)()
                    For Each col As DataGridViewColumn In grid.Columns
                        If Not col.Visible Then
                            Continue For
                        End If

                        Dim cellValue As Object = row.Cells(col.Index).Value
                        values.Add(EscapeCsv(If(cellValue Is Nothing, String.Empty, cellValue.ToString())))
                    Next
                    writer.WriteLine(String.Join(",", values))
                Next
            End Using

            MessageBox.Show("Exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Private Shared Function EscapeCsv(value As String) As String
        Dim clean As String = If(value, String.Empty).Replace(ChrW(34), ChrW(34) & ChrW(34))
        Return $"""{clean}"""
    End Function
End Class
