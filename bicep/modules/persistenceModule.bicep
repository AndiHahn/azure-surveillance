param queueStorageConnectionString string

module resultPersistenceFunction 'shared/functionAppServerless.bicep' = {
  name: 'resultPersistenceFunction'
  params: {
    functionAppName: 'surv-persistence'
    additionalAppSettings: [
      {
        name: 'ProcessedImageQueueStorageConnectionString'
        value: queueStorageConnectionString
      }
    ]
  }
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2021-10-15' = {
  name: 'surv-cosmos-db'
  location: resourceGroup().location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: 'West Europe'
        failoverPriority: 0
        isZoneRedundant: true
      }
      {
        locationName: 'North Europe'
        failoverPriority: 1
      }
    ]
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 144
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Local'
      }
    }
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    isVirtualNetworkFilterEnabled: false
    enableAutomaticFailover: true
    enableFreeTier: false
  }
}

resource db 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-10-15' = {
  name: '${cosmosDb.name}/surveillance-db'
  location: resourceGroup().location
  properties: {
    resource: {
      id: 'surveillance-db'
    }
    options: {
      throughput: 400
    }
  }
}

module secret 'shared/keyvaultsecret.bicep' = {
  name: 'secret'
  params: {
    secretKey: 'CosmosDbConnectionString'
    secretValue: cosmosDb.listConnectionStrings().connectionStrings[0].connectionString
  }
}

module appConfigConnectionString 'shared/appConfigVaultRef.bicep' = {
  name: 'appConfigConnectionString'
  params: {
    key: 'CosmosDbConnectionString'
    secretName: 'CosmosDbConnectionString'
  }
}

module appConfigDatabaseName 'shared/appConfig.bicep' = {
  name: 'appConfigDatabaseName'
  params: {
    key: 'CosmosDbDatabaseName'
    value: 'surveillance-db'
  }
}

module appConfigContainerName 'shared/appConfig.bicep' = {
  name: 'appConfigContainerName'
  params: {
    key: 'CosmosDbContainer'
    value: 'surveillance-container'
  }
}
