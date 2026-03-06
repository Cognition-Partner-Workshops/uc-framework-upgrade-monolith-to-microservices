package io.spring.comments.controller;

import io.spring.comments.model.Comment;
import io.spring.comments.repository.CommentMapper;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import javax.validation.Valid;
import javax.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/comments")
@AllArgsConstructor
public class CommentsController {

  private CommentMapper commentMapper;

  @PostMapping
  public ResponseEntity<Comment> createComment(@Valid @RequestBody CreateCommentRequest request) {
    Comment comment = new Comment(request.getBody(), request.getUserId(), request.getArticleId());
    if (request.getId() != null) {
      comment.setId(request.getId());
    }
    if (request.getCreatedAt() != null) {
      comment.setCreatedAt(request.getCreatedAt());
    }
    commentMapper.insert(comment);
    return ResponseEntity.status(201).body(comment);
  }

  @GetMapping("/{id}")
  public ResponseEntity<Comment> getComment(@PathVariable("id") String id) {
    Comment comment = commentMapper.findById(id);
    if (comment == null) {
      return ResponseEntity.notFound().build();
    }
    return ResponseEntity.ok(comment);
  }

  @GetMapping
  public ResponseEntity<Map<String, Object>> getCommentsByArticle(
      @RequestParam("articleId") String articleId) {
    List<Comment> comments = commentMapper.findByArticleId(articleId);
    Map<String, Object> response = new HashMap<>();
    response.put("comments", comments);
    return ResponseEntity.ok(response);
  }

  @GetMapping("/find")
  public ResponseEntity<Comment> findByArticleAndId(
      @RequestParam("articleId") String articleId, @RequestParam("id") String id) {
    Comment comment = commentMapper.findByArticleIdAndId(articleId, id);
    if (comment == null) {
      return ResponseEntity.notFound().build();
    }
    return ResponseEntity.ok(comment);
  }

  @DeleteMapping("/{id}")
  public ResponseEntity<Void> deleteComment(@PathVariable("id") String id) {
    commentMapper.delete(id);
    return ResponseEntity.noContent().build();
  }
}

@Getter
@Setter
@NoArgsConstructor
class CreateCommentRequest {
  private String id;

  @NotBlank(message = "can't be empty")
  private String body;

  @NotBlank(message = "can't be empty")
  private String userId;

  @NotBlank(message = "can't be empty")
  private String articleId;

  private org.joda.time.DateTime createdAt;
}
