param secretKey string
param secretValue string

var config = json(loadTextContent('../../bicepconfig.json'))

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' = {
  name: '${config['key-vault-resource-name']}/${secretKey}'
  properties: {
    attributes: {
      enabled: true
    }
    value: secretValue
  }
}
