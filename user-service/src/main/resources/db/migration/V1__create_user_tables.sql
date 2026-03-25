create table if not exists users (
  id varchar(255) primary key,
  username varchar(255) UNIQUE,
  password varchar(255),
  email varchar(255) UNIQUE,
  bio text,
  image varchar(511)
);

create table if not exists follows (
  user_id varchar(255) not null,
  follow_id varchar(255) not null
);
