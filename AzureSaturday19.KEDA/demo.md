# KEDA demo

## Build docker image

```bash
# build the app
dotnet build -c Release

# build the image
docker build -t kedafunc:v2 .

# save storage connection in a variable
STORAGECONN=$(az storage account show-connection-string -g AzureSaturday19 -n azuresaturday19 -o tsv)

# start container to test
docker run -e AzureWEbJobsStorage=$STORAGECONN -p 7071:80 kedafunc:v2

# push on docker hub
docker tag kedafunc:v2 andrekiba/kedafunc:v2
docker push andrekiba/kedafunc:v2
```

## Install KEDA on minikube

```bash
# set kubectl context
kubectl config use-context minikube

# start minikube
minikube start

# install keda
func kubernetes install --namespace keda

# generate deploy file
# then replace LoadBalancer with NodePort (only if Minikube)
func kubernetes deploy --name kedademo --image-name andrekiba/kedafunc:v2 --dry-run > deploy.yml

# applay the deployment
kubectl apply -f ./deploy.yml

# get the service url for test
minikube service kedademo-http --url
```

## Run test

```bash
# start dashboard
minikube dashboard

# get something :-)
kubectl get deploy -w
kubectl get pods -w
kubectl get hpa

# set max pods to 30
kubectl edit hpa keda-hpa-kedademo

# push messages to kedaqueue

# clean up
kubectl delete deploy kedademo
kubectl delete ScaledObject kedademo
kubectl delete secret kedademo

# unistall KEDA
func kubernetes remove --namespace keda
```
