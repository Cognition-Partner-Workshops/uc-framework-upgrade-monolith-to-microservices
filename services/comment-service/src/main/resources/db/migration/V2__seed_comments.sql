-- Seed Comments (article_id stores slug for routing, user_id references User Service)
INSERT INTO comments (id, body, article_id, user_id, created_at, updated_at) VALUES
  ('comment-1', 'Great article on Spring Boot! Very helpful for beginners.', 'getting-started-with-spring-boot', 'user-2', '2024-01-15 12:00:00', '2024-01-15 12:00:00'),
  ('comment-2', 'I would love to see more examples of microservices patterns.', 'microservices-architecture-patterns', 'user-3', '2024-01-16 14:00:00', '2024-01-16 14:00:00'),
  ('comment-3', 'Docker best practices are essential for modern development.', 'docker-best-practices', 'user-1', '2024-01-17 15:00:00', '2024-01-17 15:00:00'),
  ('comment-4', 'Kubernetes is the future of container orchestration!', 'kubernetes-deployment-strategies', 'user-1', '2024-01-18 16:00:00', '2024-01-18 16:00:00'),
  ('comment-5', 'CI/CD pipelines save so much time in the development workflow.', 'ci-cd-pipeline-setup', 'user-2', '2024-01-19 17:00:00', '2024-01-19 17:00:00');
