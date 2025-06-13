CREATE DATABASE CalculosDeNomina;
GO
USE CalculosDeNomina;

-- Se crea la tabla usuarios
CREATE TABLE Usuario(
	ID INT PRIMARY KEY identity(1,1),
	NombreUsuario VARCHAR(100) NOT NULL UNIQUE,
	Password VARBINARY(8000) NOT NULL
);

-- 
CREATE TABLE CalculosDeNomina (
	ID INT PRIMARY KEY IDENTITY(1,1),
	NombreCalculo VARCHAR(150) NOT NULL,
	FechaCalculo DATETIME,
	FilasAfectadas INT DEFAULT 0,
	Estado BIT DEFAULT 0 NOT NULL,
	ArchivoNomina VARBINARY(MAX)
);

-- Procedimiento para agregar un nuevo usuario del sistema
CREATE PROCEDURE RegistrarUsuario
	@nombreUsuario VARCHAR(100),
	@password NVARCHAR(30)
AS
BEGIN
	INSERT INTO Usuario VALUES(
	@nombreUsuario,
	ENCRYPTBYPASSPHRASE('password', @password)
	)
END
GO

-- Procedimiento para agregar un nuevo calculo de nomina
CREATE PROCEDURE CrearCalculoDeNomina
	@nombreCalculo VARCHAR(150)
AS
BEGIN
	INSERT INTO CalculosDeNomina VALUES(
	@nombreCalculo,
	GETDATE(),
	null,
	0,
	null
	)
END
GO


-- Procedimiento para validar credenciales de usuario
CREATE PROCEDURE LoginUsuario
	@nombreUsuario VARCHAR(100),
	@password NVARCHAR(30)
AS
BEGIN
	SELECT * FROM Usuario WHERE NombreUsuario = @nombreUsuario AND CONVERT(VARCHAR(MAX), DECRYPTBYPASSPHRASE('password', Password)) = @password
END
GO


-- Se insentan los datos de prueba para el proyecto
INSERT INTO Usuario VALUES('UsuarioPrueba', ENCRYPTBYPASSPHRASE('password', 'Prueba1234'));

EXEC CrearCalculoDeNomina @nombreCalculo = 'Nómina 2da quincena de mayo';
EXEC CrearCalculoDeNomina @nombreCalculo = 'Nómina 1ra quincena de mayo';
EXEC CrearCalculoDeNomina @nombreCalculo = 'Nómina 2da quincena de junio';
EXEC CrearCalculoDeNomina @nombreCalculo = 'Nómina 2da quincena de abril';
EXEC CrearCalculoDeNomina @nombreCalculo = 'Nómina 1ra quincena de abril';