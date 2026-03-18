package io.spring.favorite.api;

import io.spring.favorite.application.data.ArticleFavoriteCount;
import io.spring.favorite.infrastructure.mybatis.readservice.ArticleFavoritesReadService;
import java.util.List;
import java.util.Set;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "/api/favorites")
@AllArgsConstructor
public class FavoritesQueryApi {
  private ArticleFavoritesReadService articleFavoritesReadService;

  @GetMapping("/is-favorited")
  public ResponseEntity<Boolean> isUserFavorite(
      @RequestParam String userId, @RequestParam String articleId) {
    return ResponseEntity.ok(articleFavoritesReadService.isUserFavorite(userId, articleId));
  }

  @GetMapping("/count")
  public ResponseEntity<Integer> articleFavoriteCount(@RequestParam String articleId) {
    return ResponseEntity.ok(articleFavoritesReadService.articleFavoriteCount(articleId));
  }

  @GetMapping("/counts")
  public ResponseEntity<List<ArticleFavoriteCount>> articlesFavoriteCount(
      @RequestParam List<String> ids) {
    return ResponseEntity.ok(articleFavoritesReadService.articlesFavoriteCount(ids));
  }

  @GetMapping("/user-favorites")
  public ResponseEntity<Set<String>> userFavorites(
      @RequestParam List<String> ids, @RequestParam String userId) {
    return ResponseEntity.ok(articleFavoritesReadService.userFavorites(ids, userId));
  }
}
