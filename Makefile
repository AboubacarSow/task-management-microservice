dev:
	docker-compose -f infrastructure/docker-compose.yml \
	               -f infrastructure/docker-compose.override.yml \
	               up --build

prod:
	docker-compose -f infrastructure/docker-compose.yml up --build

down:
	docker-compose -f infrastructure/docker-compose.yml down

clean:
	docker-compose -f infrastructure/docker-compose.yml down -v