package io.spring.application.article;

import io.spring.core.article.Article;
import io.spring.core.article.ArticleRepository;
import io.spring.core.user.User;
import java.util.Collections;
import java.util.List;
import javax.validation.Valid;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.client.RestTemplate;

@Service
@Validated
public class ArticleCommandService {

  private final ArticleRepository articleRepository;
  private final RestTemplate restTemplate;
  private final String tagsServiceUrl;

  public ArticleCommandService(
      ArticleRepository articleRepository,
      RestTemplate restTemplate,
      @Value("${tags.service.url:http://localhost:8081}") String tagsServiceUrl) {
    this.articleRepository = articleRepository;
    this.restTemplate = restTemplate;
    this.tagsServiceUrl = tagsServiceUrl;
  }

  public Article createArticle(@Valid NewArticleParam newArticleParam, User creator) {
    Article article =
        new Article(
            newArticleParam.getTitle(),
            newArticleParam.getDescription(),
            newArticleParam.getBody(),
            newArticleParam.getTagList(),
            creator.getId());
    articleRepository.save(article);
    syncTagsToService(newArticleParam.getTagList());
    return article;
  }

  public Article updateArticle(Article article, @Valid UpdateArticleParam updateArticleParam) {
    article.update(
        updateArticleParam.getTitle(),
        updateArticleParam.getDescription(),
        updateArticleParam.getBody());
    articleRepository.save(article);
    return article;
  }

  private void syncTagsToService(List<String> tagNames) {
    if (tagNames == null) {
      return;
    }
    for (String tagName : tagNames) {
      try {
        restTemplate.postForEntity(
            tagsServiceUrl + "/tags", Collections.singletonMap("name", tagName), Object.class);
      } catch (Exception e) {
        // Log but don't fail article creation if tags service is unavailable
      }
    }
  }
}
