package io.spring.infrastructure.repository;

import io.spring.core.comment.Comment;
import io.spring.core.comment.CommentRepository;
import java.util.Optional;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

@Component
public class HttpCommentRepository implements CommentRepository {

  private final RestTemplate restTemplate;
  private final String commentsServiceUrl;

  public HttpCommentRepository(
      RestTemplate restTemplate, @Value("${comments.service.url}") String commentsServiceUrl) {
    this.restTemplate = restTemplate;
    this.commentsServiceUrl = commentsServiceUrl;
  }

  @Override
  public void save(Comment comment) {
    CreateCommentRequest request = new CreateCommentRequest();
    request.setId(comment.getId());
    request.setBody(comment.getBody());
    request.setUserId(comment.getUserId());
    request.setArticleId(comment.getArticleId());
    request.setCreatedAt(comment.getCreatedAt());
    restTemplate.postForEntity(commentsServiceUrl + "/api/comments", request, Object.class);
  }

  @Override
  public Optional<Comment> findById(String articleId, String id) {
    try {
      ResponseEntity<Comment> response =
          restTemplate.getForEntity(
              commentsServiceUrl + "/api/comments/find?articleId={articleId}&id={id}",
              Comment.class,
              articleId,
              id);
      return Optional.ofNullable(response.getBody());
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  @Override
  public void remove(Comment comment) {
    restTemplate.delete(commentsServiceUrl + "/api/comments/{id}", comment.getId());
  }
}
