Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data

'=======================
'FriedParts CORE Library
'=======================
'- Contains utilities and debugging/notification functions of general interest

Public Module fpCore

    'Takes in a DataTable (which came back from a Database Query) and the name of an ID column
    '   ...and returns an array of those ID's found without any duplicates -- e.g. the unique ID's
    '       found in the list.
    Public Function fpUniqueID(ByRef DT As DataTable, ByVal IDname As String) As Int32()
        Dim retval() As Int32
        Dim blahQ As New Queue()
        Dim intTemp As Int32
        'We allow a Distributor to have many names (aliases) to deal with different distributors calling the same people different names
        'Eliminate multiple names for the same Distributor to distill it to just its unique ID
        For Each r As DataRow In DT.Rows
            intTemp = r.Field(Of Int32)(IDname)
            If Not blahQ.Contains(intTemp) Then
                blahQ.Enqueue(intTemp)
            End If
        Next
        'Transfer the Queue structure, which we used to unique-ify 
        '   the return dataset, to an array
        ReDim retval(blahQ.Count - 1) 'Set to new upper bound, but arrays are indexed from 0 so fix by -1 offset. Whew! That was more complicated than it needed to be!
        blahQ.CopyTo(retval, 0)
        Return retval
    End Function

    'Generate a unique negative number within the int32 number space
    'If this function returns the same number for two different users at the same time BAD THINGS WILL HAPPEN!
    Public Function procGenTempUID() As Int32
        Dim r As New Random(Now.Millisecond)
        Return (-1 * (Now.Ticks Mod (2 ^ 32 / 2 - 1)) + r.Next()) Mod (2 ^ 32 / 2 - 1)
    End Function

End Module
