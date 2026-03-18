package io.spring.favorite.client;

import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
public class ArticleResponse {
  private String id;
  private String slug;
  private String title;
}
