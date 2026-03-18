package io.spring.profileservice.infrastructure.repository;

import io.spring.profileservice.core.FollowRelation;
import io.spring.profileservice.core.FollowRelationRepository;
import io.spring.profileservice.infrastructure.mybatis.mapper.FollowRelationMapper;
import java.util.Optional;
import org.springframework.stereotype.Repository;

@Repository
public class MyBatisFollowRelationRepository implements FollowRelationRepository {
  private final FollowRelationMapper followRelationMapper;

  public MyBatisFollowRelationRepository(FollowRelationMapper followRelationMapper) {
    this.followRelationMapper = followRelationMapper;
  }

  @Override
  public void saveRelation(FollowRelation followRelation) {
    if (!findRelation(followRelation.getUserId(), followRelation.getTargetId()).isPresent()) {
      followRelationMapper.saveRelation(followRelation);
    }
  }

  @Override
  public Optional<FollowRelation> findRelation(String userId, String targetId) {
    return Optional.ofNullable(followRelationMapper.findRelation(userId, targetId));
  }

  @Override
  public void removeRelation(FollowRelation followRelation) {
    followRelationMapper.deleteRelation(followRelation);
  }
}
