Imports System.Xml

Public Class Entry
    Private Shared Function Normalize(value As String) As String
        Return If(value, String.Empty)
    End Function

    Public Sub New(xElement As XmlElement)
        Application.DoEvents()
        Me.JournalName = xElement.GetAttribute("JournalName")
        Me.PublisherName = xElement.GetAttribute("PublisherName")
        Me.ISSNs = xElement.GetAttribute("ISSNs")
        Me.Source = xElement.GetAttribute("Source")
        Me.Ranking = xElement.GetAttribute("Ranking")
        Me.Rating = xElement.GetAttribute("Rating")
        Me.HIndex = xElement.GetAttribute("HIndex")
        Me.Country = xElement.GetAttribute("Country")
        Me.Categories = xElement.GetAttribute("Categories")
        Me.Areas = xElement.GetAttribute("Areas")
        '-- Remember, user fields are in another xml doc, not the main one
    End Sub
    Public Sub New()

    End Sub
    Public Sub LoadUserFields(xElement As XmlElement)
        '-- Remember, user fields are in another xml doc, not the main one, this is the user element


    End Sub
    Private m_journalName As String = String.Empty
    Public Property JournalName As String
        Get
            Return m_journalName
        End Get
        Set(value As String)
            m_journalName = Normalize(value)
        End Set
    End Property

    Private m_publisherName As String = String.Empty
    Public Property PublisherName As String
        Get
            Return m_publisherName
        End Get
        Set(value As String)
            m_publisherName = Normalize(value)
        End Set
    End Property

    Private m_issns As String = String.Empty
    Public Property ISSNs As String
        Get
            Return m_issns
        End Get
        Set(value As String)
            m_issns = Normalize(value)
        End Set
    End Property

    Private m_source As String = String.Empty
    Public Property Source As String
        Get
            Return m_source
        End Get
        Set(value As String)
            m_source = Normalize(value)
        End Set
    End Property

    Private m_ranking As String = String.Empty
    Public Property Ranking As String
        Get
            Return m_ranking
        End Get
        Set(value As String)
            m_ranking = Normalize(value)
        End Set
    End Property

    Private m_rating As String = String.Empty
    Public Property Rating As String
        Get
            Return m_rating
        End Get
        Set(value As String)
            m_rating = Normalize(value)
        End Set
    End Property

    Private m_hIndex As String = String.Empty
    Public Property HIndex As String
        Get
            Return m_hIndex
        End Get
        Set(value As String)
            m_hIndex = Normalize(value)
        End Set
    End Property

    Private m_country As String = String.Empty
    Public Property Country As String
        Get
            Return m_country
        End Get
        Set(value As String)
            m_country = Normalize(value)
        End Set
    End Property

    Private m_categories As String = String.Empty
    Public Property Categories As String
        Get
            Return m_categories
        End Get
        Set(value As String)
            m_categories = Normalize(value)
        End Set
    End Property

    Private m_areas As String = String.Empty
    Public Property Areas As String
        Get
            Return m_areas
        End Get
        Set(value As String)
            m_areas = Normalize(value)
        End Set
    End Property

    Private m_infoURL As String = String.Empty
    Public Property InfoURL As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_infoURL
        End Get
        Set(value As String)
            m_infoURL = Normalize(value)
        End Set
    End Property

    Private m_submitURL As String = String.Empty
    Public Property SubmitURL As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_submitURL
        End Get
        Set(value As String)
            m_submitURL = Normalize(value)
        End Set
    End Property

    Private m_submitHistory As String = String.Empty
    Public Property SubmitHistory As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_submitHistory
        End Get
        Set(value As String)
            m_submitHistory = Normalize(value)
        End Set
    End Property

    Private m_submitFee As String = String.Empty
    Public Property SubmitFee As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_submitFee
        End Get
        Set(value As String)
            m_submitFee = Normalize(value)
        End Set
    End Property

    Private m_apc As String = String.Empty
    Public Property APC As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_apc
        End Get
        Set(value As String)
            m_apc = Normalize(value)
        End Set
    End Property

    Private m_notes As String = String.Empty
    Public Property Notes As String '-- User-entered, maintained in separate file from main data
        Get
            Return m_notes
        End Get
        Set(value As String)
            m_notes = Normalize(value)
        End Set
    End Property

    Public Property Flagged As Boolean = False '-- temporary, only while running, used to do other action (replace all ___ for flagged)

End Class
