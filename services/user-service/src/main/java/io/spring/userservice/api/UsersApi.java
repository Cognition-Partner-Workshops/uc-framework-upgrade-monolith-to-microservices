package io.spring.userservice.api;

import com.fasterxml.jackson.annotation.JsonRootName;
import io.spring.shared.data.UserData;
import io.spring.shared.data.UserWithToken;
import io.spring.shared.exception.InvalidAuthenticationException;
import io.spring.shared.exception.InvalidRequestException;
import io.spring.shared.security.JwtService;
import io.spring.userservice.application.UserQueryService;
import io.spring.userservice.application.user.RegisterParam;
import io.spring.userservice.application.user.UserService;
import io.spring.userservice.core.user.User;
import io.spring.userservice.core.user.UserRepository;
import java.util.HashMap;
import java.util.Map;
import javax.validation.Valid;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/users")
public class UsersApi {
  private UserRepository userRepository;
  private UserQueryService userQueryService;
  private UserService userService;
  private JwtService jwtService;
  private PasswordEncoder passwordEncoder;

  @Autowired
  public UsersApi(
      UserRepository userRepository,
      UserQueryService userQueryService,
      UserService userService,
      JwtService jwtService,
      PasswordEncoder passwordEncoder) {
    this.userRepository = userRepository;
    this.userQueryService = userQueryService;
    this.userService = userService;
    this.jwtService = jwtService;
    this.passwordEncoder = passwordEncoder;
  }

  @PostMapping
  public ResponseEntity<?> createUser(
      @Valid @RequestBody RegisterParam registerParam, BindingResult bindingResult) {
    if (bindingResult.hasErrors()) {
      throw new InvalidRequestException(bindingResult);
    }
    User user = userService.createUser(registerParam);
    UserData userData =
        userQueryService.findById(user.getId()).orElseThrow(RuntimeException::new);
    return ResponseEntity.status(201)
        .body(userResponse(new UserWithToken(userData, jwtService.toToken(user.getId()))));
  }

  @PostMapping(path = "/login")
  public ResponseEntity<?> userLogin(@Valid @RequestBody LoginParam loginParam) {
    User user =
        userRepository
            .findByEmail(loginParam.getEmail())
            .orElseThrow(InvalidAuthenticationException::new);
    if (passwordEncoder.matches(loginParam.getPassword(), user.getPassword())) {
      UserData userData =
          userQueryService.findById(user.getId()).orElseThrow(RuntimeException::new);
      return ResponseEntity.ok(
          userResponse(new UserWithToken(userData, jwtService.toToken(user.getId()))));
    } else {
      throw new InvalidAuthenticationException();
    }
  }

  private Map<String, Object> userResponse(UserWithToken userWithToken) {
    return new HashMap<String, Object>() {
      {
        put("user", userWithToken);
      }
    };
  }

  @Getter
  @JsonRootName("user")
  @NoArgsConstructor
  @AllArgsConstructor
  static class LoginParam {
    private String email;
    private String password;
  }
}
