package io.spring.articleservice.api;

import io.spring.articleservice.application.TagsQueryService;
import java.util.HashMap;
import java.util.Map;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(path = "/tags")
public class TagsApi {
  private TagsQueryService tagsQueryService;

  @Autowired
  public TagsApi(TagsQueryService tagsQueryService) {
    this.tagsQueryService = tagsQueryService;
  }

  @GetMapping
  public ResponseEntity<?> getTags() {
    Map<String, Object> result = new HashMap<>();
    result.put("tags", tagsQueryService.allTags());
    return ResponseEntity.ok(result);
  }
}
