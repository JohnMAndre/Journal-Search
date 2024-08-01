Public Class EntryCompareByJournalName
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.JournalName.CompareTo(y.JournalName)
    End Function
End Class

Public Class EntryCompareByPublisherName
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.PublisherName.CompareTo(y.PublisherName)
    End Function
End Class
Public Class EntryCompareByISSN1
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.ISSN1.CompareTo(y.ISSN1)
    End Function
End Class
Public Class EntryCompareByISSN2
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.ISSN2.CompareTo(y.ISSN2)
    End Function
End Class
Public Class EntryCompareBySource
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Source.CompareTo(y.Source)
    End Function
End Class
Public Class EntryCompareByRanking
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Ranking.CompareTo(y.Ranking)
    End Function
End Class
Public Class EntryCompareByRating
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Rating.CompareTo(y.Rating)
    End Function
End Class
Public Class EntryCompareByHIndex
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.HIndex.CompareTo(y.HIndex)
    End Function
End Class
Public Class EntryCompareByCountry
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Country.CompareTo(y.Country)
    End Function
End Class
Public Class EntryCompareByCategories
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Categories.CompareTo(y.Categories)
    End Function
End Class
Public Class EntryCompareByAreas
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Areas.CompareTo(y.Areas)
    End Function
End Class
Public Class EntryCompareByNotes
    Implements IComparer(Of Entry)

    Public Function Compare(x As Entry, y As Entry) As Integer Implements IComparer(Of Entry).Compare
        Return x.Notes.CompareTo(y.Notes)
    End Function
End Class
