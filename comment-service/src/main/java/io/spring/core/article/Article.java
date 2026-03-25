package io.spring.core.article;

import java.util.UUID;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.NoArgsConstructor;
import org.joda.time.DateTime;

@Getter
@NoArgsConstructor
@EqualsAndHashCode(of = {"id"})
public class Article {
  private String userId;
  private String id;
  private String slug;
  private String title;
  private String description;
  private String body;
  private DateTime createdAt;
  private DateTime updatedAt;

  public Article(String title, String description, String body, String userId) {
    this.id = UUID.randomUUID().toString();
    this.slug = toSlug(title);
    this.title = title;
    this.description = description;
    this.body = body;
    this.userId = userId;
    this.createdAt = new DateTime();
    this.updatedAt = new DateTime();
  }

  private static String toSlug(String title) {
    return title.toLowerCase().replaceAll("[\\s\\?\\,\\.]+", "-");
  }
}
