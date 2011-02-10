Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web

<ServiceContract(Namespace:="")> _
<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
Public Class SearchService

    ' Add <WebGet()> attribute to use HTTP GET 
    <OperationContract()> _
    Public Sub DoWork()
        ' Add your operation implementation here
    End Sub

    ' Add more operations here and mark them with <OperationContract()>

End Class
