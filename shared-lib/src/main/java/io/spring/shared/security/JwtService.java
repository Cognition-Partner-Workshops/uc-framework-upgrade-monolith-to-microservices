package io.spring.shared.security;

import java.util.Optional;
import org.springframework.stereotype.Service;

@Service
public interface JwtService {
  String toToken(String userId);

  Optional<String> getSubFromToken(String token);
}
