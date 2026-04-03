package io.spring.articleservice.api.security;

import io.spring.shared.security.JwtService;
import io.spring.shared.security.JwtTokenFilter;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

@Component
public class ArticleServiceJwtTokenFilter extends JwtTokenFilter {

  private final JwtService jwtService;

  @Autowired
  public ArticleServiceJwtTokenFilter(JwtService jwtService) {
    this.jwtService = jwtService;
  }

  @Override
  protected JwtService getJwtService() {
    return jwtService;
  }

  @Override
  protected Optional<Object> findUserById(String id) {
    // For article service, we just store the user ID as the principal
    return Optional.of(id);
  }
}
