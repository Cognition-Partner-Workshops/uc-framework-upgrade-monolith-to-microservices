package io.spring.graphql;

import com.netflix.graphql.dgs.DgsComponent;
import com.netflix.graphql.dgs.DgsData;
import graphql.execution.DataFetcherResult;
import graphql.schema.DataFetchingEnvironment;
import io.spring.application.data.UserData;
import io.spring.core.user.AuthUser;
import io.spring.graphql.DgsConstants.QUERY;
import io.spring.graphql.DgsConstants.USERPAYLOAD;
import io.spring.graphql.types.User;
import io.spring.infrastructure.client.UserServiceClient;
import lombok.AllArgsConstructor;
import org.springframework.security.authentication.AnonymousAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RequestHeader;

@DgsComponent
@AllArgsConstructor
public class MeDatafetcher {
  private UserServiceClient userServiceClient;

  @DgsData(parentType = DgsConstants.QUERY_TYPE, field = QUERY.Me)
  public DataFetcherResult<User> getMe(
      @RequestHeader(value = "Authorization") String authorization,
      DataFetchingEnvironment dataFetchingEnvironment) {
    Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
    if (authentication instanceof AnonymousAuthenticationToken
        || authentication.getPrincipal() == null) {
      return null;
    }
    AuthUser authUser = (AuthUser) authentication.getPrincipal();
    UserData userData =
        userServiceClient
            .findUserById(authUser.getId())
            .orElse(new UserData(authUser.getId(), authUser.getEmail(), authUser.getUsername(), null, null));
    String token = authorization.split(" ")[1];
    User result =
        User.newBuilder()
            .email(userData.getEmail())
            .username(userData.getUsername())
            .token(token)
            .build();
    return DataFetcherResult.<User>newResult().data(result).localContext(authUser).build();
  }

  @DgsData(parentType = USERPAYLOAD.TYPE_NAME, field = USERPAYLOAD.User)
  public DataFetcherResult<User> getUserPayloadUser(
      DataFetchingEnvironment dataFetchingEnvironment) {
    AuthUser authUser = dataFetchingEnvironment.getLocalContext();
    UserData userData =
        userServiceClient
            .findUserById(authUser.getId())
            .orElse(new UserData(authUser.getId(), authUser.getEmail(), authUser.getUsername(), null, null));
    User result =
        User.newBuilder()
            .email(userData.getEmail())
            .username(userData.getUsername())
            .build();
    return DataFetcherResult.<User>newResult().data(result).localContext(authUser).build();
  }
}
