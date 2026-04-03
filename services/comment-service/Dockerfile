# Stage 1: Build
FROM gradle:7.4-jdk11 AS build
WORKDIR /app
COPY ../.. .
RUN gradle :services:comment-service:bootJar --no-daemon

# Stage 2: Runtime
FROM openjdk:11-jre-slim
WORKDIR /app
COPY --from=build /app/services/comment-service/build/libs/comment-service-*.jar app.jar
EXPOSE 8083
ENTRYPOINT ["java", "-jar", "app.jar"]
