package io.spring.commentservice.core.comment;

import java.util.Optional;
import org.springframework.stereotype.Repository;

@Repository
public interface CommentRepository {
  void save(Comment comment);

  Optional<Comment> findById(String articleId, String id);

  void remove(Comment comment);
}
