-- insert-delete

-- create table test
-- (
--     id INT,
--     name Char(8)
-- );
create table test
(
    id INT,
    name Char(8),
    primary key (id)
);

insert into test values (1, "Tom");
insert into test values (2, "Tim");
insert into test values (3, "Tai");

select * from test;

delete from test where name = "Tai";

select * from test;

delete from test where id < 3;

select * from test;

-- type

create table typetest
(
    id INT,
    name Char(8),
    height FLOAT,
    primary key (id)
);

insert into typetest values (3, "test1", 12.3);
insert into typetest values (3, "test2", 332.1);

select * from typetest;

-- table collision

create table test
(
    id INT,
    name Char(8),
    height FLOAT,
    primary key (id)
);
create table test
(
    id INT,
    name Char(8),
    height FLOAT,
    primary key (id)
);

-- inconsistent number of values

create table test
(
    id INT,
    name Char(8),
    height FLOAT,
    primary key (id)
);

insert into test values (1, "Tom");

-- inconsistent types of values

create table test
(
    id INT,
    name Char(8),
    height FLOAT,
    primary key (id)
);

insert into test values (1, 1123.4, "Tom");
