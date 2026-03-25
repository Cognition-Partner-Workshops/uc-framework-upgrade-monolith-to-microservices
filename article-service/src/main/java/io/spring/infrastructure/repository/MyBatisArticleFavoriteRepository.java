package io.spring.infrastructure.repository;

import io.spring.core.favorite.ArticleFavorite;
import io.spring.core.favorite.ArticleFavoriteRepository;
import io.spring.infrastructure.mybatis.mapper.ArticleFavoriteMapper;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

@Repository
public class MyBatisArticleFavoriteRepository implements ArticleFavoriteRepository {
  private ArticleFavoriteMapper articleFavoriteMapper;

  @Autowired
  public MyBatisArticleFavoriteRepository(ArticleFavoriteMapper articleFavoriteMapper) {
    this.articleFavoriteMapper = articleFavoriteMapper;
  }

  @Override
  public void save(ArticleFavorite articleFavorite) {
    if (find(articleFavorite.getArticleId(), articleFavorite.getUserId()).isEmpty()) {
      articleFavoriteMapper.insert(articleFavorite);
    }
  }

  @Override
  public Optional<ArticleFavorite> find(String articleId, String userId) {
    return Optional.ofNullable(articleFavoriteMapper.find(articleId, userId));
  }

  @Override
  public void remove(ArticleFavorite favorite) {
    articleFavoriteMapper.delete(favorite);
  }
}
