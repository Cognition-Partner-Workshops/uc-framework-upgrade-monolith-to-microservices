# Stage 1: Build
FROM gradle:7.4-jdk11 AS build
WORKDIR /app
COPY ../.. .
RUN gradle :services:api-gateway:bootJar --no-daemon

# Stage 2: Runtime
FROM openjdk:11-jre-slim
WORKDIR /app
COPY --from=build /app/services/api-gateway/build/libs/api-gateway-*.jar app.jar
EXPOSE 8080
ENTRYPOINT ["java", "-jar", "app.jar"]
