# Makefile

# Development — uses override for dev config
dev:
	docker-compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		-f infrastructure/docker-compose.override.yml \
		up --build

dev-simple:
	docker-compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		-f infrastructure/docker-compose.override.yml \
		up

# Production — only base compose
prod:
	docker-compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		up --build

# Stop containers (without removing volumes)
down:
	docker-compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		down

pron:
	docker system prune -f 

# Stop containers and remove volumes
clean:
	docker-compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		down -v
	

# Load tests
load-test:
	docker-compose -f infrastructure/docker-compose.yml \
	               -f infrastructure/docker-compose.override.yml \
	               run --rm k6 run \
	               --out influxdb=http://influxdb:8086/k6 \
	               /scripts/load-test.js

load-test-50:
	docker-compose -f infrastructure/docker-compose.yml \
	               -f infrastructure/docker-compose.override.yml \
	               run --rm k6 run \
	               --out influxdb=http://influxdb:8086/k6 \
	               /scripts/scenarios/50-users.js

load-test-100:
	docker-compose -f infrastructure/docker-compose.yml \
	               -f infrastructure/docker-compose.override.yml \
	               run --rm k6 run \
	               --out influxdb=http://influxdb:8086/k6 \
	               /scripts/scenarios/100-users.js