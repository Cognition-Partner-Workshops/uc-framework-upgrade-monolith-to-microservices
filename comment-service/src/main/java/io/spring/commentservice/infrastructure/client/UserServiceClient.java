package io.spring.commentservice.infrastructure.client;

import java.util.Map;
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
      RestTemplate restTemplate,
      @Value("${user-service.url}") String userServiceUrl) {
    this.restTemplate = restTemplate;
    this.userServiceUrl = userServiceUrl;
  }

  /**
   * Retrieves the current user by forwarding the Authorization token to the User Service.
   * Returns a map with user data including "id", "username", "email", "bio", "image" fields,
   * or null if the token is invalid or the user is not found.
   */
  @SuppressWarnings("unchecked")
  public Map<String, Object> getCurrentUser(String authorizationHeader) {
    try {
      HttpHeaders headers = new HttpHeaders();
      headers.set("Authorization", authorizationHeader);
      HttpEntity<Void> entity = new HttpEntity<>(headers);
      ResponseEntity<Map> response =
          restTemplate.exchange(
              userServiceUrl + "/user", HttpMethod.GET, entity, Map.class);
      Map<String, Object> body = response.getBody();
      if (body != null && body.containsKey("user")) {
        return (Map<String, Object>) body.get("user");
      }
      return body;
    } catch (HttpClientErrorException e) {
      return null;
    }
  }
}
