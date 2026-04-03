package io.spring.articleservice.api;

import io.spring.articleservice.application.ArticleQueryService;
import io.spring.articleservice.application.data.ArticleData;
import io.spring.articleservice.core.article.ArticleRepository;
import io.spring.articleservice.core.favorite.ArticleFavorite;
import io.spring.articleservice.core.favorite.ArticleFavoriteRepository;
import io.spring.shared.exception.ResourceNotFoundException;
import java.util.HashMap;
import java.util.Map;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/articles/{slug}/favorite")
public class ArticleFavoriteApi {
  private ArticleFavoriteRepository articleFavoriteRepository;
  private ArticleRepository articleRepository;
  private ArticleQueryService articleQueryService;

  @Autowired
  public ArticleFavoriteApi(
      ArticleFavoriteRepository articleFavoriteRepository,
      ArticleRepository articleRepository,
      ArticleQueryService articleQueryService) {
    this.articleFavoriteRepository = articleFavoriteRepository;
    this.articleRepository = articleRepository;
    this.articleQueryService = articleQueryService;
  }

  @PostMapping
  public ResponseEntity<?> favoriteArticle(
      @PathVariable("slug") String slug, @AuthenticationPrincipal String userId) {
    return articleRepository
        .findBySlug(slug)
        .map(
            article -> {
              ArticleFavorite articleFavorite =
                  new ArticleFavorite(article.getId(), userId);
              articleFavoriteRepository.save(articleFavorite);
              return articleQueryService
                  .findBySlug(slug, userId)
                  .map(a -> ResponseEntity.ok(articleResponse(a)))
                  .orElseThrow(ResourceNotFoundException::new);
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  @DeleteMapping
  public ResponseEntity<?> unfavoriteArticle(
      @PathVariable("slug") String slug, @AuthenticationPrincipal String userId) {
    return articleRepository
        .findBySlug(slug)
        .map(
            article -> {
              articleFavoriteRepository
                  .find(article.getId(), userId)
                  .ifPresent(articleFavoriteRepository::remove);
              return articleQueryService
                  .findBySlug(slug, userId)
                  .map(a -> ResponseEntity.ok(articleResponse(a)))
                  .orElseThrow(ResourceNotFoundException::new);
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  private Map<String, Object> articleResponse(ArticleData articleData) {
    return new HashMap<String, Object>() {
      {
        put("article", articleData);
      }
    };
  }
}
