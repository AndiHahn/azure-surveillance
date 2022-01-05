var config = json(loadTextContent('../bicepconfig.json'))

resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' = {
  name: config['key-vault-resource-name']
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: false
    accessPolicies: [
      {
        objectId: config['admin-aad-user-object-id']
        tenantId: tenant().tenantId
        permissions: {
          certificates: [
            'all'
          ]
          keys: [
            'all'
          ]
          secrets: [
            'all'
          ]
          storage: [
            'all'
          ]
        }
      }
    ]
  }
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview' = {
  name: config['app-config-resource-name']
  location: resourceGroup().location 
  sku: {
    name: 'free'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

module appConfigSecret 'shared/keyvaultsecret.bicep' = {
  name: 'appConfigSecret'
  params: {
    secretKey: 'AzureAppConfigurationConnectionString'
    secretValue: appConfiguration.listKeys().value[0].connectionString
  }
}
