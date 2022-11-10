
create table filter_words
(
    filterRecordId int not null,
    wordRecordId   int not null,
    primary key (filterRecordId, wordRecordId)
)
    charset = utf8mb3;

create table filters
(
    recordId    int auto_increment
        primary key,
    filter      varchar(40)  not null,
    description varchar(255) null,
    title       varchar(255) null
)
    charset = utf8mb3;


create table words
(
    recordId int auto_increment
        primary key,
    word     varchar(255) null,
    constraint Words_RecordId_uindex
        unique (recordId)
)
    charset = latin1;

create index unique_words
    on words (word);

