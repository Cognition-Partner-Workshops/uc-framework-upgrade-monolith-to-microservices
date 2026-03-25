# Business Requirements Document

## RealWorld (Conduit) Blogging Platform

---

## Table of Contents

1. [Overview](#1-overview)
2. [User Management Requirements](#2-user-management-requirements)
3. [Follow/Unfollow Requirements](#3-followunfollow-requirements)
4. [Article Management Requirements](#4-article-management-requirements)
5. [Tag Requirements](#5-tag-requirements)
6. [Comment Requirements](#6-comment-requirements)
7. [Favorite Requirements](#7-favorite-requirements)
8. [Authorization & Security Requirements](#8-authorization--security-requirements)
9. [Data Integrity Requirements](#9-data-integrity-requirements)
10. [Non-Functional Requirements](#10-non-functional-requirements)

---

## 1. Overview

This application is a **Medium.com clone** (known as "Conduit") — a full-featured blogging platform built as a monolith using **Spring Boot** and **MyBatis**. It provides both **REST** and **GraphQL** APIs for a frontend to consume.

The platform allows users to:
- Register and authenticate via JWT tokens
- Create, read, update, and delete articles with tags
- Comment on articles
- Follow other users and view a personalized feed
- Favorite articles

The codebase follows **Domain-Driven Design (DDD)** principles with a clear separation between the web layer (`api`/`graphql`), business domain (`core`), application services (`application`), and infrastructure (`infrastructure`). It employs the **CQRS (Command Query Responsibility Segregation)** pattern to separate read and write models.

**Key source references:**
- `README.md`
- `src/main/java/io/spring/` (all packages)

---

## 2. User Management Requirements

### Registration

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| UR-001 | The system shall allow new users to register by providing an email, username, and password. | `src/main/java/io/spring/api/UsersApi.java` (line 39-45), `src/main/java/io/spring/application/user/RegisterParam.java` |
| UR-002 | Email is required and must not be blank. | `src/main/java/io/spring/application/user/RegisterParam.java` (line 15: `@NotBlank`) |
| UR-003 | Email must be a valid email format. | `src/main/java/io/spring/application/user/RegisterParam.java` (line 16: `@Email`) |
| UR-004 | Email must be unique across all users (no duplicate emails allowed during registration). | `src/main/java/io/spring/application/user/RegisterParam.java` (line 17: `@DuplicatedEmailConstraint`), `src/main/java/io/spring/application/user/DuplicatedEmailValidator.java` |
| UR-005 | Username is required and must not be blank. | `src/main/java/io/spring/application/user/RegisterParam.java` (line 20: `@NotBlank`) |
| UR-006 | Username must be unique across all users (no duplicate usernames allowed during registration). | `src/main/java/io/spring/application/user/RegisterParam.java` (line 21: `@DuplicatedUsernameConstraint`), `src/main/java/io/spring/application/user/DuplicatedUsernameValidator.java` |
| UR-007 | Password is required and must not be blank. | `src/main/java/io/spring/application/user/RegisterParam.java` (line 24: `@NotBlank`) |
| UR-008 | Passwords must be encrypted using BCrypt before storage. | `src/main/java/io/spring/application/user/UserService.java` (line 39: `passwordEncoder.encode()`), `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 31-33: `BCryptPasswordEncoder`) |
| UR-009 | Upon successful registration, a new user is assigned a UUID-based unique identifier. | `src/main/java/io/spring/core/user/User.java` (line 21: `UUID.randomUUID()`) |
| UR-010 | Upon successful registration, the system shall assign a default profile image to the user. The default image URL is configured via `image.default` property. | `src/main/java/io/spring/application/user/UserService.java` (line 27: `@Value("${image.default}")`), `src/main/resources/application.properties` (line 7: `image.default=https://static.productionready.io/images/smiley-cyrus.jpg`) |
| UR-011 | Upon successful registration, the user's bio is initialized as an empty string. | `src/main/java/io/spring/application/user/UserService.java` (line 40) |
| UR-012 | Upon successful registration, the system shall return the user data along with a JWT token. | `src/main/java/io/spring/api/UsersApi.java` (line 42-44) |
| UR-013 | The registration endpoint shall be `POST /users`. | `src/main/java/io/spring/api/UsersApi.java` (line 39) |
| UR-014 | Registration is also available via GraphQL mutation `createUser`. | `src/main/java/io/spring/graphql/UserMutation.java` (line 36-53) |

### Authentication (Login)

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| UR-015 | The system shall allow users to log in with email and password. | `src/main/java/io/spring/api/UsersApi.java` (line 47-58) |
| UR-016 | Login email is required, must not be blank, and must be a valid email format. | `src/main/java/io/spring/api/UsersApi.java` (line 73-74: `@NotBlank`, `@Email` on `LoginParam`) |
| UR-017 | Login password is required and must not be blank. | `src/main/java/io/spring/api/UsersApi.java` (line 77: `@NotBlank` on `LoginParam`) |
| UR-018 | The system shall verify the password against the stored BCrypt hash. | `src/main/java/io/spring/api/UsersApi.java` (line 51: `passwordEncoder.matches()`) |
| UR-019 | Upon successful login, the system shall return the user data along with a JWT token. | `src/main/java/io/spring/api/UsersApi.java` (line 52-54) |
| UR-020 | If authentication fails (wrong email or password), the system shall throw an `InvalidAuthenticationException`. | `src/main/java/io/spring/api/UsersApi.java` (line 56) |
| UR-021 | The login endpoint shall be `POST /users/login`. | `src/main/java/io/spring/api/UsersApi.java` (line 47) |
| UR-022 | Login is also available via GraphQL mutation `login`. | `src/main/java/io/spring/graphql/UserMutation.java` (line 55-67) |

### Get Current User

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| UR-023 | The system shall allow authenticated users to retrieve their own profile information. | `src/main/java/io/spring/api/CurrentUserApi.java` (line 31-38) |
| UR-024 | The response shall include email, username, bio, image, and the current JWT token. | `src/main/java/io/spring/application/data/UserWithToken.java`, `src/main/java/io/spring/api/CurrentUserApi.java` (line 36-37) |
| UR-025 | The get current user endpoint shall be `GET /user` (requires authentication). | `src/main/java/io/spring/api/CurrentUserApi.java` (line 24, 31) |
| UR-026 | Current user information is also available via GraphQL query `me`. | `src/main/java/io/spring/graphql/MeDatafetcher.java` (line 27-47) |

### Update User Profile

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| UR-027 | The system shall allow authenticated users to update their own profile (email, username, password, bio, image). | `src/main/java/io/spring/api/CurrentUserApi.java` (line 40-49) |
| UR-028 | All update fields are optional; only non-empty fields are applied (partial update). | `src/main/java/io/spring/core/user/User.java` (line 29-49: `update()` method checks `Util.isEmpty()`), `src/main/java/io/spring/application/user/UpdateUserParam.java` (defaults to empty string) |
| UR-029 | If email is provided during update, it must be a valid email format. | `src/main/java/io/spring/application/user/UpdateUserParam.java` (line 18: `@Email`) |
| UR-030 | If email is changed to one already used by another user, validation fails with "email already exist". | `src/main/java/io/spring/application/user/UserService.java` (line 80-81, 91-95: `UpdateUserValidator`) |
| UR-031 | If username is changed to one already used by another user, validation fails with "username already exist". | `src/main/java/io/spring/application/user/UserService.java` (line 82-86, 97-101: `UpdateUserValidator`) |
| UR-032 | The update user endpoint shall be `PUT /user` (requires authentication). | `src/main/java/io/spring/api/CurrentUserApi.java` (line 24, 40) |
| UR-033 | User profile update is also available via GraphQL mutation `updateUser`. | `src/main/java/io/spring/graphql/UserMutation.java` (line 69-92) |

---

## 3. Follow/Unfollow Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| FR-001 | The system shall allow an authenticated user to follow another user by username. | `src/main/java/io/spring/api/ProfileApi.java` (line 37-49) |
| FR-002 | A follow relationship is stored as a directed relation from the follower's user ID to the target's user ID. | `src/main/java/io/spring/core/user/FollowRelation.java`, `src/main/java/io/spring/api/ProfileApi.java` (line 44) |
| FR-003 | The follow endpoint shall be `POST /profiles/{username}/follow` (requires authentication). | `src/main/java/io/spring/api/ProfileApi.java` (line 22, 37) |
| FR-004 | The system shall allow an authenticated user to unfollow a previously followed user. | `src/main/java/io/spring/api/ProfileApi.java` (line 51-68) |
| FR-005 | Unfollowing requires that a follow relationship exists; otherwise, a `ResourceNotFoundException` is thrown. | `src/main/java/io/spring/api/ProfileApi.java` (line 57-64) |
| FR-006 | The unfollow endpoint shall be `DELETE /profiles/{username}/follow` (requires authentication). | `src/main/java/io/spring/api/ProfileApi.java` (line 22, 51) |
| FR-007 | The system shall allow any user (authenticated or not) to view a user's profile by username. | `src/main/java/io/spring/api/ProfileApi.java` (line 28-35) |
| FR-008 | The profile response shall include username, bio, image, and a `following` flag indicating whether the current authenticated user follows the profile owner. | `src/main/java/io/spring/application/data/ProfileData.java`, `src/main/java/io/spring/application/ProfileQueryService.java` (line 23-31) |
| FR-009 | The get profile endpoint shall be `GET /profiles/{username}`. | `src/main/java/io/spring/api/ProfileApi.java` (line 22, 28) |
| FR-010 | If the requested profile username does not exist, the system shall return a `ResourceNotFoundException`. | `src/main/java/io/spring/api/ProfileApi.java` (line 34) |
| FR-011 | Follow is also available via GraphQL mutation `followUser`. | `src/main/java/io/spring/graphql/RelationMutation.java` (line 25-38) |
| FR-012 | Unfollow is also available via GraphQL mutation `unfollowUser`. | `src/main/java/io/spring/graphql/RelationMutation.java` (line 40-54) |
| FR-013 | Profile query is also available via GraphQL query `profile`. | `src/main/java/io/spring/graphql/ProfileDatafetcher.java` (line 51-56) |

---

## 4. Article Management Requirements

### Create Article

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-001 | The system shall allow authenticated users to create articles. | `src/main/java/io/spring/api/ArticlesApi.java` (line 28-38) |
| AR-002 | Article title is required and must not be blank. | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 17: `@NotBlank`) |
| AR-003 | Article title must be unique (validated by checking if the generated slug already exists). | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 18: `@DuplicatedArticleConstraint`), `src/main/java/io/spring/application/article/DuplicatedArticleValidator.java` |
| AR-004 | Article description is required and must not be blank. | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 21: `@NotBlank`) |
| AR-005 | Article body is required and must not be blank. | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 24: `@NotBlank`) |
| AR-006 | An article may optionally include a list of tags. | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 27: `tagList` with no validation) |
| AR-007 | Duplicate tags in the tag list are automatically deduplicated (using a `HashSet`). | `src/main/java/io/spring/core/article/Article.java` (line 45: `new HashSet<>(tagList)`) |
| AR-008 | A URL-friendly slug is automatically generated from the article title by converting to lowercase and replacing special characters with hyphens. | `src/main/java/io/spring/core/article/Article.java` (line 41, 67-69: `toSlug()`) |
| AR-009 | Each article is assigned a UUID-based unique identifier. | `src/main/java/io/spring/core/article/Article.java` (line 40: `UUID.randomUUID()`) |
| AR-010 | The article's `createdAt` and `updatedAt` timestamps are set upon creation. | `src/main/java/io/spring/core/article/Article.java` (line 47-48) |
| AR-011 | The create article endpoint shall be `POST /articles` (requires authentication). | `src/main/java/io/spring/api/ArticlesApi.java` (line 22, 28) |
| AR-012 | Article creation is also available via GraphQL mutation `createArticle`. | `src/main/java/io/spring/graphql/ArticleMutation.java` (line 35-51) |

### Read / Get Article

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-013 | The system shall allow any user (authenticated or not) to retrieve a single article by its slug. | `src/main/java/io/spring/api/ArticleApi.java` (line 35-42) |
| AR-014 | If the article slug is not found, the system shall return a `ResourceNotFoundException`. | `src/main/java/io/spring/api/ArticleApi.java` (line 41) |
| AR-015 | The article response includes: id, slug, title, description, body, tag list, createdAt, updatedAt, favorited status, favorites count, and author profile (with following status). | `src/main/java/io/spring/application/data/ArticleData.java`, `src/main/java/io/spring/application/ArticleQueryService.java` (line 175-183: `fillExtraInfo`) |
| AR-016 | If the requester is authenticated, the response enriches the article with: whether the user has favorited it, and whether the user follows the author. | `src/main/java/io/spring/application/ArticleQueryService.java` (line 35-37, 125-131) |
| AR-017 | The get article endpoint shall be `GET /articles/{slug}`. | `src/main/java/io/spring/api/ArticleApi.java` (line 28, 35) |
| AR-018 | Single article query is also available via GraphQL query `article(slug)`. | `src/main/java/io/spring/graphql/ArticleDatafetcher.java` (line 342-357) |

### Update Article

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-019 | The system shall allow the article author to update their own article. | `src/main/java/io/spring/api/ArticleApi.java` (line 44-63) |
| AR-020 | Only the article author can update the article; otherwise a `NoAuthorizationException` is thrown. | `src/main/java/io/spring/api/ArticleApi.java` (line 53-55), `src/main/java/io/spring/core/service/AuthorizationService.java` (line 8-10) |
| AR-021 | Article update supports partial updates: title, description, and body are all optional. Only non-empty fields are applied. | `src/main/java/io/spring/application/article/UpdateArticleParam.java` (defaults to empty string), `src/main/java/io/spring/core/article/Article.java` (line 51-65: `update()` checks `Util.isEmpty()`) |
| AR-022 | If the title is updated, the slug is automatically regenerated from the new title. | `src/main/java/io/spring/core/article/Article.java` (line 53-54) |
| AR-023 | When any field is updated, the `updatedAt` timestamp is refreshed. | `src/main/java/io/spring/core/article/Article.java` (line 55, 59, 63) |
| AR-024 | The update article endpoint shall be `PUT /articles/{slug}` (requires authentication). | `src/main/java/io/spring/api/ArticleApi.java` (line 28, 44) |
| AR-025 | Article update is also available via GraphQL mutation `updateArticle`. | `src/main/java/io/spring/graphql/ArticleMutation.java` (line 53-70) |

### Delete Article

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-026 | The system shall allow the article author to delete their own article. | `src/main/java/io/spring/api/ArticleApi.java` (line 65-79) |
| AR-027 | Only the article author can delete the article; otherwise a `NoAuthorizationException` is thrown. | `src/main/java/io/spring/api/ArticleApi.java` (line 72-74), `src/main/java/io/spring/core/service/AuthorizationService.java` (line 8-10) |
| AR-028 | Upon successful deletion, the system returns HTTP 204 No Content. | `src/main/java/io/spring/api/ArticleApi.java` (line 76) |
| AR-029 | The delete article endpoint shall be `DELETE /articles/{slug}` (requires authentication). | `src/main/java/io/spring/api/ArticleApi.java` (line 28, 65) |
| AR-030 | Article deletion is also available via GraphQL mutation `deleteArticle`, which returns a `DeletionStatus` with `success: true`. | `src/main/java/io/spring/graphql/ArticleMutation.java` (line 102-114) |

### List Articles

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-031 | The system shall allow any user to list articles with pagination (offset/limit). | `src/main/java/io/spring/api/ArticlesApi.java` (line 48-59) |
| AR-032 | Default pagination is offset=0, limit=20. | `src/main/java/io/spring/api/ArticlesApi.java` (line 50-51) |
| AR-033 | Maximum pagination limit is 100. | `src/main/java/io/spring/application/Page.java` (line 9: `MAX_LIMIT = 100`) |
| AR-034 | Articles can be filtered by tag name. | `src/main/java/io/spring/api/ArticlesApi.java` (line 52: `tag` param) |
| AR-035 | Articles can be filtered by author username. | `src/main/java/io/spring/api/ArticlesApi.java` (line 54: `author` param) |
| AR-036 | Articles can be filtered by username of a user who favorited them. | `src/main/java/io/spring/api/ArticlesApi.java` (line 53: `favorited` param) |
| AR-037 | The list articles response includes the articles and a total count (`articlesCount`). | `src/main/java/io/spring/application/data/ArticleDataList.java`, `src/main/java/io/spring/application/ArticleQueryService.java` (line 100-111) |
| AR-038 | The list articles endpoint shall be `GET /articles`. | `src/main/java/io/spring/api/ArticlesApi.java` (line 22, 48) |
| AR-039 | Article listing is also available via GraphQL query `articles` with cursor-based pagination (first/after, last/before) and filters (authoredBy, favoritedBy, withTag). | `src/main/java/io/spring/graphql/ArticleDatafetcher.java` (line 245-298), `src/main/resources/schema/schema.graphqls` (line 4-12) |

### Article Feed

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| AR-040 | The system shall provide a personalized feed of articles from users that the authenticated user follows. | `src/main/java/io/spring/api/ArticlesApi.java` (line 40-46) |
| AR-041 | The feed supports pagination (offset/limit, default offset=0, limit=20). | `src/main/java/io/spring/api/ArticlesApi.java` (line 42-43) |
| AR-042 | If the user follows no one, the feed returns an empty list with count 0. | `src/main/java/io/spring/application/ArticleQueryService.java` (line 115-116) |
| AR-043 | The feed endpoint shall be `GET /articles/feed` (requires authentication). | `src/main/java/io/spring/api/ArticlesApi.java` (line 40), `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 55-56) |
| AR-044 | Article feed is also available via GraphQL query `feed` with cursor-based pagination. | `src/main/java/io/spring/graphql/ArticleDatafetcher.java` (line 42-86), `src/main/resources/schema/schema.graphqls` (line 14) |

---

## 5. Tag Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| TR-001 | The system shall allow articles to be tagged with one or more string tags at creation time. | `src/main/java/io/spring/application/article/NewArticleParam.java` (line 27), `src/main/java/io/spring/core/article/Article.java` (line 45) |
| TR-002 | Each tag is a named entity with a UUID-based unique identifier. | `src/main/java/io/spring/core/article/Tag.java` (line 15-17) |
| TR-003 | Tags are associated with articles through a many-to-many relationship (via `article_tags` join table). | `src/main/resources/db/migration/V1__create_tables.sql` (line 37-40) |
| TR-004 | The system shall provide an endpoint to list all existing tags. | `src/main/java/io/spring/api/TagsApi.java` (line 17-25) |
| TR-005 | The list tags endpoint shall be `GET /tags` (public, no authentication required). | `src/main/java/io/spring/api/TagsApi.java` (line 12, 17), `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 59-60) |
| TR-006 | Tag listing is also available via GraphQL query `tags`. | `src/main/java/io/spring/graphql/TagDatafetcher.java` (line 15-18), `src/main/resources/schema/schema.graphqls` (line 16) |
| TR-007 | Tags are equality-compared by name (not by ID). | `src/main/java/io/spring/core/article/Tag.java` (line 10: `@EqualsAndHashCode(of = "name")`) |

---

## 6. Comment Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| CR-001 | The system shall allow authenticated users to add a comment to an article (identified by slug). | `src/main/java/io/spring/api/CommentsApi.java` (line 40-51) |
| CR-002 | Comment body is required and must not be blank. | `src/main/java/io/spring/api/CommentsApi.java` (line 100: `@NotBlank` on `NewCommentParam`) |
| CR-003 | Each comment is assigned a UUID-based unique identifier. | `src/main/java/io/spring/core/comment/Comment.java` (line 20: `UUID.randomUUID()`) |
| CR-004 | Each comment records the user ID of the author and the article ID it belongs to. | `src/main/java/io/spring/core/comment/Comment.java` (line 15-16) |
| CR-005 | Each comment has a `createdAt` timestamp set upon creation. | `src/main/java/io/spring/core/comment/Comment.java` (line 24: `new DateTime()`) |
| CR-006 | Upon successful creation, the system returns HTTP 201 with the comment data. | `src/main/java/io/spring/api/CommentsApi.java` (line 49) |
| CR-007 | The create comment endpoint shall be `POST /articles/{slug}/comments` (requires authentication). | `src/main/java/io/spring/api/CommentsApi.java` (line 33, 40) |
| CR-008 | The system shall allow any user (authenticated or not) to list all comments on an article. | `src/main/java/io/spring/api/CommentsApi.java` (line 53-65) |
| CR-009 | The list comments endpoint shall be `GET /articles/{slug}/comments`. | `src/main/java/io/spring/api/CommentsApi.java` (line 33, 53) |
| CR-010 | The system shall allow authorized users to delete a comment. | `src/main/java/io/spring/api/CommentsApi.java` (line 67-85) |
| CR-011 | A comment can be deleted by either the **article author** or the **comment author**. | `src/main/java/io/spring/core/service/AuthorizationService.java` (line 12-14: `canWriteComment` checks both `article.getUserId()` and `comment.getUserId()`) |
| CR-012 | If a non-authorized user attempts to delete a comment, a `NoAuthorizationException` is thrown. | `src/main/java/io/spring/api/CommentsApi.java` (line 78-80) |
| CR-013 | Upon successful deletion, the system returns HTTP 204 No Content. | `src/main/java/io/spring/api/CommentsApi.java` (line 82) |
| CR-014 | The delete comment endpoint shall be `DELETE /articles/{slug}/comments/{id}` (requires authentication). | `src/main/java/io/spring/api/CommentsApi.java` (line 33, 67) |
| CR-015 | Comment creation is also available via GraphQL mutation `addComment`. | `src/main/java/io/spring/graphql/CommentMutation.java` (line 31-47) |
| CR-016 | Comment deletion is also available via GraphQL mutation `deleteComment`, returning a `DeletionStatus`. | `src/main/java/io/spring/graphql/CommentMutation.java` (line 49-67) |
| CR-017 | Comments on an article are also available via the GraphQL `Article.comments` field with cursor-based pagination. | `src/main/java/io/spring/graphql/CommentDatafetcher.java` (line 50-100), `src/main/resources/schema/schema.graphqls` (line 50) |

---

## 7. Favorite Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| FV-001 | The system shall allow authenticated users to favorite an article (identified by slug). | `src/main/java/io/spring/api/ArticleFavoriteApi.java` (line 29-37) |
| FV-002 | A favorite is a relationship between a user ID and an article ID. | `src/main/java/io/spring/core/favorite/ArticleFavorite.java` (line 11-12) |
| FV-003 | The favorite article endpoint shall be `POST /articles/{slug}/favorite` (requires authentication). | `src/main/java/io/spring/api/ArticleFavoriteApi.java` (line 22, 29) |
| FV-004 | The system shall allow authenticated users to unfavorite a previously favorited article. | `src/main/java/io/spring/api/ArticleFavoriteApi.java` (line 39-51) |
| FV-005 | Unfavoriting is idempotent: if the favorite does not exist, no error is thrown (uses `ifPresent`). | `src/main/java/io/spring/api/ArticleFavoriteApi.java` (line 44-49) |
| FV-006 | The unfavorite article endpoint shall be `DELETE /articles/{slug}/favorite` (requires authentication). | `src/main/java/io/spring/api/ArticleFavoriteApi.java` (line 22, 39) |
| FV-007 | Each article response includes a `favoritesCount` indicating the total number of users who favorited the article. | `src/main/java/io/spring/application/data/ArticleData.java` (line 21), `src/main/java/io/spring/application/ArticleQueryService.java` (line 148-159, 177) |
| FV-008 | Each article response includes a `favorited` boolean indicating whether the currently authenticated user has favorited the article. | `src/main/java/io/spring/application/data/ArticleData.java` (line 20), `src/main/java/io/spring/application/ArticleQueryService.java` (line 161-173, 176) |
| FV-009 | Articles can be filtered by the username of a user who favorited them (via the `favorited` query parameter on the list articles endpoint). | `src/main/java/io/spring/api/ArticlesApi.java` (line 53) |
| FV-010 | Favoriting is also available via GraphQL mutation `favoriteArticle`. | `src/main/java/io/spring/graphql/ArticleMutation.java` (line 72-83) |
| FV-011 | Unfavoriting is also available via GraphQL mutation `unfavoriteArticle`. | `src/main/java/io/spring/graphql/ArticleMutation.java` (line 85-100) |

---

## 8. Authorization & Security Requirements

### Authentication Mechanism

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| SC-001 | The system uses **JWT (JSON Web Token)** based authentication. Tokens are stateless — no server-side sessions are maintained. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 45-46: `SessionCreationPolicy.STATELESS`), `src/main/java/io/spring/core/service/JwtService.java` |
| SC-002 | JWT tokens are passed via the `Authorization` HTTP header using the `Token` prefix (e.g., `Authorization: Token <jwt>`). | `src/main/java/io/spring/api/security/JwtTokenFilter.java` (line 22, 28, 50-61) |
| SC-003 | The JWT secret key is configured via `jwt.secret` property. | `src/main/resources/application.properties` (line 9) |
| SC-004 | The JWT session time (token validity duration) is configured via `jwt.sessionTime` property (default: 86400 seconds = 24 hours). | `src/main/resources/application.properties` (line 10) |
| SC-005 | The `JwtTokenFilter` extracts the JWT from the Authorization header, validates it, retrieves the user by ID from the token subject, and sets the Spring Security authentication context. | `src/main/java/io/spring/api/security/JwtTokenFilter.java` (line 24-48) |
| SC-006 | Passwords are hashed using **BCrypt** algorithm. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 31-33) |

### Public vs Protected Endpoints

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| SC-007 | The following endpoints are **public** (no authentication required): `POST /users` (register), `POST /users/login` (login). | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 57-58) |
| SC-008 | The following endpoints are **public** for GET requests: `GET /articles/**`, `GET /profiles/**`, `GET /tags`. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 59-60) |
| SC-009 | The feed endpoint `GET /articles/feed` **requires authentication** (explicitly marked as `.authenticated()`). | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 55-56) |
| SC-010 | All other endpoints **require authentication**. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 61-62: `.anyRequest().authenticated()`) |
| SC-011 | GraphQL endpoints (`/graphql`, `/graphiql`) are publicly accessible at the HTTP level; authentication is handled within resolvers via `SecurityUtil.getCurrentUser()`. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 51-54), `src/main/java/io/spring/graphql/SecurityUtil.java` |
| SC-012 | OPTIONS requests are permitted for all endpoints (CORS preflight support). | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 49-50) |
| SC-013 | Unauthenticated access to protected endpoints returns **HTTP 401 Unauthorized**. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 43: `HttpStatusEntryPoint(HttpStatus.UNAUTHORIZED)`) |

### Ownership-Based Authorization

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| SC-014 | Only the **article author** (user whose ID matches `article.userId`) can update or delete an article. | `src/main/java/io/spring/core/service/AuthorizationService.java` (line 8-10: `canWriteArticle`) |
| SC-015 | A comment can be deleted by the **article author** OR the **comment author**. | `src/main/java/io/spring/core/service/AuthorizationService.java` (line 12-14: `canWriteComment`) |
| SC-016 | Unauthorized write operations throw `NoAuthorizationException`. | `src/main/java/io/spring/api/ArticleApi.java` (line 54), `src/main/java/io/spring/api/CommentsApi.java` (line 79) |

### CORS Configuration

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| SC-017 | CORS is enabled for all origins (`*`). | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 70) |
| SC-018 | Allowed HTTP methods: HEAD, GET, POST, PUT, DELETE, PATCH. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 71) |
| SC-019 | Allowed headers: Authorization, Cache-Control, Content-Type. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 78) |
| SC-020 | CSRF protection is disabled (stateless API with JWT auth). | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 38-39) |

---

## 9. Data Integrity Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| DI-001 | **User ID** must be unique (primary key). | `src/main/resources/db/migration/V1__create_tables.sql` (line 2) |
| DI-002 | **Username** must be unique across all users. | `src/main/resources/db/migration/V1__create_tables.sql` (line 3: `UNIQUE`), `src/main/java/io/spring/application/user/DuplicatedUsernameValidator.java` |
| DI-003 | **Email** must be unique across all users. | `src/main/resources/db/migration/V1__create_tables.sql` (line 5: `UNIQUE`), `src/main/java/io/spring/application/user/DuplicatedEmailValidator.java` |
| DI-004 | **Article ID** must be unique (primary key). | `src/main/resources/db/migration/V1__create_tables.sql` (line 11) |
| DI-005 | **Article slug** must be unique across all articles. | `src/main/resources/db/migration/V1__create_tables.sql` (line 13: `UNIQUE`), `src/main/java/io/spring/application/article/DuplicatedArticleValidator.java` |
| DI-006 | **Article favorites** use a composite primary key of `(article_id, user_id)`, ensuring a user can only favorite an article once. | `src/main/resources/db/migration/V1__create_tables.sql` (line 24: `primary key(article_id, user_id)`) |
| DI-007 | **Comment ID** must be unique (primary key). | `src/main/resources/db/migration/V1__create_tables.sql` (line 43) |
| DI-008 | **Tag ID** must be unique (primary key). | `src/main/resources/db/migration/V1__create_tables.sql` (line 33) |
| DI-009 | **Tag name** is required (NOT NULL). | `src/main/resources/db/migration/V1__create_tables.sql` (line 34) |
| DI-010 | Articles require **`created_at`** and **`updated_at`** timestamps (NOT NULL). `updated_at` defaults to `CURRENT_TIMESTAMP`. | `src/main/resources/db/migration/V1__create_tables.sql` (line 17-18) |
| DI-011 | Comments require **`created_at`** and **`updated_at`** timestamps (NOT NULL). `updated_at` defaults to `CURRENT_TIMESTAMP`. | `src/main/resources/db/migration/V1__create_tables.sql` (line 47-48) |
| DI-012 | The `article_favorites` table requires both `article_id` and `user_id` to be NOT NULL. | `src/main/resources/db/migration/V1__create_tables.sql` (line 22-23) |
| DI-013 | The `follows` table requires both `user_id` and `follow_id` to be NOT NULL. | `src/main/resources/db/migration/V1__create_tables.sql` (line 28-29) |
| DI-014 | The `article_tags` join table requires both `article_id` and `tag_id` to be NOT NULL. | `src/main/resources/db/migration/V1__create_tables.sql` (line 38-39) |
| DI-015 | All entity IDs (users, articles, comments, tags) are generated as UUIDs stored as `varchar(255)`. | `src/main/java/io/spring/core/user/User.java` (line 21), `src/main/java/io/spring/core/article/Article.java` (line 40), `src/main/java/io/spring/core/comment/Comment.java` (line 20), `src/main/java/io/spring/core/article/Tag.java` (line 16) |
| DI-016 | Registration validation: email must not already exist in the database. | `src/main/java/io/spring/application/user/DuplicatedEmailValidator.java` (line 14-16) |
| DI-017 | Registration validation: username must not already exist in the database. | `src/main/java/io/spring/application/user/DuplicatedUsernameValidator.java` (line 14-16) |
| DI-018 | Article creation validation: the slug derived from the title must not already exist. | `src/main/java/io/spring/application/article/DuplicatedArticleValidator.java` (line 14-17) |
| DI-019 | User update validation: if email is changed, it must not conflict with another user's email. | `src/main/java/io/spring/application/user/UserService.java` (line 80-81, 91-95) |
| DI-020 | User update validation: if username is changed, it must not conflict with another user's username. | `src/main/java/io/spring/application/user/UserService.java` (line 82-86, 97-101) |

---

## 10. Non-Functional Requirements

| Req ID | Requirement | Source File(s) |
|--------|-------------|----------------|
| NF-001 | **CQRS Pattern**: The application separates read (query) and write (command) models. Command services (`ArticleCommandService`, `UserService`) handle writes, while query services (`ArticleQueryService`, `CommentQueryService`, `ProfileQueryService`, `TagsQueryService`, `UserQueryService`) handle reads with dedicated read-optimized data models. | `src/main/java/io/spring/application/article/ArticleCommandService.java`, `src/main/java/io/spring/application/ArticleQueryService.java`, `src/main/java/io/spring/application/CommentQueryService.java` |
| NF-002 | **Domain-Driven Design (DDD)**: The codebase separates concerns into layers — `core` (domain entities and services), `application` (use cases/services), `api` (REST controllers), `graphql` (GraphQL resolvers), and `infrastructure` (persistence/technical details). | `README.md` (line 28-34), package structure |
| NF-003 | **Dual API Support**: The application exposes both REST API and GraphQL API simultaneously, sharing the same domain and application layers. | `README.md` (line 13), `src/main/java/io/spring/api/` (REST), `src/main/java/io/spring/graphql/` (GraphQL) |
| NF-004 | **REST Pagination**: REST endpoints support offset/limit based pagination with configurable defaults (offset=0, limit=20) and a maximum limit of 100. | `src/main/java/io/spring/application/Page.java` (line 9-11) |
| NF-005 | **GraphQL Cursor-Based Pagination**: GraphQL queries implement Relay-style cursor-based pagination with `first`/`after` (forward) and `last`/`before` (backward) parameters, returning `Connection`/`Edge`/`PageInfo` types. | `src/main/java/io/spring/graphql/ArticleDatafetcher.java`, `src/main/java/io/spring/graphql/CommentDatafetcher.java`, `src/main/resources/schema/schema.graphqls` (line 61-69, 81-89, 95-100) |
| NF-006 | **Data Mapper Pattern**: MyBatis is used as the persistence framework implementing the Data Mapper pattern, with XML-based mapper configurations. | `README.md` (line 25), `src/main/resources/application.properties` (line 17: `mybatis.mapper-locations`) |
| NF-007 | **SQLite Database**: The application uses SQLite for persistence (configurable to other databases). | `src/main/resources/application.properties` (line 1: `jdbc:sqlite:dev.db`) |
| NF-008 | **Database Migrations**: Flyway is used for database schema versioning and migrations. | `src/main/resources/db/migration/V1__create_tables.sql`, `src/main/resources/db/migration/V2__seed_data.sql` |
| NF-009 | **Stateless Architecture**: The application is fully stateless (no server-side sessions), enabling horizontal scalability. | `src/main/java/io/spring/api/security/WebSecurityConfig.java` (line 46: `SessionCreationPolicy.STATELESS`) |
| NF-010 | **MyBatis Caching**: MyBatis caching is enabled with a default statement timeout of 3000ms. | `src/main/resources/application.properties` (line 12-13) |
| NF-011 | **JSON Root Wrapping**: Jackson is configured to unwrap root values in request deserialization (e.g., `{"user": {...}}`). | `src/main/resources/application.properties` (line 5: `UNWRAP_ROOT_VALUE=true`) |
| NF-012 | **DGS Framework**: GraphQL support is implemented using Netflix's DGS (Domain Graph Service) framework. | `src/main/java/io/spring/graphql/ArticleDatafetcher.java` (line 3: `com.netflix.graphql.dgs`), `README.md` (line 19) |
| NF-013 | **Bean Validation**: The application uses JSR 380 Bean Validation (`javax.validation`) annotations for input validation, with custom constraint validators for business rules (duplicate email, duplicate username, duplicate article). | `src/main/java/io/spring/application/user/RegisterParam.java`, `src/main/java/io/spring/application/article/NewArticleParam.java` |
| NF-014 | **Seed Data**: The application ships with sample seed data including 3 users, 5 articles, 7 tags, 5 comments, 6 favorites, and 4 follow relationships for development and testing. | `src/main/resources/db/migration/V2__seed_data.sql`, `README.md` (line 55-61) |
| NF-015 | **GraphQL Profile Sub-Queries**: Via GraphQL, a user's profile can be queried to retrieve their authored articles, favorited articles, and feed — all with cursor-based pagination. | `src/main/java/io/spring/graphql/ArticleDatafetcher.java` (line 88-243), `src/main/resources/schema/schema.graphqls` (line 108-110) |

---

## Appendix: API Endpoint Summary

### REST API Endpoints

| Method | Path | Auth Required | Description | Controller |
|--------|------|---------------|-------------|------------|
| POST | `/users` | No | Register a new user | `UsersApi` |
| POST | `/users/login` | No | Login / authenticate | `UsersApi` |
| GET | `/user` | Yes | Get current user | `CurrentUserApi` |
| PUT | `/user` | Yes | Update current user | `CurrentUserApi` |
| GET | `/profiles/{username}` | No | Get user profile | `ProfileApi` |
| POST | `/profiles/{username}/follow` | Yes | Follow a user | `ProfileApi` |
| DELETE | `/profiles/{username}/follow` | Yes | Unfollow a user | `ProfileApi` |
| POST | `/articles` | Yes | Create article | `ArticlesApi` |
| GET | `/articles` | No | List/filter articles | `ArticlesApi` |
| GET | `/articles/feed` | Yes | Get user feed | `ArticlesApi` |
| GET | `/articles/{slug}` | No | Get article by slug | `ArticleApi` |
| PUT | `/articles/{slug}` | Yes | Update article | `ArticleApi` |
| DELETE | `/articles/{slug}` | Yes | Delete article | `ArticleApi` |
| POST | `/articles/{slug}/comments` | Yes | Add comment | `CommentsApi` |
| GET | `/articles/{slug}/comments` | No | List comments | `CommentsApi` |
| DELETE | `/articles/{slug}/comments/{id}` | Yes | Delete comment | `CommentsApi` |
| POST | `/articles/{slug}/favorite` | Yes | Favorite article | `ArticleFavoriteApi` |
| DELETE | `/articles/{slug}/favorite` | Yes | Unfavorite article | `ArticleFavoriteApi` |
| GET | `/tags` | No | List all tags | `TagsApi` |

### GraphQL Operations

| Type | Operation | Auth Required | Description | Datafetcher/Mutation |
|------|-----------|---------------|-------------|----------------------|
| Query | `article(slug)` | No | Get single article | `ArticleDatafetcher` |
| Query | `articles(...)` | No | List/filter articles (cursor pagination) | `ArticleDatafetcher` |
| Query | `feed(...)` | Yes | Get user feed (cursor pagination) | `ArticleDatafetcher` |
| Query | `me` | Yes | Get current user | `MeDatafetcher` |
| Query | `profile(username)` | No | Get user profile | `ProfileDatafetcher` |
| Query | `tags` | No | List all tags | `TagDatafetcher` |
| Mutation | `createUser(input)` | No | Register user | `UserMutation` |
| Mutation | `login(email, password)` | No | Authenticate user | `UserMutation` |
| Mutation | `updateUser(changes)` | Yes | Update current user | `UserMutation` |
| Mutation | `followUser(username)` | Yes | Follow user | `RelationMutation` |
| Mutation | `unfollowUser(username)` | Yes | Unfollow user | `RelationMutation` |
| Mutation | `createArticle(input)` | Yes | Create article | `ArticleMutation` |
| Mutation | `updateArticle(slug, changes)` | Yes | Update article | `ArticleMutation` |
| Mutation | `deleteArticle(slug)` | Yes | Delete article | `ArticleMutation` |
| Mutation | `favoriteArticle(slug)` | Yes | Favorite article | `ArticleMutation` |
| Mutation | `unfavoriteArticle(slug)` | Yes | Unfavorite article | `ArticleMutation` |
| Mutation | `addComment(slug, body)` | Yes | Add comment | `CommentMutation` |
| Mutation | `deleteComment(slug, id)` | Yes | Delete comment | `CommentMutation` |

---

## Appendix: Database Schema Summary

| Table | Primary Key | Unique Constraints | Foreign Key References |
|-------|------------|-------------------|----------------------|
| `users` | `id` | `username`, `email` | — |
| `articles` | `id` | `slug` | `user_id` -> `users.id` |
| `article_favorites` | `(article_id, user_id)` | — | `article_id` -> `articles.id`, `user_id` -> `users.id` |
| `follows` | — | — | `user_id` -> `users.id`, `follow_id` -> `users.id` |
| `tags` | `id` | — | — |
| `article_tags` | — | — | `article_id` -> `articles.id`, `tag_id` -> `tags.id` |
| `comments` | `id` | — | `article_id` -> `articles.id`, `user_id` -> `users.id` |

> **Note:** Foreign key references are logical (enforced by application logic) rather than explicit SQL foreign key constraints in the schema.

**Source:** `src/main/resources/db/migration/V1__create_tables.sql`
