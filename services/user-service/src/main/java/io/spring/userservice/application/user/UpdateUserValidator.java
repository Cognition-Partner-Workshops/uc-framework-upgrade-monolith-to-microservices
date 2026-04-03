package io.spring.userservice.application.user;

import io.spring.shared.util.Util;
import io.spring.userservice.core.user.UserRepository;
import javax.validation.ConstraintValidator;
import javax.validation.ConstraintValidatorContext;
import org.springframework.beans.factory.annotation.Autowired;

class UpdateUserValidator implements ConstraintValidator<UpdateUserConstraint, UpdateUserCommand> {

  @Autowired private UserRepository userRepository;

  @Override
  public boolean isValid(UpdateUserCommand value, ConstraintValidatorContext context) {
    String inputEmail = value.getParam().getEmail();
    String inputUsername = value.getParam().getUsername();
    boolean isEmailValid =
        Util.isEmpty(inputEmail)
            || inputEmail.equals(value.getTargetUser().getEmail())
            || !userRepository.findByEmail(inputEmail).isPresent();
    boolean isUsernameValid =
        Util.isEmpty(inputUsername)
            || inputUsername.equals(value.getTargetUser().getUsername())
            || !userRepository.findByUsername(inputUsername).isPresent();
    return isEmailValid && isUsernameValid;
  }
}
