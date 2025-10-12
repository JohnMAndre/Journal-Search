Public Class ImportForm
    Dim m_lstTemp As New List(Of Entry) '-- just for use on this form

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub PasteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PasteToolStripMenuItem.Click
        Try
            Dim strRows() As String
            Dim obj As Entry
            dgvData.DataSource = Nothing

            Dim strClipboard As String = Clipboard.GetText()
            If strClipboard Is Nothing Then
                MessageBox.Show("The clipboard is empty.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            ElseIf Not strClipboard.Contains(vbTab) Then
                MessageBox.Show("The clipboard data does not contain tabs. It must be spreadsheet data.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If


            If strRows Is Nothing Then
                strRows = strClipboard.Split(Environment.NewLine)
            End If

            Dim row() As String
            For intCounter As Integer = 0 To strRows.Count - 1
                obj = New Entry()
                If strRows(intCounter).Trim.Length = 0 Then
                    Continue For '-- skip this row, it has no data
                End If
                row = strRows(intCounter).Split(vbTab)

                If row.Length >= 1 Then
                    obj.JournalName = GetDecodedText(row(0))
                End If

                If obj.JournalName.ToLower() = JournalNameColumn.HeaderText.ToLower() Then
                    Continue For '-- skip this row, it is just the column headers
                End If

                If row.Length >= 2 Then
                    obj.PublisherName = GetDecodedText(row(1))
                End If
                If row.Length >= 3 Then
                    obj.ISSNs = GetDecodedText(row(2))
                End If
                If row.Length >= 4 Then
                    obj.Source = GetDecodedText(row(3))
                End If
                If row.Length >= 5 Then
                    obj.Ranking = GetDecodedText(row(4))
                End If
                If row.Length >= 6 Then
                    obj.Rating = GetDecodedText(row(5))
                End If
                If row.Length >= 7 Then
                    obj.HIndex = GetDecodedText(row(6))
                End If
                If row.Length >= 8 Then
                    obj.Country = GetDecodedText(row(7))
                End If
                If row.Length >= 9 Then
                    obj.Categories = GetDecodedText(row(8))
                End If
                If row.Length >= 10 Then
                    obj.Areas = GetDecodedText(row(9))
                End If


                m_lstTemp.Add(obj)
                Me.Text = "Import Data (pasting " & intCounter.ToString("#,##0") & " of " & strRows.Count.ToString("#,##0") & ")"
                Application.DoEvents()
            Next

            '-- Update source and set background color so user does not forget
            If m_lstTemp.Count > 0 Then
                txtSourceToDelete.Text = m_lstTemp(0).Source
                txtSourceToDelete.BackColor = Color.Yellow
            End If

            '-- Add notes
            AddNotesToJournals(m_lstTemp)


            '-- Add to DGV
            dgvData.AutoGenerateColumns = False
            dgvData.DataSource = m_lstTemp

            dgvData.AutoResizeColumns()

            Me.Text = "Import - " & m_lstTemp.Count().ToString("#,##0")


            Me.btnImport.Enabled = True

        Catch ex As Exception
            MessageBox.Show("There was a problem pasting (" & ex.Message & ").", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        Try

            If txtSourceToDelete.Text.Trim().Length > 0 Then
                '-- delete from existing list
                Dim strSource As String = txtSourceToDelete.Text.Trim().ToLower()
                Dim lstToDelete As New List(Of Entry)
                For Each obj As Entry In JournalList
                    If obj.Source.ToLower() = strSource Then
                        lstToDelete.Add(obj)
                    End If
                Next

                '-- Now remove them (cannot modify a collection that we are iterating through)
                For Each obj As Entry In lstToDelete
                    JournalList.Remove(obj)
                Next
            End If

            Dim intCounter As Integer
            For Each obj As Entry In m_lstTemp
                JournalList.Add(obj)
                intCounter += 1
                Me.Text = "Import Data (added " & intCounter.ToString("#,##0") & " out of " & m_lstTemp.Count.ToString("#,##0") & ")"
                Application.DoEvents()
            Next

            Me.DialogResult = DialogResult.OK


        Catch ex As Exception
            MessageBox.Show("There was a problem pasting (" & ex.Message & ").", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub CopyColumnsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyColumnsToolStripMenuItem.Click
        Clipboard.SetText(GetColumnHeaderTexts)
    End Sub
    Private Function GetColumnHeaderTexts() As String
        Dim strReturn As String = String.Empty
        For Each col As DataGridViewColumn In dgvData.Columns
            strReturn &= col.HeaderText & vbTab
        Next

        strReturn = strReturn.Substring(0, strReturn.Length - 1)
        Return strReturn
    End Function
End Class