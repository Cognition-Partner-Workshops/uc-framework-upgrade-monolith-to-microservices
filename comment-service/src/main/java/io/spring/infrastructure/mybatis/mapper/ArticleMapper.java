package io.spring.infrastructure.mybatis.mapper;

import io.spring.core.article.Article;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

@Mapper
public interface ArticleMapper {
  void insert(@Param("article") Article article);

  Article findBySlug(@Param("slug") String slug);
}
