package io.spring.tags.api;

import io.spring.tags.model.Tag;
import io.spring.tags.repository.TagMapper;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import lombok.AllArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "/tags")
@AllArgsConstructor
public class TagsController {
  private TagMapper tagMapper;

  @GetMapping
  public ResponseEntity<Map<String, Object>> getTags() {
    List<String> tags = tagMapper.allTagNames();
    Map<String, Object> response = new HashMap<>();
    response.put("tags", tags);
    return ResponseEntity.ok(response);
  }

  @PostMapping
  public ResponseEntity<Tag> createTag(@RequestBody Map<String, String> body) {
    String name = body.get("name");
    if (name == null || name.isBlank()) {
      return ResponseEntity.badRequest().build();
    }
    Tag existing = tagMapper.findByName(name);
    if (existing != null) {
      return ResponseEntity.ok(existing);
    }
    Tag tag = new Tag(name);
    tagMapper.insert(tag);
    return ResponseEntity.status(201).body(tag);
  }
}
