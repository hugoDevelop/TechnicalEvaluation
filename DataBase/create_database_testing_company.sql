-- Crear la base de datos
CREATE DATABASE testing_company;

-- Conectar a la base de datos
\c testing_company;

-- Crear esquema para usuarios
CREATE SCHEMA IF NOT EXISTS users;

-- crear esquema para ubicación
CREATE SCHEMA IF NOT EXISTS locations;

-- Crear tabla de paises en el esquema locations
CREATE TABLE IF NOT EXISTS locations.countries (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

-- Crear tabla de departamentos en el esquema locations
CREATE TABLE IF NOT EXISTS locations.departments (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    country_id INT NOT NULL,
    FOREIGN KEY (country_id) REFERENCES locations.countries(id) ON DELETE CASCADE
);

-- crear tabla de municipios en el esquema locations
CREATE TABLE IF NOT EXISTS locations.municipalities (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    department_id INT NOT NULL,
    FOREIGN KEY (department_id) REFERENCES locations.departments(id) ON DELETE CASCADE
);

-- Crear tabla de usuarios en el esquema users
CREATE TABLE IF NOT EXISTS users.users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    cellphone VARCHAR(10) NOT NULL,
    address VARCHAR(100) NOT NULL,
    municipality_id INT NOT NULL,
    FOREIGN KEY (municipality_id) REFERENCES locations.municipalities(id) ON DELETE CASCADE
);
