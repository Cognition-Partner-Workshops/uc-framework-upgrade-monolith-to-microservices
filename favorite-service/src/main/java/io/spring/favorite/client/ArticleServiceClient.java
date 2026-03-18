package io.spring.favorite.client;

import java.util.Map;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

@Component
public class ArticleServiceClient {
  private final RestTemplate restTemplate;
  private final String articleServiceUrl;

  public ArticleServiceClient(
      RestTemplate restTemplate, @Value("${article.service.url}") String articleServiceUrl) {
    this.restTemplate = restTemplate;
    this.articleServiceUrl = articleServiceUrl;
  }

  public Optional<ArticleResponse> findBySlug(String slug) {
    try {
      @SuppressWarnings("unchecked")
      Map<String, Object> wrapper =
          restTemplate.getForObject(
              articleServiceUrl + "/articles/{slug}", Map.class, slug);
      if (wrapper == null) {
        return Optional.empty();
      }
      @SuppressWarnings("unchecked")
      Map<String, Object> articleMap = (Map<String, Object>) wrapper.get("article");
      if (articleMap == null) {
        return Optional.empty();
      }
      ArticleResponse article = new ArticleResponse();
      article.setId((String) articleMap.get("id"));
      article.setSlug((String) articleMap.get("slug"));
      article.setTitle((String) articleMap.get("title"));
      return Optional.of(article);
    } catch (Exception e) {
      return Optional.empty();
    }
  }
}
