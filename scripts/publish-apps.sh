#!/bin/bash

rootdir=$(pwd)

cd ../sidecar/src
docker build -t utilitysidecar:v1 .
cd $rootdir

cd ../main-app/src
docker build -t albumapi:v1 .
cd $rootdir
