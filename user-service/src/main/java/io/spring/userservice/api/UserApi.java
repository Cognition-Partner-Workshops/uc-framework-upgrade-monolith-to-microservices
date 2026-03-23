package io.spring.userservice.api;

import com.fasterxml.jackson.annotation.JsonRootName;
import io.spring.userservice.core.FollowRelation;
import io.spring.userservice.core.JwtService;
import io.spring.userservice.core.User;
import io.spring.userservice.core.UserRepository;
import jakarta.validation.Valid;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

@RestController
@AllArgsConstructor
public class UserApi {
  private UserRepository userRepository;
  private PasswordEncoder passwordEncoder;
  private JwtService jwtService;

  @PostMapping("/users")
  public ResponseEntity<Map<String, Object>> createUser(
      @Valid @RequestBody RegisterParam registerParam) {
    User user =
        new User(
            registerParam.getEmail(),
            registerParam.getUsername(),
            passwordEncoder.encode(registerParam.getPassword()),
            "",
            "");
    userRepository.save(user);
    return ResponseEntity.status(201).body(userResponse(user));
  }

  @PostMapping("/users/login")
  public ResponseEntity<Map<String, Object>> login(
      @Valid @RequestBody LoginParam loginParam) {
    Optional<User> optional = userRepository.findByEmail(loginParam.getEmail());
    if (optional.isPresent()
        && passwordEncoder.matches(loginParam.getPassword(), optional.get().getPassword())) {
      return ResponseEntity.ok(userResponse(optional.get()));
    } else {
      throw new ResponseStatusException(HttpStatus.UNPROCESSABLE_ENTITY, "Invalid credentials");
    }
  }

  @GetMapping("/user")
  public ResponseEntity<Map<String, Object>> currentUser(@AuthenticationPrincipal User user) {
    return ResponseEntity.ok(userResponse(user));
  }

  @PutMapping("/user")
  public ResponseEntity<Map<String, Object>> updateUser(
      @AuthenticationPrincipal User user, @Valid @RequestBody UpdateUserParam updateUserParam) {
    user.update(
        updateUserParam.getEmail(),
        updateUserParam.getUsername(),
        updateUserParam.getPassword() != null
            ? passwordEncoder.encode(updateUserParam.getPassword())
            : null,
        updateUserParam.getBio(),
        updateUserParam.getImage());
    userRepository.save(user);
    return ResponseEntity.ok(userResponse(user));
  }

  @GetMapping("/profiles/{username}")
  public ResponseEntity<Map<String, Object>> getProfile(
      @PathVariable("username") String username, @AuthenticationPrincipal User user) {
    return userRepository
        .findByUsername(username)
        .map(target -> ResponseEntity.ok(profileResponse(target, user)))
        .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
  }

  @PostMapping("/profiles/{username}/follow")
  public ResponseEntity<Map<String, Object>> follow(
      @PathVariable("username") String username, @AuthenticationPrincipal User user) {
    return userRepository
        .findByUsername(username)
        .map(
            target -> {
              userRepository.saveRelation(new FollowRelation(user.getId(), target.getId()));
              return ResponseEntity.ok(profileResponse(target, user));
            })
        .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
  }

  @DeleteMapping("/profiles/{username}/follow")
  public ResponseEntity<Map<String, Object>> unfollow(
      @PathVariable("username") String username, @AuthenticationPrincipal User user) {
    return userRepository
        .findByUsername(username)
        .map(
            target -> {
              userRepository
                  .findRelation(user.getId(), target.getId())
                  .ifPresent(userRepository::removeRelation);
              return ResponseEntity.ok(profileResponse(target, user));
            })
        .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
  }

  private Map<String, Object> userResponse(User user) {
    Map<String, Object> response = new HashMap<>();
    Map<String, Object> userData = new HashMap<>();
    userData.put("email", user.getEmail());
    userData.put("username", user.getUsername());
    userData.put("bio", user.getBio());
    userData.put("image", user.getImage());
    userData.put("token", jwtService.toToken(user));
    response.put("user", userData);
    return response;
  }

  private Map<String, Object> profileResponse(User target, User currentUser) {
    boolean following = false;
    if (currentUser != null) {
      following = userRepository.findRelation(currentUser.getId(), target.getId()).isPresent();
    }
    Map<String, Object> response = new HashMap<>();
    Map<String, Object> profile = new HashMap<>();
    profile.put("username", target.getUsername());
    profile.put("bio", target.getBio());
    profile.put("image", target.getImage());
    profile.put("following", following);
    response.put("profile", profile);
    return response;
  }
}

@Getter
@JsonRootName("user")
@NoArgsConstructor
class RegisterParam {
  @NotBlank(message = "can't be empty")
  @Email(message = "should be an email")
  private String email;

  @NotBlank(message = "can't be empty")
  private String username;

  @NotBlank(message = "can't be empty")
  private String password;
}

@Getter
@JsonRootName("user")
@NoArgsConstructor
class LoginParam {
  @NotBlank(message = "can't be empty")
  @Email(message = "should be an email")
  private String email;

  @NotBlank(message = "can't be empty")
  private String password;
}

@Getter
@JsonRootName("user")
@NoArgsConstructor
class UpdateUserParam {
  @Email(message = "should be an email")
  private String email;

  private String username;
  private String password;
  private String bio;
  private String image;
}
