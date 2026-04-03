# Stage 1: Build
FROM gradle:7.4-jdk11 AS build
WORKDIR /app
COPY ../.. .
RUN gradle :services:user-service:bootJar --no-daemon

# Stage 2: Runtime
FROM openjdk:11-jre-slim
WORKDIR /app
COPY --from=build /app/services/user-service/build/libs/user-service-*.jar app.jar
EXPOSE 8081
ENTRYPOINT ["java", "-jar", "app.jar"]
