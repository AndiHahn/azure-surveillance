param key string
param secretName string
param label string = 'prod'

var config = json(loadTextContent('../../bicepconfig.json'))

var keyName = empty(label) ? key : '${key}$${label}'

resource appConfig 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-03-01-preview' = {
  name: '${config['app-config-resource-name']}/${keyName}'
  properties: {
    value: '${config['key-vault-resource-name']}${environment().suffixes.keyvaultDns}/secrets/${secretName}'
    contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
  }
}