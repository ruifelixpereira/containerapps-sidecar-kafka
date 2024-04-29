#!/bin/bash

rootdir=$(pwd)

cd ../grpc-test/GrpcGreeter
docker build -t greeterservice:v1 .
cd $rootdir

cd ../grpc-test/GrpcGreeterClient
docker build -t greeterclient:v1 .
cd $rootdir

#cd ../main-app/src
#docker build -t albumapi:v1 .+


