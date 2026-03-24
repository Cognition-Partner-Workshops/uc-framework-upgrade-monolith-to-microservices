.PHONY: build run stop test lint clean backend frontend logs

# Build all Docker images
build:
	docker compose build

# Start all services
run:
	docker compose up -d

# Stop all services
stop:
	docker compose down

# View logs
logs:
	docker compose logs -f

# Run backend tests
test:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew test -x jacocoTestCoverageVerification --no-daemon

# Run lint checks
lint:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew spotlessCheck --no-daemon

# Apply lint fixes
lint-fix:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew spotlessApply --no-daemon

# Build backend JAR locally (without Docker)
backend:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew bootJar --no-daemon

# Run backend locally (without Docker)
backend-run:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew bootRun --no-daemon

# Install frontend dependencies
frontend:
	cd frontend && npm install

# Run frontend locally (without Docker)
frontend-run:
	cd frontend && npm run dev

# Clean build artifacts
clean:
	JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64 ./gradlew clean --no-daemon
	docker compose down --rmi local -v 2>/dev/null || true
