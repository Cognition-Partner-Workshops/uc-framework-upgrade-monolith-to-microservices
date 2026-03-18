package io.spring.userauth.api.exception;

import java.util.List;
import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class ErrorResource {
  private List<FieldErrorResource> fieldErrors;
}
