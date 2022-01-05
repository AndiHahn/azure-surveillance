param key string
param value string
param label string = 'prod'

var config = json(loadTextContent('../../bicepconfig.json'))

var keyName = empty(label) ? key : '${key}$${label}'

resource appConfig 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-03-01-preview' = {
  name: '${config['app-config-resource-name']}/${keyName}'
  properties: {
    value: value
  }
}
