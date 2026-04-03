package io.spring.commentservice.infrastructure.client;

import io.spring.shared.data.UserData;
import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpEntity;
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

  public Set<String> followingAuthors(String userId, List<String> authorIds) {
    try {
      ResponseEntity<Set<String>> response =
          restTemplate.exchange(
              userServiceUrl + "/internal/users/following-authors?userId=" + userId,
              HttpMethod.POST,
              new HttpEntity<>(authorIds),
              new ParameterizedTypeReference<Set<String>>() {});
      return response.getBody();
    } catch (Exception e) {
      return Collections.emptySet();
    }
  }
}
