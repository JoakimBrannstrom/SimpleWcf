param([switch] $UseFiddler)

<#
$oldMachineName = "some-machine"
$mySession = New-PSSession $oldMachineName

invoke-command -Session $mySession -ScriptBlock {
}

remove-pssession $mySession
#>

. .\wcf.ps1

if ($UseFiddler) {
  $hostname = 'localhost.fiddler'
}
else {
  $hostname = 'localhost'
}

# $wsdlImporter = Get-WsdlImporter "http://localhost:8090/Console/Service/mex"
# Get-WsdlImporter "http://$($hostname):8090/Console/Service" -HttpGet
# Get-WsdlImporter "http://$($hostname):8090/Console/Service?wsdl" -HttpGet 

# $proxyType = Get-WcfProxyType $wsdlImporter

# $endpoints = $wsdlImporter.ImportAllEndpoints();

# $proxy = New-Object $proxyType($endpoints[0].Binding, $endpoints[0].Address);

$proxy = Get-WcfProxy "http://$($hostname):8090/Console/Service/mex"
# $proxy = Get-WcfProxy $wsdlImporter # "http://$($hostname):8090/Console/Service" (New-Object System.ServiceModel.WSHttpBinding)
# $proxy = Get-WcfProxy 'http://localhost:8090/Console/Service/mex' 'http://localhost:8090/Console/Service/basic' (New-Object System.ServiceModel.BasicHttpBinding)

if($proxy -ne $null)
{
	# $proxy | Get-member
	Write-Host ""
	Write-Host "[Connection information]"
	Write-Host "	Address: " $proxy.Endpoint.Address
	Write-Host "	Binding: "$proxy.Endpoint.Binding
	Write-Host "	Contract name: " $proxy.Endpoint.Contract.ConfigurationName
	$operations = $proxy.Endpoint.Contract.Operations | Select -ExpandProperty Name 
	$operations = [string]::join(", ", $operations)
	Write-Host "	Contract Operations: " $operations

	# $proxy.Endpoint.Contract.Operations

	Write-Host ""
	Write-Host "---------------------"

	Write-Host ""
	Write-Host "HelloServer()"
	$proxy.HelloServer();

	Write-Host ""
	Write-Host "GetData(""Hello world!"")"
	$proxy.GetData("Hello world!");

	Write-Host ""
}
else
{
	Write-Host ""
	Write-Host "ERROR: Could not create a wcf proxy!"
	Write-Host ""
}

#Get-Help Get-WsdlImporter
#Get-Help Get-WcfProxyType
#Get-Help Get-WcfProxy
