package io.spring.userservice.core;

import java.util.Optional;

public interface JwtService {
  String toToken(User user);

  Optional<String> getSubFromToken(String token);
}
