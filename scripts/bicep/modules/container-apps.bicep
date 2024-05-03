targetScope = 'resourceGroup'

// ------------------
//    PARAMETERS
// ------------------

@description('The location where the resources will be created.')
param location string = resourceGroup().location

@description('Optional. The tags to be assigned to the created resources.')
param tags object = {}

@description('The resource Id of the container apps environment.')
param containerAppsEnvironmentId string

@description('The name of the service for the main app.')
param mainAppName string

@description('The name of the service for the sidecar.')
param sidecarName string

// Service Bus
@description('The name of the event hub namespace.')
param eventHubNamespaceName string

@description('The name of the event hub (topic).')
param eventHubName string

@description('Specifies the event hub send policy name.')
param eventHubSendPolicyName string

// Container Registry & Image
@description('The name of the container registry.')
param containerRegistryName string

@description('The image for the main app.')
param mainAppImage string

@description('The image for the sidecar service.')
param sidecarImage string

@description('The target port for the main web app.')
param mainAppPortNumber int

// ------------------
// VARIABLES
// ------------------

var containerRegistryPullRoleGuid='7f951dda-4ed3-4680-a7ca-43fe172d538d'

// ------------------
// RESOURCES
// ------------------

resource eventHubRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationRules@2022-01-01-preview' existing = {
  name: '${eventHubNamespaceName}/${eventHubName}/${eventHubSendPolicyName}'
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' existing = {
  name: containerRegistryName
}

resource containerRegistryUserAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'aca-user-identity-${uniqueString(resourceGroup().id)}'
  location: location
  tags: tags
}

resource containerRegistryPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if(!empty(containerRegistryName)) {
  name: guid(subscription().id, containerRegistry.id, containerRegistryUserAssignedIdentity.id) 
  scope: containerRegistry
  properties: {
    principalId: containerRegistryUserAssignedIdentity.properties.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', containerRegistryPullRoleGuid)
    principalType: 'ServicePrincipal'
  }
}

resource mainAppService 'Microsoft.App/containerApps@2022-06-01-preview' = {
  name: mainAppName
  location: location
  tags: tags
  identity: {
    type: 'SystemAssigned,UserAssigned'
    userAssignedIdentities: {
        '${containerRegistryUserAssignedIdentity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        external: true
        targetPort: mainAppPortNumber
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
      registries: !empty(containerRegistryName) ? [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: containerRegistryUserAssignedIdentity.id
        }
      ] : []
    }
    template: {
      containers: [
        {
          name: mainAppName
          image: mainAppImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'SIDECAR_URI'
              value: 'https://localhost:7001'
            }
          ]
        }
        {
          name: sidecarName
          image: sidecarImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: [
            {
              name: 'EH_FQDN'
              value: '${eventHubNamespaceName}.servicebus.windows.net:9093'
            }
            {
              name: 'EH_CONNECTION_STRING'
              value: eventHubRule.listkeys().primaryConnectionString
            }
            {
              name: 'EH_NAME'
              value: eventHubName
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}


// ------------------
// OUTPUTS
// ------------------

@description('The name of the container app for the main app.')
output mainAppContainerAppName string = mainAppService.name

@description('The FQDN of the main app service.')
output mainAppFQDN string = mainAppService.properties.configuration.ingress.fqdn
