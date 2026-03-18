package io.spring.graphql;

import io.spring.core.user.AuthUser;
import java.util.Optional;
import org.springframework.security.authentication.AnonymousAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;

public class SecurityUtil {
  public static Optional<AuthUser> getCurrentUser() {
    Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
    if (authentication instanceof AnonymousAuthenticationToken
        || authentication.getPrincipal() == null) {
      return Optional.empty();
    }
    AuthUser currentUser = (AuthUser) authentication.getPrincipal();
    return Optional.of(currentUser);
  }
}
