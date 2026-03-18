package io.spring.favorite.client;

import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
public class UserResponse {
  private String id;
  private String email;
  private String username;
  private String bio;
  private String image;
}
