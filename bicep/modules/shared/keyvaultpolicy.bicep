@description('Principal Id of the Azure resource (Managed Identity).')
param principalId string

@description('Assigned permissions for Principal Id (Managed Identity)')
param keyVaultPermissions object

@allowed([
  'add'
  'remove'
  'replace'
])
@description('Policy action.')
param policyAction string

var config = json(loadTextContent('../../bicepconfig.json'))

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: config['key-vault-resource-name']
  resource keyVaultPolicies 'accessPolicies' = {
    name: policyAction
    properties: {    
      accessPolicies: [
        {
          objectId: principalId
          permissions: keyVaultPermissions
          tenantId: subscription().tenantId
        }
      ]
    }
  }  
}
