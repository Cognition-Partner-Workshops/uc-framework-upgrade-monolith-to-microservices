package io.spring.profileservice.infrastructure.mybatis.mapper;

import io.spring.profileservice.core.FollowRelation;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

@Mapper
public interface FollowRelationMapper {
  FollowRelation findRelation(@Param("userId") String userId, @Param("targetId") String targetId);

  void saveRelation(@Param("followRelation") FollowRelation followRelation);

  void deleteRelation(@Param("followRelation") FollowRelation followRelation);
}
