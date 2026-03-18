package io.spring.profileservice.api;

import io.spring.profileservice.api.exception.ResourceNotFoundException;
import io.spring.profileservice.application.ProfileQueryService;
import io.spring.profileservice.application.data.ProfileData;
import io.spring.profileservice.application.data.UserData;
import io.spring.profileservice.client.UserServiceClient;
import io.spring.profileservice.core.FollowRelation;
import io.spring.profileservice.core.FollowRelationRepository;
import java.util.HashMap;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "profiles/{username}")
@AllArgsConstructor
public class ProfileApi {
  private final ProfileQueryService profileQueryService;
  private final UserServiceClient userServiceClient;
  private final FollowRelationRepository followRelationRepository;

  @GetMapping
  public ResponseEntity getProfile(
      @PathVariable("username") String username, @AuthenticationPrincipal UserData user) {
    String currentUserId = user != null ? user.getId() : null;
    return profileQueryService
        .findByUsername(username, currentUserId)
        .map(this::profileResponse)
        .orElseThrow(ResourceNotFoundException::new);
  }

  @PostMapping(path = "follow")
  public ResponseEntity follow(
      @PathVariable("username") String username, @AuthenticationPrincipal UserData user) {
    return userServiceClient
        .findByUsername(username)
        .map(
            target -> {
              FollowRelation followRelation = new FollowRelation(user.getId(), target.getId());
              followRelationRepository.saveRelation(followRelation);
              return profileResponse(
                  profileQueryService.findByUsername(username, user.getId()).get());
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  @DeleteMapping(path = "follow")
  public ResponseEntity unfollow(
      @PathVariable("username") String username, @AuthenticationPrincipal UserData user) {
    return userServiceClient
        .findByUsername(username)
        .map(
            target ->
                followRelationRepository
                    .findRelation(user.getId(), target.getId())
                    .map(
                        relation -> {
                          followRelationRepository.removeRelation(relation);
                          return profileResponse(
                              profileQueryService
                                  .findByUsername(username, user.getId())
                                  .get());
                        })
                    .orElseThrow(ResourceNotFoundException::new))
        .orElseThrow(ResourceNotFoundException::new);
  }

  private ResponseEntity profileResponse(ProfileData profile) {
    return ResponseEntity.ok(
        new HashMap<String, Object>() {
          {
            put("profile", profile);
          }
        });
  }
}
