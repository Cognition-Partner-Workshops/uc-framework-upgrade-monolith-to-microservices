package io.spring.userservice.application.user;

import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import javax.validation.Constraint;
import javax.validation.Payload;

@Constraint(validatedBy = UpdateUserValidator.class)
@Retention(RetentionPolicy.RUNTIME)
@interface UpdateUserConstraint {
  String message() default "invalid update param";

  Class<?>[] groups() default {};

  Class<? extends Payload>[] payload() default {};
}
