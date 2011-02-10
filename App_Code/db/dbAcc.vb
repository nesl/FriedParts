Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

'PROVIDES DATA ACCESS FUNCTIONALITY
'   SQL STATEMENT EXECUTION
'   

Public Module dbAcc

    Public Const cnxStr As String = _
        "Data Source = friedparts.nesl.ucla.edu;" & _
        "Initial Catalog = FriedParts;" & _
        "Persist Security Info = True;" & _
        "User ID = fp_system;" & _
        "Password = 1"
    '------------------------------------------------------------------------------------------------

    'Returns the first table returned by the submitted query
    'This is a static method (of course -- it's in a "module"). For web-scaling it does not do any 
    '   memory allocation, you must pass in the pointer to the datastructure that will get filled 
    '   and returned via the return parameter (.NET weirdness)
    Public Function SelectRows(ByRef returnTable As DataTable, ByRef connectionString As String, _
        ByRef queryString As String) As DataTable
        
        Using connection As New SqlConnection(connectionString)
            Dim adapter As New SqlDataAdapter()
            adapter.SelectCommand = New SqlCommand( _
                queryString, connection)
            adapter.Fill(returnTable)
            'CLEANUP! (important or connection will persist)
            adapter.Dispose()
            connection.Close()
            Return returnTable
        End Using

    End Function

    'OVERLOAD: Returns the first table returned by the submitted query
    'Same thing, but uses default connection string (saves time!)
    Public Function SelectRows(ByRef returnTable As DataTable, ByRef queryString As String) As DataTable
        Return SelectRows(returnTable, cnxStr, queryString)
    End Function


    'Execute a SQL Statement! This is for updates, deletes, adds, etc... <-- e.g. no data returned
    Public Sub SQLexe(ByVal SQL_Command As String)
        Dim Connection As New SqlConnection(cnxStr)
        Dim Command As New SqlCommand(SQL_Command, Connection)
        Connection.Open()
        Command.ExecuteReader()
        Command.Dispose()
        Connection.Close()
    End Sub

    'Returns a SQLDataReader object containing the results of the SQLtxt query. The standard connect string is used.
    'This function is more useful than SQLexe/SQLSelectRows when working with GridViews since it returns an object
    '   which can be used as a data source for the GridView.
    Public Function SQLasDataSource(ByVal SQL_Command As String) As SqlDataSource
        Dim retval As New SqlDataSource
        retval.ConnectionString = cnxStr
        retval.SelectCommand = SQL_Command
        Return retval
    End Function
End Module
