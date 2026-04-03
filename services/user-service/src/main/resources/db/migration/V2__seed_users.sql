-- Seed Users (password for all: password123)
INSERT INTO users (id, username, password, email, bio, image) VALUES
  ('user-1', 'johndoe', '$2a$10$AbglDchyhkogGBIxNoHdN.pBDK86VNXtF.Vh6N72G9s1rjw7z2b4u', 'johndoe@example.com', 'I am John Doe, a software developer.', 'https://static.productionready.io/images/smiley-cyrus.jpg'),
  ('user-2', 'janedoe', '$2a$10$AbglDchyhkogGBIxNoHdN.pBDK86VNXtF.Vh6N72G9s1rjw7z2b4u', 'janedoe@example.com', 'I am Jane Doe, a tech writer.', 'https://static.productionready.io/images/smiley-cyrus.jpg'),
  ('user-3', 'bobsmith', '$2a$10$AbglDchyhkogGBIxNoHdN.pBDK86VNXtF.Vh6N72G9s1rjw7z2b4u', 'bobsmith@example.com', 'I am Bob Smith, a DevOps engineer.', 'https://static.productionready.io/images/smiley-cyrus.jpg');

-- Follow relationships
INSERT INTO follows (user_id, follow_id) VALUES
  ('user-1', 'user-2'),
  ('user-2', 'user-1'),
  ('user-3', 'user-1');
