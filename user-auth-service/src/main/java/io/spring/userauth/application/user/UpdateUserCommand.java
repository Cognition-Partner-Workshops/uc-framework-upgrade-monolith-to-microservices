package io.spring.userauth.application.user;

import io.spring.userauth.core.User;
import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
@UpdateUserConstraint
public class UpdateUserCommand {

  private User targetUser;
  private UpdateUserParam param;
}
