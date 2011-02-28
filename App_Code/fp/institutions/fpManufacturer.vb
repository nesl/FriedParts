Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data
Imports apiOctopart

'Contains all code to manipulate the Manufacturer related database tables

Namespace fpManufacturer



    ''' <summary>
    ''' Accesses/Modifies Manufacturer values. Data pertains the Manufacturer
    ''' itself -- not it's parts/products.
    ''' </summary>
    ''' <remarks>To make this class as fast as possible, we cache only the 
    ''' likely data -- meaning the default name and common data. Alternate name
    ''' data is retrieved as needed.</remarks>
    Public Class fpMfr

        ''' <summary>
        ''' Internal variable. FriedParts Manufacturer UID.
        ''' </summary>
        ''' <remarks></remarks>
        Protected iMfrID As Int32

        ''' <summary>
        ''' Cache of the default manufacturer data.
        ''' </summary>
        ''' <remarks>from [view-mfr]</remarks>
        Protected iRow As DataRow

        ''' <summary>
        ''' The default name of this Manufacturer
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>READ ONLY. Should change this to read/write in the future to 
        ''' support allowing the user to change it from the Web GUI.</remarks>
        Public ReadOnly Property DefaultName As String
            Get
                Return iRow("mfrName")
            End Get
        End Property

        ''' <summary>
        ''' All of the names assigned for this Manufacturer
        ''' </summary>
        ''' <returns>a collection of strings (names) keyed by their mfrNameID's</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AllNames() As Collection
            Get
                Static dt As New DataTable
                Static TheNames As New Collection
                If dt.Rows.Count = 0 Then
                    dt = dbAcc.SelectRows(dt, _
                        "SELECT [mfrName]" & _
                        "  FROM [FriedParts].[dbo].[mfr-Names]" & _
                        " WHERE [mfrID] = " & iMfrID)
                    If dt.Rows.Count = 0 Then
                        Throw New ManufacturerNotFoundException("[mfr-Names] is corrupt. Could located any entries for mfrID: " & iMfrID)
                    Else
                        For Each dr As DataRow In dt.Rows
                            TheNames.Add(dr.Field(Of String)("mfrName"), dr.Field(Of Int32)("mfrNameID"))
                        Next
                    End If
                End If
                Return TheNames
            End Get
        End Property

        ''' <summary>
        ''' FriedParts UID for this Manufacturer (MfrID)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property MfrID As Int32
            Get
                Return iMfrID
            End Get
        End Property

        ''' <summary>
        ''' The Octopart UID for this manufacturer
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DefaultOctopartID As Int64
            Get
                Return iRow("mfrOctopartID")
            End Get
        End Property

        ''' <summary>
        ''' Adds a new name to this manufacturer.
        ''' </summary>
        ''' <returns>The mfrNameID assigned to the new name</returns>
        ''' <remarks>Manufacturers may have several "names" assigned due 
        ''' to legal variations and varying abbreviations across data
        ''' providers.</remarks>
        Public Function AddName(ByRef TheName As String, Optional ByRef OctopartID As Int64 = OctopartErrors.ID_UNKNOWN) As Int32
            'Defang!
            Dim safeName As String = sysText.txtDefangSQL(Trim(TheName))
            Dim safeOctoID As String
            If OctopartID = OctopartErrors.ID_UNKNOWN Then
                safeOctoID = "NULL"
            Else
                safeOctoID = OctopartID
            End If
            'Confirm uniqueness!
            Dim sqltxt As String = _
                "SELECT [mfrNameID]" & _
                "  FROM [FriedParts].[dbo].[mfr-Names]" & _
                "  WHERE [mfrID] = " & iMfrID & _
                "      AND [mfrName] = '" & safeName & "';"
            Dim dt As New DataTable
            dbAcc.SelectRows(dt, sqltxt)
            If dt.Rows.Count > 0 Then
                'Exists!
                Throw New Exception("Name is not unique! This name is already assigned to this manufacturer!")
            End If
            'Create New Manufacturer Record (NAMES ALIAS TABLE)
            dbAcc.SQLexe( _
                "INSERT INTO [FriedParts].[dbo].[mfr-Names]" & _
                "           ([mfrID]" & _
                "           ,[mfrName]" & _
                "           ,[mfrOctopartID])" & _
                "     VALUES (" & _
                "           " & iMfrID & "," & _
                "           '" & safeName & "'" & _
                "           " & safeOctoID & "" & _
                "           )")
            'Retrieve the mfrNameID of the name we just added
            dt.Clear() 'Reset
            dbAcc.SelectRows(dt, sqltxt)
            If dt.Rows.Count = 0 Then
                Throw New Exception("Failed to add Manfacturer name!")
            ElseIf dt.Rows.Count > 1 Then
                Throw New Exception("Multiple identical Manufacturer names found! Impossible!")
            End If
            Return dt.Rows(0).Field(Of Int32)("mfrNameID")
        End Function

        ''' <summary>
        ''' Adds a new name to this manufacturer, by assigning an existing one.
        ''' </summary>
        ''' <param name="TheMfrNameID">The mfrNameID of the name to assign 
        ''' to this manufacturer. Must be valid or an exception is thrown.</param>
        ''' <remarks>If this mfrNameID is currently assigned to another manufacturer, all 
        ''' parts and names for that manufacturer will be assigned to this one and the
        ''' other manufacturer will be deleted.</remarks>
        Public Sub AddName(ByRef TheMfrNameID As Int32)
            'xxx VERY COMPLICATED -- ESPECIALLY IF YOU WANT TO SUPPORT UNDO!
        End Sub

        ''' <summary>
        ''' Gets/Sets the Manufacturer's Website address.
        ''' </summary>
        ''' <value>The new website URL as a string.</value>
        ''' <returns>The current website URL on file.</returns>
        ''' <remarks>Logs to the system service log on change.</remarks>
        Public Property Website As String
            Get
                Return iRow("mfrWebsite")
            End Get
            Set(ByVal value As String)
                Dim sT As String = _
                    "UPDATE [FriedParts].[dbo].[mfr-Common]" & _
                    "   SET " & _
                    "       [mfrWebsite] = '" & value & "'" & _
                    "   WHERE [mfrID] = '" & iMfrID & "'"
                SQLexe(sT)
                logMfr(Nothing, sysUser.suGetUserID, MfrID, True)
            End Set
        End Property

        Protected Sub iNew(ByRef mfrID As Int32)
            'mfrID
            iMfrID = mfrID

            '[iRow] Data cache
            'Core code -- needs error handling 'xxx
            Dim sqlTxt As String = _
                "SELECT     dbo.[mfr-Common].mfrID, dbo.[mfr-Names].mfrName, dbo.[mfr-Common].mfrWebsite, dbo.[mfr-Names].mfrOctopartID " & _
                "  FROM     dbo.[mfr-Common] LEFT OUTER JOIN dbo.[mfr-Names] " & _
                "    ON     dbo.[mfr-Common].mfrNameID = dbo.[mfr-Names].mfrNameID " & _
                " WHERE     dbo.[mfr-Common].mfrNameID = dbo.[mfr-Names].mfrNameID "
            Dim dt As New DataTable
            SelectRows(dt, sqlTxt)
            mfrTestUnique(dt) 'sanity check
            iRow = dt.Rows(0)
        End Sub

        ''' <summary>
        ''' Constructor. Routes Int16 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="mfrID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef mfrID As Int16)
            iNew(CInt(mfrID))
        End Sub

        ''' <summary>
        ''' Constructor. Routes UInt32 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="mfrID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef mfrID As UInt32)
            iNew(CInt(mfrID))
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws exception if error.
        ''' </summary>
        ''' <param name="mfrID">The FriedParts ID for this manufacturer</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef mfrID As Int32)
            iNew(mfrID)
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws an exception if error. 
        ''' Uses the Octopart manufacturerID instead of the FriedParts manufacturer ID.
        ''' </summary>
        ''' <param name="OctopartID">The Octopart issued ID for this mfr</param>
        ''' <remarks>Casting is very important in overload resolution!!!</remarks>
        Public Sub New(ByRef OctopartID As Int64)
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, _
                    "SELECT [FriedParts].[dbo].[view-mfr].[mfrID] " & _
                    "FROM [FriedParts].[dbo].[view-mfr] " & _
                    "WHERE [FriedParts].[dbo].[view-mfr].[mfrOctopartID] = " & OctopartID)
            mfrTestUnique(dt) 'Sanity Check
            iNew(dt.Rows(0).Field(Of Int32)("mfrID"))
        End Sub

        ''' <summary>
        ''' Deny unqualified construction. If you don't tell me which manufacturer you want, I can't help you! ;-)
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Throw New ManufacturerNotFoundException("Cannot instantiate this class without specifying the manufacturer to load.")
        End Sub

        ''' <summary>
        ''' Constructor. Loads a manufacturer based on its name.
        ''' </summary>
        ''' <param name="MfrName">The manufacturer's name to search for and load if found.</param>
        ''' <param name="Exact"></param>
        ''' <remarks>Throws an exception if not found or too many found</remarks>
        Public Sub New(ByVal MfrName As String, Optional ByVal Exact As Boolean = True)
            Dim dt As New DataTable
            Dim retval() As Int32 = {sysErrors.ERR_NOTFOUND}
            If Exact Then
                dt = dbAcc.SelectRows(dt, _
                    "SELECT DISTINCT [FriedParts].[dbo].[view-mfr].[mfrID] " & _
                    "FROM [FriedParts].[dbo].[view-mfr] " & _
                    "WHERE [FriedParts].[dbo].[view-mfr].[mfrName] = '" & Trim(MfrName) & "';")
            Else
                dt = dbAcc.SelectRows(dt, _
                    "SELECT DISTINCT [FriedParts].[dbo].[view-mfr].[mfrID] " & _
                    "FROM [FriedParts].[dbo].[view-mfr] " & _
                    "WHERE [FriedParts].[dbo].[view-mfr].[mfrName] LIKE '%" & Trim(MfrName) & "%';")
            End If
            Select Case dt.Rows.Count
                Case 0
                    Throw New ManufacturerNotFoundException("Specified Manufacturer name, " & MfrName & ", not found!")
                Case 1
                    iNew(dt.Rows(0).Field(Of Int32)("mfrID"))
                Case Else
                    Throw New ManufacturerNotUniqueException("Specified Manufacturer name, " & MfrName & ", not unique! Found " & dt.Rows.Count & "!")
            End Select
        End Sub
    End Class

    ''' <summary>
    ''' Exception. For reporting NOT FOUND errors.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ManufacturerNotFoundException
        Inherits Exception
        Public Sub New(ByRef Message As String)
            MyBase.New(Message)
        End Sub
    End Class

    ''' <summary>
    ''' Exception. For reporting NOT UNIQUE errors.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ManufacturerNotUniqueException
        Inherits Exception
        Public Sub New(ByRef Message As String)
            MyBase.New(Message)
        End Sub
    End Class

    '=========================================================================================
    '== MANUFACTURER STATIC
    '=========================================================================================

    Public Module fpMfrStatic

        Public Const fpMfr_ADD_YOUR_OWN As String = "(...or if it isn't already here, enter the manufacturer in the box below!)"

        ''' <summary>
        ''' Tests the datatable for uniqueness. If it has just a single row this sub exits 
        ''' normally. Otherwise it throws an appropriate manufacturer exception. You may
        ''' still want to handle these conditions directly in order to provide more relevant
        ''' debugging messages.
        ''' </summary>
        ''' <param name="dt">A datatable -- presumably resulting from searching for a 
        ''' manufacturer.</param>
        ''' <remarks>This is a *very* common set of test conditions so I wrote this to
        ''' keep the code easier to read.</remarks>
        Public Sub mfrTestUnique(ByRef dt As DataTable)
            Select Case dt.Rows.Count
                Case 0
                    Throw New ManufacturerNotFoundException("")
                Case 1
                    'Do nothing -- exit normally
                Case Else
                    Throw New ManufacturerNotUniqueException("")
            End Select
        End Sub


        ''' <summary>
        ''' Finds the manufacturer name and adds the OctopartID to it. Throws an exception if
        ''' not found. Search is case sensitive. Name must match exactly. Logs to system.
        ''' </summary>
        ''' <param name="OctopartID">The Manufacturer's UID as assigned by Octopart</param>
        ''' <param name="MfrName">The Manufacturer's name as assigned by Octopart</param>
        ''' <returns>The mfrNameID to which this OctopartID was assigned</returns>
        ''' <remarks>We do it this way to ensure that the name has been properly created 
        ''' (e.g. that the name is wanted for use in FriedParts) before we care enough to
        ''' assign its OctopartID -- assuming one exists.</remarks>
        Public Function mfrAddOctopartID(ByRef OctopartID As Int64, ByRef MfrName As String) As Int32
            Dim st As String = _
                "SELECT [mfrID]" & _
                "      ,[mfrName]" & _
                "      ,[mfrNameID]" & _
                "      ,[mfrOctopartID]" & _
                "  FROM [FriedParts].[dbo].[mfr-Names] " & _
                " WHERE [mfrName] = '" & MfrName & "'"
            Dim dt As New DataTable
            SelectRows(dt, st)
            mfrTestUnique(dt)
            If dt.Rows(0).Field(Of Int64)("mfrOctopartID") <> OctopartID Then
                SQLexe( _
                    "UPDATE [FriedParts].[dbo].[mfr-Names]" & _
                    "   SET " & _
                    "      [mfrOctopartID] = " & OctopartID & _
                    "   WHERE [mfrNameID] = " & dt.Rows(0).Field(Of Int32)("mfrNameID") _
                )
                logMfrName(Nothing, sysUser.suGetUserID, dt.Rows(0).Field(Of Int32)("mfrNameID"), True)
            Else
                'User asked us to update the field to its current value! (e.g. no change required)
            End If
            Return dt.Rows(0).Field(Of Int32)("mfrNameID")
        End Function




        ''' <summary>
        ''' Returns the name for the specified mfrNameID. This handles the specific names directly. 
        ''' Whereas mfrGetName works with the default name for the manufacturer
        ''' </summary>
        ''' <param name="mfrNameID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function mfrGetSpecificName(ByVal mfrNameID As Int16) As String
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, _
                "SELECT [mfrName]" & _
                "  FROM [FriedParts].[dbo].[mfr-Names]" & _
                " WHERE [mfrNameID] = " & mfrNameID)
            If dt.Rows.Count = 0 Then
                Throw New ManufacturerNotFoundException("Could not find this mfrNameID")
            Else
                Return dt.Rows(0).Field(Of String)("mfrName")
            End If
        End Function

        ''' <summary>
        ''' 'Gets the default name for the FriedParts ID-specified manufacturer.
        ''' </summary>
        ''' <param name="mfrID"></param>
        ''' <returns></returns>
        ''' <remarks>This method is not efficient for multiple-user or when accessing multiple 
        ''' properties of the same manufacturer because each call instantiates the class (which
        ''' involves database lookups) and destroys it. If you need to do a few things with this
        ''' manufacturer just create the fpMfr object yourself and use it directly.</remarks>
        Public Function mfrGetName(ByVal mfrID As Int32) As String
            Return New fpMfr(mfrID).DefaultName
        End Function

        'If exists returns an array of matching MfrID's, Else returns -1
        'The returned array consists only of unique manufacturer ID's 
        '   (in case it is found under multiple names -- likely -- as in "Panasonic ECG", "Panasonic, Inc.", "Panasonic Corporation").
        ''' <summary>
        ''' Checks for the existence of a manufacturer name. Can return the fpMfr object if desired.
        ''' </summary>
        ''' <param name="MfrName">The name to search for.</param>
        ''' <param name="Exact">Perform an exact search or a fuzzy one.</param>
        ''' <param name="AllowMultiple">If multiple manufacturer records match and AllowMultiple is set to True,
        ''' The function will return true (Result will still be Nothing however).</param>
        ''' <param name="Result">[Output value] If only one MfrID was found, we return the object. Otherwise this is set to Nothing. [Usage] dim m as New fpMfr.</param>
        ''' <returns>The boolean value reporting the existence of this manufacturer name. Follows the rules specified by the other parameters. To retrieve the actual object see the Result parameter.</returns>
        ''' <remarks></remarks>
        Public Function mfrExists(ByRef MfrName As String, Optional ByRef Exact As Boolean = True, Optional ByRef AllowMultiple As Boolean = False, Optional ByRef Result As fpMfr = Nothing) As Boolean
            Try
                Result = New fpMfr(MfrName, Exact)
            Catch ex As Exception
                Result = Nothing
                If TypeOf ex Is ManufacturerNotFoundException Then
                    Return False
                ElseIf TypeOf ex Is ManufacturerNotUniqueException Then
                    If AllowMultiple Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Throw New Exception("Couldn't create Manufacturer Class during ExistsName testing", ex)
                End If
            End Try
            Return True
        End Function

        '===============================================================================
        '== ADD NEW!
        '===============================================================================

        
        ''' <summary>
        ''' Adds a new Manufacturer to the Database. Throws a NOT UNIQUE exception if already exists.
        ''' </summary>
        ''' <param name="MfrName"></param>
        ''' <param name="MfrWebsite"></param>
        ''' <param name="MfrOctopartID"></param>
        ''' <param name="Me_Page"></param>
        ''' <returns>The MfrID assigned to this new manufacturer</returns>
        ''' <remarks></remarks>
        Public Function mfrAdd(ByVal MfrName As String, Optional ByVal MfrWebsite As String = "", Optional ByRef MfrOctopartID As Int64 = OctopartErrors.ID_UNKNOWN, Optional ByRef Me_Page As Page = Nothing) As Int32
            Dim dt As New DataTable
            Dim MfrObject As fpMfr
            If Not mfrExists(MfrName, True, True, MfrObject) Then
                'NOT FOUND! ADD THIS MANUFACTURER!
                'This is actually a bit of a tricky process
                '
                '[STEP 1] CREATE A NEW MANUFACTURER ID
                '
                dbLog.logDebug("[Add Mfr] '" & MfrName & "' Not Found! Adding!")
                Dim newMfrID As Int32
                'Generate Temporary ID
                Dim tempUID As Int32 = procGenTempUID()
                'Create New Manufacturer Record (COMMON)
                Dim strcmd As String = _
                    "INSERT INTO [FriedParts].[dbo].[mfr-Common]" & _
                    "           ([mfrNameID]" & _
                    "           ,[mfrWebsite])" & _
                    "     VALUES (" & _
                    "           " & tempUID & "," & _
                    "           '" & MfrWebsite & "'" & _
                    "           )"
                'We set the mfrNameID to a unique negative number to mark it for error detection. In theory, there should never be any -1 records in the common table since every mfr-common entry corresponds to at least one mfr-name entry.
                dbAcc.SQLexe(strcmd)
                '
                '[STEP 2] FIND THE NEW MFR-ID WE JUST CREATED
                '
                dt = dbAcc.SelectRows(dt, _
                    "SELECT [FriedParts].[dbo].[mfr-Common].[mfrID] " & _
                    "FROM [FriedParts].[dbo].[mfr-Common] " & _
                    "WHERE [FriedParts].[dbo].[mfr-Common].[mfrNameID] = " & tempUID & ";")
                Select Case dt.Rows.Count
                    Case 0
                        'Not found! ERROR!!! (should never happen)
                        dbLog.logSys(0, "[Add Mfr] '" & MfrName & "' Not Found! During Add Using Temp ID: " & tempUID)
                        Throw New ManufacturerNotFoundException("[Add Mfr] '" & MfrName & "' Not Found! During Add Using Temp ID: " & tempUID)
                    Case 1
                        'Yay! Perfect (this is exactly what should happen)
                        newMfrID = dt.Rows(0).Field(Of Int32)(0) 'query only selected one row
                    Case Else
                        'Found more than one!!! Oh No!!! (shouldn't happen)
                        dbLog.logSys(0, "[Add Mfr] '" & MfrName & "' Matched multiple using Temp ID: " & tempUID)
                        Throw New ManufacturerNotUniqueException("[Add Mfr] '" & MfrName & "' Matched multiple using Temp ID: " & tempUID)
                End Select
                '
                '[STEP 3] ADD THE NEW MFR-NAME RECORD
                '
                Dim newMfrNameID As Int32 = New fpMfr(newMfrID).AddName(MfrName, MfrOctopartID)
                '
                '[STEP 4] GO BACK AND ADD THE CORRECT RELATIONSHIP TO THE NEW NAME
                '
                strcmd = _
                    "UPDATE [FriedParts].[dbo].[mfr-Common]" & _
                    "   SET [mfrNameID] = " & newMfrNameID & _
                    " WHERE [mfrID] = " & newMfrID & ";"
                dbAcc.SQLexe(strcmd)
                '
                '[STEP 5] LOG AND REPORT
                '
                If Not Me_Page Is Nothing Then
                    'User Request
                    logMfr(Me_Page, HttpContext.Current.Session("user.UserID"), newMfrID)
                Else
                    'System Request
                    logMfr(Me_Page, sysEnv.SysUserID, newMfrID)
                End If
                'WE'RE DONE! EXIT FUNCTION!!!
                Return newMfrID
            Else
                'Already exists!
                Throw New ManufacturerNotUniqueException("Cannot add Manufacturer! Already exists as: " & MfrObject.DefaultName & " (ID: " & MfrObject.MfrID & ")")
            End If
        End Function



        '=========================================================================================
        '== [END] MANUFACTURER
        '=========================================================================================

        ''' <summary>
        ''' Logs a service activity into the FriedParts system log.
        ''' </summary>
        ''' <param name="Me_Page"></param>
        ''' <param name="UserID"></param>
        ''' <param name="MfrID"></param>
        ''' <param name="IsAnUpdate">If true, marks this as an update; False, it's adding an entirely new manufacturer</param>
        ''' <remarks></remarks>
        Public Sub logMfr(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal MfrID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            If IsAnUpdate Then
                'Don't have UNDO implemented right now... needs a lot of work to do that... 'xxx
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " updated manufacturer " & _
                                   mfrGetName(MfrID), "", "", "MfrID", MfrID, "MfrName", mfrGetName(MfrID) _
                                   )
            Else
                Dim sqlUndo As String = _
                    "DELETE FROM [FriedParts].[dbo].[mfr-Names]" & _
                    " WHERE [MfrID] = " & MfrID & vbCrLf & _
                    "GO" & vbCrLf & _
                    "DELETE FROM [FriedParts].[dbo].[mfr-Common]" & _
                    " WHERE [MfrID] = " & MfrID & vbCrLf & _
                    "GO" & vbCrLf
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " added a new manufacturer, " & _
                                   mfrGetName(MfrID), "", sqlUndo, "MfrID", MfrID, "MfrName", mfrGetName(MfrID), _
                                   )
            End If
        End Sub

        Public Sub logMfrName(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal MfrNameID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            If IsAnUpdate Then
                'Don't have UNDO implemented right now... needs a lot of work to do that... 'xxx
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & _
                    " updated manufacturer name information for " & mfrGetSpecificName(MfrNameID), _
                    "", "", "MfrNameID", MfrNameID, "MfrName", mfrGetSpecificName(MfrNameID))
            Else
                Dim sqlUndo As String = _
                    "DELETE FROM [FriedParts].[dbo].[mfr-Names]" & _
                    " WHERE [mfrNameID] = " & MfrNameID
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & _
                    " added the manufacturer name " & mfrGetSpecificName(MfrNameID), _
                    "", sqlUndo, "MfrNameID", MfrNameID, "MfrName", mfrGetSpecificName(MfrNameID))
            End If
        End Sub
    End Module
End Namespace
