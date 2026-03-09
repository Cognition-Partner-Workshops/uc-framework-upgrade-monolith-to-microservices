package io.spring.auth.api;

import io.spring.auth.application.data.UserData;
import io.spring.auth.core.service.JwtService;
import io.spring.auth.infrastructure.mybatis.readservice.UserReadService;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "/auth")
@AllArgsConstructor
public class AuthVerifyApi {

  private JwtService jwtService;
  private UserReadService userReadService;

  @GetMapping(path = "/verify")
  public ResponseEntity<Map<String, Object>> verifyToken(@RequestParam("token") String token) {
    Optional<String> userIdOpt = jwtService.getSubFromToken(token);
    if (userIdOpt.isPresent()) {
      UserData userData = userReadService.findById(userIdOpt.get());
      if (userData != null) {
        Map<String, Object> result = new HashMap<>();
        result.put("id", userData.getId());
        result.put("username", userData.getUsername());
        result.put("email", userData.getEmail());
        return ResponseEntity.ok(result);
      }
    }
    return ResponseEntity.status(401).build();
  }
}
