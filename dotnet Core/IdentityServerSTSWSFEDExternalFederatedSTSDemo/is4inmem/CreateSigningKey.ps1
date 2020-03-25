#Requires -RunAsAdministrator
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Remove old stuff
Remove-Module -name "New-SelfSignedCertificateEx" -Force -ErrorAction SilentlyContinue -Confirm:$false
Remove-Item "$HOME\Downloads\New-SelfSignedCertificateEx.zip" -Force -ErrorAction SilentlyContinue -Confirm:$false
Remove-Item "$HOME\Downloads\New-SelfSignedCertificateEx" -Recurse -Force -ErrorAction SilentlyContinue -Confirm:$false
Get-ChildItem "cert:\LocalMachine\My" | Where-Object {$_.Subject -eq "CN=sign"} | remove-item

# Add new / overwrite existing
Invoke-WebRequest -Uri "https://gallery.technet.microsoft.com/scriptcenter/Self-signed-certificate-5920a7c6/file/101251/2/New-SelfSignedCertificateEx.zip" -OutFile "$HOME\Downloads\New-SelfSignedCertificateEx.zip"

Expand-Archive -Path "$HOME\Downloads\New-SelfSignedCertificateEx.zip" -DestinationPath "$HOME\Downloads\New-SelfSignedCertificateEx"

Import-Module -FullyQualifiedName "$HOME\Downloads\New-SelfSignedCertificateEx\New-SelfSignedCertificateEx.ps1"

New-SelfsignedCertificateEx -Subject "CN=sign" -SubjectAlternativeName "CN=localhost" -EKU "Server Authentication", "Client authentication" -KeyUsage "KeyEncipherment, DigitalSignature" -AllowSMIME -Path "$HOME\Downloads\sign.pfx" -Password (ConvertTo-SecureString "password" -AsPlainText -Force) -Exportable 

$newcert = Import-PfxCertificate -FilePath "$HOME\Downloads\sign.pfx" -CertStoreLocation "cert:\localmachine\my" -Password (ConvertTo-SecureString "password" -AsPlainText -Force)
