Imports System.Drawing.Text
Imports System.Xml

Public Class MainForm
    Private m_intCounter As Integer

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ImportUpdateDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportUpdateDatabaseToolStripMenuItem.Click
        Dim frm As New ImportForm()
        If frm.ShowDialog() = DialogResult.OK Then
            '-- reload data
            ReloadData()
        End If
    End Sub
    Public Sub LoadData()
        dgvData.DataSource = Nothing

        JournalList.Clear()
        Dim xDoc As New Xml.XmlDocument()
        xDoc.Load("JournalSearchData.xml")
        Dim xList As XmlNodeList = xDoc.SelectNodes("//Journal")
        Dim obj As Entry

        m_intCounter = 0
        prgStatusBar.Maximum = xList.Count

        For Each xElement As XmlElement In xList
            m_intCounter += 1
            If m_intCounter Mod 10 = 0 Then
                UpdateLoadingProgress()
            End If

            obj = New Entry(xElement)
            JournalList.Add(obj)
        Next

        UpdateLoadingProgress()

        '-- Now we must add the notes
        If System.IO.File.Exists("JournalSearchDataExtra.xml") Then
            xDoc.Load("JournalSearchDataExtra.xml")
            xList = xDoc.SelectNodes("//JournalNote")
            Dim strName As String
            Dim strNotes As String

            m_intCounter = 0
            prgStatusBar.Maximum = xList.Count

            For Each xElement As XmlElement In xList
                m_intCounter += 1
                UpdateLoadingProgress()

                strName = xElement.GetAttribute("JournalName").ToLower()
                strNotes = xElement.InnerText
                If strNotes.Length > 0 Then
                    For Each obj In JournalList
                        If obj.JournalName.ToLower() = strName Then
                            obj.Notes = xElement.InnerText
                        End If
                    Next
                End If
            Next
        End If

        UpdateLoadingProgress()

        dgvData.DataSource = JournalList

        Me.Text = "Journal Search - " & JournalList.Count().ToString("#,##0")
    End Sub
    Public Sub UpdateLoadingProgress()
        If prgStatusBar.Value < prgStatusBar.Maximum Then
            prgStatusBar.Visible = True
            prgStatusBar.Value = m_intCounter
            lblStatusLabel1.Text = "Loading journal " & m_intCounter.ToString("#,##0") & " of " & prgStatusBar.Maximum.ToString("#,##0")
            Application.DoEvents()
        Else
            prgStatusBar.Visible = False
            lblStatusLabel1.Text = String.Empty
        End If
    End Sub
    Public Sub ReloadData()
        dgvData.DataSource = Nothing
        dgvData.DataSource = JournalList
        dgvData.AutoResizeColumns()

        Me.Text = "Journal Search - " & JournalList.Count().ToString("#,##0")
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Show()
        Application.DoEvents()

        LoadData()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        PersistData()
    End Sub
    Private Sub PersistData()
        '-- save back data to main data file
        Dim xDoc1 As New Xml.XmlDocument()
        Dim xDoc2 As New Xml.XmlDocument()
        Dim root1 As Xml.XmlElement = xDoc1.AppendChild(xDoc1.CreateElement("Journals"))
        Dim root2 As Xml.XmlElement = xDoc2.AppendChild(xDoc2.CreateElement("JournalNotes"))
        Dim element1 As Xml.XmlElement
        Dim element2 As Xml.XmlElement
        For Each obj As Entry In JournalList
            element1 = xDoc1.CreateElement("Journal")
            element2 = xDoc2.CreateElement("JournalNote")

            element1.SetAttribute("JournalName", obj.JournalName)
            element1.SetAttribute("PublisherName", obj.PublisherName)
            element1.SetAttribute("ISSN1", obj.ISSN1)
            element1.SetAttribute("ISSN2", obj.ISSN2)
            element1.SetAttribute("Source", obj.Source)
            element1.SetAttribute("Ranking", obj.Ranking)
            element1.SetAttribute("Rating", obj.Rating)
            element1.SetAttribute("HIndex", obj.HIndex)
            element1.SetAttribute("Country", obj.Country)
            element1.SetAttribute("Categories", obj.Categories)

            root1.AppendChild(element1)

            '-- Notes
            If obj.Notes IsNot Nothing AndAlso obj.Notes.Length > 0 Then
                element2.SetAttribute("JournalName", obj.JournalName)
                element2.InnerText = obj.Notes
                root2.AppendChild(element2)
            End If
        Next

        xDoc1.Save("JournalSearchData.xml")
        xDoc2.Save("JournalSearchDataExtra.xml")

    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        ApplyFilter()
    End Sub
    Private Sub txtFilter_KeyDown(sender As Object, e As KeyEventArgs) Handles txtFilter.KeyDown
        If e.KeyCode = Keys.Enter Then
            If ModifierKeys = ModifierKeys.Control Then
                '-- remove quotes, then wrap in quotes
                txtFilter.Text = WrapTextInQuotes(txtFilter.Text)
                txtFilter.Refresh()
            End If
            ApplyFilter()
        End If
    End Sub
    Private Function WrapTextInQuotes(text As String) As String
        Return """" & text.Replace("""", String.Empty) & """"
    End Function
    Private Sub ApplyFilter()
        Try
            dgvData.DataSource = Nothing
            dgvData.Refresh()
            lblFilteredCount.Text = String.Empty
            lblFilteredCount.Refresh()

            Dim strSearchFor As String
            Dim lstFiltered As List(Of Entry)
            If txtFilter.Text.Length = 0 Then
                dgvData.DataSource = JournalList
                lblFilteredCount.Text = String.Empty
            Else
                txtFilter.Text = txtFilter.Text.Trim()

                If txtFilter.Text.Contains(" ") AndAlso Not txtFilter.Text.StartsWith("""") And Not chkPartialMatch.Checked Then
                    '-- it has a space, not starting with a quote, but not set to partial, change to  partial
                    txtFilter.Text = WrapTextInQuotes(txtFilter.Text)
                    txtFilter.Refresh()
                End If

                strSearchFor = txtFilter.Text.ToLower()
                lblStatusLabel1.Text = String.Empty

                If strSearchFor.Contains(" ") AndAlso Not strSearchFor.StartsWith("""") Then
                    Dim strSearchFor2, strSearchFor3, strSearchFor4 As String

                    Dim str As String() = strSearchFor.Split(" ")
                    If str.Length > 0 Then
                        strSearchFor = str(0)
                    Else
                        Exit Sub
                    End If
                    If str.Length > 1 Then
                        strSearchFor2 = str(1)
                    End If
                    If str.Length > 2 Then
                        strSearchFor3 = str(2)
                    End If
                    If str.Length > 3 Then
                        strSearchFor4 = str(3)
                    End If

                    If str.Length > 4 Then
                        lblStatusLabel1.Text = "Queries over four words are
treated as a phrase"
                    End If

                    Select Case str.Length
                        Case 1
                            strSearchFor = strSearchFor.Replace("""", "").Trim '-- must strip quotes, just in case it is a phrase

                            If chkAllColumns.Checked Then
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                                x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                                x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                                x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                                x.Source.ToLower.Contains(strSearchFor) OrElse
                                                                x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                                x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                                x.Country.ToLower.Contains(strSearchFor)).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor OrElse
                                                                x.PublisherName.ToLower() = strSearchFor OrElse
                                                                x.ISSN1.ToLower() = strSearchFor OrElse
                                                                x.ISSN2.ToLower() = strSearchFor OrElse
                                                                x.Source.ToLower() = strSearchFor OrElse
                                                                x.Categories.ToLower() = strSearchFor OrElse
                                                                x.Rating.ToLower() = strSearchFor OrElse
                                                                x.Country.ToLower() = strSearchFor).ToList()
                                End If
                            Else
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor)).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor).ToList()
                                End If
                            End If
                        Case 2
                            If chkAllColumns.Checked Then
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                               x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                               x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                               x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                               x.Source.ToLower.Contains(strSearchFor) OrElse
                                                               x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                               x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                               x.Country.ToLower.Contains(strSearchFor)) AndAlso
                                                                (x.JournalName.ToLower.Contains(strSearchFor2) OrElse
                                                               x.PublisherName.ToLower.Contains(strSearchFor2) OrElse
                                                               x.ISSN1.ToLower.Contains(strSearchFor2) OrElse
                                                               x.ISSN2.ToLower.Contains(strSearchFor2) OrElse
                                                               x.Source.ToLower.Contains(strSearchFor2) OrElse
                                                               x.Categories.ToLower.Contains(strSearchFor2) OrElse
                                                               x.Rating.ToLower.Contains(strSearchFor2) OrElse
                                                               x.Country.ToLower.Contains(strSearchFor2))).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower() = strSearchFor OrElse
                                                               x.PublisherName.ToLower() = strSearchFor OrElse
                                                               x.ISSN1.ToLower() = strSearchFor OrElse
                                                               x.ISSN2.ToLower() = strSearchFor OrElse
                                                               x.Source.ToLower() = strSearchFor OrElse
                                                               x.Categories.ToLower() = strSearchFor OrElse
                                                               x.Rating.ToLower() = strSearchFor OrElse
                                                               x.Country.ToLower() = strSearchFor) AndAlso
                                                                (x.JournalName.ToLower() = strSearchFor2 OrElse
                                                               x.PublisherName.ToLower() = strSearchFor2 OrElse
                                                               x.ISSN1.ToLower() = strSearchFor2 OrElse
                                                               x.ISSN2.ToLower() = strSearchFor2 OrElse
                                                               x.Source.ToLower() = strSearchFor2 OrElse
                                                               x.Categories.ToLower() = strSearchFor2 OrElse
                                                               x.Rating.ToLower() = strSearchFor2 OrElse
                                                               x.Country.ToLower() = strSearchFor2)).ToList()
                                End If
                            Else
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor)) AndAlso
                                                                (x.JournalName.ToLower.Contains(strSearchFor2))).ToList()
                                Else
                                    '-- This branch does not make sense
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor AndAlso
                                                                x.JournalName.ToLower() = strSearchFor2).ToList()
                                End If
                            End If

                        Case 3
                            If chkAllColumns.Checked Then
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor2) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor2) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor2) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor2)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor3) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor3) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor3) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor3))).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower() = strSearchFor OrElse
                                                              x.PublisherName.ToLower() = strSearchFor OrElse
                                                              x.ISSN1.ToLower() = strSearchFor OrElse
                                                              x.ISSN2.ToLower() = strSearchFor OrElse
                                                              x.Source.ToLower() = strSearchFor OrElse
                                                              x.Categories.ToLower() = strSearchFor OrElse
                                                              x.Rating.ToLower() = strSearchFor OrElse
                                                              x.Country.ToLower() = strSearchFor) AndAlso
                                                               (x.JournalName.ToLower() = strSearchFor2 OrElse
                                                              x.PublisherName.ToLower() = strSearchFor2 OrElse
                                                              x.ISSN1.ToLower() = strSearchFor2 OrElse
                                                              x.ISSN2.ToLower() = strSearchFor2 OrElse
                                                              x.Source.ToLower() = strSearchFor2 OrElse
                                                              x.Categories.ToLower() = strSearchFor2 OrElse
                                                              x.Rating.ToLower() = strSearchFor2 OrElse
                                                              x.Country.ToLower() = strSearchFor2) AndAlso
                                                               (x.JournalName.ToLower() = strSearchFor3 OrElse
                                                              x.PublisherName.ToLower() = strSearchFor3 OrElse
                                                              x.ISSN1.ToLower() = strSearchFor3 OrElse
                                                              x.ISSN2.ToLower() = strSearchFor3 OrElse
                                                              x.Source.ToLower() = strSearchFor3 OrElse
                                                              x.Categories.ToLower() = strSearchFor3 OrElse
                                                              x.Rating.ToLower() = strSearchFor3 OrElse
                                                              x.Country.ToLower() = strSearchFor3)).ToList()
                                End If
                            Else
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor2)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor3))).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor AndAlso
                                                               x.JournalName.ToLower() = strSearchFor2 AndAlso
                                                               x.JournalName.ToLower() = strSearchFor3).ToList()
                                End If
                            End If

                        Case 4
                            If chkAllColumns.Checked Then
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor2) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor2) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor2) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor2) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor2)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor3) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor3) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor3) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor3) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor3)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor4) OrElse
                                                              x.PublisherName.ToLower.Contains(strSearchFor4) OrElse
                                                              x.ISSN1.ToLower.Contains(strSearchFor4) OrElse
                                                              x.ISSN2.ToLower.Contains(strSearchFor4) OrElse
                                                              x.Source.ToLower.Contains(strSearchFor4) OrElse
                                                              x.Categories.ToLower.Contains(strSearchFor4) OrElse
                                                              x.Rating.ToLower.Contains(strSearchFor4) OrElse
                                                              x.Country.ToLower.Contains(strSearchFor4))).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower() = strSearchFor OrElse
                                                              x.PublisherName.ToLower() = strSearchFor OrElse
                                                              x.ISSN1.ToLower() = strSearchFor OrElse
                                                              x.ISSN2.ToLower() = strSearchFor OrElse
                                                              x.Source.ToLower() = strSearchFor OrElse
                                                              x.Categories.ToLower() = strSearchFor OrElse
                                                              x.Rating.ToLower() = strSearchFor OrElse
                                                              x.Country.ToLower() = strSearchFor) AndAlso
                                                               (x.JournalName.ToLower() = strSearchFor2 OrElse
                                                              x.PublisherName.ToLower() = strSearchFor2 OrElse
                                                              x.ISSN1.ToLower() = strSearchFor2 OrElse
                                                              x.ISSN2.ToLower() = strSearchFor2 OrElse
                                                              x.Source.ToLower() = strSearchFor2 OrElse
                                                              x.Categories.ToLower() = strSearchFor2 OrElse
                                                              x.Rating.ToLower() = strSearchFor2 OrElse
                                                              x.Country.ToLower() = strSearchFor2) AndAlso
                                                               (x.JournalName.ToLower() = strSearchFor3 OrElse
                                                              x.PublisherName.ToLower() = strSearchFor3 OrElse
                                                              x.ISSN1.ToLower() = strSearchFor3 OrElse
                                                              x.ISSN2.ToLower() = strSearchFor3 OrElse
                                                              x.Source.ToLower() = strSearchFor3 OrElse
                                                              x.Categories.ToLower() = strSearchFor3 OrElse
                                                              x.Rating.ToLower() = strSearchFor3 OrElse
                                                              x.Country.ToLower() = strSearchFor3) AndAlso
                                                               (x.JournalName.ToLower() = strSearchFor4 OrElse
                                                              x.PublisherName.ToLower() = strSearchFor4 OrElse
                                                              x.ISSN1.ToLower() = strSearchFor4 OrElse
                                                              x.ISSN2.ToLower() = strSearchFor4 OrElse
                                                              x.Source.ToLower() = strSearchFor4 OrElse
                                                              x.Categories.ToLower() = strSearchFor4 OrElse
                                                              x.Rating.ToLower() = strSearchFor4 OrElse
                                                              x.Country.ToLower() = strSearchFor4)).ToList()
                                End If
                            Else
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(strSearchFor)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor2)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor3)) AndAlso
                                                               (x.JournalName.ToLower.Contains(strSearchFor4))).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor AndAlso
                                                               x.JournalName.ToLower() = strSearchFor2 AndAlso
                                                               x.JournalName.ToLower() = strSearchFor3 AndAlso
                                                               x.JournalName.ToLower() = strSearchFor4).ToList()
                                End If
                            End If
                        Case Else
                            '-- catch all
                            strSearchFor = txtFilter.Text.ToLower()
                            strSearchFor = strSearchFor.Replace("""", "").Trim '-- must strip quotes, just in case it is a phrase

                            If chkAllColumns.Checked Then
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                                    x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                                    x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                                    x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                                    x.Source.ToLower.Contains(strSearchFor) OrElse
                                                                    x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                                    x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                                    x.Country.ToLower.Contains(strSearchFor)).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor OrElse
                                                                    x.PublisherName.ToLower() = strSearchFor OrElse
                                                                    x.ISSN1.ToLower() = strSearchFor OrElse
                                                                    x.ISSN2.ToLower() = strSearchFor OrElse
                                                                    x.Source.ToLower() = strSearchFor OrElse
                                                                    x.Categories.ToLower() = strSearchFor OrElse
                                                                    x.Rating.ToLower() = strSearchFor OrElse
                                                                    x.Country.ToLower() = strSearchFor).ToList()
                                End If
                            Else
                                If chkPartialMatch.Checked Then
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor)).ToList()
                                Else
                                    lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor).ToList()
                                End If
                            End If
                    End Select


                Else
                    strSearchFor = strSearchFor.Replace("""", "") '-- must strip quotes, just in case it is a phrase

                    If chkAllColumns.Checked Then
                        If chkPartialMatch.Checked Then
                            lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor) OrElse
                                                                        x.PublisherName.ToLower.Contains(strSearchFor) OrElse
                                                                        x.ISSN1.ToLower.Contains(strSearchFor) OrElse
                                                                        x.ISSN2.ToLower.Contains(strSearchFor) OrElse
                                                                        x.Source.ToLower.Contains(strSearchFor) OrElse
                                                                        x.Categories.ToLower.Contains(strSearchFor) OrElse
                                                                        x.Rating.ToLower.Contains(strSearchFor) OrElse
                                                                        x.Country.ToLower.Contains(strSearchFor)).ToList()
                        Else
                            lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor OrElse
                                                                        x.PublisherName.ToLower() = strSearchFor OrElse
                                                                        x.ISSN1.ToLower() = strSearchFor OrElse
                                                                        x.ISSN2.ToLower() = strSearchFor OrElse
                                                                        x.Source.ToLower() = strSearchFor OrElse
                                                                        x.Categories.ToLower() = strSearchFor OrElse
                                                                        x.Rating.ToLower() = strSearchFor OrElse
                                                                        x.Country.ToLower() = strSearchFor).ToList()
                        End If
                    Else
                        If chkPartialMatch.Checked Then
                            lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(strSearchFor)).ToList()
                        Else
                            lstFiltered = JournalList.Where(Function(x) x.JournalName.ToLower() = strSearchFor).ToList()
                        End If
                    End If

                End If
                'If strSearchFor.Contains(":") Then
                '    Dim intPos As Integer = strSearchFor.IndexOf(":")
                '    Dim strField As String = strSearchFor.Substring(0, intPos).Trim().ToLower()
                '    strSearchFor = strSearchFor.Substring(intPos + 1).Trim()
                '    Select Case strField
                '        Case "group"
                '            lstFiltered = m_lstCurrentListOfStudents.Where(Function(x) x.StudentGroup.Equals(CInt(strSearchFor))).ToList()
                '        Case "team"
                '            lstFiltered = m_lstCurrentListOfStudents.Where(Function(x) x.StudentTeam.ToLower.Equals(strSearchFor.ToLower)).ToList()
                '        Case "tags"
                '            lstFiltered = m_lstCurrentListOfStudents.Where(Function(x) x.Tags.ToLower.Contains(strSearchFor.ToLower)).ToList()
                '        Case "nickname"
                '            lstFiltered = m_lstCurrentListOfStudents.Where(Function(x) x.Nickname.ToLower.Contains(strSearchFor.ToLower)).ToList()
                '    End Select


                'todo: could add filtering on multiple keywords, not just phrases (so "education technolog" would find everywith "education" and also "technolog" not just things with "education technolog"

                'End If

                dgvData.DataSource = lstFiltered
                lblFilteredCount.Text = "(" & lstFiltered.Count.ToString("#,##0") & " filtered)"
            End If
        Catch ex As Exception
            MessageBox.Show("There was an error filtering: " & ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub dgvData_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvData.KeyDown
        If e.KeyCode = Keys.C AndAlso ModifierKeys = ModifierKeys.Control Then
            Clipboard.SetText(dgvData.CurrentCell.Value)
        End If
    End Sub

    Private Sub AutosizeColumnsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutosizeColumnsToolStripMenuItem.Click
        dgvData.AutoResizeColumns()
    End Sub

    Private Sub ClearFilterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearFilterToolStripMenuItem.Click
        If txtFilter.Text.Length > 0 Then
            txtFilter.Text = String.Empty
            ApplyFilter()
        End If
        txtFilter.Focus()
    End Sub

    Private Sub MinisizeColumnsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinisizeColumnsToolStripMenuItem.Click
        dgvData.Columns(0).Width = 300
        dgvData.Columns(1).Width = 250
        dgvData.Columns(2).Width = 100
        dgvData.Columns(3).Width = 100
        dgvData.Columns(4).Width = 100
        dgvData.Columns(5).Width = 50
        dgvData.Columns(6).Width = 50
        dgvData.Columns(7).Width = 50
        dgvData.Columns(8).Width = 100
        dgvData.Columns(9).Width = 500
    End Sub

    Private Sub AddOneEntryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddOneEntryToolStripMenuItem.Click
        Dim obj As New Entry()
        obj.Source = "ManualEntry"
        obj.JournalName = "New journal"
        JournalList.Add(obj)
        ReloadData()
        txtFilter.Text = "ManualEntry"
        chkAllColumns.Checked = True
        ApplyFilter()
    End Sub

    Private Sub dgvData_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvData.ColumnHeaderMouseClick
        '-- Manually sort
        Try
            Dim col As DataGridViewColumn = dgvData.Columns(e.ColumnIndex)
            Dim lst As List(Of Entry) = dgvData.DataSource
            Dim cmp As IComparer(Of Entry)
            Dim boolSort As Boolean = True

            If col IsNot Nothing Then
                Select Case col.DataPropertyName
                    Case "JournalName"
                        cmp = New EntryCompareByJournalName
                    Case "PublisherName"
                        cmp = New EntryCompareByPublisherName
                    Case "ISSN1"
                        cmp = New EntryCompareByISSN1
                    Case "ISSN2"
                        cmp = New EntryCompareByISSN2
                    Case "Source"
                        cmp = New EntryCompareBySource
                    Case "Ranking"
                        cmp = New EntryCompareByRanking
                    Case "Rating"
                        cmp = New EntryCompareByRating
                    Case "HIndex"
                        cmp = New EntryCompareByHIndex
                    Case "Country"
                        cmp = New EntryCompareByCountry
                    Case "Categories"
                        cmp = New EntryCompareByCategories
                    Case "Notes"
                        cmp = New EntryCompareByNotes
                    Case Else
                        boolSort = False
                End Select

                If boolSort Then
                    lst.Sort(cmp)
                    dgvData.DataSource = Nothing
                    dgvData.Refresh()
                    dgvData.DataSource = lst
                    dgvData.Refresh()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("There was an error sorting: " & ex.Message)
        End Try

    End Sub
End Class
