package io.spring.userauth.core;

import java.util.Map;
import java.util.Optional;
import org.springframework.stereotype.Service;

@Service
public interface JwtService {
  String toToken(User user);

  Optional<String> getSubFromToken(String token);

  Optional<Map<String, String>> getClaimsFromToken(String token);
}
