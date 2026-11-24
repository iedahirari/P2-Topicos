-- Execute em um banco vazio escolhido para este aplicativo.
-- O script nao apaga nem transforma tabelas existentes.
CREATE TABLE IF NOT EXISTS Clientes (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(120) NOT NULL,
    Telefone VARCHAR(11) NOT NULL,
    Endereco VARCHAR(200) NOT NULL,
    Numero VARCHAR(20) NOT NULL,
    Bairro VARCHAR(80) NOT NULL,
    Cidade VARCHAR(80) NOT NULL,
    Cep CHAR(8) NOT NULL,
    CONSTRAINT UQ_Clientes_Telefone UNIQUE (Telefone)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Pets (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ClienteID BIGINT NOT NULL,
    Nome VARCHAR(80) NOT NULL,
    Raca VARCHAR(80) NOT NULL,
    Idade SMALLINT NOT NULL,
    CONSTRAINT CK_Pets_Idade CHECK (Idade >= 0),
    CONSTRAINT UQ_Pets_Cliente UNIQUE (Id, ClienteID),
    CONSTRAINT FK_Pets_Cliente FOREIGN KEY (ClienteID) REFERENCES Clientes(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Agendamentos (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    ClienteID BIGINT NOT NULL,
    PetID BIGINT NOT NULL,
    Inicio DATETIME NOT NULL,
    Servico VARCHAR(20) NOT NULL,
    CONSTRAINT UQ_Agendamentos_Inicio UNIQUE (Inicio),
    CONSTRAINT FK_Agendamentos_Cliente FOREIGN KEY (ClienteID) REFERENCES Clientes(Id),
    CONSTRAINT FK_Agendamentos_Pet FOREIGN KEY (PetID, ClienteID) REFERENCES Pets(Id, ClienteID),
    CONSTRAINT CK_Agendamentos_Servico CHECK (Servico IN ('Banho', 'Tosa', 'Banho e tosa'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
