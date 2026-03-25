package io.spring.core.service;

import io.spring.core.user.User;

public class AuthorizationService {
  public static boolean canWriteArticle(User user, String articleUserId) {
    return user.getId().equals(articleUserId);
  }

  public static boolean canWriteComment(
      User user, String articleUserId, String commentUserId) {
    return user.getId().equals(articleUserId) || user.getId().equals(commentUserId);
  }
}
