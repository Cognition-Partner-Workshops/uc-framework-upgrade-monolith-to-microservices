package io.spring.core.service;

import io.spring.core.article.Article;
import io.spring.core.comment.Comment;
import io.spring.core.user.AuthUser;

public class AuthorizationService {
  public static boolean canWriteArticle(AuthUser user, Article article) {
    return user.getId().equals(article.getUserId());
  }

  public static boolean canWriteComment(AuthUser user, Article article, Comment comment) {
    return user.getId().equals(article.getUserId()) || user.getId().equals(comment.getUserId());
  }
}
