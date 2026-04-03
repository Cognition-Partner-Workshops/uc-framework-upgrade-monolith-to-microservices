package io.spring.userservice.api;

import io.spring.shared.data.ProfileData;
import io.spring.shared.data.UserData;
import io.spring.userservice.application.ProfileQueryService;
import io.spring.userservice.application.UserQueryService;
import io.spring.userservice.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.List;
import java.util.Set;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/internal")
public class InternalUserApi {

  private UserQueryService userQueryService;
  private ProfileQueryService profileQueryService;
  private UserRelationshipQueryService userRelationshipQueryService;

  @Autowired
  public InternalUserApi(
      UserQueryService userQueryService,
      ProfileQueryService profileQueryService,
      UserRelationshipQueryService userRelationshipQueryService) {
    this.userQueryService = userQueryService;
    this.profileQueryService = profileQueryService;
    this.userRelationshipQueryService = userRelationshipQueryService;
  }

  @GetMapping("/users/{id}")
  public ResponseEntity<UserData> getUserById(@PathVariable("id") String id) {
    return userQueryService
        .findById(id)
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }

  @GetMapping("/profiles/{username}")
  public ResponseEntity<ProfileData> getProfileByUsername(
      @PathVariable("username") String username,
      @RequestParam(value = "currentUserId", required = false) String currentUserId) {
    return profileQueryService
        .findByUsername(username, null)
        .map(ResponseEntity::ok)
        .orElse(ResponseEntity.notFound().build());
  }

  @GetMapping("/users/{userId}/following")
  public ResponseEntity<Boolean> isFollowing(
      @PathVariable("userId") String userId, @RequestParam("targetId") String targetId) {
    return ResponseEntity.ok(userRelationshipQueryService.isUserFollowing(userId, targetId));
  }

  @PostMapping("/users/following-authors")
  public ResponseEntity<Set<String>> followingAuthors(
      @RequestParam("userId") String userId, @RequestBody List<String> authorIds) {
    return ResponseEntity.ok(userRelationshipQueryService.followingAuthors(userId, authorIds));
  }

  @GetMapping("/users/{userId}/followed-users")
  public ResponseEntity<List<String>> followedUsers(@PathVariable("userId") String userId) {
    return ResponseEntity.ok(userRelationshipQueryService.followedUsers(userId));
  }
}
