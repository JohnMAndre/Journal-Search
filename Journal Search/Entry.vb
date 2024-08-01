﻿Imports System.Xml

Public Class Entry
    Public Sub New(xElement As XmlElement)
        Application.DoEvents()
        Me.JournalName = xElement.GetAttribute("JournalName")
        Me.PublisherName = xElement.GetAttribute("PublisherName")
        Me.ISSN1 = xElement.GetAttribute("ISSN1")
        Me.ISSN2 = xElement.GetAttribute("ISSN2")
        Me.Source = xElement.GetAttribute("Source")
        Me.Ranking = xElement.GetAttribute("Ranking")
        Me.Rating = xElement.GetAttribute("Rating")
        Me.HIndex = xElement.GetAttribute("HIndex")
        Me.Country = xElement.GetAttribute("Country")
        Me.Categories = xElement.GetAttribute("Categories")
    End Sub
    Public Sub New()

    End Sub
    Public Property JournalName As String = String.Empty
    Public Property PublisherName As String = String.Empty
    Public Property ISSN1 As String = String.Empty
    Public Property ISSN2 As String = String.Empty
    Public Property Source As String = String.Empty
    Public Property Ranking As String = String.Empty
    Public Property Rating As String = String.Empty
    Public Property HIndex As String = String.Empty
    Public Property Country As String = String.Empty
    Public Property Categories As String = String.Empty
    Public Property Notes As String = String.Empty '-- User-entered, maintained in separate file from main data
End Class
