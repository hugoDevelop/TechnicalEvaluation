-- Descripción: Script para crear procedimientos almacenados y funciones en la base de datos para la entidad de usuarios.

-- Conectar a la base de datos
\c testing_company;

-- Crear procedimiento almacenado para insertar un usuario
CREATE OR REPLACE PROCEDURE users.sp_insert_user(
    p_name VARCHAR(100),
    p_cellphone VARCHAR(10),
    p_address VARCHAR(100),
    p_country_id INT DEFAULT NULL,
    p_department_id INT DEFAULT NULL,
    p_municipality_id INT DEFAULT NULL,
    p_country_name VARCHAR(100) DEFAULT NULL,
    p_department_name VARCHAR(100) DEFAULT NULL,
    p_municipality_name VARCHAR(100) DEFAULT NULL
) 
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables para almacenar los IDs si se utilizan nombres
    v_country_id INT;
    v_department_id INT;
    v_municipality_id INT;
BEGIN
    -- Validación de que no se permitan valores nulos
    IF p_name IS NULL OR p_cellphone IS NULL OR p_address IS NULL THEN
        RAISE EXCEPTION 'No se permiten valores nulos para el nombre, celular o direccion.';
    END IF;

    -- Validación de que se use solo un método de inserción (por ID o por nombre)
    IF (p_country_id IS NOT NULL OR p_department_id IS NOT NULL OR p_municipality_id IS NOT NULL) 
       AND (p_country_name IS NOT NULL OR p_department_name IS NOT NULL OR p_municipality_name IS NOT NULL) THEN
        RAISE EXCEPTION 'Proporcione solo IDs o nombres para pais, departamento y municipio, no ambos.';
    END IF;

    -- Buscar IDs si se proporcionan nombres
    IF p_country_name IS NOT NULL THEN
        -- Quitamos espacios en blanco al inicio y al final
        p_country_name := TRIM(p_country_name);

        SELECT id INTO v_country_id FROM locations.countries WHERE LOWER(name) = LOWER(p_country_name);
        IF v_country_id IS NULL THEN
            RAISE EXCEPTION 'Pais no encontrado: %', p_country_name;
        END IF;
    ELSE
        v_country_id := p_country_id;
    END IF;

    IF p_department_name IS NOT NULL THEN
        -- Quitamos espacios en blanco al inicio y al final
        p_department_name := TRIM(p_department_name);

        SELECT id INTO v_department_id FROM locations.departments WHERE LOWER(name) = LOWER(p_department_name) AND country_id = v_country_id;
        IF v_department_id IS NULL THEN
            RAISE EXCEPTION 'Departamento no encontrado o no asociado con el pais: %', p_department_name;
        END IF;
    ELSE
        v_department_id := p_department_id;
    END IF;

    IF p_municipality_name IS NOT NULL THEN
        -- Quitamos espacios en blanco al inicio y al final
        p_municipality_name := TRIM(p_municipality_name);

        SELECT id INTO v_municipality_id FROM locations.municipalities WHERE LOWER(name) = LOWER(p_municipality_name) AND department_id = v_department_id;
        IF v_municipality_id IS NULL THEN
            RAISE EXCEPTION 'Municipio no encontrado o no asociado con el departamento: %', p_municipality_name;
        END IF;
    ELSE
        v_municipality_id := p_municipality_id;
    END IF;

    -- Validar relaciones jerárquicas usando los IDs
    IF v_country_id IS NOT NULL AND v_department_id IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM locations.departments WHERE id = v_department_id AND country_id = v_country_id) THEN
            RAISE EXCEPTION 'El departamento con id % no esta asociado con el pais con id %.', v_department_id, v_country_id;
        END IF;
    END IF;

    IF v_department_id IS NOT NULL AND v_municipality_id IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM locations.municipalities WHERE id = v_municipality_id AND department_id = v_department_id) THEN
            RAISE EXCEPTION 'El municipio con id % no esta asociado con el departamento con id %.', v_municipality_id, v_department_id;
        END IF;
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);
    p_cellphone := TRIM(p_cellphone);
    p_address := TRIM(p_address);

    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre contiene caracteres invalidos.';
    END IF;

    -- Validación de caracteres especiales en el número de celular
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_cellphone, '^[0-9]+$')) THEN
        RAISE EXCEPTION 'El numero de celular contiene caracteres invalidos.';
    END IF;

    -- Verificar que el usuario no exista por número de celular o nombre
    IF EXISTS (SELECT 1 FROM users.users WHERE cellphone = p_cellphone OR name = p_name) THEN
        RAISE EXCEPTION 'El usuario con el nombre o numero de celular ya existe.';
    END IF;

    -- Insertar el usuario
    INSERT INTO users.users (name, cellphone, address, municipality_id)
    VALUES (p_name, p_cellphone, p_address, v_municipality_id);
END;
$$;

-- Crear función para obtener un usuario por su número de celular
CREATE OR REPLACE FUNCTION users.fn_get_user(
    cellphone VARCHAR(10)
)
RETURNS SETOF users.users
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validación que no se permitan valores nulos
    IF cellphone IS NULL THEN
        RAISE EXCEPTION 'No se permiten valores nulos.';
    END IF;

    -- Validación de caracteres especiales en el número de celular
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(cellphone, '^[0-9]+$')) THEN
        RAISE EXCEPTION 'El número de celular contiene caracteres inválidos.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    cellphone := TRIM(cellphone);

    -- Verificar si el usuario existe
    IF NOT EXISTS (SELECT 1 FROM users.users WHERE cellphone = cellphone) THEN
        RAISE EXCEPTION 'El usuario no existe';
    END IF;

    -- Obtener el usuario
    RETURN QUERY SELECT * FROM users.users WHERE cellphone = cellphone;
END;
$$;

-- Crear función para obtener todos los usuarios con municipios, departamentos y países
CREATE OR REPLACE FUNCTION users.fn_get_users()
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    cellphone VARCHAR(10),
    address VARCHAR(100),
    municipality_id INT,
    municipality_name VARCHAR(100),
    department_id INT,
    department_name VARCHAR(100),
    country_id INT,
    country_name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 
    SELECT 
        u.id,
        u.name,
        u.cellphone,
        u.address,
        m.id AS municipality_id,
        m.name AS municipality_name,
        d.id AS department_id,
        d.name AS department_name,
        c.id AS country_id,
        c.name AS country_name
    FROM users.users u
    JOIN locations.municipalities m ON u.municipality_id = m.id
    JOIN locations.departments d ON m.department_id = d.id
    JOIN locations.countries c ON d.country_id = c.id;
END;
$$;

-- Crear procedimiento almacenado para actualizar un usuario
CREATE OR REPLACE PROCEDURE users.sp_update_user(
    p_id INT,
    p_name VARCHAR(100),
    p_cellphone VARCHAR(10),
    p_address VARCHAR(100),
    p_municipality_id INT,
    p_municipality_name VARCHAR(100)  -- Parámetro opcional para el nombre del municipio
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_municipality_id INT;
BEGIN
    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);
    p_cellphone := TRIM(p_cellphone);
    p_address := TRIM(p_address);

    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre contiene caracteres inválidos.';
    END IF;

    -- Validación id de usuario
    IF p_id IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar un ID de usuario.';
    END IF;

    -- Verificar si el usuario existe
    IF NOT EXISTS (SELECT 1 FROM users.users WHERE id = p_id) THEN
        RAISE EXCEPTION 'El usuario no existe';
    END IF;

    -- Verificar si el usuario ya existe por número de celular
    IF EXISTS (SELECT 1 FROM users.users WHERE cellphone = p_cellphone AND id != p_id) THEN
        RAISE EXCEPTION 'El numero de celular ya esta en uso por otro usuario.';
    END IF;

    -- Verificar si el usuario ya existe por nombre
    IF EXISTS (SELECT 1 FROM users.users WHERE LOWER(name) = LOWER(p_name) AND id != p_id) THEN
        RAISE EXCEPTION 'El nombre ya esta en uso por otro usuario.';
    END IF;

    -- Verificar la existencia del municipio
    IF p_municipality_id IS NOT NULL THEN
        -- Verificar si el municipio existe por ID
        IF NOT EXISTS (SELECT 1 FROM locations.municipalities WHERE id = p_municipality_id) THEN
            RAISE EXCEPTION 'El municipio con ID % no existe', p_municipality_id;
        END IF;
        v_municipality_id := p_municipality_id;
    ELSE
        -- Verificar si el municipio existe por nombre
        IF p_municipality_name IS NULL THEN
            RAISE EXCEPTION 'Se debe proporcionar un ID o un nombre de municipio.';
        END IF;

        SELECT id INTO v_municipality_id
        FROM locations.municipalities
        WHERE LOWER(name) = LOWER(p_municipality_name);

        IF v_municipality_id IS NULL THEN
            RAISE EXCEPTION 'El municipio con nombre % no existe', p_municipality_name;
        END IF;
    END IF;

    -- Actualizar el usuario
    UPDATE users.users
    SET name = p_name, cellphone = p_cellphone, address = p_address, municipality_id = v_municipality_id
    WHERE id = p_id;
END;
$$;

-- Crear procedimiento almacenado para eliminar un usuario por ID, nombre o número de celular
CREATE OR REPLACE PROCEDURE users.sp_delete_user(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL,
    p_cellphone VARCHAR(10) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificar si se proporciona al menos un parámetro
    IF p_id IS NULL AND p_name IS NULL AND p_cellphone IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los siguientes parámetros: id, nombre, o número de celular.';
    END IF;

    -- Verificar la existencia del usuario según el parámetro proporcionado
    IF p_id IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM users.users WHERE id = p_id) THEN
            RAISE EXCEPTION 'El usuario con ID % no existe.', p_id;
        END IF;
        DELETE FROM users.users WHERE id = p_id;
    ELSIF p_name IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM users.users WHERE LOWER(name) = LOWER(p_name)) THEN
            RAISE EXCEPTION 'El usuario con nombre "%s" no existe.', p_name;
        END IF;
        DELETE FROM users.users WHERE LOWER(name) = LOWER(p_name);
    ELSIF p_cellphone IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM users.users WHERE cellphone = p_cellphone) THEN
            RAISE EXCEPTION 'El usuario con número de celular "%s" no existe.', p_cellphone;
        END IF;
        DELETE FROM users.users WHERE cellphone = p_cellphone;
    END IF;
END;
$$;

-- Crear función para obtener los usuarios de un municipio
CREATE OR REPLACE FUNCTION users.sp_get_users_by_municipality(
    municipality_id INT
)
RETURNS SETOF users.users
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener los usuarios del municipio
    RETURN QUERY SELECT * FROM users.users WHERE municipality_id = municipality_id;
END;
$$;

-- Crear función para obtener los usuarios de un departamento
CREATE OR REPLACE FUNCTION users.sp_get_users_by_department(
    department_id INT
)
RETURNS SETOF users.users
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener los usuarios del departamento
    RETURN QUERY SELECT * FROM users.users WHERE municipality_id IN (SELECT id FROM locations.municipalities WHERE department_id = department_id);
END;
$$;

-- Crear función para obtener los usuarios de un país
CREATE OR REPLACE FUNCTION users.sp_get_users_by_country(
    country_id INT
)
RETURNS SETOF users.users
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener los usuarios del país
    RETURN QUERY SELECT * FROM users.users WHERE municipality_id IN (SELECT id FROM locations.municipalities WHERE department_id IN (SELECT id FROM locations.departments WHERE country_id = country_id));
END;
$$;