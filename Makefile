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
		up --build

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