package io.spring.application;

import io.spring.application.data.CommentData;
import io.spring.core.user.User;
import io.spring.infrastructure.mybatis.readservice.CommentReadService;
import io.spring.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.List;
import java.util.Optional;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@AllArgsConstructor
public class CommentQueryService {
  private CommentReadService commentReadService;
  private UserRelationshipQueryService userRelationshipQueryService;

  public Optional<CommentData> findById(String articleId, String id) {
    CommentData commentData = commentReadService.findById(id);
    if (commentData == null) {
      return Optional.empty();
    } else {
      return Optional.of(commentData);
    }
  }

  public List<CommentData> findByArticleId(String articleId, User user) {
    List<CommentData> comments = commentReadService.findByArticleId(articleId);
    if (user != null) {
      comments.forEach(
          commentData -> {
            if (commentData.getProfileData() != null) {
              commentData
                  .getProfileData()
                  .setFollowing(
                      userRelationshipQueryService.isUserFollowing(
                          user.getId(), commentData.getProfileData().getId()));
            }
          });
    }
    return comments;
  }
}
