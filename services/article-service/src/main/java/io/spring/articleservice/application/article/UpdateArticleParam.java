package io.spring.articleservice.application.article;

import com.fasterxml.jackson.annotation.JsonRootName;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;

@Getter
@NoArgsConstructor
@AllArgsConstructor
@JsonRootName("article")
@Builder
public class UpdateArticleParam {
  @Builder.Default private String title = "";
  @Builder.Default private String body = "";
  @Builder.Default private String description = "";
}
