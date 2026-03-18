package io.spring.infrastructure.client;

import io.spring.application.data.UserData;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

@Component
public class UserServiceClient {

  private final RestTemplate restTemplate;
  private final String userAuthServiceUrl;

  public UserServiceClient(
      RestTemplate restTemplate,
      @Value("${user-auth-service.url}") String userAuthServiceUrl) {
    this.restTemplate = restTemplate;
    this.userAuthServiceUrl = userAuthServiceUrl;
  }

  public Optional<UserData> findUserById(String id) {
    try {
      ResponseEntity<UserData> response =
          restTemplate.getForEntity(
              userAuthServiceUrl + "/api/internal/users/{id}", UserData.class, id);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  public Optional<UserData> findUserByUsername(String username) {
    try {
      ResponseEntity<UserData> response =
          restTemplate.getForEntity(
              userAuthServiceUrl + "/api/internal/users/by-username/{username}",
              UserData.class,
              username);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  public boolean isUserFollowing(String userId, String targetId) {
    try {
      ResponseEntity<Map<String, Object>> response =
          restTemplate.exchange(
              userAuthServiceUrl + "/api/internal/users/{userId}/follows/{targetId}",
              HttpMethod.GET,
              null,
              new ParameterizedTypeReference<Map<String, Object>>() {},
              userId,
              targetId);
      Map<String, Object> body = response.getBody();
      return body != null && Boolean.TRUE.equals(body.get("following"));
    } catch (Exception e) {
      return false;
    }
  }

  @SuppressWarnings("unchecked")
  public Set<String> followingAuthors(String userId, List<String> authorIds) {
    try {
      Map<String, Object> request = new HashMap<>();
      request.put("userId", userId);
      request.put("targetIds", authorIds);
      ResponseEntity<Map<String, Object>> response =
          restTemplate.exchange(
              userAuthServiceUrl + "/api/internal/users/following",
              HttpMethod.POST,
              new org.springframework.http.HttpEntity<>(request),
              new ParameterizedTypeReference<Map<String, Object>>() {});
      Map<String, Object> body = response.getBody();
      if (body != null && body.get("followingIds") != null) {
        List<String> ids = (List<String>) body.get("followingIds");
        return new HashSet<>(ids);
      }
      return Collections.emptySet();
    } catch (Exception e) {
      return Collections.emptySet();
    }
  }

  @SuppressWarnings("unchecked")
  public List<String> followedUsers(String userId) {
    try {
      ResponseEntity<Map<String, Object>> response =
          restTemplate.exchange(
              userAuthServiceUrl + "/api/internal/users/{userId}/followed-users",
              HttpMethod.GET,
              null,
              new ParameterizedTypeReference<Map<String, Object>>() {},
              userId);
      Map<String, Object> body = response.getBody();
      if (body != null && body.get("followedUsers") != null) {
        return (List<String>) body.get("followedUsers");
      }
      return Collections.emptyList();
    } catch (Exception e) {
      return Collections.emptyList();
    }
  }
}
