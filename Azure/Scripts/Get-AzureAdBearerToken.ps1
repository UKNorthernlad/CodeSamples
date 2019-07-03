# Load ADAL Assemblies
$adal = "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ResourceManager\AzureResourceManager\AzureRM.Profile\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
$adalforms = "C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ResourceManager\AzureResourceManager\AzureRM.Profile\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll"
[System.Reflection.Assembly]::LoadFrom($adal)
[System.Reflection.Assembly]::LoadFrom($adalforms)

# Set Azure AD Tenant name
$adTenant = "?????????.onmicrosoft.com" 

# Set well-known client ID for AzurePowerShell
$clientId = "1950a258-227b-4e31-a9cf-717495945fc2" 

# Set redirect URI for Azure PowerShell
$redirectUri = "urn:ietf:wg:oauth:2.0:oob"

# Set Resource App URI as ARM
$resourceAppIdURI = "https://management.azure.com/"
# Set Authority to Azure AD Tenant
$authority = "https://login.windows.net/$adTenant"

# Create Authentication Context tied to Azure AD Tenant
$authContext = New-Object "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" -ArgumentList $authority

# Acquire token
$authResult = $authContext.AcquireToken($resourceAppIdURI, $clientId, $redirectUri, "always")

# Output bearer token
$authHeader = $authResult.CreateAuthorizationHeader()
$authHeader

$result = Invoke-RestMethod -Headers $Headers -Uri "https://management.azure.com/subscriptions/30eb243b-127d-452b-a744-e3ffffac46ac/resourcegroups/?api-version=2018-02-01"
$result.value

#$authHeader | Out-File jwt.txt
#$Headers = @{'Authorization'=$authHeader}
#Invoke-RestMethod -Headers $Headers -Uri "https://management.azure.com/subscriptions/30eb243b-127d-452b-a744-e3ffffac46ac/resourcegroups/Blah/providers/Microsoft.StreamAnalytics/streamingjobs/jobby?$expand=Inputs,Transformation,Outputs,Functions&api-version=2015-10-01"


