package io.spring.profileservice.core;

import java.util.Optional;

public interface FollowRelationRepository {
  void saveRelation(FollowRelation followRelation);

  Optional<FollowRelation> findRelation(String userId, String targetId);

  void removeRelation(FollowRelation followRelation);
}
