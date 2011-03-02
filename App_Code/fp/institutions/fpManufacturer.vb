Option Explicit On
Imports Microsoft.VisualBasic
Imports System.Net
Imports System.Data
Imports fpInstitution

Namespace fpManufacturer
    Public Class fpMfr
        Inherits fpInstitution.fpInst
        Public Const Prefix As String = "mfr"
        Public Const TypeName As String = "manufacturer"




        '----------------------------------------------------------------
        '-- fpInstitution Plumbing (No need to touch it in this class)
        '----------------------------------------------------------------
#Region "Conversion Operators"
#End Region

#Region "Shared Methods"
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
        Public Overloads Shared Function AddOctopartID(ByRef OctopartID As Int64, ByRef Name As String) As Int32
            Return AddOctopartID(New InstDetails(Prefix, TypeName), OctopartID, Name)
        End Function

        ''' <summary>
        ''' Returns the name for the specified mfrNameID. This handles the specific names directly. 
        ''' Whereas mfrGetName works with the default name for the manufacturer
        ''' </summary>
        ''' <param name="NameID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function GetSpecificName(ByVal NameID As Int16) As String
            Return GetSpecificName(New InstDetails(Prefix, TypeName), NameID)
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
        Public Overloads Shared Function GetName(ByVal UID As Int32) As String
            Return GetName(New InstDetails(Prefix, TypeName), UID)
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
        ''' <returns>The boolean value reporting the existence of this manufacturer name. Follows the rules specified by the other parameters. To retrieve the actual object see the Result parameter.</returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function Exists(ByRef Name As String, Optional ByRef Exact As Boolean = True, Optional ByRef AllowMultiple As Boolean = False) As Boolean
            Return Exists(New InstDetails(Prefix, TypeName), Name, Exact, AllowMultiple)
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
        Public Overloads Shared Function Add(ByVal Name As String, Optional ByVal Website As String = "", Optional ByRef OctopartID As Int64 = apiOctopart.OctopartErrors.ID_UNKNOWN, Optional ByRef Me_Page As Page = Nothing) As Int32
            Return Add(New InstDetails(Prefix, TypeName), Name, Website, OctopartID, Me_Page)
        End Function
#End Region

#Region "Logging Methods (Shared)"
        ''' <summary>
        ''' Logs a service activity into the FriedParts system log.
        ''' </summary>
        ''' <param name="Me_Page"></param>
        ''' <param name="UserID"></param>
        ''' <param name="UID"></param>
        ''' <param name="IsAnUpdate">If true, marks this as an update; False, it's adding an entirely new manufacturer</param>
        ''' <remarks></remarks>
        Protected Overloads Shared Sub logInst(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal UID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            logInst(New InstDetails(Prefix, TypeName), Me_Page, UserID, UID, IsAnUpdate)
        End Sub

        Protected Overloads Shared Sub logInstName(ByRef Me_Page As Page, ByVal UserID As Int16, ByVal NameID As Int16, Optional ByRef IsAnUpdate As Boolean = False)
            logInstName(New InstDetails(Prefix, TypeName), Me_Page, UserID, NameID, IsAnUpdate)
        End Sub

#End Region

#Region "Constructors"
        ''' <summary>
        ''' Constructor. Routes Int16 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef UID As Int16)
            MyBase.New(New InstDetails(Prefix, TypeName), UID)
        End Sub

        ''' <summary>
        ''' Constructor. Routes UInt32 datatype to FriedPartsID
        ''' </summary>
        ''' <param name="UID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef UID As UInt32)
            MyBase.New(New InstDetails(Prefix, TypeName), UID)
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws exception if error.
        ''' </summary>
        ''' <param name="UID">The FriedParts ID for this manufacturer</param>
        ''' <remarks></remarks>
        Public Sub New(ByRef UID As Int32)
            MyBase.New(New InstDetails(Prefix, TypeName), UID)
        End Sub

        ''' <summary>
        ''' Constructor. Loads the specified manufacturer or throws an exception if error. 
        ''' Uses the Octopart manufacturerID instead of the FriedParts manufacturer ID.
        ''' </summary>
        ''' <param name="OctopartID">The Octopart issued ID for this mfr</param>
        ''' <remarks>Casting is very important in overload resolution!!!</remarks>
        Public Sub New(ByRef OctopartID As Int64)
            MyBase.New(New InstDetails(Prefix, TypeName), OctopartID)
        End Sub

        ''' <summary>
        ''' Deny unqualified construction. If you don't tell me which manufacturer you want, I can't help you! ;-)
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Constructor. Loads an institution based on its name.
        ''' </summary>
        ''' <param name="Name">The institution's name to search for and load if found.</param>
        ''' <param name="Exact"></param>
        ''' <remarks>Throws an exception if not found or too many found</remarks>
        Public Sub New(ByVal Name As String, Optional ByVal Exact As Boolean = True)
            MyBase.New(New InstDetails(Prefix, TypeName), Name, Exact)
        End Sub
#End Region

    End Class

End Namespace



