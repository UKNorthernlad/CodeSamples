# https://learn.microsoft.com/en-us/azure/event-grid/custom-event-quickstart-powershell

# login-azaccount -Tenant xxxx.onmicrosoft.com
# get-azsubscription -SubscriptionId xxxxx -TenantId xxxx | Select-azsubscription

$location = "westeurope"

New-AzResourceGroup -Name gridResourceGroup -Location $location
Register-AzResourceProvider -ProviderNamespace Microsoft.EventGrid

# An event grid topic provides a user-defined endpoint that you post your events to.
$topicname="testing123"
New-AzEventGridTopic -ResourceGroupName gridResourceGroup -Location $location -Name $topicname

# Any application which subscribes to the topic will receive a copy of the sent message.
# Create a receiving application.
$sitename="daveblah99"
New-AzResourceGroupDeployment -ResourceGroupName gridResourceGroup -TemplateUri "https://raw.githubusercontent.com/Azure-Samples/azure-event-grid-viewer/master/azuredeploy.json" -siteName $sitename -hostingPlanName viewerhost

# Setup a subscription to send the message from the EG topic to the receiving application
$endpoint="https://$sitename.azurewebsites.net/api/updates"
New-AzEventGridSubscription -EventSubscriptionName demoViewerSub -Endpoint $endpoint -ResourceGroupName gridResourceGroup -TopicName $topicname


# Send an event to your topic
$endpoint = (Get-AzEventGridTopic -ResourceGroupName gridResourceGroup -Name $topicname).Endpoint
$keys = Get-AzEventGridTopicKey -ResourceGroupName gridResourceGroup -Name $topicname


# Construct a request 
$eventID = Get-Random 99999
#Date format should be SortableDateTimePattern (ISO 8601)
$eventDate = Get-Date -Format s
#Construct body using Hashtable
$htbody = @{
    id= $eventID
    eventType="recordInserted"
    subject="myapp/vehicles/motorcycles"
    eventTime= $eventDate   
    data= @{
        make="Ducati"
        model="Monster"
    }
    dataVersion="1.0"
}

#Use ConvertTo-Json to convert event body from Hashtable to JSON Object
#Append square brackets to the converted JSON payload since they are expected in the event's JSON payload syntax
$body = "["+(ConvertTo-Json $htbody)+"]"

# Send the request
Invoke-WebRequest -Uri $endpoint -Method POST -Body $body -Headers @{"aeg-sas-key" = $keys.Key1}

# Send a load of requests
$x=0
do {
    Invoke-WebRequest -Uri $endpoint -Method POST -Body $body -Headers @{"aeg-sas-key" = $keys.Key1}
    $x++
   }
while ($x -le 1000)
