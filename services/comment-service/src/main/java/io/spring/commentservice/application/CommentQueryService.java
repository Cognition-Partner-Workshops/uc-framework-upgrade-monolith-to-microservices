package io.spring.commentservice.application;

import io.spring.commentservice.application.data.CommentData;
import io.spring.commentservice.infrastructure.client.UserServiceClient;
import io.spring.commentservice.infrastructure.mybatis.readservice.CommentReadService;
import io.spring.shared.data.ProfileData;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.stream.Collectors;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Service;

@Service
@AllArgsConstructor
public class CommentQueryService {
  private CommentReadService commentReadService;
  private UserServiceClient userServiceClient;

  public Optional<CommentData> findById(String id, String currentUserId) {
    CommentData commentData = commentReadService.findById(id);
    if (commentData == null) {
      return Optional.empty();
    }
    fillExtraInfo(List.of(commentData), currentUserId);
    return Optional.of(commentData);
  }

  public List<CommentData> findByArticleId(String articleId, String currentUserId) {
    List<CommentData> comments = commentReadService.findByArticleId(articleId);
    if (!comments.isEmpty()) {
      fillExtraInfo(comments, currentUserId);
    }
    return comments;
  }

  private void fillExtraInfo(List<CommentData> comments, String currentUserId) {
    if (currentUserId != null) {
      List<String> authorIds =
          comments.stream()
              .filter(c -> c.getProfileData() != null)
              .map(c -> c.getProfileData().getId())
              .distinct()
              .collect(Collectors.toList());
      if (!authorIds.isEmpty()) {
        Set<String> followingAuthors =
            userServiceClient.followingAuthors(currentUserId, authorIds);
        for (CommentData comment : comments) {
          if (comment.getProfileData() != null) {
            comment
                .getProfileData()
                .setFollowing(followingAuthors.contains(comment.getProfileData().getId()));
          }
        }
      }
    }
  }
}
