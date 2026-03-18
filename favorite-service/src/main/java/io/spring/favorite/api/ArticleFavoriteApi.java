package io.spring.favorite.api;

import io.spring.favorite.api.exception.ResourceNotFoundException;
import io.spring.favorite.client.ArticleResponse;
import io.spring.favorite.client.ArticleServiceClient;
import io.spring.favorite.client.UserResponse;
import io.spring.favorite.core.ArticleFavorite;
import io.spring.favorite.core.ArticleFavoriteRepository;
import io.spring.favorite.infrastructure.mybatis.readservice.ArticleFavoritesReadService;
import java.util.HashMap;
import java.util.Map;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "articles/{slug}/favorite")
@AllArgsConstructor
public class ArticleFavoriteApi {
  private ArticleFavoriteRepository articleFavoriteRepository;
  private ArticleServiceClient articleServiceClient;
  private ArticleFavoritesReadService articleFavoritesReadService;

  @PostMapping
  public ResponseEntity<Map<String, Object>> favoriteArticle(
      @PathVariable("slug") String slug, @AuthenticationPrincipal UserResponse user) {
    ArticleResponse article =
        articleServiceClient.findBySlug(slug).orElseThrow(ResourceNotFoundException::new);
    ArticleFavorite articleFavorite = new ArticleFavorite(article.getId(), user.getId());
    articleFavoriteRepository.save(articleFavorite);
    return responseFavoriteData(article, user.getId());
  }

  @DeleteMapping
  public ResponseEntity<Map<String, Object>> unfavoriteArticle(
      @PathVariable("slug") String slug, @AuthenticationPrincipal UserResponse user) {
    ArticleResponse article =
        articleServiceClient.findBySlug(slug).orElseThrow(ResourceNotFoundException::new);
    articleFavoriteRepository
        .find(article.getId(), user.getId())
        .ifPresent(favorite -> articleFavoriteRepository.remove(favorite));
    return responseFavoriteData(article, user.getId());
  }

  private ResponseEntity<Map<String, Object>> responseFavoriteData(
      ArticleResponse article, String userId) {
    boolean favorited =
        articleFavoritesReadService.isUserFavorite(userId, article.getId());
    int favoritesCount =
        articleFavoritesReadService.articleFavoriteCount(article.getId());
    Map<String, Object> favoriteData = new HashMap<>();
    favoriteData.put("articleId", article.getId());
    favoriteData.put("slug", article.getSlug());
    favoriteData.put("favorited", favorited);
    favoriteData.put("favoritesCount", favoritesCount);
    Map<String, Object> response = new HashMap<>();
    response.put("favorite", favoriteData);
    return ResponseEntity.ok(response);
  }
}
