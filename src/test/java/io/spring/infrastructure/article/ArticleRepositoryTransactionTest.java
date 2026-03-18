package io.spring.infrastructure.article;

import io.spring.core.article.Article;
import io.spring.core.article.ArticleRepository;
import io.spring.infrastructure.mybatis.mapper.ArticleMapper;
import java.util.Arrays;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

@ActiveProfiles("test")
@SpringBootTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
public class ArticleRepositoryTransactionTest {
  @Autowired private ArticleRepository articleRepository;

  @Autowired private ArticleMapper articleMapper;

  @Test
  public void transactional_test() {
    String userId = "test-user-id";
    Article article =
        new Article("test", "desc", "body", Arrays.asList("java", "spring"), userId);
    articleRepository.save(article);
    Article anotherArticle =
        new Article("test", "desc", "body", Arrays.asList("java", "spring", "other"), userId);
    try {
      articleRepository.save(anotherArticle);
    } catch (Exception e) {
      Assertions.assertNull(articleMapper.findTag("other"));
    }
  }
}
