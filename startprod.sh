#! /bin/bash
read -p "Enter vault root token: " vaultRoot
export VaultRoot="$vaultRoot"
docker-compose -f compose-prod.yml up -d

read -p "Enter vault unseal key1: " vaultunseal1
read -p "Enter vault unseal key2: " vaultunseal2
read -p "Enter vault unseal key3: " vaultunseal3

docker exec -it vault bin/sh -c "vault operator unseal $vaultunseal1"
docker exec -it vault bin/sh -c "vault operator unseal $vaultunseal2"
docker exec -it vault bin/sh -c "vault operator unseal $vaultunseal3"