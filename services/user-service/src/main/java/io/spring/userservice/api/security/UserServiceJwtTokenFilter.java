package io.spring.userservice.api.security;

import io.spring.shared.security.JwtService;
import io.spring.shared.security.JwtTokenFilter;
import io.spring.userservice.core.user.UserRepository;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

@Component
public class UserServiceJwtTokenFilter extends JwtTokenFilter {

  private final JwtService jwtService;
  private final UserRepository userRepository;

  @Autowired
  public UserServiceJwtTokenFilter(JwtService jwtService, UserRepository userRepository) {
    this.jwtService = jwtService;
    this.userRepository = userRepository;
  }

  @Override
  protected JwtService getJwtService() {
    return jwtService;
  }

  @Override
  protected Optional<Object> findUserById(String id) {
    return userRepository.findById(id).map(u -> u);
  }
}
