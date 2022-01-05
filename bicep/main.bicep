var config = json(loadTextContent('bicepconfig.json'))

module core 'modules/coreModule.bicep' = {
  name: 'coreModule'
  scope: resourceGroup(config['resource-group'])
}

module storageAcct 'modules/storageModule.bicep' = {
  name: 'storageModule'
  scope: resourceGroup(config['resource-group'])
  dependsOn: [
    core
  ]
}

module processing 'modules/processingModule.bicep' = {
  name: 'processingModule'
  scope: resourceGroup(config['resource-group'])
  dependsOn: [
    core
  ]
}

module persistence 'modules/persistenceModule.bicep' = {
  name: 'persistenceModule'
  scope: resourceGroup(config['resource-group'])
  dependsOn: [
    core
  ]
}
