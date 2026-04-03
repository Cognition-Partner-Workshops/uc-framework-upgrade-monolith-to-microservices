package io.spring.articleservice.infrastructure.client;

import io.spring.shared.data.ProfileData;
import io.spring.shared.data.UserData;
import java.util.List;
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
  private final String userServiceUrl;

  public UserServiceClient(
      RestTemplate restTemplate,
      @Value("${services.user-service.url}") String userServiceUrl) {
    this.restTemplate = restTemplate;
    this.userServiceUrl = userServiceUrl;
  }

  public Optional<UserData> getUserById(String id) {
    try {
      ResponseEntity<UserData> response =
          restTemplate.getForEntity(userServiceUrl + "/internal/users/" + id, UserData.class);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  public Optional<ProfileData> getProfileByUsername(String username) {
    try {
      ResponseEntity<ProfileData> response =
          restTemplate.getForEntity(
              userServiceUrl + "/internal/profiles/" + username, ProfileData.class);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  public boolean isFollowing(String userId, String targetId) {
    try {
      ResponseEntity<Boolean> response =
          restTemplate.getForEntity(
              userServiceUrl + "/internal/users/" + userId + "/following?targetId=" + targetId,
              Boolean.class);
      return Boolean.TRUE.equals(response.getBody());
    } catch (Exception e) {
      return false;
    }
  }

  public Set<String> followingAuthors(String userId, List<String> authorIds) {
    try {
      ResponseEntity<Set<String>> response =
          restTemplate.exchange(
              userServiceUrl + "/internal/users/following-authors?userId=" + userId,
              HttpMethod.POST,
              new org.springframework.http.HttpEntity<>(authorIds),
              new ParameterizedTypeReference<Set<String>>() {});
      return response.getBody();
    } catch (Exception e) {
      return java.util.Collections.emptySet();
    }
  }

  public List<String> followedUsers(String userId) {
    try {
      ResponseEntity<List<String>> response =
          restTemplate.exchange(
              userServiceUrl + "/internal/users/" + userId + "/followed-users",
              HttpMethod.GET,
              null,
              new ParameterizedTypeReference<List<String>>() {});
      return response.getBody();
    } catch (Exception e) {
      return java.util.Collections.emptyList();
    }
  }
}
