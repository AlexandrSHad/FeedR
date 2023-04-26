Start infrastructure and services:
```
docker compose -f infrastructure.yml up -d
docker compose -f services.yml up -d
```

Connect to redis:
```
docker exec redis -it redis-cli
```

Get all redis keys:
```
keys *
```