param imageStorageConnectionString string
param queueStorageConnectionString string

module imageProcessingFunction 'shared/functionAppServerless.bicep' = {
  name: 'imageProcessingFunction'
  params: {
    functionAppName: 'surv-image-processing'
    additionalAppSettings: [
      {
        name: 'ImageUploadedQueueStorageConnectionString'
        value: queueStorageConnectionString
      }
      {
        name: 'ProcessedImageQueueStorageConnectionString'
        value: queueStorageConnectionString
      }
      {
        name: 'PersonDetectedQueueStorageConnectionString'
        value: queueStorageConnectionString
      }
      {
        name: 'ImageStorageConnectionString'
        value: imageStorageConnectionString
      }
    ]
  }
}

resource cognitiveServices 'Microsoft.CognitiveServices/accounts@2021-10-01' = {
  name: 'surv-cognitive-services'
  location: resourceGroup().location
  kind: 'CognitiveServices'
  sku: {
    name: 'S0'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    publicNetworkAccess: 'Enabled'
  }
}

module cognitiveServicesKey 'shared/keyvaultsecret.bicep' = {
  name: 'cognitiveServicesKey'
  params: {
    secretKey: 'CognitiveServicesApiKey'
    secretValue: cognitiveServices.listKeys().key1
  }
}

module cognitiveServicesConfigApiKey 'shared/appConfigVaultRef.bicep' = {
  name: 'cognitiveServicesConfigKey'
  params: {
    key: 'CognitiveServicesApiKey'
    secretName: 'CognitiveServicesApiKey'
  }
}

module cognitiveServicesConfigEndpoint 'shared/appConfig.bicep' = {
  name: 'cognitiveServicesConfigEndpoint'
  params: {
    key: 'CognitiveServicesEndpoint'
    value: cognitiveServices.properties.endpoint
  }
}
