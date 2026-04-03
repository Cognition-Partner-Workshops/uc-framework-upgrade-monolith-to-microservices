package io.spring.commentservice.api;

import io.spring.commentservice.application.CommentQueryService;
import io.spring.commentservice.application.data.CommentData;
import io.spring.commentservice.core.comment.Comment;
import io.spring.commentservice.core.comment.CommentRepository;
import io.spring.shared.exception.InvalidRequestException;
import io.spring.shared.exception.NoAuthorizationException;
import io.spring.shared.exception.ResourceNotFoundException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import javax.validation.Valid;
import javax.validation.constraints.NotBlank;
import com.fasterxml.jackson.annotation.JsonRootName;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/articles/{slug}/comments")
public class CommentsApi {
  private CommentRepository commentRepository;
  private CommentQueryService commentQueryService;

  @Autowired
  public CommentsApi(
      CommentRepository commentRepository, CommentQueryService commentQueryService) {
    this.commentRepository = commentRepository;
    this.commentQueryService = commentQueryService;
  }

  @PostMapping
  public ResponseEntity<?> createComment(
      @PathVariable("slug") String slug,
      @AuthenticationPrincipal String userId,
      @Valid @RequestBody NewCommentParam newCommentParam,
      BindingResult bindingResult) {
    if (bindingResult.hasErrors()) {
      throw new InvalidRequestException(bindingResult);
    }
    Comment comment = new Comment(newCommentParam.getBody(), userId, slug);
    commentRepository.save(comment);
    return commentQueryService
        .findById(comment.getId(), userId)
        .map(c -> ResponseEntity.status(201).body(commentResponse(c)))
        .orElseThrow(ResourceNotFoundException::new);
  }

  @GetMapping
  public ResponseEntity<?> getComments(
      @PathVariable("slug") String slug,
      @AuthenticationPrincipal String userId) {
    List<CommentData> comments = commentQueryService.findByArticleId(slug, userId);
    Map<String, Object> result = new HashMap<>();
    result.put("comments", comments);
    return ResponseEntity.ok(result);
  }

  @DeleteMapping(path = "/{id}")
  public ResponseEntity<?> deleteComment(
      @PathVariable("slug") String slug,
      @PathVariable("id") String commentId,
      @AuthenticationPrincipal String userId) {
    return commentRepository
        .findById(slug, commentId)
        .map(
            comment -> {
              if (!comment.getUserId().equals(userId)) {
                throw new NoAuthorizationException();
              }
              commentRepository.remove(comment);
              return ResponseEntity.noContent().build();
            })
        .orElseThrow(ResourceNotFoundException::new);
  }

  private Map<String, Object> commentResponse(CommentData commentData) {
    return new HashMap<String, Object>() {
      {
        put("comment", commentData);
      }
    };
  }

  @Getter
  @NoArgsConstructor
  @AllArgsConstructor
  @JsonRootName("comment")
  static class NewCommentParam {
    @NotBlank(message = "can't be empty")
    private String body;
  }
}
