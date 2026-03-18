package io.spring.api;

import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.spring.application.data.UserData;
import io.spring.core.user.AuthUser;
import io.spring.infrastructure.client.UserServiceClient;
import java.util.Date;
import java.util.Optional;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import org.junit.jupiter.api.BeforeEach;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.mock.mockito.MockBean;

import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.when;

abstract class TestWithCurrentUser {
  @MockBean protected UserServiceClient userServiceClient;

  @Value("${jwt.secret}")
  protected String jwtSecret;

  protected AuthUser user;
  protected UserData userData;
  protected String token;
  protected String email;
  protected String username;
  protected String defaultAvatar;

  protected void userFixture() {
    email = "john@jacob.com";
    username = "johnjacob";
    defaultAvatar = "https://static.productionready.io/images/smiley-cyrus.jpg";

    String userId = "user-id-123";
    user = new AuthUser(userId, username, email);

    userData = new UserData(user.getId(), email, username, "", defaultAvatar);
    when(userServiceClient.findUserById(eq(user.getId()))).thenReturn(Optional.of(userData));
    when(userServiceClient.findUserByUsername(eq(username))).thenReturn(Optional.of(userData));

    // Generate a real JWT token so JwtTokenFilter can parse it
    SecretKey signingKey =
        new SecretKeySpec(jwtSecret.getBytes(), SignatureAlgorithm.HS512.getJcaName());
    token =
        Jwts.builder()
            .setSubject(userId)
            .claim("username", username)
            .claim("email", email)
            .setExpiration(new Date(System.currentTimeMillis() + 86400000))
            .signWith(signingKey, SignatureAlgorithm.HS512)
            .compact();
  }

  @BeforeEach
  public void setUp() throws Exception {
    userFixture();
  }
}
