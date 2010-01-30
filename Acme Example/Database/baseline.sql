create table changelog (
  change_number integer,
  delta_set varchar(10),
  start_dt datetime,
  complete_dt datetime,
  applied_by varchar(100),
  description varchar(500)
);