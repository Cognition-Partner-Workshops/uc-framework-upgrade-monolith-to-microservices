-- Seed Tags
INSERT INTO tags (id, name) VALUES
  ('tag-1', 'java'),
  ('tag-2', 'spring'),
  ('tag-3', 'microservices'),
  ('tag-4', 'docker'),
  ('tag-5', 'kubernetes'),
  ('tag-6', 'devops'),
  ('tag-7', 'testing');

-- Seed Articles (user IDs reference User Service)
INSERT INTO articles (id, user_id, slug, title, description, body, created_at, updated_at) VALUES
  ('article-1', 'user-1', 'getting-started-with-spring-boot', 'Getting Started with Spring Boot', 'A comprehensive guide to Spring Boot', 'Spring Boot makes it easy to create stand-alone, production-grade Spring based Applications...', '2024-01-15 10:00:00', '2024-01-15 10:00:00'),
  ('article-2', 'user-1', 'microservices-architecture-patterns', 'Microservices Architecture Patterns', 'Learn about common microservices patterns', 'Microservices architecture has become the de facto standard for building modern applications...', '2024-01-16 11:00:00', '2024-01-16 11:00:00'),
  ('article-3', 'user-2', 'docker-best-practices', 'Docker Best Practices', 'Tips for writing efficient Dockerfiles', 'Docker containers have revolutionized the way we deploy applications...', '2024-01-17 12:00:00', '2024-01-17 12:00:00'),
  ('article-4', 'user-2', 'kubernetes-deployment-strategies', 'Kubernetes Deployment Strategies', 'Different ways to deploy on K8s', 'Kubernetes offers several deployment strategies including rolling updates...', '2024-01-18 13:00:00', '2024-01-18 13:00:00'),
  ('article-5', 'user-3', 'ci-cd-pipeline-setup', 'CI/CD Pipeline Setup', 'Setting up automated pipelines', 'Continuous Integration and Continuous Deployment are essential practices...', '2024-01-19 14:00:00', '2024-01-19 14:00:00');

-- Article-Tag Relations
INSERT INTO article_tags (article_id, tag_id) VALUES
  ('article-1', 'tag-1'),
  ('article-1', 'tag-2'),
  ('article-2', 'tag-3'),
  ('article-2', 'tag-2'),
  ('article-3', 'tag-4'),
  ('article-4', 'tag-5'),
  ('article-4', 'tag-4'),
  ('article-5', 'tag-6'),
  ('article-5', 'tag-7');

-- Article Favorites
INSERT INTO article_favorites (article_id, user_id) VALUES
  ('article-1', 'user-2'),
  ('article-1', 'user-3'),
  ('article-2', 'user-2'),
  ('article-3', 'user-1'),
  ('article-5', 'user-1');
