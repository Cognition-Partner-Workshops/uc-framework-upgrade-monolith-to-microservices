package io.spring.application.comment;

import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.when;

import io.spring.application.CommentQueryService;
import io.spring.application.UserProfileFetcher;
import io.spring.application.data.CommentData;
import io.spring.application.data.ProfileData;
import io.spring.core.user.User;
import io.spring.infrastructure.mybatis.readservice.UserRelationshipQueryService;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;
import org.joda.time.DateTime;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.http.ResponseEntity;
import org.springframework.web.client.RestTemplate;

@ExtendWith(MockitoExtension.class)
public class CommentQueryServiceTest {

  @Mock private UserRelationshipQueryService userRelationshipQueryService;

  @Mock private RestTemplate restTemplate;

  @Mock private UserProfileFetcher userProfileFetcher;

  private CommentQueryService commentQueryService;

  private User user;

  @BeforeEach
  public void setUp() {
    user = new User("aisensiy@test.com", "aisensiy", "123", "", "");
    commentQueryService =
        new CommentQueryService(
            userRelationshipQueryService,
            restTemplate,
            "http://localhost:8081",
            userProfileFetcher);
  }

  @Test
  public void should_read_comment_success() {
    CommentQueryService.CommentResponse commentResponse = new CommentQueryService.CommentResponse();
    commentResponse.setId("comment-id");
    commentResponse.setBody("content");
    commentResponse.setUserId(user.getId());
    commentResponse.setArticleId("123");
    commentResponse.setCreatedAt(new DateTime());

    when(restTemplate.getForEntity(
            anyString(), eq(CommentQueryService.CommentResponse.class), eq("comment-id")))
        .thenReturn(ResponseEntity.ok(commentResponse));
    when(userProfileFetcher.fetchProfile(eq(user.getId())))
        .thenReturn(new ProfileData(user.getId(), user.getUsername(), "", "", false));

    Optional<CommentData> optional = commentQueryService.findById("comment-id", user);
    Assertions.assertTrue(optional.isPresent());
    CommentData commentData = optional.get();
    Assertions.assertEquals(commentData.getProfileData().getUsername(), user.getUsername());
  }

  @Test
  public void should_read_comments_of_article() {
    CommentQueryService.CommentResponse comment1 = new CommentQueryService.CommentResponse();
    comment1.setId("c1");
    comment1.setBody("content1");
    comment1.setUserId(user.getId());
    comment1.setArticleId("article-1");
    comment1.setCreatedAt(new DateTime());

    CommentQueryService.CommentResponse comment2 = new CommentQueryService.CommentResponse();
    comment2.setId("c2");
    comment2.setBody("content2");
    comment2.setUserId("user-2");
    comment2.setArticleId("article-1");
    comment2.setCreatedAt(new DateTime());

    CommentQueryService.CommentsListResponse listResponse =
        new CommentQueryService.CommentsListResponse();
    listResponse.setComments(Arrays.asList(comment1, comment2));

    when(restTemplate.getForEntity(
            anyString(), eq(CommentQueryService.CommentsListResponse.class), eq("article-1")))
        .thenReturn(ResponseEntity.ok(listResponse));
    when(userProfileFetcher.fetchProfile(eq(user.getId())))
        .thenReturn(new ProfileData(user.getId(), user.getUsername(), "", "", false));
    when(userProfileFetcher.fetchProfile(eq("user-2")))
        .thenReturn(new ProfileData("user-2", "user2", "", "", false));

    List<CommentData> comments = commentQueryService.findByArticleId("article-1", user);
    Assertions.assertEquals(comments.size(), 2);
  }
}
