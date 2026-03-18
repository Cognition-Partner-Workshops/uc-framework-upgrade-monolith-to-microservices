package io.spring.application.comment;

import io.spring.application.CommentQueryService;
import io.spring.application.data.CommentData;
import io.spring.core.article.Article;
import io.spring.core.article.ArticleRepository;
import io.spring.core.comment.Comment;
import io.spring.core.comment.CommentRepository;
import io.spring.core.user.AuthUser;
import io.spring.infrastructure.DbTestBase;
import io.spring.infrastructure.client.UserServiceClient;
import io.spring.infrastructure.repository.MyBatisArticleRepository;
import io.spring.infrastructure.repository.MyBatisCommentRepository;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Optional;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.context.annotation.Import;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

@Import({
  MyBatisCommentRepository.class,
  CommentQueryService.class,
  MyBatisArticleRepository.class
})
public class CommentQueryServiceTest extends DbTestBase {
  @Autowired private CommentRepository commentRepository;

  @Autowired private CommentQueryService commentQueryService;

  @Autowired private ArticleRepository articleRepository;

  @MockBean private UserServiceClient userServiceClient;

  private AuthUser user;

  @BeforeEach
  public void setUp() {
    user = new AuthUser("user-id-1", "aisensiy", "aisensiy@test.com");
    insertUserRow(user);
  }

  @Test
  public void should_read_comment_success() {
    Comment comment = new Comment("content", user.getId(), "123");
    commentRepository.save(comment);
    when(userServiceClient.isUserFollowing(any(), any())).thenReturn(false);

    Optional<CommentData> optional = commentQueryService.findById(comment.getId(), user);
    Assertions.assertTrue(optional.isPresent());
    CommentData commentData = optional.get();
    Assertions.assertEquals(commentData.getProfileData().getUsername(), user.getUsername());
  }

  @Test
  public void should_read_comments_of_article() {
    Article article = new Article("title", "desc", "body", Arrays.asList("java"), user.getId());
    articleRepository.save(article);

    AuthUser user2 = new AuthUser("user-id-2", "user2", "user2@email.com");
    insertUserRow(user2);
    when(userServiceClient.followingAuthors(any(), any()))
        .thenReturn(new HashSet<>(Arrays.asList(user2.getId())));

    Comment comment1 = new Comment("content1", user.getId(), article.getId());
    commentRepository.save(comment1);
    Comment comment2 = new Comment("content2", user2.getId(), article.getId());
    commentRepository.save(comment2);

    List<CommentData> comments = commentQueryService.findByArticleId(article.getId(), user);
    Assertions.assertEquals(comments.size(), 2);
  }
}
