package io.spring.userservice.api;

import io.spring.shared.data.ProfileData;
import io.spring.shared.exception.ResourceNotFoundException;
import io.spring.userservice.application.ProfileQueryService;
import io.spring.userservice.core.user.FollowRelation;
import io.spring.userservice.core.user.User;
import io.spring.userservice.core.user.UserRepository;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/profiles/{username}")
public class ProfileApi {
  private ProfileQueryService profileQueryService;
  private UserRepository userRepository;

  @Autowired
  public ProfileApi(ProfileQueryService profileQueryService, UserRepository userRepository) {
    this.profileQueryService = profileQueryService;
    this.userRepository = userRepository;
  }

  @GetMapping
  public ResponseEntity<?> getProfile(
      @PathVariable("username") String username,
      @AuthenticationPrincipal User currentUser) {
    return profileQueryService
        .findByUsername(username, currentUser)
        .map(this::profileResponse)
        .map(ResponseEntity::ok)
        .orElseThrow(ResourceNotFoundException::new);
  }

  @PostMapping(path = "/follow")
  public ResponseEntity<?> follow(
      @PathVariable("username") String username,
      @AuthenticationPrincipal User currentUser) {
    return userRepository
        .findByUsername(username)
        .map(
            target -> {
              FollowRelation followRelation =
                  new FollowRelation(currentUser.getId(), target.getId());
              userRepository.saveRelation(followRelation);
              return profileQueryService
                  .findByUsername(username, currentUser)
                  .map(this::profileResponse)
                  .map(ResponseEntity::ok)
                  .orElseThrow(ResourceNotFoundException::new);
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  @DeleteMapping(path = "/follow")
  public ResponseEntity<?> unfollow(
      @PathVariable("username") String username,
      @AuthenticationPrincipal User currentUser) {
    Optional<User> userOptional = userRepository.findByUsername(username);
    if (userOptional.isPresent()) {
      User target = userOptional.get();
      userRepository.removeRelation(new FollowRelation(currentUser.getId(), target.getId()));
    }
    return profileQueryService
        .findByUsername(username, currentUser)
        .map(this::profileResponse)
        .map(ResponseEntity::ok)
        .orElseThrow(ResourceNotFoundException::new);
  }

  private Map<String, Object> profileResponse(ProfileData profileData) {
    return new HashMap<String, Object>() {
      {
        put("profile", profileData);
      }
    };
  }
}
