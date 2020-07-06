Imports System.Data.SqlClient

Module SQL
    Public Sub SqlInsert(TheQuery As String, TheDatabase As String, TheTable As String)
        Try

            Dim SqlConnectionString As String = "Server=" & Constants.SqlServerHostname & "," & Constants.SqlPort & ";Database=" & TheDatabase & ";User Id=" & Constants.SqlServerUser & ";Password=" & Constants.SqlServerPass & ";"
            Dim sqlConnection1 As New SqlConnection(SqlConnectionString)
            Dim cmd As New SqlCommand
            Dim reader As SqlDataReader
            cmd.CommandText = TheQuery
            cmd.CommandType = CommandType.Text
            cmd.Connection = sqlConnection1
            sqlConnection1.Open()
            reader = cmd.ExecuteReader()
            reader.Close()
            sqlConnection1.Close()
        Catch ex As Exception
            Debug.Print(ex.ToString)
        End Try
    End Sub

    Public Function SqlSelect(TheQuery As String, TheDatabase As String, TheTable As String) As String
        Dim SqlConnectionString As String = "Server=" & Constants.SqlServerHostname & "," & Constants.SqlPort & ";Database=" & TheDatabase & ";User Id=" & Constants.SqlServerUser & ";Password=" & Constants.SqlServerPass & ";"

        Try
            Dim sqlConnection1 As New SqlConnection(SqlConnectionString)
            Dim cmd As New SqlCommand
            Dim reader As SqlDataReader

            cmd.CommandText = TheQuery
            cmd.CommandType = CommandType.Text
            cmd.Connection = sqlConnection1
            sqlConnection1.Open()
            reader = cmd.ExecuteReader()

            Dim TheReturn As String = ""

            If reader.HasRows Then
                Do While reader.Read()
                    TheReturn = reader.GetValue(0)
                    TheReturn = TheReturn & "|" & reader.GetValue(1)
                    TheReturn = TheReturn & "|" & reader.GetValue(2)
                    TheReturn = TheReturn & "|" & reader.GetValue(3)
                    TheReturn = TheReturn & "|" & reader.GetValue(4)
                    TheReturn = TheReturn & "|" & reader.GetValue(5)
                    TheReturn = TheReturn & "|" & reader.GetValue(6)
                    Exit Do
                Loop
            End If

            reader.Close()
            sqlConnection1.Close()

            Return TheReturn
        Catch ex As Exception
            'WriteServerLog("SqlSelect Error " & ex.ToString & " " & TheQuery & " " & TheDatabase & " " & TheTable)
            Debug.Print(ex.ToString)
        End Try
    End Function

    Public Sub SqlUpdate(TheQuery As String, TheDatabase As String, TheTable As String)
        Dim SqlConnectionString As String = "Server=" & Constants.SqlServerHostname & "," & Constants.SqlPort & ";Database=" & TheDatabase & ";User Id=" & Constants.SqlServerUser & ";Password=" & Constants.SqlServerPass & ";"

        Try
            Dim sqlConnection1 As New SqlConnection(SqlConnectionString)
            Dim cmd As New SqlCommand
            Dim reader As SqlDataReader
            cmd.CommandText = TheQuery
            cmd.CommandType = CommandType.Text
            cmd.Connection = sqlConnection1
            sqlConnection1.Open()
            reader = cmd.ExecuteReader()
            reader.Close()
            sqlConnection1.Close()
        Catch ex As Exception
            'WriteServerLog("SqlInsert error " & ex.ToString & " " & TheQuery & " " & TheDatabase & " " & TheTable)
            Debug.Print(ex.ToString)
        End Try
    End Sub
End Module
