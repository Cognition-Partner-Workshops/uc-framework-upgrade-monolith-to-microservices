package io.spring.api.security;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jws;
import io.jsonwebtoken.Jwts;
import io.spring.core.user.AuthUser;
import java.io.IOException;
import java.util.Collections;
import java.util.Optional;
import javax.crypto.SecretKey;
import javax.crypto.spec.SecretKeySpec;
import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.web.filter.OncePerRequestFilter;

@SuppressWarnings("SpringJavaAutowiringInspection")
public class JwtTokenFilter extends OncePerRequestFilter {
  private final String header = "Authorization";
  private SecretKey signingKey;

  @Autowired
  public void setJwtSecret(@Value("${jwt.secret}") String secret) {
    this.signingKey =
        new SecretKeySpec(secret.getBytes(), io.jsonwebtoken.SignatureAlgorithm.HS512.getJcaName());
  }

  @Override
  protected void doFilterInternal(
      HttpServletRequest request, HttpServletResponse response, FilterChain filterChain)
      throws ServletException, IOException {
    getTokenString(request.getHeader(header))
        .ifPresent(
            token -> {
              try {
                Jws<Claims> claimsJws =
                    Jwts.parserBuilder().setSigningKey(signingKey).build().parseClaimsJws(token);
                Claims body = claimsJws.getBody();
                String id = body.getSubject();
                String username = (String) body.get("username");
                String email = (String) body.get("email");
                if (id != null
                    && SecurityContextHolder.getContext().getAuthentication() == null) {
                  AuthUser authUser = new AuthUser(id, username, email);
                  UsernamePasswordAuthenticationToken authenticationToken =
                      new UsernamePasswordAuthenticationToken(
                          authUser, null, Collections.emptyList());
                  authenticationToken.setDetails(
                      new WebAuthenticationDetailsSource().buildDetails(request));
                  SecurityContextHolder.getContext().setAuthentication(authenticationToken);
                }
              } catch (Exception e) {
                // Invalid token - do nothing, request proceeds unauthenticated
              }
            });

    filterChain.doFilter(request, response);
  }

  private Optional<String> getTokenString(String header) {
    if (header == null) {
      return Optional.empty();
    } else {
      String[] split = header.split(" ");
      if (split.length < 2) {
        return Optional.empty();
      } else {
        return Optional.ofNullable(split[1]);
      }
    }
  }
}
