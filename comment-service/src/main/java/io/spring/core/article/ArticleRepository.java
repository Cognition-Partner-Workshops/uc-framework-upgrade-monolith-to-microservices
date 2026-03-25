package io.spring.core.article;

import java.util.Optional;

public interface ArticleRepository {
  void save(Article article);

  Optional<Article> findBySlug(String slug);
}
