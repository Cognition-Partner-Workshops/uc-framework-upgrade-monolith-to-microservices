package io.spring.article.client;

import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
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
      @Value("${user-service.url:http://localhost:8081}") String userServiceUrl) {
    this.restTemplate = new RestTemplate();
    this.userServiceUrl = userServiceUrl;
  }

  public Optional<UserDTO> findUserById(String userId) {
    try {
      ResponseEntity<UserDTO> response =
          restTemplate.getForEntity(userServiceUrl + "/users/" + userId, UserDTO.class);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    } catch (Exception e) {
      return Optional.empty();
    }
  }

  public Optional<UserDTO> findUserByUsername(String username) {
    try {
      ResponseEntity<UserDTO> response =
          restTemplate.getForEntity(
              userServiceUrl + "/users/by-username/" + username, UserDTO.class);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    } catch (Exception e) {
      return Optional.empty();
    }
  }

  public Optional<UserDTO> resolveToken(String token) {
    try {
      HttpHeaders headers = new HttpHeaders();
      headers.set("Authorization", "Token " + token);
      HttpEntity<Void> entity = new HttpEntity<>(headers);
      ResponseEntity<UserDTO> response =
          restTemplate.exchange(
              userServiceUrl + "/user", HttpMethod.GET, entity, UserDTO.class);
      return Optional.ofNullable(response.getBody());
    } catch (Exception e) {
      return Optional.empty();
    }
  }

  public List<String> followedUsers(String userId) {
    try {
      ResponseEntity<String[]> response =
          restTemplate.getForEntity(
              userServiceUrl + "/users/" + userId + "/following", String[].class);
      String[] body = response.getBody();
      return body != null ? Arrays.asList(body) : Collections.emptyList();
    } catch (Exception e) {
      return Collections.emptyList();
    }
  }

  public boolean isUserFollowing(String userId, String anotherUserId) {
    try {
      ResponseEntity<Boolean> response =
          restTemplate.getForEntity(
              userServiceUrl + "/users/" + userId + "/following/" + anotherUserId, Boolean.class);
      return Boolean.TRUE.equals(response.getBody());
    } catch (Exception e) {
      return false;
    }
  }

  public Set<String> followingAuthors(String userId, List<String> authorIds) {
    try {
      String ids = String.join(",", authorIds);
      ResponseEntity<String[]> response =
          restTemplate.getForEntity(
              userServiceUrl + "/users/" + userId + "/following-among?ids=" + ids, String[].class);
      String[] body = response.getBody();
      return body != null ? new HashSet<>(Arrays.asList(body)) : Collections.emptySet();
    } catch (Exception e) {
      return Collections.emptySet();
    }
  }
}
