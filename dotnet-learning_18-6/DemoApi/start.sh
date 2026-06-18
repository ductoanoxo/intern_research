#!/bin/bash

echo "Docker build"
docker build -t dotnet-api .

echo "Docker run"
docker run -d -p 8080:8080 --name dotnet-api dotnet-api


