package io.spring.articleservice.api;

import io.spring.articleservice.application.ArticleQueryService;
import io.spring.articleservice.application.Page;
import io.spring.articleservice.application.article.ArticleCommandService;
import io.spring.articleservice.application.article.NewArticleParam;
import io.spring.articleservice.application.article.UpdateArticleParam;
import io.spring.articleservice.application.data.ArticleData;
import io.spring.articleservice.application.data.ArticleDataList;
import io.spring.articleservice.core.article.Article;
import io.spring.articleservice.core.article.ArticleRepository;
import io.spring.shared.exception.InvalidRequestException;
import io.spring.shared.exception.NoAuthorizationException;
import io.spring.shared.exception.ResourceNotFoundException;
import java.util.HashMap;
import java.util.Map;
import javax.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/articles")
public class ArticlesApi {
  private ArticleRepository articleRepository;
  private ArticleQueryService articleQueryService;
  private ArticleCommandService articleCommandService;

  @Autowired
  public ArticlesApi(
      ArticleRepository articleRepository,
      ArticleQueryService articleQueryService,
      ArticleCommandService articleCommandService) {
    this.articleRepository = articleRepository;
    this.articleQueryService = articleQueryService;
    this.articleCommandService = articleCommandService;
  }

  @PostMapping
  public ResponseEntity<?> createArticle(
      @AuthenticationPrincipal String userId,
      @Valid @RequestBody NewArticleParam newArticleParam,
      BindingResult bindingResult) {
    if (bindingResult.hasErrors()) {
      throw new InvalidRequestException(bindingResult);
    }
    Article article = articleCommandService.createArticle(newArticleParam, userId);
    return articleQueryService
        .findById(article.getId(), userId)
        .map(a -> ResponseEntity.status(201).body(articleResponse(a)))
        .orElseThrow(ResourceNotFoundException::new);
  }

  @GetMapping(path = "/feed")
  public ResponseEntity<?> getFeed(
      @AuthenticationPrincipal String userId,
      @RequestParam(value = "offset", defaultValue = "0") int offset,
      @RequestParam(value = "limit", defaultValue = "20") int limit) {
    return ResponseEntity.ok(articleQueryService.findUserFeed(userId, new Page(offset, limit)));
  }

  @GetMapping
  public ResponseEntity<?> getArticles(
      @RequestParam(value = "offset", defaultValue = "0") int offset,
      @RequestParam(value = "limit", defaultValue = "20") int limit,
      @RequestParam(value = "tag", required = false) String tag,
      @RequestParam(value = "favorited", required = false) String favoritedBy,
      @RequestParam(value = "author", required = false) String author,
      @AuthenticationPrincipal String userId) {
    return ResponseEntity.ok(
        articleQueryService.findRecentArticles(tag, author, favoritedBy, new Page(offset, limit), userId));
  }

  @GetMapping(path = "/{slug}")
  public ResponseEntity<?> getArticle(
      @PathVariable("slug") String slug,
      @AuthenticationPrincipal String userId) {
    return articleQueryService
        .findBySlug(slug, userId)
        .map(a -> ResponseEntity.ok(articleResponse(a)))
        .orElseThrow(ResourceNotFoundException::new);
  }

  @PutMapping(path = "/{slug}")
  public ResponseEntity<?> updateArticle(
      @PathVariable("slug") String slug,
      @AuthenticationPrincipal String userId,
      @Valid @RequestBody UpdateArticleParam updateArticleParam) {
    return articleRepository
        .findBySlug(slug)
        .map(
            article -> {
              if (!article.getUserId().equals(userId)) {
                throw new NoAuthorizationException();
              }
              articleCommandService.updateArticle(article, updateArticleParam);
              return articleQueryService
                  .findBySlug(article.getSlug(), userId)
                  .map(a -> ResponseEntity.ok(articleResponse(a)))
                  .orElseThrow(ResourceNotFoundException::new);
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  @DeleteMapping(path = "/{slug}")
  public ResponseEntity<?> deleteArticle(
      @PathVariable("slug") String slug, @AuthenticationPrincipal String userId) {
    return articleRepository
        .findBySlug(slug)
        .map(
            article -> {
              if (!article.getUserId().equals(userId)) {
                throw new NoAuthorizationException();
              }
              articleRepository.remove(article);
              return ResponseEntity.noContent().build();
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
