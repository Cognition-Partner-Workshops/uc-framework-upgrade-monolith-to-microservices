package io.spring.userservice.application.user;

import io.spring.userservice.core.user.User;
import javax.validation.Valid;
import javax.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
@UpdateUserConstraint
public class UpdateUserCommand {
  @NotNull private User targetUser;
  @NotNull @Valid private UpdateUserParam param;
}
