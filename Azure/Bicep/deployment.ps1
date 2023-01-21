New-AzResourceGroup -Name exampleRG -Location "west europe"

$result = New-AzResourceGroupDeployment -ResourceGroupName exampleRG -TemplateFile ./main.bicep -storageName "dfkljhlksjdhr4343"

write-host "String output = " $result.Outputs.link.value
write-host "Object output = " (($result.Outputs.urls.value).tostring() | convertfrom-json).web