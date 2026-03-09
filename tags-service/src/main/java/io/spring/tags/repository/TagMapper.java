package io.spring.tags.repository;

import io.spring.tags.model.Tag;
import java.util.List;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

@Mapper
public interface TagMapper {
  List<String> allTagNames();

  Tag findByName(@Param("name") String name);

  void insert(@Param("tag") Tag tag);
}
