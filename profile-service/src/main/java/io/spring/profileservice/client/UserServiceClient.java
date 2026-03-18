package io.spring.profileservice.client;

import io.spring.profileservice.application.data.UserData;
import java.util.Map;
import java.util.Optional;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

@Component
public class UserServiceClient {

  private static final Logger log = LoggerFactory.getLogger(UserServiceClient.class);

  private final RestTemplate restTemplate;
  private final String userServiceBaseUrl;

  public UserServiceClient(
      RestTemplate restTemplate,
      @Value("${user-service.base-url}") String userServiceBaseUrl) {
    this.restTemplate = restTemplate;
    this.userServiceBaseUrl = userServiceBaseUrl;
  }

  public Optional<UserData> findByUsername(String username) {
    try {
      String url = userServiceBaseUrl + "/api/users/by-username/" + username;
      ResponseEntity<Map<String, UserData>> response =
          restTemplate.exchange(
              url, HttpMethod.GET, null, new ParameterizedTypeReference<Map<String, UserData>>() {});
      Map<String, UserData> body = response.getBody();
      if (body != null && body.containsKey("user")) {
        return Optional.of(body.get("user"));
      }
      return Optional.empty();
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    } catch (Exception e) {
      log.error("Error calling User Service for username: {}", username, e);
      return Optional.empty();
    }
  }

  public Optional<UserData> findById(String id) {
    try {
      String url = userServiceBaseUrl + "/api/users/by-id/" + id;
      ResponseEntity<Map<String, UserData>> response =
          restTemplate.exchange(
              url, HttpMethod.GET, null, new ParameterizedTypeReference<Map<String, UserData>>() {});
      Map<String, UserData> body = response.getBody();
      if (body != null && body.containsKey("user")) {
        return Optional.of(body.get("user"));
      }
      return Optional.empty();
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    } catch (Exception e) {
      log.error("Error calling User Service for id: {}", id, e);
      return Optional.empty();
    }
  }
}
