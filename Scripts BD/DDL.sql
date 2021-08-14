/* USUÁRIOS */
create table USUARIOS(
    CPF varchar(100) constraint NN_USUARIO_USERNAME not null,
    NOME varchar(150) constraint NN_USUARIO_NOME not null,
    DATA_CRIACAO date constraint NN_USUARIO_DTCRIACAO not null,
	DATA_ALTERACAO date constraint NN_USUARIO_DTALTERACAO not null,
    ATIVO number(1) default 1 constraint NN_USUARIO_ATIVO not null,
    constraint PK_USUARIOS primary key (CPF)
);

create or replace trigger USUARIOS_BIR
before insert on USUARIOS
for each row
begin
    :new.USERNAME := upper(:new.USERNAME);
end;
/
show errors;

/* DEPENDENTES */
create table DEPENDENTES(
    CODIGO integer constraint NN_DEPEND_CODIGO not null,
    TITULAR varchar(100) constraint NN_DEPEND_TITULAR not null,
    USUARIO varchar(100) constraint NN_DEPEND_USUARIO not null,
    constraint PK_DEPENDENTES primary key (CODIGO),
    constraint FK_DEPEND_TITULAR foreign key (TITULAR) references USUARIOS(CPF) on delete cascade,
    constraint FK_DEPEND_USUARIO foreign key (USUARIO) references USUARIOS(CPF) on delete cascade,
	constraint UK_DEPENDENTES unique (TITULAR, USUARIO)
);

create sequence DEPENDENTES_SEQUENCE
	start with 1
	increment by 1;

create or replace trigger DEPENDENTES_BIR
	before insert on DEPENDENTES
	for each row
begin
	:new.CODIGO := DEPENDENTES_SEQUENCE.nextval;
end;
/
show errors;

/* CADASTRADORES */
create table CADASTRADORES(
    USUARIO varchar(100) constraint NN_CADAST_USUARIO not null,
    SENHA varchar(100) constraint NN_CADAST_SENHA not null,
	ATIVO number(1) default 1 constraint NN_CADAST_ATIVO not null,
	COD_PERMISSAO integer,
	DATA_CRIACAO date constraint NN_CADAST_DTCRIACAO not null,
	DATA_ALTERACAO date constraint NN_CADAST_DTALTERACAO not null,
    constraint PK_CADASTRADORES primary key (USUARIO),
    constraint FK_CADAST_PERMISSAO foreign key (COD_PERMISSAO) references PERMISSOES(CODIGO) on delete cascade
);

/* PERMISSOES */
create table PERMISSOES(
    CODIGO integer constraint NN_PERMISS_CODIGO not null,
	NOME varchar(150) constraint NN_PERMISS_NOME not null,
	CRIAR number(1) default 1 constraint NN_PERMISS_CRIAR not null,
    LER number(1) default 1 constraint NN_PERMISS_LER not null,
	ATUALIZAR number(1) default 1 constraint NN_PERMISS_ATUALIZAR not null,
	DELETAR number(1) default 1 constraint NN_PERMISS_DELETAR not null,
    ADMINISTRADOR number(1) default 1 constraint NN_PERMISS_ADMIN not null,
	DATA_CRIACAO date constraint NN_PERMISS_DTCRIACAO not null,
	DATA_ALTERACAO date constraint NN_PERMISS_DTALTERACAO not null,
    constraint PK_PERMISSOES primary key (CODIGO),
    constraint UK_PERMISSOES unique (NOME)
);

create sequence PERMISSOES_SEQUENCE
	start with 1
	increment by 1;

create or replace trigger PERMISSOES_BIR
	before insert on PERMISSOES
	for each row
begin
	:new.CODIGO := PERMISSOES_SEQUENCE.nextval;
end;
/
show errors;