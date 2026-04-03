package io.spring.articleservice.core.article;

import io.spring.shared.util.Util;
import java.util.Arrays;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;
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
  private List<Tag> tags;
  private DateTime createdAt;
  private DateTime updatedAt;

  public Article(
      String title, String description, String body, String[] tagList, String userId) {
    this(title, description, body, tagList, userId, new DateTime());
  }

  public Article(
      String title,
      String description,
      String body,
      String[] tagList,
      String userId,
      DateTime createdAt) {
    this.id = UUID.randomUUID().toString();
    this.slug = toSlug(title);
    this.title = title;
    this.description = description;
    this.body = body;
    this.tags =
        Arrays.stream(tagList).collect(Collectors.toList()).stream()
            .map(Tag::new)
            .collect(Collectors.toList());
    this.userId = userId;
    this.createdAt = createdAt;
    this.updatedAt = createdAt;
  }

  public void update(String title, String description, String body) {
    if (!Util.isEmpty(title)) {
      this.title = title;
      this.slug = toSlug(title);
    }
    if (!Util.isEmpty(description)) {
      this.description = description;
    }
    if (!Util.isEmpty(body)) {
      this.body = body;
    }
    this.updatedAt = new DateTime();
  }

  public static String toSlug(String title) {
    return title.toLowerCase().replaceAll("[&|\\uFE30-\\uFFA0'\"\\s?,\\.]+", "-");
  }
}
