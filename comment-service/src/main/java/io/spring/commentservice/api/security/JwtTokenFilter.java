package io.spring.commentservice.api.security;

import io.spring.commentservice.infrastructure.client.UserServiceClient;
import io.spring.commentservice.infrastructure.service.DefaultJwtService;
import java.io.IOException;
import java.util.Collections;
import java.util.Map;
import java.util.Optional;
import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.web.filter.OncePerRequestFilter;

public class JwtTokenFilter extends OncePerRequestFilter {
  @Autowired private DefaultJwtService jwtService;
  @Autowired private UserServiceClient userServiceClient;

  private final String header = "Authorization";

  @Override
  protected void doFilterInternal(
      HttpServletRequest request, HttpServletResponse response, FilterChain filterChain)
      throws ServletException, IOException {

    String authHeader = request.getHeader(header);
    getTokenString(authHeader)
        .flatMap(token -> jwtService.getSubFromToken(token))
        .ifPresent(
            userId -> {
              if (SecurityContextHolder.getContext().getAuthentication() == null) {
                // Build a simple principal map with the user ID from the JWT token.
                // For operations that need full user data, the API layer calls the User Service.
                Map<String, Object> userPrincipal =
                    Map.of("id", userId, "authHeader", authHeader != null ? authHeader : "");
                UsernamePasswordAuthenticationToken authenticationToken =
                    new UsernamePasswordAuthenticationToken(
                        userPrincipal, null, Collections.emptyList());
                authenticationToken.setDetails(
                    new WebAuthenticationDetailsSource().buildDetails(request));
                SecurityContextHolder.getContext().setAuthentication(authenticationToken);
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
