-- Descripción: Script que contiene los procedimientos almacenados y funciones para la gestión de países, departamentos y municipios.

-- Conectarse a la base de datos
\c testing_company;

-- Crear procedimiento almacenado para crear un país
CREATE OR REPLACE PROCEDURE locations.sp_insert_country(
    p_name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validar que se proporcione el nombre del país
    IF p_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar el nombre del país.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final del nombre
    p_name := TRIM(p_name);
    
    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del pais contiene caracteres invalidos.';
    END IF;

    -- Verificar si el país ya existe
    IF EXISTS (SELECT 1 FROM locations.countries WHERE LOWER(name) = LOWER(p_name)) THEN
        RAISE EXCEPTION 'El pais ya existe';
    END IF;

    -- Insertar el país
    INSERT INTO locations.countries (name)
    VALUES (p_name);
END;
$$;

-- Crear función para obtener un país por ID o nombre
CREATE OR REPLACE FUNCTION locations.fn_get_country(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificar si se proporciona el ID
    IF p_id IS NOT NULL THEN
        RETURN QUERY SELECT * FROM locations.countries WHERE id = p_id;
    -- Verificar si se proporciona el nombre
    ELSIF p_name IS NOT NULL THEN
        RETURN QUERY SELECT * FROM locations.countries WHERE LOWER(name) = LOWER(p_name);
    ELSE
        RAISE EXCEPTION 'Debe proporcionar el ID o el nombre del país.';
    END IF;
END;
$$;

-- Crear procedimiento almacenado para actualizar un país
CREATE OR REPLACE PROCEDURE locations.sp_update_country(
    p_id INT,
    p_new_name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validar que se proporcione el nuevo nombre
    IF p_new_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar el nuevo nombre del pais.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_new_name := TRIM(p_new_name);

    -- Validación de caracteres especiales en el nuevo nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_new_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nuevo nombre del pais contiene caracteres invalidos.';
    END IF;

    -- Verificar si el país con el ID proporcionado existe
    IF NOT EXISTS (SELECT 1 FROM locations.countries WHERE id = p_id) THEN
        RAISE EXCEPTION 'El pais con el ID proporcionado no existe.';
    END IF;

    -- Verificar si el nuevo nombre ya existe para otro país
    IF EXISTS (SELECT 1 FROM locations.countries WHERE LOWER(name) = LOWER(p_new_name) AND id != p_id) THEN
        RAISE EXCEPTION 'El nuevo nombre del pais ya está en uso.';
    END IF;

    -- Actualizar el país
    UPDATE locations.countries
    SET name = p_new_name
    WHERE id = p_id;
END;
$$;

-- Crear procedimiento almacenado para eliminar un país
CREATE OR REPLACE PROCEDURE locations.sp_delete_country(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_country_id INT;
BEGIN
    -- Validar que al menos uno de los parámetros esté proporcionado
    IF p_id IS NULL AND p_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parametros: p_id o p_name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);

    -- Obtener el ID del país si se proporciona el nombre del país
    IF p_id IS NULL AND p_name IS NOT NULL THEN
        SELECT id INTO v_country_id 
        FROM locations.countries 
        WHERE LOWER(name) = LOWER(p_name);
        
        -- Verificar si el país existe
        IF v_country_id IS NULL THEN
            RAISE EXCEPTION 'El pais con el nombre proporcionado no existe.';
        END IF;
    ELSE
        v_country_id := p_id;
        
        -- Verificar si el país existe
        IF NOT EXISTS (SELECT 1 FROM locations.countries WHERE id = v_country_id) THEN
            RAISE EXCEPTION 'El pais con el ID proporcionado no existe.';
        END IF;
    END IF;

    -- Verificar si el país tiene departamentos
    IF EXISTS (SELECT 1 FROM locations.departments WHERE country_id = v_country_id) THEN
        RAISE EXCEPTION 'El pais tiene departamentos asociados.';
    END IF;

    -- Verificar si el país tiene municipios
    IF EXISTS (SELECT 1 FROM locations.municipalities WHERE department_id IN (SELECT id FROM locations.departments WHERE country_id = v_country_id)) THEN
        RAISE EXCEPTION 'El pais tiene municipios asociados.';
    END IF;

    -- Eliminar el país
    DELETE FROM locations.countries WHERE id = v_country_id;
END;
$$;

-- Crear función para obtener todos los países
CREATE OR REPLACE FUNCTION locations.fn_get_countries()
RETURNS TABLE (
    id INT,
    name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener todos los países
    RETURN QUERY SELECT * FROM locations.countries;
END;
$$;

-- Crear procedimiento almacenado para crear un departamento
CREATE OR REPLACE PROCEDURE locations.sp_insert_department(
    p_name VARCHAR(100),
    p_country_id INT DEFAULT NULL,
    p_country_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_country_id INT;
BEGIN
    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del departamento contiene caracteres invalidos.';
    END IF;

    -- Validación de caracteres especiales en el nombre del país
    IF p_country_name IS NOT NULL AND NOT EXISTS (SELECT 1 FROM regexp_matches(p_country_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del pais contiene caracteres invalidos.';
    END IF;

    -- Validar que al menos uno de los parámetros del país esté proporcionado
    IF p_country_id IS NULL AND p_country_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parametros del pais: p_country_id o p_country_name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_country_name := TRIM(p_country_name);

    -- Obtener el ID del país si se proporciona el nombre del país
    IF p_country_id IS NULL AND p_country_name IS NOT NULL THEN
        SELECT id INTO v_country_id 
        FROM locations.countries 
        WHERE LOWER(name) = LOWER(p_country_name);
        
        -- Verificar si el país existe y mostrar el nombre del país en el mensaje de error
        IF v_country_id IS NULL THEN
            RAISE EXCEPTION 'El pais con el nombre "%" no existe.', p_country_name;
        END IF;
    ELSE
        v_country_id := p_country_id;
        
        -- Verificar si el país existe
        IF NOT EXISTS (SELECT 1 FROM locations.countries WHERE id = v_country_id) THEN
            RAISE EXCEPTION 'El pais con el ID "%" no existe.', v_country_id;
        END IF;
    END IF;

    -- Verificar si el departamento ya existe
    IF EXISTS (SELECT 1 FROM locations.departments WHERE LOWER(name) = LOWER(p_name) AND country_id = v_country_id) THEN
        RAISE EXCEPTION 'El departamento ya existe.';
    END IF;

    -- Insertar el departamento
    INSERT INTO locations.departments (name, country_id)
    VALUES (p_name, v_country_id);
END;
$$;

-- Crear función para obtener un departamento por ID o nombre
CREATE OR REPLACE FUNCTION locations.fn_get_department(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    country_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validar que al menos uno de los parámetros esté proporcionado
    IF p_id IS NULL AND p_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parámetros: id o name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);

    -- Obtener el departamento basado en el parámetro proporcionado
    RETURN QUERY
    SELECT id, name, country_id
    FROM locations.departments
    WHERE (p_id IS NOT NULL AND id = p_id)
       OR (p_name IS NOT NULL AND LOWER(name) = LOWER(p_name));
END;
$$;

-- Crear procedimiento almacenado para actualizar un departamento
CREATE OR REPLACE PROCEDURE locations.sp_update_department(
    p_id INT,
    p_name VARCHAR(100),
    p_country_id INT DEFAULT NULL,
    p_country_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_country_id INT;
BEGIN
    -- Validar parámetros
    IF p_country_id IS NULL AND p_country_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parametros: country_id o country_name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_country_name := TRIM(p_country_name);

    -- Obtener el ID del país si se proporciona el nombre
    IF p_country_id IS NULL THEN
        -- Validación de caracteres especiales en el nombre del país
        IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_country_name, '^[a-zA-Z\s]+$')) THEN
            RAISE EXCEPTION 'El nombre del pais contiene caracteres invalidos.';
        END IF;

        -- Obtener el ID del país por nombre
        SELECT id INTO v_country_id FROM locations.countries WHERE LOWER(name) = LOWER(p_country_name);

        -- Verificar si el país existe
        IF v_country_id IS NULL THEN
            RAISE EXCEPTION 'El pais no existe';
        END IF;

        p_country_id := v_country_id;
    ELSE
        p_country_id := p_country_id;
    END IF;

    -- Validación de caracteres especiales en el nombre del departamento
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del departamento contiene caracteres invalidos.';
    END IF;

    -- Verificar si el departamento existe
    IF NOT EXISTS (SELECT 1 FROM locations.departments WHERE id = p_id) THEN
        RAISE EXCEPTION 'El departamento no existe';
    END IF;

    -- Verificar si el país existe
    IF NOT EXISTS (SELECT 1 FROM locations.countries WHERE id = p_country_id) THEN
        RAISE EXCEPTION 'El pais no existe';
    END IF;

    -- Verificar si el departamento ya existe
    IF EXISTS (SELECT 1 FROM locations.departments WHERE LOWER(name) = LOWER(p_name) AND country_id = p_country_id AND id != p_id) THEN
        RAISE EXCEPTION 'El departamento ya existe';
    END IF;

    -- Actualizar el departamento
    UPDATE locations.departments
    SET name = p_name, country_id = p_country_id
    WHERE id = p_id;
END;
$$;

-- Crear procedimiento almacenado para eliminar un departamento
CREATE OR REPLACE PROCEDURE locations.sp_delete_department(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_department_id INT;
BEGIN
    -- Validar parámetros
    IF p_id IS NULL AND p_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parámetros: id o name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);

    -- Obtener el ID del departamento si se proporciona el nombre
    IF p_id IS NULL THEN
        -- Validación de caracteres especiales en el nombre
        IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
            RAISE EXCEPTION 'El nombre del departamento contiene caracteres invalidos.';
        END IF;

        -- Obtener el ID del departamento por nombre
        SELECT id INTO v_department_id FROM locations.departments WHERE LOWER(name) = LOWER(p_name);

        -- Verificar si el departamento existe
        IF v_department_id IS NULL THEN
            RAISE EXCEPTION 'El departamento no existe';
        END IF;

        p_id := v_department_id;
    ELSE
        p_id := p_id;
    END IF;

    -- Verificar si el departamento existe
    IF NOT EXISTS (SELECT 1 FROM locations.departments WHERE id = p_id) THEN
        RAISE EXCEPTION 'El departamento no existe';
    END IF;

    -- Verificar si el departamento tiene municipios
    IF EXISTS (SELECT 1 FROM locations.municipalities WHERE department_id = p_id) THEN
        RAISE EXCEPTION 'El departamento tiene municipios asociados';
    END IF;

    -- Verificar si el departamento tiene usuarios
    IF EXISTS (SELECT 1 FROM users.users WHERE municipality_id IN (SELECT id FROM locations.municipalities WHERE department_id = p_id)) THEN
        RAISE EXCEPTION 'El departamento tiene usuarios asociados';
    END IF;

    -- Eliminar el departamento
    DELETE FROM locations.departments WHERE id = p_id;
END;
$$;

-- Crear función para obtener todos los departamentos con los detalles del país
CREATE OR REPLACE FUNCTION locations.fn_get_departments()
RETURNS TABLE (
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
        d.id AS department_id,
        d.name AS department_name,
        c.id AS country_id,
        c.name AS country_name
    FROM locations.departments d
    JOIN locations.countries c ON d.country_id = c.id;
END;
$$;


-- Crear procedimiento almacenado para insertar un municipio
CREATE OR REPLACE PROCEDURE locations.sp_insert_municipality(
    p_name VARCHAR(100),
    p_department_id INT DEFAULT NULL,
    p_department_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_department_id INT;
BEGIN
    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del municipio contiene caracteres invalidos.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);
    p_department_name := TRIM(p_department_name);

    -- Determinar el ID del departamento
    IF p_department_id IS NOT NULL THEN
        v_department_id := p_department_id;
    ELSE
        -- Validar el nombre del departamento si el ID no es proporcionado
        IF p_department_name IS NULL THEN
            RAISE EXCEPTION 'Debe proporcionar al menos uno de los parámetros: department_id o department_name.';
        END IF;

        -- Validación de caracteres especiales en el nombre del departamento
        IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_department_name, '^[a-zA-Z\s]+$')) THEN
            RAISE EXCEPTION 'El nombre del departamento contiene caracteres invalidos.';
        END IF;

        -- Obtener el ID del departamento por su nombre
        SELECT id INTO v_department_id FROM locations.departments WHERE LOWER(name) = LOWER(p_department_name);

        -- Verificar si el departamento existe y mostrar el nombre del departamento en el mensaje de error
        IF v_department_id IS NULL THEN
            RAISE EXCEPTION 'El departamento con el nombre "%" no existe.', p_department_name;
        END IF;
    END IF;

    -- Verificar si el departamento existe y mostrar el id si no existe
    IF NOT EXISTS (SELECT 1 FROM locations.departments WHERE id = v_department_id) THEN
        RAISE EXCEPTION 'El departamento con el ID % no existe.', v_department_id;
    END IF;

    -- Verificar si el municipio ya existe
    IF EXISTS (SELECT 1 FROM locations.municipalities WHERE LOWER(name) = LOWER(p_name) AND department_id = v_department_id) THEN
        RAISE EXCEPTION 'El municipio ya existe';
    END IF;

    -- Insertar el municipio
    INSERT INTO locations.municipalities (name, department_id)
    VALUES (p_name, v_department_id);
END;
$$;

-- Crear función para obtener un municipio
CREATE OR REPLACE FUNCTION locations.fn_get_municipality(
    p_id INT DEFAULT NULL,
    p_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    department_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validar parámetros
    IF p_id IS NULL AND p_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los parámetros: id o name.';
    END IF;

    IF p_name IS NOT NULL AND NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del municipio contiene caracteres invalidos.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);

    -- Obtener el municipio según el parámetro proporcionado
    RETURN QUERY
    SELECT id, name, department_id
    FROM locations.municipalities
    WHERE (p_id IS NOT NULL AND id = p_id)
       OR (p_name IS NOT NULL AND LOWER(name) = LOWER(p_name));
END;
$$;

-- Crear procedimiento almacenado para actualizar un municipio
CREATE OR REPLACE PROCEDURE locations.sp_update_municipality(
    p_id INT,
    p_name VARCHAR(100),
    p_department_id INT DEFAULT NULL,
    p_department_name VARCHAR(100) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_department_id INT;
BEGIN
    -- Validación de caracteres especiales en el nombre
    IF NOT EXISTS (SELECT 1 FROM regexp_matches(p_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del municipio contiene caracteres invalidos.';
    END IF;

    IF p_department_name IS NOT NULL AND NOT EXISTS (SELECT 1 FROM regexp_matches(p_department_name, '^[a-zA-Z\s]+$')) THEN
        RAISE EXCEPTION 'El nombre del departamento contiene caracteres invalidos.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_name := TRIM(p_name);
    p_department_name := TRIM(p_department_name);

    -- Obtener el id del departamento si se proporciona el nombre del departamento
    IF p_department_name IS NOT NULL THEN
        SELECT id INTO v_department_id FROM locations.departments WHERE LOWER(name) = LOWER(p_department_name);

        IF v_department_id IS NULL THEN
            RAISE EXCEPTION 'El departamento con nombre "%" no existe.', p_department_name;
        END IF;
    ELSE
        v_department_id := p_department_id;
    END IF;

    -- Verificar si el municipio existe
    IF NOT EXISTS (SELECT 1 FROM locations.municipalities WHERE id = p_id) THEN
        RAISE EXCEPTION 'El municipio con ID %s no existe.', p_id;
    END IF;

    -- Verificar si el departamento existe
    IF NOT EXISTS (SELECT 1 FROM locations.departments WHERE id = v_department_id) THEN
        RAISE EXCEPTION 'El departamento con ID %s no existe.', v_department_id;
    END IF;

    -- Verificar si el municipio ya existe
    IF EXISTS (SELECT 1 FROM locations.municipalities WHERE LOWER(name) = LOWER(p_name) AND department_id = v_department_id AND id != p_id) THEN
        RAISE EXCEPTION 'El municipio con nombre "%s" ya existe en el departamento con ID %s.', p_name, v_department_id;
    END IF;

    -- Actualizar el municipio
    UPDATE locations.municipalities
    SET name = p_name, department_id = v_department_id
    WHERE id = p_id;
END;
$$;

-- Crear procedimiento almacenado para eliminar un municipio
CREATE OR REPLACE PROCEDURE locations.sp_delete_municipality(
    p_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificar si el municipio existe
    IF NOT EXISTS (SELECT 1 FROM locations.municipalities WHERE id = p_id) THEN
        RAISE EXCEPTION 'El municipio no existe';
    END IF;

    -- Verificar si el municipio tiene usuarios
    IF EXISTS (SELECT 1 FROM users.users WHERE municipality_id = p_id) THEN
        RAISE EXCEPTION 'El municipio tiene usuarios asociados';
    END IF;

    -- Eliminar el municipio
    DELETE FROM locations.municipalities WHERE id = p_id;
END;
$$;

-- Crear función para obtener todos los municipios con los detalles del departamento
CREATE OR REPLACE FUNCTION locations.fn_get_municipalities()
RETURNS TABLE (
    municipality_id INT,
    municipality_name VARCHAR(100),
    department_id INT,
    department_name VARCHAR(100),
    department_country_id INT,
    department_country_name VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener todos los municipios y los detalles del departamento asociado
    RETURN QUERY 
    SELECT 
        m.id AS municipality_id,
        m.name AS municipality_name,
        d.id AS department_id,
        d.name AS department_name,
        d.country_id AS department_country_id,
        c.name AS department_country_name
    FROM locations.municipalities m
    JOIN locations.departments d ON m.department_id = d.id
    JOIN locations.countries c ON d.country_id = c.id;
END;
$$;

-- Crear función para obtener municipios por país (ID o Nombre)
CREATE OR REPLACE FUNCTION locations.fn_get_municipalities_by_country(
    p_country_id INT DEFAULT NULL,
    p_country_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    department_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_country_id IS NOT NULL THEN
        -- Obtener municipios por ID de país
        RETURN QUERY 
        SELECT * 
        FROM locations.municipalities 
        WHERE department_id IN (
            SELECT id 
            FROM locations.departments 
            WHERE country_id = p_country_id
        );

    ELSIF p_country_name IS NOT NULL THEN
        -- Quitar espacios en blanco al inicio y al final
        p_country_name := TRIM(p_country_name);
        -- Obtener municipios por nombre de país
        RETURN QUERY 
        SELECT * 
        FROM locations.municipalities 
        WHERE department_id IN (
            SELECT id 
            FROM locations.departments 
            WHERE country_id = (
                SELECT id 
                FROM locations.countries 
                WHERE LOWER(name) = LOWER(p_country_name)
            )
        );

    ELSE
        RAISE EXCEPTION 'Debe proporcionar el ID o el nombre del país.';
    END IF;
END;
$$;

-- Crear función para obtener los departamentos de un país
CREATE OR REPLACE FUNCTION locations.fn_get_departments_by_country(
    p_country_id INT
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    country_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Obtener los departamentos del país
    RETURN QUERY 
    SELECT * 
    FROM locations.departments 
    WHERE country_id = p_country_id;
END;
$$;

-- Crear función para obtener todos los usuarios de un país por ID o nombre del país
CREATE OR REPLACE FUNCTION users.fn_get_users_by_country(
    p_country_id INT DEFAULT NULL,
    p_country_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    cellphone VARCHAR(10),
    address VARCHAR(100),
    municipality_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Verificar si se proporciona al menos un parámetro
    IF p_country_id IS NULL AND p_country_name IS NULL THEN
        RAISE EXCEPTION 'Debe proporcionar al menos uno de los siguientes parámetros: country_id o country_name.';
    END IF;

    -- Quitar espacios en blanco al inicio y al final
    p_country_name := TRIM(p_country_name);

    -- Obtener los usuarios según el parámetro proporcionado
    RETURN QUERY
    SELECT u.id, u.name, u.cellphone, u.address, u.municipality_id
    FROM users.users u
    WHERE u.municipality_id IN (
        SELECT m.id
        FROM locations.municipalities m
        WHERE m.department_id IN (
            SELECT d.id
            FROM locations.departments d
            WHERE d.country_id = COALESCE(
                p_country_id,
                (SELECT id FROM locations.countries WHERE LOWER(name) = LOWER(p_country_name))
            )
        )
    );
END;
$$;

-- Crear función para obtener usuarios por departamento (ID o Nombre)
CREATE OR REPLACE FUNCTION users.fn_get_users_by_department(
    p_department_id INT DEFAULT NULL,
    p_department_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    cellphone VARCHAR(10),
    address VARCHAR(100),
    municipality_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_department_id IS NOT NULL THEN
        -- Obtener usuarios por ID de departamento
        RETURN QUERY 
        SELECT * 
        FROM users.users 
        WHERE municipality_id IN (
            SELECT id 
            FROM locations.municipalities 
            WHERE department_id = p_department_id
        );

    ELSIF p_department_name IS NOT NULL THEN
        -- Obtener usuarios por nombre de departamento
        -- Quitar espacios en blanco al inicio y al final
        p_department_name := TRIM(p_department_name);
        RETURN QUERY 
        SELECT * 
        FROM users.users 
        WHERE municipality_id IN (
            SELECT id 
            FROM locations.municipalities 
            WHERE department_id = (
                SELECT id 
                FROM locations.departments 
                WHERE LOWER(name) = LOWER(p_department_name)
            )
        );

    ELSE
        RAISE EXCEPTION 'Debe proporcionar el ID o el nombre del departamento.';
    END IF;
END;
$$;

-- Crear función para obtener usuarios por municipio (ID o Nombre)
CREATE OR REPLACE FUNCTION users.fn_get_users_by_municipality(
    p_municipality_id INT DEFAULT NULL,
    p_municipality_name VARCHAR(100) DEFAULT NULL
)
RETURNS TABLE (
    id INT,
    name VARCHAR(100),
    cellphone VARCHAR(10),
    address VARCHAR(100),
    municipality_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_municipality_id IS NOT NULL THEN
        -- Obtener usuarios por ID de municipio
        RETURN QUERY 
        SELECT * 
        FROM users.users 
        WHERE municipality_id = p_municipality_id;
    ELSIF p_municipality_name IS NOT NULL THEN
        -- Obtener usuarios por nombre de municipio
        -- Quitar espacios en blanco al inicio y al final
        p_municipality_name := TRIM(p_municipality_name);
        RETURN QUERY 
        SELECT * 
        FROM users.users 
        WHERE municipality_id = (
            SELECT id 
            FROM locations.municipalities 
            WHERE LOWER(name) = LOWER(p_municipality_name)
        );
    ELSE
        RAISE EXCEPTION 'Debe proporcionar el ID o el nombre del municipio.';
    END IF;
END;
$$;
