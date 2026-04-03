package io.spring.userservice.api;

import io.spring.shared.data.UserData;
import io.spring.shared.data.UserWithToken;
import io.spring.shared.exception.InvalidRequestException;
import io.spring.shared.exception.ResourceNotFoundException;
import io.spring.shared.security.JwtService;
import io.spring.userservice.application.UserQueryService;
import io.spring.userservice.application.user.UpdateUserCommand;
import io.spring.userservice.application.user.UpdateUserParam;
import io.spring.userservice.application.user.UserService;
import io.spring.userservice.core.user.User;
import java.util.HashMap;
import java.util.Map;
import javax.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/user")
public class CurrentUserApi {
  private UserQueryService userQueryService;
  private UserService userService;
  private JwtService jwtService;

  @Autowired
  public CurrentUserApi(
      UserQueryService userQueryService, UserService userService, JwtService jwtService) {
    this.userQueryService = userQueryService;
    this.userService = userService;
    this.jwtService = jwtService;
  }

  @GetMapping
  public ResponseEntity<?> currentUser(@AuthenticationPrincipal User currentUser) {
    UserData userData =
        userQueryService
            .findById(currentUser.getId())
            .orElseThrow(ResourceNotFoundException::new);
    return ResponseEntity.ok(
        userResponse(new UserWithToken(userData, jwtService.toToken(currentUser.getId()))));
  }

  @PutMapping
  public ResponseEntity<?> updateProfile(
      @AuthenticationPrincipal User currentUser,
      @Valid @RequestBody UpdateUserParam updateUserParam,
      BindingResult bindingResult) {
    if (bindingResult.hasErrors()) {
      throw new InvalidRequestException(bindingResult);
    }
    userService.updateUser(new UpdateUserCommand(currentUser, updateUserParam));
    UserData userData =
        userQueryService
            .findById(currentUser.getId())
            .orElseThrow(ResourceNotFoundException::new);
    return ResponseEntity.ok(
        userResponse(new UserWithToken(userData, jwtService.toToken(currentUser.getId()))));
  }

  private Map<String, Object> userResponse(UserWithToken userWithToken) {
    return new HashMap<String, Object>() {
      {
        put("user", userWithToken);
      }
    };
  }
}
