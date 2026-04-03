package io.spring.userservice.application.user;

import io.spring.shared.exception.InvalidRequestException;
import io.spring.userservice.core.user.User;
import io.spring.userservice.core.user.UserRepository;
import javax.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.validation.BeanPropertyBindingResult;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;

@Service
public class UserService {
  private UserRepository userRepository;
  private String defaultImage;
  private PasswordEncoder passwordEncoder;
  private Validator validator;

  @Autowired
  public UserService(
      UserRepository userRepository,
      @Value("${image.default}") String defaultImage,
      PasswordEncoder passwordEncoder,
      Validator validator) {
    this.userRepository = userRepository;
    this.defaultImage = defaultImage;
    this.passwordEncoder = passwordEncoder;
    this.validator = validator;
  }

  public User createUser(RegisterParam registerParam) {
    User user =
        new User(
            registerParam.getEmail(),
            registerParam.getUsername(),
            passwordEncoder.encode(registerParam.getPassword()),
            "",
            defaultImage);
    userRepository.save(user);
    return user;
  }

  public void updateUser(@Valid UpdateUserCommand command) {
    Errors errors = new BeanPropertyBindingResult(command, "user");
    validator.validate(command, errors);
    if (errors.hasErrors()) {
      throw new InvalidRequestException(errors);
    }
    User user = command.getTargetUser();
    UpdateUserParam param = command.getParam();
    user.update(
        param.getEmail(),
        param.getUsername(),
        param.getPassword().isEmpty() ? "" : passwordEncoder.encode(param.getPassword()),
        param.getBio(),
        param.getImage());
    userRepository.save(user);
  }
}
