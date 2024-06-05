#!/bin/bash

# Проверяем количество аргументов
if [ "$#" -lt 2 ]; then
    echo "Использование: $0 <hostname> <registry>"
    exit 1
fi
HOSTNAME=$1
REGISTRY=$2
## Этап 1: Собираем Angular проект
cd angular-client && npm install && npm run build --prod && cd..

# Этап 2: Сборка клиента
docker build -t "$REGISTRY"/storypoker.client.web.api:latest -f Client/StoryPoker.Client.Web.Api/Dockerfile .
docker build -t "$REGISTRY"/storypoker.server.silo:latest -f Server/StoryPoker.Server.Silo/Dockerfile .

# Этап 4: Пушим образы в Docker Registry
docker push "$REGISTRY"/storypoker.client.web.api:latest
docker push "$REGISTRY"/storypoker.server.silo:latest

# Останавливаем контейнеры

ssh root@"HOSTNAME" docker stop story-poker-client
ssh root@"HOSTNAME" docker stop story-poker-silo

# Удаляем контейнеры
ssh root@"HOSTNAME" docker rm story-poker-client
ssh root@"HOSTNAME" docker rm story-poker-silo

# пулим новые образы
ssh root@"HOSTNAME" docker pull "$REGISTRY"/storypoker.client.web.api:latest
ssh root@"HOSTNAME" docker pull "$REGISTRY"/storypoker.server.silo:latest

# запускаем новые образы
ssh root@"HOSTNAME" docker run -d --name story-poker-client "$REGISTRY"/storypoker.client.web.api:latest --ports 8015:8015
ssh root@"HOSTNAME" docker run -d --name story-poker-silo "$REGISTRY"/storypoker.server.silo:latest --ports 8025:8025
echo "Развертывание завершено."
