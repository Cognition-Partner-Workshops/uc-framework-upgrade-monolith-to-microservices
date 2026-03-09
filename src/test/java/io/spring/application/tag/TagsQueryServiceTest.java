package io.spring.application.tag;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.when;

import io.spring.application.TagsQueryService;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.client.RestTemplate;

@ExtendWith(MockitoExtension.class)
public class TagsQueryServiceTest {
  @Mock private RestTemplate restTemplate;

  private TagsQueryService tagsQueryService;

  @BeforeEach
  public void setUp() {
    tagsQueryService = new TagsQueryService(restTemplate, "http://localhost:8081");
  }

  @Test
  public void should_get_all_tags() {
    Map<String, Object> responseBody = new HashMap<>();
    responseBody.put("tags", Arrays.asList("java", "spring-boot"));

    when(restTemplate.exchange(
            eq("http://localhost:8081/tags"),
            eq(HttpMethod.GET),
            any(),
            any(ParameterizedTypeReference.class)))
        .thenReturn(new ResponseEntity<>(responseBody, HttpStatus.OK));

    List<String> tags = tagsQueryService.allTags();
    Assertions.assertTrue(tags.contains("java"));
    Assertions.assertTrue(tags.contains("spring-boot"));
  }

  @Test
  public void should_return_empty_list_when_service_unavailable() {
    when(restTemplate.exchange(
            eq("http://localhost:8081/tags"),
            eq(HttpMethod.GET),
            any(),
            any(ParameterizedTypeReference.class)))
        .thenThrow(new RuntimeException("Connection refused"));

    List<String> tags = tagsQueryService.allTags();
    Assertions.assertTrue(tags.isEmpty());
  }
}
