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


    Public Sub AddNotesToJournals(journals As List(Of Entry))
        '--  add the notes from the notes file
        Dim xDoc As New Xml.XmlDocument()
        Dim xList As XmlNodeList

        If System.IO.File.Exists("JournalSearchDataExtra.xml") Then
            xDoc.Load("JournalSearchDataExtra.xml")
            xList = xDoc.SelectNodes("//JournalNote")
            Dim strName As String
            Dim strNotes As String

            For Each xElement As XmlElement In xList
                strName = xElement.GetAttribute("JournalName").ToLower()
                strNotes = xElement.InnerText
                If strNotes.Length > 0 Then
                    For Each obj In journals
                        If obj.JournalName.ToLower() = strName OrElse
                            obj.JournalName.Replace(DISCONTINUED_IDENTIFIER, String.Empty).ToLower() = strName.Replace(DISCONTINUED_IDENTIFIER, String.Empty) Then
                            obj.Notes = xElement.InnerText
                        End If
                    Next
                End If
            Next
        End If

    End Sub
End Module
