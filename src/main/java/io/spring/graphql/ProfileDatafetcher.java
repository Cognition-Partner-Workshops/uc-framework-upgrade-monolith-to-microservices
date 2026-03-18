package io.spring.graphql;

import com.netflix.graphql.dgs.DgsComponent;
import com.netflix.graphql.dgs.DgsData;
import com.netflix.graphql.dgs.InputArgument;
import graphql.schema.DataFetchingEnvironment;
import io.spring.api.exception.ResourceNotFoundException;
import io.spring.application.data.ArticleData;
import io.spring.application.data.CommentData;
import io.spring.application.data.UserData;
import io.spring.core.user.AuthUser;
import io.spring.graphql.DgsConstants.ARTICLE;
import io.spring.graphql.DgsConstants.COMMENT;
import io.spring.graphql.DgsConstants.QUERY;
import io.spring.graphql.DgsConstants.USER;
import io.spring.graphql.types.Article;
import io.spring.graphql.types.Comment;
import io.spring.graphql.types.Profile;
import io.spring.graphql.types.ProfilePayload;
import io.spring.infrastructure.client.UserServiceClient;
import java.util.Map;
import lombok.AllArgsConstructor;

@DgsComponent
@AllArgsConstructor
public class ProfileDatafetcher {

  private UserServiceClient userServiceClient;

  @DgsData(parentType = USER.TYPE_NAME, field = USER.Profile)
  public Profile getUserProfile(DataFetchingEnvironment dataFetchingEnvironment) {
    AuthUser user = dataFetchingEnvironment.getLocalContext();
    String username = user.getUsername();
    return queryProfile(username);
  }

  @DgsData(parentType = ARTICLE.TYPE_NAME, field = ARTICLE.Author)
  public Profile getAuthor(DataFetchingEnvironment dataFetchingEnvironment) {
    Map<String, ArticleData> map = dataFetchingEnvironment.getLocalContext();
    Article article = dataFetchingEnvironment.getSource();
    return queryProfile(map.get(article.getSlug()).getProfileData().getUsername());
  }

  @DgsData(parentType = COMMENT.TYPE_NAME, field = COMMENT.Author)
  public Profile getCommentAuthor(DataFetchingEnvironment dataFetchingEnvironment) {
    Comment comment = dataFetchingEnvironment.getSource();
    Map<String, CommentData> map = dataFetchingEnvironment.getLocalContext();
    return queryProfile(map.get(comment.getId()).getProfileData().getUsername());
  }

  @DgsData(parentType = DgsConstants.QUERY_TYPE, field = QUERY.Profile)
  public ProfilePayload queryProfile(
      @InputArgument("username") String username,
      DataFetchingEnvironment dataFetchingEnvironment) {
    Profile profile = queryProfile(dataFetchingEnvironment.getArgument("username"));
    return ProfilePayload.newBuilder().profile(profile).build();
  }

  private Profile queryProfile(String username) {
    AuthUser current = SecurityUtil.getCurrentUser().orElse(null);
    UserData userData =
        userServiceClient
            .findUserByUsername(username)
            .orElseThrow(ResourceNotFoundException::new);
    boolean following =
        current != null && userServiceClient.isUserFollowing(current.getId(), userData.getId());
    return Profile.newBuilder()
        .username(userData.getUsername())
        .bio(userData.getBio())
        .image(userData.getImage())
        .following(following)
        .build();
  }
}
