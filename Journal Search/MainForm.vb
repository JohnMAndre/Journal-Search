Imports System.Drawing.Text
Imports System.Xml

Public Class MainForm
    Private m_intCounter As Integer
    Private m_boolSkipPersistDataOnSave As Boolean = True '-- default to true unless overridden using menu option

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        m_boolSkipPersistDataOnSave = True
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

        AddNotesToJournals(JournalList)

        ''-- Now we must add the notes
        'If System.IO.File.Exists("JournalSearchDataExtra.xml") Then
        '    xDoc.Load("JournalSearchDataExtra.xml")
        '    xList = xDoc.SelectNodes("//JournalNote")
        '    Dim strName As String
        '    Dim strNotes As String

        '    m_intCounter = 0
        '    prgStatusBar.Maximum = xList.Count

        '    For Each xElement As XmlElement In xList
        '        m_intCounter += 1
        '        UpdateLoadingProgress()

        '        strName = xElement.GetAttribute("JournalName").ToLower()
        '        strNotes = xElement.InnerText
        '        If strNotes.Length > 0 Then
        '            For Each obj In JournalList
        '                If obj.JournalName.ToLower() = strName Then
        '                    obj.Notes = xElement.InnerText
        '                End If
        '            Next
        '        End If
        '    Next
        'End If

        UpdateLoadingProgress()

        dgvData.AutoGenerateColumns = False
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
        cboFilterColumn.SelectedIndex = SearchColumnEnum.AllColumns
        cboMatchType.SelectedIndex = 0
        Application.DoEvents()

        LoadData()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If m_boolSkipPersistDataOnSave Then
            PersistData()
        End If
    End Sub
    Private Sub PersistData()
        '-- save back data to main data file
        Dim xDoc1 As New Xml.XmlDocument()
        Dim xDoc2 As New Xml.XmlDocument()
        Dim root1 As Xml.XmlElement = xDoc1.AppendChild(xDoc1.CreateElement("Journals"))
        Dim root2 As Xml.XmlElement = xDoc2.AppendChild(xDoc2.CreateElement("JournalNotes"))
        Dim element1, xUserEntered, xInfoURL, xSubmitURL, xSubmitHistory, xAPC, xNotes As Xml.XmlElement
        Dim boolAddUserEnteredData As Boolean

        For Each obj As Entry In JournalList
            '-- Main data file
            element1 = xDoc1.CreateElement("Journal")
            xUserEntered = xDoc2.CreateElement("JournalNote")
            boolAddUserEnteredData = False '-- default to not adding

            element1.SetAttribute("JournalName", obj.JournalName)
            element1.SetAttribute("PublisherName", obj.PublisherName)
            element1.SetAttribute("ISSNs", obj.ISSNs)
            element1.SetAttribute("Source", obj.Source)
            element1.SetAttribute("Ranking", obj.Ranking)
            element1.SetAttribute("Rating", obj.Rating)
            element1.SetAttribute("HIndex", obj.HIndex)
            element1.SetAttribute("Country", obj.Country)
            element1.SetAttribute("Categories", obj.Categories)
            element1.SetAttribute("Areas", obj.Areas)

            root1.AppendChild(element1)


            '-- Notes and other user-entered info in alternate file
            xUserEntered.SetAttribute("JournalName", obj.JournalName)

            If obj.InfoURL IsNot Nothing AndAlso obj.InfoURL.Length > 0 Then
                xInfoURL = xUserEntered.AppendChild(xDoc2.CreateElement("InfoURL"))
                xInfoURL.InnerText = obj.InfoURL
                boolAddUserEnteredData = True
            End If

            If obj.SubmitURL IsNot Nothing AndAlso obj.SubmitURL.Length > 0 Then
                xSubmitURL = xUserEntered.AppendChild(xDoc2.CreateElement("SubmitURL"))
                xSubmitURL.InnerText = obj.SubmitURL
                boolAddUserEnteredData = True
            End If

            If obj.SubmitHistory IsNot Nothing AndAlso obj.SubmitHistory.Length > 0 Then
                xSubmitHistory = xUserEntered.AppendChild(xDoc2.CreateElement("SubmitHistory"))
                xSubmitHistory.InnerText = obj.SubmitHistory
                boolAddUserEnteredData = True
            End If

            If obj.APC IsNot Nothing AndAlso obj.APC.Length > 0 Then
                xAPC = xUserEntered.AppendChild(xDoc2.CreateElement("APC"))
                xAPC.InnerText = obj.APC
                boolAddUserEnteredData = True
            End If

            If obj.Notes IsNot Nothing AndAlso obj.Notes.Length > 0 Then
                xNotes = xUserEntered.AppendChild(xDoc2.CreateElement("Notes"))
                xNotes.InnerText = obj.Notes
                boolAddUserEnteredData = True
            End If

            If boolAddUserEnteredData Then
                root2.AppendChild(xUserEntered)
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
#Region " Filter routines "
    Public Function ApplyFilterAllColumns(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(item) OrElse
                                                                x.PublisherName.ToLower.Contains(item) OrElse
                                                                x.ISSNs.ToLower.Contains(item) OrElse
                                                                x.Source.ToLower.Contains(item) OrElse
                                                                x.Categories.ToLower.Contains(item) OrElse
                                                                x.Areas.ToLower.Contains(item) OrElse
                                                                x.Notes.ToLower.Contains(item) OrElse
                                                                x.Rating.ToLower.Contains(item) OrElse
                                                                x.Country.ToLower.Contains(item))).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(item) OrElse
                                                                                        x.PublisherName.ToLower.Contains(item) OrElse
                                                                                        x.ISSNs.ToLower.Contains(item) OrElse
                                                                                        x.Source.ToLower.Contains(item) OrElse
                                                                                        x.Categories.ToLower.Contains(item) OrElse
                                                                                        x.Areas.ToLower.Contains(item) OrElse
                                                                                        x.Notes.ToLower.Contains(item) OrElse
                                                                                        x.Rating.ToLower.Contains(item) OrElse
                                                                                        x.Country.ToLower.Contains(item))).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(item) OrElse
                                                                                        x.PublisherName.ToLower.Contains(item) OrElse
                                                                                        x.ISSNs.ToLower.Contains(item) OrElse
                                                                                        x.Source.ToLower.Contains(item) OrElse
                                                                                        x.Categories.ToLower.Contains(item) OrElse
                                                                                        x.Areas.ToLower.Contains(item) OrElse
                                                                                        x.Notes.ToLower.Contains(item) OrElse
                                                                                        x.Rating.ToLower.Contains(item) OrElse
                                                                                        x.Country.ToLower.Contains(item))).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) (x.JournalName.ToLower.Contains(searchPhrase) OrElse
                                                                x.PublisherName.ToLower.Contains(searchPhrase) OrElse
                                                                x.ISSNs.ToLower.Contains(searchPhrase) OrElse
                                                                x.Source.ToLower.Contains(searchPhrase) OrElse
                                                                x.Categories.ToLower.Contains(searchPhrase) OrElse
                                                                x.Areas.ToLower.Contains(searchPhrase) OrElse
                                                                x.Notes.ToLower.Contains(searchPhrase) OrElse
                                                                x.Rating.ToLower.Contains(searchPhrase) OrElse
                                                                x.Country.ToLower.Contains(searchPhrase))).ToList()
        End If

        Return lstReturn

    End Function
    Public Function ApplyFilterJournalName(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.JournalName.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.JournalName.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.JournalName.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterJournalNameExact(searchCriteria As String) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        '-- search based on phrase
        Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
        lstReturn = JournalList.Where(Function(x) x.JournalName.ToLower.Equals(searchPhrase)).ToList()

        Return lstReturn
    End Function
    Public Function ApplyFilterPublisher(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.PublisherName.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.PublisherName.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.PublisherName.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.PublisherName.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterISSNs(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.ISSNs.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.ISSNs.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.ISSNs.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.ISSNs.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterSource(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Source.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Source.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Source.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Source.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterRanking(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Ranking.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Ranking.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Ranking.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Ranking.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterRating(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Rating.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Rating.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Rating.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Rating.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterHIndex(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.HIndex.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.HIndex.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.HIndex.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.HIndex.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterCountry(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Country.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Country.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Country.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Country.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterCategories(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Categories.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Categories.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Categories.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Categories.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterAreas(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Areas.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Areas.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Areas.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Areas.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Public Function ApplyFilterNotes(searchCriteria As String, matchAllCriteria As Boolean) As List(Of Entry)
        '-- always partial match
        Dim lstReturn As List(Of Entry)

        If searchCriteria.Contains(" ") AndAlso Not searchCriteria.StartsWith("""") Then
            '-- it has a space, not starting with a quote, spit into multiple criteria
            Dim str As String() = searchCriteria.Split(" ")
            Dim combinedList As IEnumerable(Of Entry)

            For Each item As String In str
                If lstReturn Is Nothing Then
                    lstReturn = JournalList.Where(Function(x) x.Notes.ToLower.Contains(item)).ToList()
                Else
                    If matchAllCriteria Then
                        '-- match all criteria
                        combinedList = lstReturn.Intersect(JournalList.Where(Function(x) x.Notes.ToLower.Contains(item)).ToList())
                    Else
                        '-- match any criteria
                        combinedList = lstReturn.Union(JournalList.Where(Function(x) x.Notes.ToLower.Contains(item)).ToList())
                    End If
                    lstReturn = combinedList.ToList()
                End If
            Next
        Else
            '-- search based on phrase
            Dim searchPhrase As String = searchCriteria.Replace("""", "") '-- must strip quotes, just in case it is a phrase
            lstReturn = JournalList.Where(Function(x) x.Notes.ToLower.Contains(searchPhrase)).ToList()
        End If

        Return lstReturn
    End Function
    Private Sub ApplyFilter()
        Try
            Dim lstFiltered As List(Of Entry)
            Select Case cboFilterColumn.SelectedIndex
                Case SearchColumnEnum.AllColumns
                    lstFiltered = ApplyFilterAllColumns(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.JournalName
                    lstFiltered = ApplyFilterJournalName(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Publisher
                    lstFiltered = ApplyFilterPublisher(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.ISSNs
                    lstFiltered = ApplyFilterISSNs(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Source
                    lstFiltered = ApplyFilterSource(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Ranking
                    lstFiltered = ApplyFilterRanking(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Rating
                    lstFiltered = ApplyFilterRating(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.HIndex
                    lstFiltered = ApplyFilterHIndex(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Country
                    lstFiltered = ApplyFilterCountry(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Categories
                    lstFiltered = ApplyFilterCategories(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Areas
                    lstFiltered = ApplyFilterAreas(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
                Case SearchColumnEnum.Notes
                    lstFiltered = ApplyFilterNotes(txtFilter.Text.ToLower(), cboMatchType.SelectedIndex = 0)
            End Select

            dgvData.DataSource = lstFiltered
            lblFilteredCount.Text = "(" & lstFiltered.Count.ToString("#,##0") & " filtered)"
        Catch ex As Exception
            MessageBox.Show("There was an error filtering: " & ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
#End Region

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
        cboFilterColumn.SelectedIndex = SearchColumnEnum.AllColumns
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
                        cmp = New EntryCompareByISSNs
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
                    Case "Areas"
                        cmp = New EntryCompareByAreas
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

    'Private Sub dgvData_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvData.CellEndEdit
    '    '-- Persist notes (if that was the cell edited)
    '    If dgvData.Columns(e.ColumnIndex).DataPropertyName = NotesColumn.DataPropertyName Then
    '        '-- persist notes
    '        Dim strNotes As String = dgvData.CurrentCell.Value
    '        Dim obj As Entry = dgvData.Rows(e.RowIndex).DataBoundItem
    '        Dim strJournal As String = obj.JournalName
    '        PersistChangedNotes(strJournal, strNotes)
    '    End If
    'End Sub
    'Private Sub PersistChangedNotes(journalName As String, notes As String)
    '    Dim xDoc2 As New Xml.XmlDocument()
    '    xDoc2.Load("JournalSearchDataExtra.xml")
    '    Dim root2 As Xml.XmlElement = xDoc2.DocumentElement()
    '    Dim element2 As Xml.XmlElement = root2.SelectSingleNode("//JournalNote[@JournalName='" & journalName & "']")

    '    If element2 Is Nothing Then
    '        element2 = root2.AppendChild(xDoc2.CreateElement("JournalNote"))
    '    End If

    '    element2.SetAttribute("JournalName", journalName)
    '    element2.InnerText = notes

    '    xDoc2.Save("JournalSearchDataExtra.xml")

    'End Sub

    Private Sub MatchSelectedJournalNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MatchSelectedJournalNameToolStripMenuItem.Click
        Try
            Dim lstFiltered As List(Of Entry)
            lstFiltered = ApplyFilterJournalNameExact(txtFilter.Text.Trim.ToLower())
            dgvData.DataSource = lstFiltered
            lblFilteredCount.Text = "(" & lstFiltered.Count.ToString("#,##0") & " filtered)"
        Catch ex As Exception
            MessageBox.Show("There was an error filtering: " & ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub ExitWithoutSavingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitWithoutSavingToolStripMenuItem.Click
        m_boolSkipPersistDataOnSave = False
        Me.Close()
    End Sub
End Class
