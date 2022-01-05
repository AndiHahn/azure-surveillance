resource imageStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'survimgstorage'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var imageStorageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${imageStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${imageStorage.listKeys().keys[0].value}'

module imageStorageSecret 'shared/keyvaultsecret.bicep' = {
  name: 'imageStorageSecret'
  params: {
    secretKey: 'AzureBlobStorageConnectionString'
    secretValue: imageStorageConnectionString
  }
}

module imageStorageConfig 'shared/appConfigVaultRef.bicep' = {
  name: 'imageStorageConfig'
  params: {
    key: 'ImageStorageConnectionString'
    secretName: 'AzureBlobStorageConnectionString'
  }
}

resource queueStorage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'survqueuestorage'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var queueStorageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${queueStorage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${queueStorage.listKeys().keys[0].value}'

module queueStorageSecret 'shared/keyvaultsecret.bicep' = {
  name: 'keyVaultSecretQueueStorage'
  params: {
    secretKey: 'QueueStorageConnectionString'
    secretValue: queueStorageConnectionString
  }
}

module imageUploadedQueueStorageConfig 'shared/appConfigVaultRef.bicep' = {
  name: 'imageUploadedQueueStorageConfig'
  params: {
    key: 'ImageUploadedQueueStorageConnectionString'
    secretName: 'QueueStorageConnectionString'
  }
}

module processedImageQueueStorageConfig 'shared/appConfigVaultRef.bicep' = {
  name: 'processedImageQueueStorageConfig'
  params: {
    key: 'ProcessedImageQueueStorageConnectionString'
    secretName: 'QueueStorageConnectionString'
  }
}

module personDetectedQueueStorageConfig 'shared/appConfigVaultRef.bicep' = {
  name: 'personDetectedQueueStorageConfig'
  params: {
    key: 'PersonDetectedQueueStorageConnectionString'
    secretName: 'QueueStorageConnectionString'
  }
}

resource queueStorageImageUploadedQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-06-01' = {
  name: '${queueStorage.name}/default/image-uploaded-queue'
}

resource queueStoragePersonDetectedQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-06-01' = {
  name: '${queueStorage.name}/default/person-detected-queue'
}

resource queueStorageProcessedImageQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-06-01' = {
  name: '${queueStorage.name}/default/processed-image-queue'
}

resource eventGridSystemTopic 'Microsoft.EventGrid/systemTopics@2021-12-01' = {
  name: 'surv-storage-topic'
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    topicType: 'Microsoft.Storage.StorageAccounts'
    source: imageStorage.id
  }
}

@description('This is the built-in Storage quque data message sender role. See https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles')
resource queueSenderRoleDefinition 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  scope: subscription()
  name: 'c6a89b2d-59bc-44d0-9896-0f6e12d7b80a'
}

resource topicStorageAuthorization 'Microsoft.Authorization/roleAssignments@2020-08-01-preview' = {
  name: guid(subscription().id, eventGridSystemTopic.id, queueSenderRoleDefinition.id)
  scope: queueStorage
  properties: {
    principalId: eventGridSystemTopic.identity.principalId
    roleDefinitionId: queueSenderRoleDefinition.id
    principalType: 'ServicePrincipal'
  }
}

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-12-01' = {
  name: '${eventGridSystemTopic.name}/blob-created-subscription'
  properties: {
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
      enableAdvancedFilteringOnArrays: true
    }
    labels: [
    ]
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 5
      eventTimeToLiveInMinutes: 1440
    }
    deliveryWithResourceIdentity: {
      identity: {
        type: 'SystemAssigned'
      }
      destination: {
        endpointType: 'StorageQueue'
        properties: {
          resourceId: queueStorage.id
          queueName: 'image-uploaded-queue'
          queueMessageTimeToLiveInSeconds: 604800
        }
      }
    }
  }
}
