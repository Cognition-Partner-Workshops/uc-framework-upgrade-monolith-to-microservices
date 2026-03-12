# Testing Gaps: Scenarios Not Verifiable in This Environment

## 1. Selenium E2E Tests (Excluded from build)

**Location:** `src/test/java/io/spring/selenium/`  
**Status:** Excluded from `./gradlew test` via `exclude 'io/spring/selenium/**'`  
**Reason:** These tests require:
- A running application server (`bootRun`)
- A Chrome/Chromium browser with WebDriver
- Network access to the frontend application
- TestNG runner (separate `seleniumTest` task)

**Files affected:**
- `SeleniumSetupTest.java` — Verifies Selenium WebDriver setup
- `BaseTest.java` — Base test class with browser lifecycle management
- `BasePage.java` — Page object base (fixed `WebDriverWait` Duration API)
- `TestListener.java` — ExtentReports test listener

**Risk:** The `BasePage.java` change (`WebDriverWait` constructor from `long` to `Duration`) compiles correctly but has not been verified at runtime.

## 2. Integration Tests Requiring a Real Database

**Status:** All integration tests PASS using SQLite in-memory database (`jdbc:sqlite::memory:`)  
**What's NOT tested:**
- Connection pooling behavior differences between Spring Boot 2.x and 3.x HikariCP versions
- Database-specific SQL dialect issues if migrating to PostgreSQL/MySQL in production
- Flyway migration behavior with a persistent database (tests use `spring.flyway.target=1`)
- V2 seed data migration (`V2__seed_data.sql`) is skipped in test profile

## 3. Full Application Startup (bootRun)

**Status:** NOT verified  
**Reason:** `./gradlew bootRun` requires:
- A writable filesystem for `dev.db` SQLite database
- Available port 8080
- Full Flyway migration execution (V1 + V2)

**Risk:** While `RealworldApplicationTests.contextLoads()` passes (verifies Spring context loads), actual HTTP endpoint behavior has not been tested with a running server.

## 4. GraphQL Endpoint Runtime Behavior

**Status:** NOT verified at runtime  
**Reason:** The DGS GraphQL framework was upgraded from 4.9.21 (`graphql-dgs-spring-boot-starter`) to 8.5.0 (`graphql-dgs-spring-graphql-starter`), which is a major architectural change (DGS now integrates with Spring GraphQL instead of being standalone).

**What's NOT tested:**
- GraphQL query execution via `/graphql` endpoint
- GraphQL schema introspection via `/graphiql`
- Subscription support (if any)
- DataFetcher resolution at runtime
- Error handling through `GraphQLCustomizeExceptionHandler`
- `spring.graphql.schema.inspection.enabled=false` may mask schema-mapping issues

## 5. Security Configuration Runtime Behavior

**Status:** Partially tested via MockMvc in unit tests  
**What's NOT tested:**
- Actual CORS preflight request handling with real HTTP clients
- JWT token authentication end-to-end through the full filter chain
- `SecurityFilterChain` bean behavior differences from the removed `WebSecurityConfigurerAdapter`
- Session management (STATELESS) under concurrent load

## 6. Performance / Load Tests

**Status:** NOT performed  
**Reason:** No performance test suite exists in the project. The following should be benchmarked post-migration:
- Application startup time (Spring Boot 3.x may differ)
- Request throughput with the new Tomcat 10.1 embedded server
- Memory footprint differences
- GraphQL query performance with DGS 8.x + Spring GraphQL

## 7. Security Scanning

**Status:** NOT performed  
**Reason:**
- OWASP `dependencyCheckAnalyze` plugin is not configured in `build.gradle`
- No static analysis security testing (SAST) tools are configured
- Dependency vulnerability scanning was not performed

**Recommendation:** Add the OWASP dependency-check plugin and run `./gradlew dependencyCheckAnalyze` to identify known CVEs in updated dependencies.

## 8. Spotless Code Formatting

**Status:** Upgraded but verification skipped during build  
**Reason:** Spotless was upgraded from 6.2.1 to 6.25.0. The `spotlessCheck` task was excluded from the test build to avoid reformatting noise. The code compiles and tests pass, but formatting consistency has not been enforced.

**Recommendation:** Run `./gradlew spotlessApply` to reformat all code, then `./gradlew spotlessCheck` to verify.

## 9. JaCoCo Code Coverage Verification

**Status:** FAILS (pre-existing, not caused by migration)  
**Details:** Coverage ratio is 0.33 (33%) against a minimum threshold of 0.80 (80%). This failure existed before the migration and is not related to the upgrade.

## 10. No @Disabled Tests

No tests were marked as `@Disabled` during this migration. All 68 original tests continue to pass without modification.
