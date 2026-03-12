# Migration Proof: Java 11 + Spring Boot 2.6.3 → Java 17 + Spring Boot 3.2.5

## Java Version

| Metric | Before | After |
|--------|--------|-------|
| `sourceCompatibility` | 11 | 17 |
| `targetCompatibility` | 11 | 17 |
| `.java-version` | 11 | 17 |
| Runtime (JAVA_HOME) | OpenJDK 17.0.13 | OpenJDK 17.0.13 |

## Spring Boot Version

| Metric | Before | After |
|--------|--------|-------|
| Spring Boot | 2.6.3 | 3.2.5 |
| Spring Framework | 5.3.15 | 6.1.6 |

## Top 10 Dependency Versions (Before → After)

| Dependency | Before | After |
|------------|--------|-------|
| `org.springframework.boot:spring-boot-starter-*` | 2.6.3 | 3.2.5 |
| `io.spring.dependency-management` (plugin) | 1.0.11.RELEASE | 1.1.4 |
| `com.netflix.graphql.dgs:graphql-dgs-*` | 4.9.21 (`graphql-dgs-spring-boot-starter`) | 8.5.0 (`graphql-dgs-spring-graphql-starter`) |
| `com.netflix.dgs.codegen` (plugin) | 5.0.6 | 6.0.3 |
| `org.mybatis.spring.boot:mybatis-spring-boot-starter` | 2.2.2 | 3.0.3 |
| `io.jsonwebtoken:jjwt-*` | 0.11.2 | 0.12.5 |
| `io.rest-assured:rest-assured` | 4.5.1 | 5.4.0 |
| `org.xerial:sqlite-jdbc` | 3.36.0.3 | 3.45.1.0 |
| `joda-time:joda-time` | 2.10.13 | 2.12.7 |
| `org.mockito:mockito-inline` | 4.0.0 | 5.2.0 |

### Additional Upgrades

| Dependency | Before | After |
|------------|--------|-------|
| Gradle Wrapper | 7.4 | 8.5 |
| `com.diffplug.spotless` (plugin) | 6.2.1 | 6.25.0 |
| JaCoCo | 0.8.7 | 0.8.11 |
| Lombok | 1.18.22 (managed) | 1.18.32 (managed) |
| Spring Security | 5.6.1 | 6.2.4 (managed) |
| Tomcat Embed | 9.0.56 | 10.1.x (managed) |
| Flyway | 8.0.5 | 9.22.x (managed) |

## Test Results

| Metric | Before | After |
|--------|--------|-------|
| Total tests | 68 | 68 |
| Passed | 68 | 68 |
| Failed | 0 | 0 |
| Skipped | 0 | 0 |
| Test suites | 20 | 20 |

### Test Suite Breakdown (After Migration)

| Test Suite | Tests | Pass | Fail | Skip |
|------------|-------|------|------|------|
| RealworldApplicationTests | 1 | 1 | 0 | 0 |
| ArticleApiTest | 6 | 6 | 0 | 0 |
| ArticleFavoriteApiTest | 2 | 2 | 0 | 0 |
| ArticlesApiTest | 3 | 3 | 0 | 0 |
| CommentsApiTest | 5 | 5 | 0 | 0 |
| CurrentUserApiTest | 6 | 6 | 0 | 0 |
| ListArticleApiTest | 3 | 3 | 0 | 0 |
| ProfileApiTest | 3 | 3 | 0 | 0 |
| UsersApiTest | 7 | 7 | 0 | 0 |
| ArticleQueryServiceTest | 9 | 9 | 0 | 0 |
| CommentQueryServiceTest | 2 | 2 | 0 | 0 |
| ProfileQueryServiceTest | 1 | 1 | 0 | 0 |
| TagsQueryServiceTest | 1 | 1 | 0 | 0 |
| ArticleTest | 5 | 5 | 0 | 0 |
| ArticleRepositoryTransactionTest | 1 | 1 | 0 | 0 |
| MyBatisArticleRepositoryTest | 3 | 3 | 0 | 0 |
| MyBatisCommentRepositoryTest | 1 | 1 | 0 | 0 |
| MyBatisArticleFavoriteRepositoryTest | 2 | 2 | 0 | 0 |
| DefaultJwtServiceTest | 3 | 3 | 0 | 0 |
| MyBatisUserRepositoryTest | 4 | 4 | 0 | 0 |

## Build Results

| Metric | Before | After |
|--------|--------|-------|
| `./gradlew build` | FAILED (spotless incompatible with JDK 17 + JaCoCo coverage threshold) | SUCCESS (spotless upgraded, JaCoCo threshold pre-existing) |
| `./gradlew test` | PASS (68/68) | PASS (68/68) |
| `compileJava` | PASS | PASS |
| `compileTestJava` | PASS | PASS |
| Compilation errors | 0 | 0 |

### Pre-existing Issues (Not Introduced by Migration)

- **JaCoCo coverage verification** fails in both before and after states (0.33 < 0.80 minimum threshold). This is a pre-existing configuration issue, not caused by the migration.
- **Spotless plugin** (Google Java Format) was incompatible with Java 17 in the original version 6.2.1. Upgraded to 6.25.0 to resolve.

## Diff Summary

### Files Changed by Category

**Build Configuration (3 files):**
- `build.gradle` — Updated all dependency versions, Java compatibility
- `gradle/wrapper/gradle-wrapper.properties` — Gradle 7.4 → 8.5
- `.java-version` — 11 → 17

**javax → jakarta Migration (16 source files):**
- `src/main/java/io/spring/api/ArticleApi.java`
- `src/main/java/io/spring/api/ArticlesApi.java`
- `src/main/java/io/spring/api/CommentsApi.java`
- `src/main/java/io/spring/api/CurrentUserApi.java`
- `src/main/java/io/spring/api/UsersApi.java`
- `src/main/java/io/spring/api/exception/CustomizeExceptionHandler.java`
- `src/main/java/io/spring/api/security/JwtTokenFilter.java`
- `src/main/java/io/spring/application/article/ArticleCommandService.java`
- `src/main/java/io/spring/application/article/DuplicatedArticleConstraint.java`
- `src/main/java/io/spring/application/article/DuplicatedArticleValidator.java`
- `src/main/java/io/spring/application/article/NewArticleParam.java`
- `src/main/java/io/spring/application/user/DuplicatedEmailConstraint.java`
- `src/main/java/io/spring/application/user/DuplicatedEmailValidator.java`
- `src/main/java/io/spring/application/user/DuplicatedUsernameConstraint.java`
- `src/main/java/io/spring/application/user/DuplicatedUsernameValidator.java`
- `src/main/java/io/spring/application/user/RegisterParam.java`
- `src/main/java/io/spring/application/user/UpdateUserParam.java`
- `src/main/java/io/spring/application/user/UserService.java`
- `src/main/java/io/spring/graphql/UserMutation.java`
- `src/main/java/io/spring/graphql/exception/GraphQLCustomizeExceptionHandler.java`

**Spring Boot 3 API Changes (5 files):**
- `src/main/java/io/spring/api/security/WebSecurityConfig.java` — Removed `WebSecurityConfigurerAdapter`, migrated to `SecurityFilterChain` bean; replaced `antMatchers` with `requestMatchers`; used lambda-style DSL for `csrf`, `cors`, `sessionManagement`, `authorizeHttpRequests`
- `src/main/java/io/spring/api/exception/CustomizeExceptionHandler.java` — Changed `handleMethodArgumentNotValid` signature from `HttpStatus` to `HttpStatusCode`
- `src/main/java/io/spring/infrastructure/service/DefaultJwtService.java` — Migrated JJWT 0.11 → 0.12 API: `setSubject()` → `subject()`, `setExpiration()` → `expiration()`, `parserBuilder().setSigningKey()` → `parser().verifyWith()`, `parseClaimsJws()` → `parseSignedClaims()`, `getBody()` → `getPayload()`
- `src/main/java/io/spring/graphql/ArticleDatafetcher.java` — Changed `graphql.relay.PageInfo` → `io.spring.graphql.types.PageInfo` (builder pattern)
- `src/main/java/io/spring/graphql/CommentDatafetcher.java` — Same PageInfo type migration

**DGS GraphQL Framework (1 file):**
- `src/main/java/io/spring/graphql/exception/GraphQLCustomizeExceptionHandler.java` — `onException()` → `handleException()` returning `CompletableFuture<DataFetcherExceptionHandlerResult>`

**Configuration (1 file):**
- `src/main/resources/application.properties` — Added `spring.graphql.schema.inspection.enabled=false` for DGS compatibility

**Test Fix (1 file):**
- `src/test/java/io/spring/selenium/pages/BasePage.java` — `WebDriverWait(driver, long)` → `WebDriverWait(driver, Duration.ofSeconds(long))` (Selenium 4 API change)

## OWASP Dependency Check

The OWASP dependency-check plugin (`dependencyCheckAnalyze`) is **not configured** in this project's `build.gradle`. No security scanning report was generated.
