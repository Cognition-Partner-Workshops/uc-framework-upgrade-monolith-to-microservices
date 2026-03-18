package io.spring.favorite.client;

import java.util.Map;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

@Component
public class UserServiceClient {
  private final RestTemplate restTemplate;
  private final String userServiceUrl;

  public UserServiceClient(
      RestTemplate restTemplate, @Value("${user.service.url}") String userServiceUrl) {
    this.restTemplate = restTemplate;
    this.userServiceUrl = userServiceUrl;
  }

  public Optional<UserResponse> findById(String userId) {
    try {
      @SuppressWarnings("unchecked")
      Map<String, Object> response =
          restTemplate.getForObject(
              userServiceUrl + "/api/users/{id}", Map.class, userId);
      if (response == null) {
        return Optional.empty();
      }
      UserResponse user = new UserResponse();
      user.setId((String) response.get("id"));
      user.setUsername((String) response.get("username"));
      user.setEmail((String) response.get("email"));
      user.setBio((String) response.get("bio"));
      user.setImage((String) response.get("image"));
      return Optional.of(user);
    } catch (Exception e) {
      return Optional.empty();
    }
  }
}
