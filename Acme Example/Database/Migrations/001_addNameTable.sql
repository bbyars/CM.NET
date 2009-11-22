create table Names
(
  Id int NOT NULL,
  Name varchar(50)
)

grant select on Names to $DBUSER
