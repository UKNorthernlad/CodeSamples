# Read Azure Key Valult Secrets from a Pod inside AKS

> References
* https://learn.microsoft.com/en-us/azure/aks/csi-secrets-store-driver
* https://learn.microsoft.com/en-us/azure/aks/csi-secrets-store-identity-access
* https://stackoverflow.com/questions/70306209/secret-creation-with-secretproviderclass-not-working-as-aspected

## Background
This example explains how to configure Azure Kubernetes Service (AKS) to read secrets from an Azure Key Vault (KV).

There are a number of ways this can be acheived, but this example looks at only two:

1. Mounting the secrets so that each appears as a separate files inside a pod.
2. Making the secrets available to the pod as environment variables.

## Common Setup
Regardless of which approach you wish to take, you will need both AKS and KV already in place. The following steps will help you create an AKS cluster with the required add-ons enables and to configure it to connect to KV. 

> If you have an existing AKS cluster, you can enable the required add-ons after the initial cluster creation. See the references at the top of the page for details.

### 1 - Create cluster
Note here the use of the `--enable-addons azure-keyvault-secrets-provider` option to enable Key Vault as a secret provider for the cluster.
```
az group create -n myResourceGroup -l uksouth

az aks create --node-count 2 --generate-ssh-keys --node-vm-size "standard_a2_v2" --name xxxaks --resource-group myResourceGroup --enable-addons azure-keyvault-secrets-provider --enable-managed-identity
```

### 2 - Get credentials to connect to cluster
```
az aks get-credentials --name xxxaks --resource-group myResourceGroup
```

### 3 -  Find the AD Application which was created along with the AKS cluster.
It should be noted these next two lines are returning the service principal name (GUID) and application ID (GUID) of the application which runs the default node pool.
```
$appId = az aks show -n xxxaks -g myResourceGroup --query "addonProfiles.azureKeyvaultSecretsProvider.identity.clientId" --output tsv

$servicePrincipal = az aks show -n xxxaks -g myResourceGroup --query "addonProfiles.azureKeyvaultSecretsProvider.identity.objectId" --output tsv
```

### 4 - Create a key vault, add a secret and grant the AKS node pool application read rights.
```
$vault = az keyvault create -n aks-train-dev-kv -g myResourceGroup -l uksouth

az keyvault secret set --vault-name aks-train-dev-kv -n ExampleSecret --value MyAKSExampleSecret

az keyvault set-policy -n aks-train-dev-kv --secret-permissions get --spn $appId
```


You now have a choice as to how you map the secrets inside each Pod:

A) **Mount as a Volume inside the pod**. You can mount a volume inside a pod, this way an application can read the secrets from what appear to be local files.

B) **Mount as a Secret object**. A SecretObject in AKS is a collection of Key/Value pairs. These are commonly mapped as environment variables inside a Pod.

> A folder for each example exists that contains two files. The `secretProvider.yaml` contains a *SecretProviderClass* configuration which details the key vault name and the authentication details. You will need to replace the three values shown below with your own environment specific values:

**userAssignedIdentityID**
```
az aks show -n xxxaks -g myResourceGroup --query "addonProfiles.azureKeyvaultSecretsProvider.identity.clientId" --output tsv
```

**keyvaultName**
```
($vault | ConvertFrom-Json).name
```

**tenantId**
```
Copy this from the properties section of your AAD homepage in the Azure Portal.
```

#### 5 - Deploy the configuration that states which key vault to use, the AAD GUID and $appId
```
kubectl apply -f .\secretStore.yaml
```

#### 6 - Create an example Pod that mounts the secrets as a volume
```
kubectl apply -f .\pod.yaml
```

#### 7 - Check the pod started and volume mounted correctly
```
kubectl exec busybox-secrets-store-inline-user-msi -- ls /mnt/secrets-store/
```

#### 8 - Read the value of a secret
```
kubectl exec busybox-secrets-store-inline-user-msi -- cat /mnt/secrets-store/ExampleSecret
```
> You should should see the value "MyAKSExampleSecret" printed out.

#### 9 - If you have choosen to map the secrets as environment variables, check these are now present
```
kubectl exec busybox-secrets-store-inline-user-msi -- env
```
> You should a new environment variable called "SECRET_USERNAME" whos value is "MyAKSExampleSecret".