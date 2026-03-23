package io.spring.userservice.infrastructure.mybatis;

import io.spring.userservice.core.FollowRelation;
import io.spring.userservice.core.User;
import io.spring.userservice.core.UserRepository;
import java.util.Optional;
import lombok.AllArgsConstructor;
import org.springframework.stereotype.Repository;

@Repository
@AllArgsConstructor
public class MyBatisUserRepository implements UserRepository {
  private final UserMapper userMapper;

  @Override
  public void save(User user) {
    if (userMapper.findById(user.getId()) == null) {
      userMapper.insert(user);
    } else {
      userMapper.update(user);
    }
  }

  @Override
  public Optional<User> findById(String id) {
    return Optional.ofNullable(userMapper.findById(id));
  }

  @Override
  public Optional<User> findByUsername(String username) {
    return Optional.ofNullable(userMapper.findByUsername(username));
  }

  @Override
  public Optional<User> findByEmail(String email) {
    return Optional.ofNullable(userMapper.findByEmail(email));
  }

  @Override
  public void saveRelation(FollowRelation followRelation) {
    userMapper.saveRelation(followRelation);
  }

  @Override
  public Optional<FollowRelation> findRelation(String userId, String targetId) {
    return Optional.ofNullable(userMapper.findRelation(userId, targetId));
  }

  @Override
  public void removeRelation(FollowRelation followRelation) {
    userMapper.deleteRelation(followRelation);
  }
}
