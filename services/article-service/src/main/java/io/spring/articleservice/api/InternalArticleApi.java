package io.spring.articleservice.api;

import io.spring.articleservice.application.ArticleQueryService;
import io.spring.articleservice.application.data.ArticleData;
import io.spring.articleservice.core.article.Article;
import io.spring.articleservice.core.article.ArticleRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/internal")
public class InternalArticleApi {

  private ArticleRepository articleRepository;
  private ArticleQueryService articleQueryService;

  @Autowired
  public InternalArticleApi(
      ArticleRepository articleRepository, ArticleQueryService articleQueryService) {
    this.articleRepository = articleRepository;
    this.articleQueryService = articleQueryService;
  }

  @GetMapping("/articles/{slug}")
  public ResponseEntity<ArticleData> getArticleBySlug(@PathVariable("slug") String slug) {
    return articleQueryService
        .findBySlug(slug, null)
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }

  @GetMapping("/articles/id/{id}")
  public ResponseEntity<Boolean> articleExists(@PathVariable("id") String id) {
    return ResponseEntity.ok(articleRepository.findById(id).isPresent());
  }
}
