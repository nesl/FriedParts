Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Data
Imports apiOctopart

Namespace fpInstitution

    ''' <summary>
    ''' Holds all of the Institution Type-specific information. Namely a Table-Prefix ("mfr") and 
    ''' a human-readable Type description ("manufacturer")
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InstDetails
        Protected iPrefix As String
        Protected iName As String
        Public ReadOnly Property Prefix As String
            Get
                Return iPrefix
            End Get
        End Property
        Public ReadOnly Property Name As String
            Get
                Return iname
            End Get
        End Property
        Public Sub New(ByRef Prefix As String, ByRef InstitutionType As String)
            iPrefix = Prefix
            iName = InstitutionType
        End Sub
    End Class


    ''' <summary>
    ''' Accesses/Modifies Manufacturer values. Data pertains the Manufacturer
    ''' itself -- not it's parts/products.
    ''' </summary>
    ''' <remarks>To make this class as fast as possible, we cache only the 
    ''' likely data -- meaning the default name and common data. Alternate name
    ''' data is retrieved as needed.</remarks>
    Public Class fpInst

        ''' <summary>
        ''' Holds all of the Institution Type-specific information. Namely a Table-Prefix ("mfr") and 
        ''' a human-readable Type description ("manufacturer")
        ''' </summary>
        ''' <remarks>Defaults to nothing to cause NullReferenceException if you do something
        ''' stupid -- like instantiate this class directly</remarks>
        Protected Shared i As InstDetails = Nothing

        Public Class InstNotFoundException
            Inherits Exception
            Public Sub New(Optional ByRef Message As String = "")
                MyBase.New(Message)
            End Sub
        End Class

        Public Class InstNotUniqueException
            Inherits Exception
            Public Sub New(Optional ByRef Message As String = "")
                MyBase.New(Message)
            End Sub
        End Class



        ''' <summary>
        ''' Internal variable. FriedParts Institutional UID.
        ''' </summary>
        ''' <remarks></remarks>
        Protected iUID As Int32

        ''' <summary>
        ''' Cache of the default data.
        ''' </summary>
        ''' <remarks>from [view-mfr]</remarks>
        Protected iRow As DataRow

        ''' <summary>
        ''' The Institutions unique FriedParts ID. This maps to a mfrID, distID, etc...
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property fpID As Int32
            Get
                Return iUID
            End Get
        End Property

        ''' <summary>
        ''' The default name of this Manufacturer
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>READ ONLY. Should change this to read/write in the future to 
        ''' support allowing the user to change it from the Web GUI.</remarks>
        Public ReadOnly Property DefaultName As String
            Get
                Return iRow(i.Prefix & "Name")
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
                        "SELECT [" & i.Prefix & "Name]" & _
                        "  FROM [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                        " WHERE [" & i.Prefix & "ID] = " & iUID)
                    If dt.Rows.Count = 0 Then
                        Throw New InstNotFoundException("Could not find any names for this " & i.Name & _
                                                        " (" & iUID & ") in the [" & i.Prefix & "-Names] table! Data integrity " & _
                                                        "has been compromised.")
                    Else
                        For Each dr As DataRow In dt.Rows
                            TheNames.Add(dr.Field(Of String)(i.Prefix & "Name"), dr.Field(Of Int32)(i.Prefix & "NameID"))
                        Next
                    End If
                End If
                Return TheNames
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
                Return iRow(i.Prefix & "OctopartID")
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
                "SELECT [" & i.Prefix & "NameID]" & _
                "  FROM [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                "  WHERE [" & i.Prefix & "ID] = " & iUID & _
                "      AND [" & i.Prefix & "Name] = '" & safeName & "';"
            Dim dt As New DataTable
            dbAcc.SelectRows(dt, sqltxt)
            If dt.Rows.Count > 0 Then
                'Exists!
                Throw New InstNotUniqueException("Name is not unique! This name is already assigned to this " & i.Name & "!")
            End If
            'Create New Institution Record (NAMES ALIAS TABLE)
            dbAcc.SQLexe( _
                "INSERT INTO [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                "           ([" & i.Prefix & "ID]" & _
                "           ,[" & i.Prefix & "Name]" & _
                "           ,[" & i.Prefix & "OctopartID])" & _
                "     VALUES (" & _
                "           " & iUID & "," & _
                "           '" & safeName & "'," & _
                "           " & safeOctoID & "" & _
                "           )")
            'Retrieve the mfrNameID of the name we just added
            dt.Clear() 'Reset
            dbAcc.SelectRows(dt, sqltxt)
            If dt.Rows.Count = 0 Then
                Throw New InstNotFoundException("Failed to add " & i.Name & " name!")
            ElseIf dt.Rows.Count > 1 Then
                Throw New InstNotUniqueException("Multiple identical " & i.Name & " names found! Impossible!")
            End If
            logInstName(i, Nothing, sysUser.suGetUserID, dt.Rows(0).Field(Of Int32)(i.Prefix & "NameID"))
            Return dt.Rows(0).Field(Of Int32)(i.Prefix & "NameID")
        End Function

        ''' <summary>
        ''' Adds a new name to this manufacturer, by assigning an existing one.
        ''' </summary>
        ''' <param name="TheNameID">The mfrNameID of the name to assign 
        ''' to this manufacturer. Must be valid or an exception is thrown.</param>
        ''' <remarks>If this mfrNameID is currently assigned to another manufacturer, all 
        ''' parts and names for that manufacturer will be assigned to this one and the
        ''' other manufacturer will be deleted.</remarks>
        Public Sub AddName(ByRef TheNameID As Int32)
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
                Return iRow(i.Prefix & "Website")
            End Get
            Set(ByVal value As String)
                Dim sT As String = _
                    "UPDATE [FriedParts].[dbo].[" & i.Prefix & "-Common]" & _
                    "   SET " & _
                    "       [" & i.Prefix & "Website] = '" & value & "'" & _
                    "   WHERE [" & i.Prefix & "ID] = '" & iUID & "'"
                SQLexe(sT)
                logInst(i, Nothing, sysUser.suGetUserID, iUID, True)
            End Set
        End Property

        '=========================================================================================
        '== CONSTRUCTION
        '=========================================================================================
#Region "Constructors"
        ''' <summary>
        ''' Constructor worker function.
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <remarks></remarks>
        Protected Sub iNew(ByRef UID As Int32)
            'UID
            iUID = UID

            '[iRow] Data cache
            'Core code -- needs error handling 'xxx
            Dim sqlTxt As String = _
                "SELECT     dbo.[view-" & i.Prefix & "]." & i.Prefix & "ID, dbo.[view-" & i.Prefix & "]." & i.Prefix & "Name, dbo.[view-" & i.Prefix & "]." & i.Prefix & "Website, dbo.[view-" & i.Prefix & "]." & i.Prefix & "OctopartID " & _
                "  FROM     dbo.[view-" & i.Prefix & "]" & _
                " WHERE     dbo.[view-" & i.Prefix & "]." & i.Prefix & "ID = " & UID
            Dim dt As New DataTable
            SelectRows(dt, sqlTxt)
            TestUnique(dt) 'sanity check
            iRow = dt.Rows(0)
        End Sub

        ''' <summary>
        ''' Constructor. Routes Int16 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef iType As InstDetails, ByRef UID As Int16)
            i = iType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
            iNew(CInt(UID))
        End Sub

        ''' <summary>
        ''' Constructor. Routes UInt32 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef iType As InstDetails, ByRef UID As UInt32)
            i = iType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
            iNew(CInt(UID))
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws exception if error.
        ''' </summary>
        ''' <param name="UID">The FriedParts ID for this manufacturer</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef iType As InstDetails, ByRef UID As Int32)
            i = iType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
            iNew(UID)
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws an exception if error. 
        ''' Uses the Octopart manufacturerID instead of the FriedParts manufacturer ID.
        ''' </summary>
        ''' <param name="OctopartID">The Octopart issued ID for this mfr</param>
        ''' <remarks>Casting is very important in overload resolution!!!</remarks>
        Public Sub New(ByRef iType As InstDetails, ByRef OctopartID As Int64)
            i = iType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, _
                    "SELECT [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "ID] " & _
                    "FROM [FriedParts].[dbo].[view-" & i.Prefix & "] " & _
                    "WHERE [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "OctopartID] = " & OctopartID)
            TestUnique(dt) 'Sanity Check
            iNew(dt.Rows(0).Field(Of Int32)(i.Prefix & "ID"))
        End Sub

        ''' <summary>
        ''' Constructor. Loads an institution based on its name.
        ''' </summary>
        ''' <param name="Name">The institution's name to search for and load if found.</param>
        ''' <param name="Exact"></param>
        ''' <remarks>Throws an exception if not found or too many found</remarks>
        Public Sub New(ByRef iType As InstDetails, ByRef Name As String, Optional ByVal Exact As Boolean = True)
            i = iType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
            Dim dt As New DataTable
            If Exact Then
                dt = dbAcc.SelectRows(dt, _
                    "SELECT DISTINCT [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "ID] " & _
                    "FROM [FriedParts].[dbo].[view-" & i.Prefix & "] " & _
                    "WHERE [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "Name] = '" & Trim(Name) & "';")
            Else
                dt = dbAcc.SelectRows(dt, _
                    "SELECT DISTINCT [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "ID] " & _
                    "FROM [FriedParts].[dbo].[view-" & i.Prefix & "] " & _
                    "WHERE [FriedParts].[dbo].[view-" & i.Prefix & "].[" & i.Prefix & "Name] LIKE '%" & Trim(Name) & "%';")
            End If
            Select Case dt.Rows.Count
                Case 0
                    Throw New InstNotFoundException("Specified Manufacturer name, " & Name & ", not found!")
                Case 1
                    iNew(dt.Rows(0).Field(Of Int32)("mfrID"))
                Case Else
                    Throw New InstNotUniqueException("Specified Manufacturer name, " & Name & ", not unique! Found " & dt.Rows.Count & "!")
            End Select
        End Sub

        ''' <summary>
        ''' Deny unqualified construction. If you don't tell me which manufacturer you want, I can't help you! ;-)
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Throw New InstNotFoundException("Cannot instantiate this class without specifying the " & i.Name & " to load.")
        End Sub
#End Region


        '=========================================================================================
        '== SHARED METHODS
        '=========================================================================================
#Region "Shared Methods"

        ''' <summary>
        ''' This function handles installing the i variable for shared functions (for object functions
        ''' the constructors handle this).
        ''' </summary>
        ''' <param name="InstType"></param>
        ''' <remarks></remarks>
        Private Shared Sub initShared(ByRef InstType As InstDetails)
            i = InstType
            If i Is Nothing Then Throw New NullReferenceException("fpInst may not be instantiated directly. Use one of its children.")
        End Sub

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
        Protected Shared Sub TestUnique(ByRef dt As DataTable)
            Select Case dt.Rows.Count
                Case 0
                    Throw New InstNotFoundException("")
                Case 1
                    'Do nothing -- exit normally
                Case Else
                    Throw New InstNotUniqueException("")
            End Select
        End Sub


        ''' <summary>
        ''' Finds the manufacturer name and adds the OctopartID to it. Throws an exception if
        ''' not found. Search is case sensitive. Name must match exactly. Logs to system.
        ''' </summary>
        ''' <param name="OctopartID">The Manufacturer's UID as assigned by Octopart</param>
        ''' <param name="Name">The Manufacturer's name as assigned by Octopart</param>
        ''' <returns>The mfrNameID to which this OctopartID was assigned</returns>
        ''' <remarks>We do it this way to ensure that the name has been properly created 
        ''' (e.g. that the name is wanted for use in FriedParts) before we care enough to
        ''' assign its OctopartID -- assuming one exists.</remarks>
        Protected Shared Function AddOctopartID(ByRef iType As InstDetails, ByRef OctopartID As Int64, ByRef Name As String) As Int32
            initShared(iType)

            Dim st As String = _
                "SELECT [" & i.Prefix & "ID]" & _
                "      ,[" & i.Prefix & "Name]" & _
                "      ,[" & i.Prefix & "NameID]" & _
                "      ,[" & i.Prefix & "OctopartID]" & _
                "  FROM [FriedParts].[dbo].[" & i.Prefix & "-Names] " & _
                " WHERE [" & i.Prefix & "Name] = '" & Name & "'"
            Dim dt As New DataTable
            SelectRows(dt, st)
            TestUnique(dt) 'sanity check
            If dt.Rows(0).Field(Of Int64)(i.Prefix & "OctopartID") <> OctopartID Then
                SQLexe( _
                    "UPDATE [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                    "   SET " & _
                    "      [" & i.Prefix & "OctopartID] = " & OctopartID & _
                    "   WHERE [" & i.Prefix & "NameID] = " & dt.Rows(0).Field(Of Int32)(i.Prefix & "NameID") _
                )
                logInstName(i, Nothing, sysUser.suGetUserID, dt.Rows(0).Field(Of Int32)(i.Prefix & "NameID"), True)
            Else
                'User asked us to update the field to its current value! (e.g. no change required)
            End If
            Return dt.Rows(0).Field(Of Int32)(i.Prefix & "NameID")
        End Function




        ''' <summary>
        ''' Returns the name for the specified mfrNameID. This handles the specific names directly. 
        ''' Whereas mfrGetName works with the default name for the manufacturer
        ''' </summary>
        ''' <param name="NameID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function GetSpecificName(ByRef iType As InstDetails, ByVal NameID As Int16) As String
            initShared(iType)
            Dim dt As New DataTable
            dt = dbAcc.SelectRows(dt, _
                "SELECT [" & i.Prefix & "Name]" & _
                "  FROM [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                " WHERE [" & i.Prefix & "NameID] = " & NameID)
            If dt.Rows.Count = 0 Then
                Throw New InstNotFoundException("Could not find this " & i.Prefix & "NameID")
            Else
                Return dt.Rows(0).Field(Of String)(i.Prefix & "Name")
            End If
        End Function

        ''' <summary>
        ''' 'Gets the default name for the FriedParts ID-specified manufacturer.
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <returns></returns>
        ''' <remarks>This method is not efficient for multiple-user or when accessing multiple 
        ''' properties of the same manufacturer because each call instantiates the class (which
        ''' involves database lookups) and destroys it. If you need to do a few things with this
        ''' manufacturer just create the fpMfr object yourself and use it directly.</remarks>
        Protected Shared Function GetName(ByRef iType As InstDetails, ByVal UID As Int32) As String
            Return New fpInst(iType, UID).DefaultName
        End Function

        'If exists returns an array of matching MfrID's, Else returns -1
        'The returned array consists only of unique manufacturer ID's 
        '   (in case it is found under multiple names -- likely -- as in "Panasonic ECG", "Panasonic, Inc.", "Panasonic Corporation").
        ''' <summary>
        ''' Checks for the existence of a manufacturer name. Can return the fpMfr object if desired.
        ''' </summary>
        ''' <param name="Name">The name to search for.</param>
        ''' <param name="Exact">Perform an exact search or a fuzzy one.</param>
        ''' <param name="AllowMultiple">If multiple manufacturer records match and AllowMultiple is set to True,
        ''' The function will return true (Result will still be Nothing however).</param>
        ''' <param name="Result">[Output value] If only one MfrID was found, we return the object. Otherwise this is set to Nothing. [Usage] dim m as New fpMfr.</param>
        ''' <returns>The boolean value reporting the existence of this manufacturer name. Follows the rules specified by the other parameters. To retrieve the actual object see the Result parameter.</returns>
        ''' <remarks></remarks>
        Protected Shared Function Exists(ByRef iType As InstDetails, ByRef Name As String, Optional ByRef Exact As Boolean = True, Optional ByRef AllowMultiple As Boolean = False, Optional ByRef Result As fpInst = Nothing) As Boolean
            initShared(iType)
            Try
                Result = New fpInst(i, Name, Exact)
            Catch ex As Exception
                Result = Nothing
                If TypeOf ex Is InstNotFoundException Then
                    Return False
                ElseIf TypeOf ex Is InstNotUniqueException Then
                    If AllowMultiple Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Throw New Exception("Couldn't create " & i.Name & " Class during Exists testing", ex)
                End If
            End Try
            Return True
        End Function

        ''' <summary>
        ''' Adds a new Manufacturer to the Database. Throws a NOT UNIQUE exception if already exists.
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="Website"></param>
        ''' <param name="OctopartID"></param>
        ''' <param name="Me_Page"></param>
        ''' <returns>The MfrID assigned to this new manufacturer</returns>
        ''' <remarks></remarks>
        Protected Shared Function Add(ByRef iType As InstDetails, ByVal Name As String, Optional ByVal Website As String = "", Optional ByRef OctopartID As Int64 = OctopartErrors.ID_UNKNOWN, Optional ByRef Me_Page As Page = Nothing) As Int32
            initShared(iType)
            Dim dt As New DataTable
            Dim InstObject As fpInst
            If Not Exists(i, Name, True, True, InstObject) Then
                'NOT FOUND! ADD THIS MANUFACTURER!
                'This is actually a bit of a tricky process
                '
                '[STEP 1] CREATE A NEW MANUFACTURER ID
                '
                dbLog.logDebug("[Add " & i.Prefix & "] '" & Name & "' Not Found! Adding!")
                Dim newInstID As Int32
                'Generate Temporary ID
                Dim tempUID As Int32 = procGenTempUID()
                'Create New Manufacturer Record (COMMON)
                Dim strcmd As String = _
                    "INSERT INTO [FriedParts].[dbo].[" & i.Prefix & "-Common]" & _
                    "           ([" & i.Prefix & "NameID]" & _
                    "           ,[" & i.Prefix & "Website])" & _
                    "     VALUES (" & _
                    "           " & tempUID & "," & _
                    "           '" & Website & "'" & _
                    "           )"
                'We set the mfrNameID to a unique negative number to mark it for error detection. In theory, there should never be any -1 records in the common table since every mfr-common entry corresponds to at least one mfr-name entry.
                dbAcc.SQLexe(strcmd)
                '
                '[STEP 2] FIND THE NEW INST-ID WE JUST CREATED
                '
                dt = dbAcc.SelectRows(dt, _
                    "SELECT [FriedParts].[dbo].[" & i.Prefix & "-Common].[" & i.Prefix & "ID] " & _
                    "FROM [FriedParts].[dbo].[" & i.Prefix & "-Common] " & _
                    "WHERE [FriedParts].[dbo].[" & i.Prefix & "-Common].[" & i.Prefix & "NameID] = " & tempUID & ";")
                Select Case dt.Rows.Count
                    Case 0
                        'Not found! ERROR!!! (should never happen)
                        dbLog.logSys(0, "[Add " & i.Prefix & "] '" & Name & "' Not Found! During Add Using Temp ID: " & tempUID)
                        Throw New InstNotFoundException("[Add " & i.Prefix & "] '" & Name & "' Not Found! During Add Using Temp ID: " & tempUID)
                    Case 1
                        'Yay! Perfect (this is exactly what should happen)
                        newInstID = dt.Rows(0).Field(Of Int32)(0) 'query only selected one row
                    Case Else
                        'Found more than one!!! Oh No!!! (shouldn't happen)
                        dbLog.logSys(0, "[Add " & i.Prefix & "] '" & Name & "' Matched multiple using Temp ID: " & tempUID)
                        Throw New InstNotUniqueException("[Add " & i.Prefix & "] '" & Name & "' Matched multiple using Temp ID: " & tempUID)
                End Select
                '
                '[STEP 3] ADD THE NEW INST-NAME RECORD
                '
                Dim newInstNameID As Int32 = New fpInst(i, newInstID).AddName(Name, OctopartID)
                '
                '[STEP 4] GO BACK AND ADD THE CORRECT RELATIONSHIP TO THE NEW NAME
                '
                strcmd = _
                    "UPDATE [FriedParts].[dbo].[mfr-Common]" & _
                    "   SET [mfrNameID] = " & newInstNameID & _
                    " WHERE [mfrID] = " & newInstID & ";"
                dbAcc.SQLexe(strcmd)
                '
                '[STEP 5] LOG AND REPORT
                '
                logInst(i, Me_Page, sysUser.suGetUserID, newInstID)

                'WE'RE DONE! EXIT FUNCTION!!!
                Return newInstID
            Else
                'Already exists!
                Throw New InstNotUniqueException("Cannot add Manufacturer! Already exists as: " & InstObject.DefaultName & " (ID: " & InstObject.fpID & ")")
            End If
        End Function
#End Region

        '=========================================================================================
        '== LOGGING METHODS
        '=========================================================================================
#Region "Logging Functions"
        ''' <summary>
        ''' Logs a service activity into the FriedParts system log.
        ''' </summary>
        ''' <param name="Me_Page"></param>
        ''' <param name="UserID"></param>
        ''' <param name="UID"></param>
        ''' <param name="IsAnUpdate">If true, marks this as an update; False, it's adding an entirely new manufacturer</param>
        ''' <remarks></remarks>
        Protected Shared Sub logInst(ByRef iType As InstDetails, ByRef Me_Page As Page, ByVal UserID As Int16, ByVal UID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            initShared(iType)
            If IsAnUpdate Then
                'Don't have UNDO implemented right now... needs a lot of work to do that... 'xxx
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " updated " & i.Name & " " & _
                                   GetName(i, UID), "", "", "" & i.Prefix & "ID", UID, "" & i.Prefix & "Name", GetName(i, UID) _
                                   )
            Else
                Dim sqlUndo As String = _
                    "DELETE FROM [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                    " WHERE [" & i.Prefix & "ID] = " & UID & vbCrLf & _
                    "GO" & vbCrLf & _
                    "DELETE FROM [FriedParts].[dbo].[" & i.Prefix & "-Common]" & _
                    " WHERE [" & i.Prefix & "ID] = " & UID & vbCrLf & _
                    "GO" & vbCrLf
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & " added a new " & i.Name & ", " & _
                                   GetName(i, UID), "", sqlUndo, i.Prefix & "ID", UID, i.Prefix & "Name", GetName(i, UID), _
                                   )
            End If
        End Sub

        Protected Shared Sub logInstName(ByRef iType As InstDetails, ByRef Me_Page As Page, ByVal UserID As Int16, ByVal NameID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            initShared(iType)
            If IsAnUpdate Then
                'Don't have UNDO implemented right now... needs a lot of work to do that... 'xxx
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & _
                    " updated " & i.Name & " name information for " & GetSpecificName(i, NameID), _
                    "", "", i.Prefix & "NameID", NameID, i.Prefix & "Name", GetSpecificName(i, NameID))
            Else
                Dim sqlUndo As String = _
                    "DELETE FROM [FriedParts].[dbo].[" & i.Prefix & "-Names]" & _
                    " WHERE [" & i.Prefix & "NameID] = " & NameID
                logServiceActivity(Me_Page, UserID, suGetUserFirstName(UserID) & _
                    " added the " & i.Name & " name " & GetSpecificName(i, NameID), _
                    "", sqlUndo, i.Prefix & "NameID", NameID, i.Prefix & "Name", GetSpecificName(i, NameID))
            End If
        End Sub
#End Region

    End Class

End Namespace