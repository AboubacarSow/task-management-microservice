# Makefile

# Development — uses override for dev config
dev:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		-f infrastructure/docker-compose.override.yml \
		up --build

dev-simple:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		-f infrastructure/docker-compose.override.yml \
		up

list-containers:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		-f infrastructure/docker-compose.override.yml \
		ps

# Production — only base compose
prod:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		up --build

# Stop containers (without removing volumes)
down:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		down --remove-orphans

prune:
	docker system prune -f 

# Stop containers and remove volumes
clean:
	docker compose -p task-management-system \
		-f infrastructure/docker-compose.yml \
		down -v
	

# Load tests
load-test:
	docker run --rm \
		--network task-management-system_internal \
		-v $(PWD)/infrastructure/k6/scripts:/scripts \
		grafana/k6:latest run \
		--out influxdb=http://influxdb:8086/k6 \
		/scripts/load-test.js

load-test-50:
	docker run --rm \
		--network task-management-system_internal \
		-v $(PWD)/infrastructure/k6/scripts:/scripts \
		grafana/k6:latest run \
		--out influxdb=http://influxdb:8086/k6 \
		/scripts/scenarios/50-users.js

load-test-100:
	docker run --rm \
		--network task-management-system_internal \
		-v $(PWD)/infrastructure/k6/scripts:/scripts \
		grafana/k6:latest run \
		--out influxdb=http://influxdb:8086/k6 \
		/scripts/scenarios/100-users.js

load-test-200:
	docker run --rm \
		--network task-management-system_internal \
		-v $(PWD)/infrastructure/k6/scripts:/scripts \
		grafana/k6:latest run \
		--out influxdb=http://influxdb:8086/k6 \
		/scripts/scenarios/200-users.js

load-test-500:
	docker run --rm \
		--network task-management-system_internal \
		-v $(PWD)/infrastructure/k6/scripts:/scripts \
		grafana/k6:latest run \
		--out influxdb=http://influxdb:8086/k6 \
		/scripts/scenarios/500-users.js