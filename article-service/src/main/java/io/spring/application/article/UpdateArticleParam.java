package io.spring.application.article;

import com.fasterxml.jackson.annotation.JsonRootName;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;

@Getter
@NoArgsConstructor
@JsonRootName("article")
@AllArgsConstructor
public class UpdateArticleParam {
  private String title = "";
  private String body = "";
  private String description = "";
}
