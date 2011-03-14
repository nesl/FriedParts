Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Net
Imports Newtonsoft.Json.Linq

Namespace apiOctopart
    ''' <summary>
    ''' Contains all of the Octopart exclusive Part Typing (categorization) code.
    ''' </summary>
    ''' <remarks></remarks>
    Public Module OctoPartType

        ''' <summary>
        ''' Overloads. Confirm the existence of a part category by its Octopart Category ID
        ''' </summary>
        ''' <param name="OctopartPartTypeID"></param>
        ''' <param name="fpTypeID">The FriedParts PartID of this category if it already exists (out only parameter)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ptExistsPartType(ByRef OctopartPartTypeID As Int64, Optional ByRef fpTypeID As Int32 = 0) As Boolean
            Dim dt As New DataTable
            SelectRows(dt, "SELECT [TypeID] FROM [FriedParts].[dbo].[part-PartTypes] WHERE [opTypeID] = " & OctopartPartTypeID)
            Select Case dt.Rows.Count
                Case 0
                    Return False
                Case 1
                    fpTypeID = dt.Rows(0).Field(Of Int32)("TypeID")
                    Return True
                Case Else
                    Throw New Exception("PartType is corrupt! Multiple hits on opTypeID")
            End Select
        End Function

        

        ''' <summary>
        ''' Add a new category
        ''' </summary>
        ''' <param name="OctopartTypeID"></param>
        ''' <param name="OctopartParentID"></param>
        ''' <param name="TypeName"></param>
        ''' <param name="imageURL"></param>
        ''' <returns>Returns the new *FriedParts* TypeID (or the existing FriedParts ID if this Octopart ID already exists)</returns>
        ''' <remarks></remarks>
        Public Function ptAddNewPartType(ByRef OctopartTypeID As Int64, ByRef OctopartParentID As Int64, ByRef TypeName As String, ByRef imageURL As String) As Int32

            '[STEP 0] Sanity Checks
            'Check my ID for prior existence
            Dim fpPartId As Int32
            If ptExistsPartType(OctopartTypeID, fpPartId) Then
                Return fpPartId
            End If
            'Find the FP Parent ID
            Dim parentID As Int32
            If Not ptExistsPartType(OctopartParentID, parentID) Then
                Throw New NullReferenceException("Parent Type Not Found!")
            End If

            '[STEP 1] Create the new Category 
            Dim dt As New DataTable
            Dim SQLcmd As String = _
            "INSERT INTO [FriedParts].[dbo].[part-PartTypes]" & _
            "           ([opTypeID]" & _
            "           ,[Parent]" & _
            "           ,[Type]" & _
            "           ,[TypeNotes]" & _
            "           ,[ImageURL])" & _
            "     VALUES (" & _
            "           " & OctopartTypeID & "," & _
            "           " & parentID & "," & _
            "           '" & sysText.txtDefangSQL(TypeName) & "'," & _
            "           '" & "[OP] Created " & Now & "'," & _
            "           '" & sysText.txtDefangSQL(imageURL) & "'" & _
            "           )"
            dbAcc.SQLexe(SQLcmd)

            '[STEP 2] Find the TypeID that we just created
            SQLcmd = _
                "SELECT [TypeID]" & _
                "  FROM [FriedParts].[dbo].[part-PartTypes] " & _
                "  WHERE [Type] = '" & sysText.txtDefangSQL(TypeName) & "' AND [Parent] = " & parentID & ";"
            dbAcc.SelectRows(dt, SQLcmd)

            '[STEP 3] Update the TypeID record to include its path back to the root node
            If dt.Rows.Count = 1 Then
                'FOUND! 
                SQLcmd = _
                    "UPDATE [FriedParts].[dbo].[part-PartTypes] " & _
                    "   SET [Path] = '" & ptTraceLineage(dt.Rows(0).Field(Of Int32)("TypeID")) & "' " & _
                    " WHERE [TypeID] = " & dt.Rows(0).Field(Of Int32)("TypeID") & ";"
                dbAcc.SQLexe(SQLcmd)
                'Log!
                logPartType(Nothing, HttpContext.Current.Session("user.UserID"), dt.Rows(0).Field(Of Int32)("TypeID"))
                Return dt.Rows(0).Field(Of Int32)("TypeID")
            Else
                'ERROR! Could not find the record we just created!

                dbLog.logSys(0, -1, "[ptAddNewPartType] Could not locate the new Part Type we just created!")
                dbLog.logDebug("[ptAddNewPartType] Could not locate the new Part Type we just created!")

                Throw New Exception("[ptAddNewPartType] Could not locate the new Part Type we just created!")
            End If
        End Function

        ''' <summary>
        ''' Takes in an Octopart category id and adds this category and all of its parents to the FriedParts Type Tree.
        ''' THIS IS THE MAIN ENTRY POINT INTO THIS MODULE.
        ''' </summary>
        ''' <param name="OctopartTypeID">The Octopart Category ID to use as a starting point.</param>
        ''' <remarks></remarks>
        Public Sub opProcessPartTypes(ByRef OctopartTypeID As Int64)
            'Format the query
            Dim URL As String = OctoCommands.GetCategory(OctopartTypeID)

            'Get the source
            Dim myWebClient As New System.Net.WebClient()
            Dim Source As String
            Try
                Source = myWebClient.DownloadString(URL)    'Source is the text received from OCTOPARTS website
            Catch ex As Exception
                If TypeOf ex Is System.Net.WebException Then
                    'Server did not respond in time! ...or other server error!
                    'like a bad (e.g. missing) OctopartTypeID -- just log and fail quietly
                    dbLog.logSys(HttpContext.Current.Session("user.UserID"), -1, "[opProcessPartTypes] Could not locate Octopart TypeID: " & OctopartTypeID)
                    Exit Sub
                ElseIf TypeOf ex Is NotSupportedException Then
                    'This function may not be called sumultaneously from multiple threads!
                Else
                    Throw New Exception(ex.Message)
                End If
            End Try

            'Sanity/Safety check
            If Source Is Nothing Then
                Throw New NullReferenceException("Missing HTML content! The WebClient did not return anything!")
            End If

            'Parse the JSON!
            Dim rjson As jobject
            rjson = JObject.Parse(Source)

            'This OctopartPartTypeID exists?
            'If rjson.Value(Of Int64)("id") Is Nothing Then
            'Empty -- do nothing (b/c we are already done!)
            'Exit Sub
            'End If

            '[Learn stuff from it!]
            '----------------------
            Dim jT As JArray
            Dim myID As Int64

            'My ID
            myID = rjson.Value(Of Int64)("id")

            'All my parents IDs
            jT = DirectCast(rjson.SelectToken("ancestors"), JArray)
            Dim NumTypesFound = jT.Count
            If NumTypesFound > 0 Then
                Dim ThisType As JToken
                For i As UInt32 = 0 To NumTypesFound - 1
                    'The Type path in order from origin(root) to child
                    ThisType = rjson.SelectToken("ancestors[" & i & "]")
                    ptAddNewPartType( _
                        ThisType.Value(Of Int64)("id"), _
                        GetParentID(i, jT), _
                        ThisType.Value(Of String)("nodename"), _
                        GetImageURL(ThisType) _
                    )
                Next
            Else
                'Nothing found... ergo, do nothing
            End If

            'Now add me! (safe to do so because we've checked that all the parent types are in the system by this point)
            ptAddNewPartType( _
                        rjson.Value(Of Int64)("id"), _
                        rjson.Value(Of Int64)("parent_id"), _
                        rjson.Value(Of String)("nodename"), _
                        GetImageURL(rjson) _
                    )
        End Sub

        ''' <summary>
        ''' Returns an appropriate image URL from the list of possibilities. Also handles the case where no images are available by returning the empty string.
        ''' </summary>
        ''' <param name="ThisType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetImageURL(ByRef ThisType As JToken) As String
            Try
                Return ThisType.SelectToken("images[0]").Value(Of String)("url")
            Catch ex As NullReferenceException
                Return ""
            End Try
        End Function

        Private Function GetParentID(ByRef myIndex As Byte, ByRef ancestors As JArray) As Int64
            If myIndex = 0 Then
                Return 0
            Else
                Return ancestors(myIndex - 1).Value(Of Int64)("id")
            End If
        End Function

    End Module

End Namespace

