package io.spring.infrastructure;

import io.spring.core.user.AuthUser;
import org.mybatis.spring.boot.test.autoconfigure.MybatisTest;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureTestDatabase.Replace;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.test.context.ActiveProfiles;

@ActiveProfiles("test")
@AutoConfigureTestDatabase(replace = Replace.NONE)
@MybatisTest
public abstract class DbTestBase {

  @Autowired protected JdbcTemplate jdbcTemplate;

  protected void insertUserRow(AuthUser user) {
    jdbcTemplate.update(
        "INSERT INTO users (id, username, email, password, bio, image) VALUES (?, ?, ?, ?, ?, ?)",
        user.getId(),
        user.getUsername(),
        user.getEmail(),
        "password",
        "",
        "");
  }
}
