package io.spring.userservice.api.internal;

import io.spring.userservice.application.UserQueryService;
import io.spring.userservice.application.data.UserData;
import io.spring.userservice.core.service.JwtService;
import io.spring.userservice.core.user.User;
import io.spring.userservice.core.user.UserRepository;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

/**
 * Internal REST API for inter-service communication. Other microservices can call these endpoints
 * to validate tokens and look up user data without needing direct database access.
 */
@RestController
@RequestMapping("/api/internal")
@AllArgsConstructor
public class InternalUserApi {

  private UserRepository userRepository;
  private UserQueryService userQueryService;
  private JwtService jwtService;

  /** Validate a JWT token and return the associated user ID. */
  @PostMapping("/tokens/validate")
  public ResponseEntity<Map<String, Object>> validateToken(
      @RequestBody Map<String, String> request) {
    String token = request.get("token");
    if (token == null || token.isEmpty()) {
      return ResponseEntity.badRequest()
          .body(
              new HashMap<String, Object>() {
                {
                  put("valid", false);
                  put("message", "Token is required");
                }
              });
    }

    Optional<String> userId = jwtService.getSubFromToken(token);
    if (userId.isPresent()) {
      Optional<User> user = userRepository.findById(userId.get());
      if (user.isPresent()) {
        Map<String, Object> response = new HashMap<>();
        response.put("valid", true);
        response.put("userId", userId.get());
        response.put("username", user.get().getUsername());
        response.put("email", user.get().getEmail());
        return ResponseEntity.ok(response);
      }
    }

    return ResponseEntity.ok(
        new HashMap<String, Object>() {
          {
            put("valid", false);
            put("message", "Invalid or expired token");
          }
        });
  }

  /** Look up a user by ID. */
  @GetMapping("/users/{id}")
  public ResponseEntity<UserData> getUserById(@PathVariable String id) {
    return userQueryService
        .findById(id)
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }

  /** Look up a user by username. */
  @GetMapping("/users/by-username/{username}")
  public ResponseEntity<UserData> getUserByUsername(@PathVariable String username) {
    return userRepository
        .findByUsername(username)
        .flatMap(user -> userQueryService.findById(user.getId()))
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }

  /** Look up a user by email. */
  @GetMapping("/users/by-email/{email}")
  public ResponseEntity<UserData> getUserByEmail(@PathVariable String email) {
    return userRepository
        .findByEmail(email)
        .flatMap(user -> userQueryService.findById(user.getId()))
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }
}
