package io.spring.userauth.api.internal;

import io.spring.userauth.application.data.UserData;
import io.spring.userauth.core.JwtService;
import io.spring.userauth.infrastructure.mybatis.readservice.UserReadService;
import io.spring.userauth.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/internal")
@AllArgsConstructor
public class InternalUserApi {

  private UserReadService userReadService;
  private UserRelationshipQueryService userRelationshipQueryService;
  private JwtService jwtService;

  @GetMapping("/users/{id}")
  public ResponseEntity<UserData> getUserById(@PathVariable("id") String id) {
    UserData userData = userReadService.findById(id);
    if (userData == null) {
      return ResponseEntity.notFound().build();
    }
    return ResponseEntity.ok(userData);
  }

  @GetMapping("/users/by-username/{username}")
  public ResponseEntity<UserData> getUserByUsername(@PathVariable("username") String username) {
    UserData userData = userReadService.findByUsername(username);
    if (userData == null) {
      return ResponseEntity.notFound().build();
    }
    return ResponseEntity.ok(userData);
  }

  @PostMapping("/users/following")
  public ResponseEntity<Map<String, Object>> getFollowingAuthors(
      @RequestBody FollowingRequest request) {
    Set<String> followingIds =
        userRelationshipQueryService.followingAuthors(request.getUserId(), request.getTargetIds());
    Map<String, Object> response = new HashMap<>();
    response.put("followingIds", followingIds);
    return ResponseEntity.ok(response);
  }

  @GetMapping("/users/{userId}/follows/{targetId}")
  public ResponseEntity<Map<String, Object>> isFollowing(
      @PathVariable("userId") String userId, @PathVariable("targetId") String targetId) {
    boolean following = userRelationshipQueryService.isUserFollowing(userId, targetId);
    Map<String, Object> response = new HashMap<>();
    response.put("following", following);
    return ResponseEntity.ok(response);
  }

  @GetMapping("/users/{userId}/followed-users")
  public ResponseEntity<Map<String, Object>> getFollowedUsers(
      @PathVariable("userId") String userId) {
    List<String> followedUsers = userRelationshipQueryService.followedUsers(userId);
    Map<String, Object> response = new HashMap<>();
    response.put("followedUsers", followedUsers);
    return ResponseEntity.ok(response);
  }

  @PostMapping("/auth/validate")
  public ResponseEntity<Map<String, String>> validateToken(
      @RequestBody TokenValidationRequest request) {
    return jwtService
        .getClaimsFromToken(request.getToken())
        .map(claims -> ResponseEntity.ok(claims))
        .orElse(ResponseEntity.status(401).build());
  }
}

@Getter
@NoArgsConstructor
class FollowingRequest {
  private String userId;
  private List<String> targetIds;
}

@Getter
@NoArgsConstructor
class TokenValidationRequest {
  private String token;
}
