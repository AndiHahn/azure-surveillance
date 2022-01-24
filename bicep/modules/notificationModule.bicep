param queueStorageConnectionString string

module notificationFunction 'shared/functionAppServerless.bicep' = {
  name: 'notificationFunction'
  params: {
    functionAppName: 'surv-notification'
    additionalAppSettings: [
      {
        name: 'PersonDetectedQueueStorageConnectionString'
        value: queueStorageConnectionString
      }
    ]
  }
}
