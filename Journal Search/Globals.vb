Imports System.Collections.ObjectModel
Imports System.Xml

Module Globals
    Public JournalList As New List(Of Entry)


#Region " Constants "
    Public Const DISCONTINUED_IDENTIFIER As String = " (discontinued)"
#End Region

#Region " Enums "
    Public Enum SearchColumnEnum
        AllColumns
        JournalName
        Publisher
        ISSNs
        Source
        Ranking
        Rating
        HIndex
        Country
        Categories
        Areas
        Notes
    End Enum
#End Region

    Public Function GetDecodedText(codedText As String) As String
        Dim strReturn As String = codedText.Trim()
        strReturn = strReturn.Replace("&", "and")

        Return strReturn
    End Function
    Public Sub AddNotesToJournals(journals As List(Of Entry))
        '--  add the notes from the notes file
        Dim xDoc As New Xml.XmlDocument()
        Dim xList As XmlNodeList

        If System.IO.File.Exists("JournalSearchDataExtra.xml") Then
            xDoc.Load("JournalSearchDataExtra.xml")
            xList = xDoc.SelectNodes("//JournalNote")
            Dim strName As String

            For Each xElement As XmlElement In xList
                Dim strNotes As String = String.Empty
                Dim strInfoURL As String = String.Empty
                Dim strSubmitURL As String = String.Empty
                Dim strSubmitHistory As String = String.Empty
                Dim strAPC As String = String.Empty
                Dim strSubmitFee As String = String.Empty
                strName = xElement.GetAttribute("JournalName").ToLower()
                If xElement.HasChildNodes AndAlso xElement.ChildNodes(0).NodeType = XmlNodeType.Element Then
                    '-- New logic holding more variables beside just Notes
                    Application.DoEvents()
                    Dim xElement1 As Xml.XmlElement
                    xElement1 = xElement.SelectSingleNode("Notes")
                    If xElement1 IsNot Nothing Then
                        strNotes = xElement1.InnerText
                    End If

                    xElement1 = xElement.SelectSingleNode("APC")
                    If xElement1 IsNot Nothing Then
                        strAPC = xElement1.InnerText
                    End If

                    xElement1 = xElement.SelectSingleNode("SubmitFee")
                    If xElement1 IsNot Nothing Then
                        strSubmitFee = xElement1.InnerText
                    End If

                    xElement1 = xElement.SelectSingleNode("SubmitHistory")
                    If xElement1 IsNot Nothing Then
                        strSubmitHistory = xElement1.InnerText
                    End If

                    xElement1 = xElement.SelectSingleNode("SubmitURL")
                    If xElement1 IsNot Nothing Then
                        strSubmitURL = xElement1.InnerText
                    End If

                    xElement1 = xElement.SelectSingleNode("InfoURL")
                    If xElement1 IsNot Nothing Then
                        strInfoURL = xElement1.InnerText
                    End If

                    '-- Now assign the values to the journal
                    '-- Could be multiple (because the same journal could be in here from multiple
                    '   data sources) so cannot exit this loop early
                    For Each obj In journals
                        If obj.JournalName.ToLower() = strName OrElse
                           obj.JournalName.Replace(DISCONTINUED_IDENTIFIER, String.Empty).ToLower() = strName.Replace(DISCONTINUED_IDENTIFIER, String.Empty) Then
                            obj.Notes = strNotes
                            obj.APC = strAPC
                            obj.SubmitFee = strSubmitFee
                            obj.SubmitHistory = strSubmitHistory
                            obj.SubmitURL = strSubmitURL
                            obj.InfoURL = strInfoURL

                            '-- Could be multiple (because the same journal could be in here from multiple data sources) so cannot exit this loop early
                        End If
                    Next
                    'Else
                    '    '-- old logic replaced June 2025 but maintained for old data sources
                    '    strNotes = xElement.InnerText
                    '    If strNotes.Length > 0 Then
                    '        For Each obj In journals
                    '            If obj.JournalName.ToLower() = strName OrElse
                    '               obj.JournalName.Replace(DISCONTINUED_IDENTIFIER, String.Empty).ToLower() = strName.Replace(DISCONTINUED_IDENTIFIER, String.Empty) Then
                    '                obj.Notes = xElement.InnerText
                    '                '-- Could be multiple (because the same journal could be in here from multiple data sources) so cannot exit this loop early
                    '            End If
                    '        Next
                    '    End If
                End If



            Next
        End If

        'If obj.InfoURL IsNot Nothing AndAlso obj.InfoURL.Length > 0 Then
        '    xInfoURL = xUserEntered.AppendChild(xDoc2.CreateElement("InfoURL"))
        '    xInfoURL.InnerText = obj.InfoURL
        '    boolAddUserEnteredData = True
        'End If

        'If obj.SubmitURL IsNot Nothing AndAlso obj.SubmitURL.Length > 0 Then
        '    xSubmitURL = xUserEntered.AppendChild(xDoc2.CreateElement("SubmitURL"))
        '    xSubmitURL.InnerText = obj.SubmitURL
        '    boolAddUserEnteredData = True
        'End If

        'If obj.SubmitHistory IsNot Nothing AndAlso obj.SubmitHistory.Length > 0 Then
        '    xSubmitHistory = xUserEntered.AppendChild(xDoc2.CreateElement("SubmitHistory"))
        '    xSubmitHistory.InnerText = obj.SubmitHistory
        '    boolAddUserEnteredData = True
        'End If

        'If obj.APC IsNot Nothing AndAlso obj.APC.Length > 0 Then
        '    xAPC = xUserEntered.AppendChild(xDoc2.CreateElement("APC"))
        '    xAPC.InnerText = obj.APC
        '    boolAddUserEnteredData = True
        'End If

        'If obj.Notes IsNot Nothing AndAlso obj.Notes.Length > 0 Then
        '    xNotes = xUserEntered.AppendChild(xDoc2.CreateElement("Notes"))
        '    xNotes.InnerText = obj.Notes
        '    boolAddUserEnteredData = True
        'End If


    End Sub
End Module
