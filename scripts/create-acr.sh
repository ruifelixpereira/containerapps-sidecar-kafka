RESOURCE_GROUP="<your RG name>"
CONTAINER_REGISTRY_NAME="<your ACR name>"

az acr create \
    --resource-group $RESOURCE_GROUP \
    --name $CONTAINER_REGISTRY_NAME \
    --sku Basic