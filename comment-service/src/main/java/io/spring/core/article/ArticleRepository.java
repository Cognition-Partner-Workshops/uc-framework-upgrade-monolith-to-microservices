package io.spring.core.article;

import java.util.Optional;

public interface ArticleRepository {
  Optional<Article> findBySlug(String slug);
}
