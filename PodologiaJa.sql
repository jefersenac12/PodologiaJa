--CRIAR BANCO DE DADOS PodologiaJA
USE master
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PodologiaJA')
BEGIN
    PRINT 'BANCO J� EXISTE PodologiaJA'
END
ELSE
BEGIN
CREATE DATABASE PodologiaJA
	PRINT 'BANCO CRIADO PodologiaJA.'
END
GO
USE PodologiaJA
GO
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'CLIENTES')
BEGIN
    PRINT 'TABELA J� EXISTE.'
END
ELSE
BEGIN
	CREATE TABLE CLIENTES(
	Id INT IDENTITY,
	Nome_Completo NVARCHAR(100),
	Celular NVARCHAR(15),
	Email NVARCHAR(100),
	Data_Agendamento Date,
	Hora_Agendamento Time,
	Descricao NVARCHAR(200),
	CONSTRAINT PKCLIENTES PRIMARY KEY (Id)
	)
    PRINT 'TABELA CLIENTES CRIADA'
END
GO