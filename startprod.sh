#! /bin/bash

read -p "Enter vault key for dev: " vaultprodkey

export VAULT_DEV=$vaultprodkey

docker-compose -f docker-solution.yml up -d