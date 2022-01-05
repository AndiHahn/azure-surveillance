terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.65"
    }
  }

  required_version = ">= 0.14.9"
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy = true
      recover_soft_deleted_key_vaults = true
    }
  }
}

####################################
# create resource group
####################################
resource "azurerm_resource_group" "surveillance" {
  name     = "surveillance-auto"
  location = "westeurope"
}

####################################
# create keyvault, add secrets and set permissions
####################################
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "surveillance-keyvault" {
  name                        = "surv-keyvault-auto"
  location                    = azurerm_resource_group.surveillance.location
  resource_group_name         = azurerm_resource_group.surveillance.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Backup", "Restore", "Set", "Delete", "Purge", "Recover"
    ]
  }
}

####################################
# create storage for images
####################################
resource "azurerm_storage_account" "image-storage" {
  name                     = "survimgstorageauto"
  resource_group_name      = azurerm_resource_group.surveillance.name
  location                 = azurerm_resource_group.surveillance.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_key_vault_secret" "image-storage-secret" {
  name         = "AzureBlobStorageConnectionString"
  value        = azurerm_storage_account.image-storage.primary_connection_string
  key_vault_id = azurerm_key_vault.surveillance-keyvault.id
}

####################################
# create azure function with storage
####################################
resource "azurerm_storage_account" "function-storage" {
  name                     = "survimgprocauto"
  resource_group_name      = azurerm_resource_group.surveillance.name
  location                 = azurerm_resource_group.surveillance.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "function-plan" {
  name                = "surveillance-image-processing-plan"
  location            = azurerm_resource_group.surveillance.location
  resource_group_name = azurerm_resource_group.surveillance.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_application_insights" "function-app-insights" {
  name                = "surveillance-image-processing-auto"
  location            = "${azurerm_resource_group.surveillance.location}"
  resource_group_name = "${azurerm_resource_group.surveillance.name}"
  application_type    = "web"
}

resource "azurerm_function_app" "function-app" {
  name                       = "surveillance-image-processing-auto"
  location                   = azurerm_resource_group.surveillance.location
  resource_group_name        = azurerm_resource_group.surveillance.name
  app_service_plan_id        = azurerm_app_service_plan.function-plan.id
  storage_account_name       = azurerm_storage_account.function-storage.name
  storage_account_access_key = azurerm_storage_account.function-storage.primary_access_key

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.function-app-insights.instrumentation_key}"
  }
}

resource "azurerm_key_vault_access_policy" "function-app-keyvault-access" {
  key_vault_id = azurerm_key_vault.surveillance-keyvault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_function_app.function-app.identity[0].principal_id

  secret_permissions = [
    "Get",
    "List"
  ]
}

####################################
# create event grid topic and azure function subscription
####################################
resource "azurerm_eventgrid_system_topic" "storage-topic" {
  name                   = "surveillance-storage-topic-auto"
  resource_group_name    = azurerm_resource_group.surveillance.name
  location               = azurerm_resource_group.surveillance.location
  source_arm_resource_id = azurerm_storage_account.image-storage.id
  topic_type             = "Microsoft.Storage.StorageAccounts"
}

#TODO: make subscription for function (after release??) 

####################################
# create cognitive services
####################################
resource "azurerm_cognitive_account" "surveillance-cognitive" {
  name                = "surveillance-cognitiveservices-auto"
  location            = azurerm_resource_group.surveillance.location
  resource_group_name = azurerm_resource_group.surveillance.name
  kind                = "CognitiveServices"

  sku_name = "S0"
}

resource "azurerm_key_vault_secret" "cognitive-service-secret" {
  name         = "CognitiveServicesApiKey"
  value        = azurerm_cognitive_account.surveillance-cognitive.primary_access_key
  key_vault_id = azurerm_key_vault.surveillance-keyvault.id
}

####################################
# cosmos db
####################################
resource "azurerm_cosmosdb_account" "cosmos-db" {
  name                = "surveillance-cosmos-db-auto"
  location            = azurerm_resource_group.surveillance.location
  resource_group_name = azurerm_resource_group.surveillance.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  enable_automatic_failover = true

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 300
    max_staleness_prefix    = 100000
  }

  geo_location {
    location          = "North Europe"
    failover_priority = 1
  }

  geo_location {
    location          = azurerm_resource_group.surveillance.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "cosmos-sql" {
  name                = "surveillance-db"
  resource_group_name = azurerm_cosmosdb_account.cosmos-db.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmos-db.name
  throughput          = 400
}

resource "azurerm_cosmosdb_sql_container" "cosmos-container" {
  name                  = "surveillance-container"
  resource_group_name   = azurerm_cosmosdb_account.cosmos-db.resource_group_name
  account_name          = azurerm_cosmosdb_account.cosmos-db.name
  database_name         = azurerm_cosmosdb_sql_database.cosmos-sql.name
  partition_key_path    = "/PartitionKey"
  partition_key_version = 1
}

#TODO: add key to keyvault

####################################
# event grid topic
####################################
resource "azurerm_eventgrid_topic" "topic" {
  name                = "surveillance-topic-auto"
  location            = azurerm_resource_group.surveillance.location
  resource_group_name = azurerm_resource_group.surveillance.name
}

