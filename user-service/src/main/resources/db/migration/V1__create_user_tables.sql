create table if not exists users (
  id varchar(255) primary key,
  username varchar(255) unique,
  password varchar(255),
  email varchar(255) unique,
  bio text,
  image varchar(511)
);

create table if not exists follows (
  user_id varchar(255) not null,
  follow_id varchar(255) not null
);

create unique index if not exists follows_user_follow on follows (user_id, follow_id);
