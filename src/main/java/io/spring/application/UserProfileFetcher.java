package io.spring.application;

import io.spring.application.data.ProfileData;
import io.spring.application.data.UserData;
import io.spring.infrastructure.mybatis.readservice.UserReadService;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

@Service
public class UserProfileFetcher {
  private final UserReadService userReadService;
  private final String defaultImage;

  public UserProfileFetcher(
      UserReadService userReadService, @Value("${image.default}") String defaultImage) {
    this.userReadService = userReadService;
    this.defaultImage = defaultImage;
  }

  public ProfileData fetchProfile(String userId) {
    UserData userData = userReadService.findById(userId);
    if (userData == null) {
      return new ProfileData(userId, "unknown", "", defaultImage, false);
    }
    return new ProfileData(
        userData.getId(), userData.getUsername(), userData.getBio(), userData.getImage(), false);
  }
}
