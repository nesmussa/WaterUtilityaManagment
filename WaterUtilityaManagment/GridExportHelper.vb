Imports System.IO
Imports System.Text
Imports iText.IO.Font.Constants
Imports iText.Kernel.Colors
Imports iText.Kernel.Geom
Imports iText.Kernel.Pdf
Imports iText.Layout
Imports iText.Layout.Borders
Imports iText.Layout.Element
Imports iText.Layout.Properties

Public NotInheritable Class GridExportHelper
    Private Sub New()
    End Sub

    Private Shared ReadOnly HeaderColor As DeviceRgb = New DeviceRgb(52, 152, 219)
    Private Shared ReadOnly AlternateRowColor As DeviceRgb = New DeviceRgb(242, 242, 242)
    Private Shared ReadOnly BorderColor As DeviceRgb = New DeviceRgb(210, 210, 210)

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

    Public Shared Sub ExportDataGridViewToPdf(grid As DataGridView,
                                              owner As IWin32Window,
                                              title As String)
        If grid Is Nothing OrElse grid.Columns.Count = 0 Then
            MessageBox.Show("No data available to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim exportData As DataTable = BuildDataTableFromGrid(grid)
        If exportData.Columns.Count = 0 Then
            MessageBox.Show("No visible columns available to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using sfd As New SaveFileDialog()
            sfd.Filter = "PDF files (*.pdf)|*.pdf"
            sfd.FileName = "report_export.pdf"
            If sfd.ShowDialog(owner) <> DialogResult.OK Then
                Return
            End If

            ExportToPdf(exportData, title, sfd.FileName)
            MessageBox.Show("PDF exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using
    End Sub

    Public Shared Sub ExportToPdf(data As DataTable, title As String, filename As String)
        If data Is Nothing OrElse data.Columns.Count = 0 Then
            Throw New ArgumentException("No data supplied for PDF export.", NameOf(data))
        End If

        Dim reportTitle As String = If(String.IsNullOrWhiteSpace(title), "Report", title.Trim())

        Using writer As New PdfWriter(filename)
            Using pdf As New PdfDocument(writer)
                Using document As New Document(pdf, PageSize.A4.Rotate())
                    document.SetMargins(36, 24, 30, 24)

                    Dim titleParagraph As New Paragraph($"{reportTitle} - {Date.Now:yyyy-MM-dd HH:mm}")
                    titleParagraph.SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    titleParagraph.SetFontSize(14)
                    titleParagraph.SetFontColor(New DeviceRgb(44, 62, 80))
                    titleParagraph.SetMarginBottom(14)
                    document.Add(titleParagraph)

                    Dim columnCount As Integer = data.Columns.Count
                    Dim widths(columnCount - 1) As Single
                    For i As Integer = 0 To columnCount - 1
                        widths(i) = 1.0F
                    Next

                    Dim table As New Table(UnitValue.CreatePercentArray(widths))
                    table.SetWidth(UnitValue.CreatePercentValue(100))

                    For Each col As DataColumn In data.Columns
                        Dim headerParagraph As New Paragraph(col.ColumnName)
                        headerParagraph.SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        headerParagraph.SetFontSize(10)
                        headerParagraph.SetFontColor(ColorConstants.WHITE)

                        Dim headerCell As New Cell()
                        headerCell.Add(headerParagraph)
                        headerCell.SetBackgroundColor(HeaderColor)
                        headerCell.SetBorder(New SolidBorder(BorderColor, 0.8F))
                        headerCell.SetPadding(6)
                        table.AddHeaderCell(headerCell)
                    Next

                    For rowIndex As Integer = 0 To data.Rows.Count - 1
                        Dim row As DataRow = data.Rows(rowIndex)
                        Dim useAlternate As Boolean = (rowIndex Mod 2 = 1)

                        For Each col As DataColumn In data.Columns
                            Dim text As String = Convert.ToString(row(col))
                            text = PrepareCellText(text)

                            Dim valueParagraph As New Paragraph(text)
                            valueParagraph.SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                            valueParagraph.SetFontSize(9)

                            Dim cell As New Cell()
                            cell.Add(valueParagraph)
                            cell.SetBorder(New SolidBorder(BorderColor, 0.6F))
                            cell.SetPadding(5)
                            cell.SetTextAlignment(TextAlignment.LEFT)

                            If useAlternate Then
                                cell.SetBackgroundColor(AlternateRowColor)
                            End If

                            table.AddCell(cell)
                        Next
                    Next

                    document.Add(table)
                End Using
            End Using
        End Using
    End Sub

    Private Shared Function BuildDataTableFromGrid(grid As DataGridView) As DataTable
        Dim table As New DataTable()
        Dim visibleColumns As New List(Of DataGridViewColumn)()

        For Each col As DataGridViewColumn In grid.Columns
            If col.Visible Then
                visibleColumns.Add(col)
                table.Columns.Add(col.HeaderText)
            End If
        Next

        For Each row As DataGridViewRow In grid.Rows
            If row.IsNewRow Then
                Continue For
            End If

            Dim values As New List(Of Object)()
            For Each col As DataGridViewColumn In visibleColumns
                values.Add(If(row.Cells(col.Index).Value, String.Empty))
            Next
            table.Rows.Add(values.ToArray())
        Next

        Return table
    End Function

    Private Shared Function PrepareCellText(value As String) As String
        Dim normalized As String = If(value, String.Empty).Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Trim()
        Const maxLength As Integer = 180
        If normalized.Length > maxLength Then
            Return normalized.Substring(0, maxLength - 1) & "…"
        End If

        Return normalized
    End Function

    Private Shared Function EscapeCsv(value As String) As String
        Dim clean As String = If(value, String.Empty).Replace(ChrW(34), ChrW(34) & ChrW(34))
        Return $"""{clean}"""
    End Function
End Class
