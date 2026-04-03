# Stage 1: Build
FROM gradle:7.4-jdk11 AS build
WORKDIR /app
COPY ../.. .
RUN gradle :services:article-service:bootJar --no-daemon

# Stage 2: Runtime
FROM openjdk:11-jre-slim
WORKDIR /app
COPY --from=build /app/services/article-service/build/libs/article-service-*.jar app.jar
EXPOSE 8082
ENTRYPOINT ["java", "-jar", "app.jar"]
