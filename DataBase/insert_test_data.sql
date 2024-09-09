-- Conectarse a la base de datos
\c testing_company;

-- Datos de prueba para todas las tablas con el pais Colombia
INSERT INTO locations.countries (name) VALUES ('Colombia');
INSERT INTO locations.departments (name, country_id) VALUES ('Antioquia', 1);
INSERT INTO locations.municipalities (name, department_id) VALUES ('Medellin', 1);
INSERT INTO users.users (name, cellphone, address, municipality_id) VALUES ('Juan', '1234567890', 'Calle 1', 1);


INSERT INTO locations.departments (name, country_id) VALUES ('Cundinamarca', 1);
INSERT INTO locations.municipalities (name, department_id) VALUES ('Bogota', 2);
INSERT INTO users.users (name, cellphone, address, municipality_id) VALUES ('Pedro', '0987654321', 'Calle 2', 2);


INSERT INTO locations.departments (name, country_id) VALUES ('Valle del Cauca', 1);
INSERT INTO locations.municipalities (name, department_id) VALUES ('Cali', 3);
INSERT INTO users.users (name, cellphone, address, municipality_id) VALUES ('Maria', '1234567890', 'Calle 3', 3);