Public Class GenericTagging
    Public Class Status
        Public Property code As Integer
        Public Property text As String
        Public Property request_id As String
    End Class

    Public Class Tag
        Public Property prob As Double
        Public Property name As String
    End Class

    Public Class Record
        Public Property _status As Status
        Public Property _id As String
        Public Property _width As Integer
        Public Property _height As Integer
        Public Property _tags As List(Of Tag)
    End Class

    Public Class Status2
        Public Property code As Integer
        Public Property text As String
        Public Property request_id As String
        Public Property proc_id As String
    End Class

    Public Class Statistics
        Public __invalid_name__processing As Double
    End Class

    Public Class RootObject
        Public Property lang As String
        Public Property tagging_mode As String
        Public Property records As List(Of Record)
        Public Property status As Status2
        Public Property statistics As Statistics
    End Class

End Class
