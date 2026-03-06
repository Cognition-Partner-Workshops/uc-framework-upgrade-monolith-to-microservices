package io.spring.application;

import io.spring.application.data.CommentData;
import io.spring.application.data.ProfileData;
import io.spring.core.user.User;
import io.spring.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.stream.Collectors;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.joda.time.DateTime;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

@Service
public class CommentQueryService {
  private UserRelationshipQueryService userRelationshipQueryService;
  private RestTemplate restTemplate;
  private String commentsServiceUrl;
  private UserProfileFetcher userProfileFetcher;

  public CommentQueryService(
      UserRelationshipQueryService userRelationshipQueryService,
      RestTemplate restTemplate,
      @Value("${comments.service.url}") String commentsServiceUrl,
      UserProfileFetcher userProfileFetcher) {
    this.userRelationshipQueryService = userRelationshipQueryService;
    this.restTemplate = restTemplate;
    this.commentsServiceUrl = commentsServiceUrl;
    this.userProfileFetcher = userProfileFetcher;
  }

  public Optional<CommentData> findById(String id, User user) {
    try {
      ResponseEntity<CommentResponse> response =
          restTemplate.getForEntity(
              commentsServiceUrl + "/api/comments/{id}", CommentResponse.class, id);
      CommentResponse comment = response.getBody();
      if (comment == null) {
        return Optional.empty();
      }
      CommentData commentData = toCommentData(comment);
      if (user != null && commentData.getProfileData() != null) {
        commentData
            .getProfileData()
            .setFollowing(
                userRelationshipQueryService.isUserFollowing(
                    user.getId(), commentData.getProfileData().getId()));
      }
      return Optional.of(commentData);
    } catch (HttpClientErrorException.NotFound e) {
      return Optional.empty();
    }
  }

  public List<CommentData> findByArticleId(String articleId, User user) {
    ResponseEntity<CommentsListResponse> response =
        restTemplate.getForEntity(
            commentsServiceUrl + "/api/comments?articleId={articleId}",
            CommentsListResponse.class,
            articleId);
    CommentsListResponse body = response.getBody();
    if (body == null || body.getComments() == null) {
      return new ArrayList<>();
    }
    List<CommentData> comments =
        body.getComments().stream().map(this::toCommentData).collect(Collectors.toList());
    if (comments.size() > 0 && user != null) {
      Set<String> followingAuthors =
          userRelationshipQueryService.followingAuthors(
              user.getId(),
              comments.stream()
                  .map(commentData -> commentData.getProfileData().getId())
                  .collect(Collectors.toList()));
      comments.forEach(
          commentData -> {
            if (followingAuthors.contains(commentData.getProfileData().getId())) {
              commentData.getProfileData().setFollowing(true);
            }
          });
    }
    return comments;
  }

  public CursorPager<CommentData> findByArticleIdWithCursor(
      String articleId, User user, CursorPageParameter<DateTime> page) {
    List<CommentData> allComments = findByArticleId(articleId, user);

    List<CommentData> filtered = new ArrayList<>(allComments);

    if (page.getCursor() != null) {
      if (page.isNext()) {
        filtered =
            filtered.stream()
                .filter(c -> c.getCreatedAt().isBefore(page.getCursor()))
                .collect(Collectors.toList());
      } else {
        filtered =
            filtered.stream()
                .filter(c -> c.getCreatedAt().isAfter(page.getCursor()))
                .collect(Collectors.toList());
      }
    }

    if (page.isNext()) {
      filtered.sort((a, b) -> b.getCreatedAt().compareTo(a.getCreatedAt()));
    } else {
      filtered.sort((a, b) -> a.getCreatedAt().compareTo(b.getCreatedAt()));
    }

    boolean hasExtra = filtered.size() > page.getLimit();
    if (hasExtra) {
      filtered = filtered.subList(0, page.getLimit());
    }
    if (!page.isNext()) {
      Collections.reverse(filtered);
    }
    return new CursorPager<>(filtered, page.getDirection(), hasExtra);
  }

  private CommentData toCommentData(CommentResponse comment) {
    ProfileData profileData = userProfileFetcher.fetchProfile(comment.getUserId());
    return new CommentData(
        comment.getId(),
        comment.getBody(),
        comment.getArticleId(),
        comment.getCreatedAt(),
        comment.getCreatedAt(),
        profileData);
  }

  @Getter
  @Setter
  @NoArgsConstructor
  public static class CommentResponse {
    private String id;
    private String body;
    private String userId;
    private String articleId;
    private DateTime createdAt;
  }

  @Getter
  @Setter
  @NoArgsConstructor
  public static class CommentsListResponse {
    private List<CommentResponse> comments;
  }
}
