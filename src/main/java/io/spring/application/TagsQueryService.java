package io.spring.application;

import java.util.Collections;
import java.util.List;
import java.util.Map;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

@Service
public class TagsQueryService {
  private final RestTemplate restTemplate;
  private final String tagsServiceUrl;

  public TagsQueryService(
      RestTemplate restTemplate,
      @Value("${tags.service.url:http://localhost:8081}") String tagsServiceUrl) {
    this.restTemplate = restTemplate;
    this.tagsServiceUrl = tagsServiceUrl;
  }

  @SuppressWarnings("unchecked")
  public List<String> allTags() {
    try {
      ResponseEntity<Map<String, Object>> response =
          restTemplate.exchange(
              tagsServiceUrl + "/tags",
              HttpMethod.GET,
              null,
              new ParameterizedTypeReference<Map<String, Object>>() {});
      Map<String, Object> body = response.getBody();
      if (body != null && body.containsKey("tags")) {
        return (List<String>) body.get("tags");
      }
      return Collections.emptyList();
    } catch (Exception e) {
      return Collections.emptyList();
    }
  }
}
