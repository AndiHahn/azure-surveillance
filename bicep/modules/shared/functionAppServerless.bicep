param functionAppName string

var config = json(loadTextContent('../../bicepconfig.json'))

var storageAccountName = '${toLower(substring(replace(functionAppName, '-', ''),0,10))}${uniqueString(resourceGroup().id)}' 
var uniqueStr = uniqueString('${resourceGroup().id}_${functionAppName}')
var hostingPlanName = '${functionAppName}${uniqueStr}'
var appInsightsName = '${functionAppName}${uniqueStr}'
param location string = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: { 
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: true
    siteConfig: {
      appSettings: [
        {
          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~3'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'Environment'
          value: 'prod'
        }
        {
          name: 'AzureKeyVaultUri'
          value: 'https://${config['key-vault-resource-name']}${environment().suffixes.keyvaultDns}/'
        }
        {
          name: 'AzureAppConfigSecretKey'
          value: 'AzureAppConfigurationConnectionString'
        }
      ]
    }
  }
}

module keyVaultAccess 'keyvaultpolicy.bicep' = {
  name: 'keyVaultAccess'
  params: {
    keyVaultPermissions: {
      secrets: [
        'get'
        'list'
      ]
    }
    policyAction: 'add'
    principalId: functionApp.identity.principalId
  }
}
