#CONTAINER_REGISTRY_NAME="<your ACR name>"
BASE_DIR=$(pwd)

## Build Main App (Backend API) on ACR and Push to ACR
cd ../main-app/src
az acr build --registry $CONTAINER_REGISTRY_NAME \
    --image "albumapi:v1.7" \
    --file 'Dockerfile' .
cd $BASE_DIR

## Build Sidecar on ACR and Push to ACR
cd ../sidecar/src
az acr build --registry $CONTAINER_REGISTRY_NAME \
    --image "utilitysidecar:v1.7" \
    --file 'Dockerfile' .
cd $BASE_DIR
