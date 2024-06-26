# Azure Container Apps Album API

This is the companion repository for the [Azure Container Apps code-to-cloud quickstart](https://docs.microsoft.com/en-us/azure/container-apps/quickstart-code-to-cloud?tabs=bash%2Ccsharp&pivots=acr-remote).

This backend Album API sample is available in other languages:

| [JavaScript](https://github.com/azure-samples/containerapps-albumapi-javascript) | [Go](https://github.com/azure-samples/containerapps-albumapi-go) | [Python](https://github.com/azure-samples/containerapps-albumapi-python) | [Java](https://github.com/azure-samples/containerapps-albumapi-java) |
| -------------------------------------------------------------------------------- | ---------------------------------------------------------------- | ------------------------------------------------------------------------ | ---------------------------------------------------------------- |


## Test locally

```bash
cd src
docker build -t albumapi:v1 .

docker run -d --name albumapi -p 8080:8080 albumapi:v1

curl http://localhost:8080/albums
```
