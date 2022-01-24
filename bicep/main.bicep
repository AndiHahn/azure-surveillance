var config = json(loadTextContent('bicepconfig.json'))

module core 'modules/coreModule.bicep' = {
  name: 'coreModule'
  scope: resourceGroup(config['resource-group'])
}

module storages 'modules/storageModule.bicep' = {
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
    storages
  ]
  params: {
    imageStorageConnectionString: storages.outputs.imageStorageConnectionString
    queueStorageConnectionString: storages.outputs.queueStorageConnectionString
  }
}

module persistence 'modules/persistenceModule.bicep' = {
  name: 'persistenceModule'
  scope: resourceGroup(config['resource-group'])
  dependsOn: [
    core
    processing
  ]
  params: {
    queueStorageConnectionString: storages.outputs.queueStorageConnectionString
  }
}

module notification 'modules/notificationModule.bicep' = {
  name: 'notificationModule'
  scope: resourceGroup(config['resource-group'])
  dependsOn: [
    core
    persistence
  ]
  params: {
    queueStorageConnectionString: storages.outputs.queueStorageConnectionString
  }
}
