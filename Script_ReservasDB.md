# Script de creación de Base de Datos - ReservasDB

``` sql
CREATE DATABASE ReservasDB;
GO

USE ReservasDB;
GO

CREATE TABLE Usuarios
(
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(50) NOT NULL,
    NombreCompleto VARCHAR(100) NOT NULL
);
GO

CREATE TABLE Aulas
(
    AulaId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Capacidad INT NOT NULL
);
GO

CREATE TABLE Reservas
(
    ReservaId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    AulaId INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME NOT NULL,
    Motivo VARCHAR(200),

    CONSTRAINT FK_Reserva_Usuario
    FOREIGN KEY (UsuarioId)
    REFERENCES Usuarios(UsuarioId),

    CONSTRAINT FK_Reserva_Aula
    FOREIGN KEY (AulaId)
    REFERENCES Aulas(AulaId)
);
GO

INSERT INTO Usuarios (Username, Password, NombreCompleto)
VALUES
('admin','123','Administrador del Sistema'),
('juan','123','Juan Perez'),
('maria','123','Maria Lopez'),
('carlos','123','Carlos Ramirez'),
('ana','123','Ana Torres'),
('pedro','123','Pedro Sanchez'),
('lucia','123','Lucia Fernandez'),
('diego','123','Diego Flores'),
('sofia','123','Sofia Vargas'),
('miguel','123','Miguel Rojas');
GO

INSERT INTO Aulas (Nombre, Capacidad)
VALUES
('Aula 101',30),
('Aula 102',35),
('Aula 103',40),
('Aula 104',25),
('Aula 105',50),
('Laboratorio 1',25),
('Laboratorio 2',30),
('Sala Multimedia',60),
('Sala Conferencias',80),
('Auditorio',150),
('Aula Virtual 1',40),
('Aula Virtual 2',40),
('Sala Reuniones 1',20),
('Sala Reuniones 2',20),
('Taller Practico',35);
GO

INSERT INTO Reservas (UsuarioId, AulaId, Fecha, Hora, Motivo)
VALUES
(1,1,'2026-09-01','08:00','Clase de programación'),
(2,2,'2026-09-01','10:00','Reunión académica'),
(3,3,'2026-09-01','12:00','Capacitación'),
(4,4,'2026-09-02','09:00','Examen'),
(5,5,'2026-09-02','11:00','Taller'),
(6,6,'2026-09-03','08:00','Laboratorio'),
(7,7,'2026-09-03','14:00','Práctica'),
(8,8,'2026-09-04','10:00','Conferencia'),
(9,9,'2026-09-04','15:00','Evento'),
(10,10,'2026-09-05','09:00','Charla');
GO
```

## Credenciales de prueba

Usuario: `admin`\
Contraseña: `123`

## Consultas de verificación

``` sql
SELECT * FROM Usuarios;
SELECT * FROM Aulas;
SELECT * FROM Reservas;
```
