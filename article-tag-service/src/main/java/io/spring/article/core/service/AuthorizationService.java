package io.spring.article.core.service;

import io.spring.article.client.UserDTO;
import io.spring.article.core.article.Article;

public class AuthorizationService {
  public static boolean canWriteArticle(UserDTO user, Article article) {
    return user.getId().equals(article.getUserId());
  }
}
