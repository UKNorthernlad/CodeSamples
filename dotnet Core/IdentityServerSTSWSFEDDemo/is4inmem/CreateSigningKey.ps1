#Requires -RunAsAdministrator
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Remove old stuff
Remove-Module -name "New-SelfSignedCertificateEx" -Force -ErrorAction SilentlyContinue -Confirm:$false
Get-ChildItem "cert:\LocalMachine\My" | Where-Object {$_.Subject -eq "CN=sign"} | remove-item

# Add new / overwrite existing
Import-Module -FullyQualifiedName ".\New-SelfSignedCertificateEx.ps1"

New-SelfsignedCertificateEx -Subject "CN=sign" -SubjectAlternativeName "CN=localhost" -EKU "Server Authentication", "Client authentication" -KeyUsage "KeyEncipherment, DigitalSignature" -AllowSMIME -Path "$HOME\Downloads\sign.pfx" -Password (ConvertTo-SecureString "password" -AsPlainText -Force) -Exportable 

$newcert = Import-PfxCertificate -FilePath "$HOME\Downloads\sign.pfx" -CertStoreLocation "cert:\localmachine\my" -Password (ConvertTo-SecureString "password" -AsPlainText -Force)
