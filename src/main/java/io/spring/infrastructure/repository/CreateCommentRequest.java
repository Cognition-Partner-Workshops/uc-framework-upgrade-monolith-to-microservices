package io.spring.infrastructure.repository;

import lombok.Data;
import lombok.NoArgsConstructor;
import org.joda.time.DateTime;

@Data
@NoArgsConstructor
public class CreateCommentRequest {
  private String id;
  private String body;
  private String userId;
  private String articleId;
  private DateTime createdAt;
}
