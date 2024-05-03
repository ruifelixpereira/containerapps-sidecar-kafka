#RESOURCE_GROUP="<your RG name>"

az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file "./bicep/main.bicep" \
  --parameters "./bicep/main.parameters.json"