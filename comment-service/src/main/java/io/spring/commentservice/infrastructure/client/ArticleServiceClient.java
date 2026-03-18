package io.spring.commentservice.infrastructure.client;

import java.util.Map;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

@Component
public class ArticleServiceClient {

  private final RestTemplate restTemplate;
  private final String articleServiceUrl;

  public ArticleServiceClient(
      RestTemplate restTemplate,
      @Value("${article-service.url}") String articleServiceUrl) {
    this.restTemplate = restTemplate;
    this.articleServiceUrl = articleServiceUrl;
  }

  /**
   * Looks up an article by slug from the Article Service. Returns a map with article data
   * including "id" and "userId" fields, or null if not found.
   */
  @SuppressWarnings("unchecked")
  public Map<String, Object> findArticleBySlug(String slug) {
    try {
      Map<String, Object> response =
          restTemplate.getForObject(
              articleServiceUrl + "/articles/{slug}", Map.class, slug);
      if (response != null && response.containsKey("article")) {
        return (Map<String, Object>) response.get("article");
      }
      return response;
    } catch (HttpClientErrorException.NotFound e) {
      return null;
    }
  }
}
