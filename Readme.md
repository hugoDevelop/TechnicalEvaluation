# TechnicalEvaluationApiRest

API REST para la evaluación técnica que permite la gestión de países, departamentos, municipios y usuarios. A continuación, se detallan los endpoints disponibles, los métodos HTTP asociados y los parámetros requeridos para cada uno. Este proyecto consiste en una API RESTful desarrollada en C# con .NET para la gestión de usuarios. La API utiliza procedimientos almacenados (stored procedures) para realizar operaciones CRUD en la base de datos PostgreSQL.

## Estructura del Proyecto

El proyecto está estructurado siguiendo el patrón de diseño Repository y utiliza Inyección de Dependencias (Dependency Injection). Los controladores de API se encargan de manejar las solicitudes HTTP y delegan la lógica de negocio a los servicios, que a su vez interactúan con los repositorios para realizar operaciones de base de datos.

## Requisitos

Antes de ejecutar este proyecto, asegúrate de tener los siguientes requisitos instalados:

- [.NET SDK 6.0 o superior](https://dotnet.microsoft.com/download)
- [PostgreSQL 12 o superior](https://www.postgresql.org/download/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o cualquier otro IDE compatible con .NET
- [Git](https://git-scm.com/) para clonar el repositorio

## Instrucciones para Ejecutar el Proyecto

1. **Clonar el repositorio**

   Abre una terminal y clona el repositorio del proyecto:

   ```bash
   git clone https://github.com/hugoDevelop/TechnicalEvaluation.git
   cd TechnicalEvaluation

   ```

1. **Configurar la base de datos**

- Crea una base de datos PostgreSQL utilizando los scripts proporcionados en la carpeta `scripts/`:

  - Abre una terminal.
 
  - Ejecuta `psql -U tu_usuario` y despues vas a colocar tu contraseña.
 
  - Ejecuta el comando `\i 'C:\\Tu\\Ruta\\Del\\Proyecto\\TechnicalEvaluation\\DataBase\\create_database_testing_company.sql'` no olvides cambiar la ruta donde esta el proyecto y colocal la carpeta `DataBase`, este nos creara la base de datos.

  - Ejecuta el comando `\i 'C:\\Tu\\Ruta\\Del\\Proyecto\\TechnicalEvaluation\\StoredProcedures\\stored_procedures_locations.sql'` no olvides cambiar la ruta donde esta el proyecto y colocal la carpeta `StoredProcedures`, este creara los procedimientos de almacenado del esquema `locations`.

  - Ejecuta el comando `\i 'C:\\Tu\\Ruta\\Del\\Proyecto\\TechnicalEvaluation\\StoredProcedures\\stored_procedures_users.sql'` no olvides cambiar la ruta donde esta el proyecto y colocal la carpeta `StoredProcedures`, este creara los procedimientos de almacenado del esquema `users`.

  - Puedes cargar datos de prueba ejecutando el siguiente comando `\i 'C:\\Tu\\Ruta\\Del\\Proyecto\\TechnicalEvaluation\\DataBase\\insert_test_data.sql'` no olvides cambiar la ruta donde esta el proyecto y colocal la carpeta `DataBase` (Paso OPCIONAL).

- Actualiza la cadena de conexión a la base de datos en el archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=testing_company;Username=tu_usuario;Password=tu_contraseña"
  }
}
```

3. **Restaurar dependencias**

Desde la terminal o consola de tu IDE, ejecuta:

```bash
dotnet restore
```

4. **Compilar el proyecto**

Para compilar el proyecto, ejecuta:

```bash
dotnet build
```

4. **Ejecutar el proyecto**

Finalmente, para ejecutar el proyecto entras a la carpeta `TechnicalEvaluationApiRest`, utiliza:

```bash
dotnet run
```

La API estará disponible en `https://localhost:5007` o `http://localhost:5000`.

## Endpoints

A continuación, se describen los endpoints disponibles, los parámetros que requieren, y cuáles son opcionales o requeridos.

### Country

#### 1. Crear un País

- **URL:** `/api/Country/CreateCountry`
- **Método:** `POST`
- **Descripción:** Crea un nuevo país.
- **Request Body:**

  ```json
  {
    "name": "string" // (Opcional) Nombre del país
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 2. Obtener Países

- **URL:** `/api/Country/GetCountries`
- **Método:** `GET`
- **Descripción:** Recupera la lista de todos los países.
- **Respuesta Exitosa (200):** Operación exitosa.

#### 3. Actualizar un País

- **URL:** `/api/Country/UpdateCountry`
- **Método:** `PUT`
- **Descripción:** Actualiza la información de un país existente.
- **Request Body:**

  ```json
  {
    "id": 1, // (Requerido) ID del país
    "name": "string" // (Opcional) Nuevo nombre del país
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 4. Eliminar un País

- **URL:** `/api/Country/DeleteCountry`
- **Método:** `DELETE`
- **Descripción:** Elimina un país por su ID.
- **Query Parameter:**

  ```
  id (int32): (Requerido) ID del país a eliminar.
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

### Country

#### 1. Crear un Departamento

- **URL:** `/api/Department/CreateDepartment`
- **Método:** `POST`
- **Descripción:** Crea un nuevo departamento.
- **Request Body:**

  ```json
  {
    "name": "string", // (Requerido) Nombre del departamento
    "countryId": 1, // (Opcional) ID del país asociado
    "countryName": "string" // (Opcional) Nombre del país asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 2. Obtener Departamentos

- **URL:** `/api/Department/GetDepartments`
- **Método:** `GET`
- **Descripción:** Recupera la lista de todos los departamentos.
- **Respuesta Exitosa (200):** Operación exitosa.

#### 3. Actualizar un Departamento

- **URL:** `/api/Department/UpdateDepartment`
- **Método:** `PUT`
- **Descripción:** Actualiza la información de un departamento existente.
- **Request Body:**

  ```json
  {
    "id": 1, // (Requerido) ID del departamento
    "name": "string", // (Requerido) Nuevo nombre del departamento
    "countryId": 1, // (Opcional) ID del país asociado
    "countryName": "string" // (Opcional) Nombre del país asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 4. Eliminar un Departamento

- **URL:** `/api/Department/DeleteDepartment`
- **Método:** `DELETE`
- **Descripción:** Elimina un departamento por su ID.
- **Query Parameter:**

  ```
  id (int32): (Requerido) ID del departamento a eliminar.
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

### Municipality

#### 1. Crear un Municipio

- **URL:** `/api/Municipality/CreateMunicipality`
- **Método:** `POST`
- **Descripción:** Crea un nuevo municipio.
- **Request Body:**

  ```json
  {
    "name": "string", // (Requerido) Nombre del municipio
    "departmentId": 1, // (Opcional) ID del departamento asociado
    "departmentName": "string" // (Opcional) Nombre del departamento asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 2. Obtener Municipios

- **URL:** `/api/Municipality/GetMunicipalities`
- **Método:** `GET`
- **Descripción:** Recupera la lista de todos los municipios.
- **Respuesta Exitosa (200):** Operación exitosa.

#### 3. Actualizar un Municipio

- **URL:** `/api/Municipality/UpdateMunicipality`
- **Método:** `PUT`
- **Descripción:** Actualiza la información de un municipio existente.
- **Request Body:**

  ```json
  {
    "id": 1, // (Requerido) ID del municipio
    "name": "string", // (Requerido) Nuevo nombre del municipio
    "departmentId": 1, // (Opcional) ID del departamento asociado
    "departmentName": "string" // (Opcional) Nombre del departamento asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 4. Eliminar un Municipio

- **URL:** `/api/Municipality/DeleteMunicipality`
- **Método:** `DELETE`
- **Descripción:** Elimina un municipio por su ID.
- **Query Parameter:**

  ```
  id (int32): (Requerido) ID del municipio a eliminar.
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

### Users

#### 1. Crear un Usuario

- **URL:** `/api/Users/CreateUser`
- **Método:** `POST`
- **Descripción:** Crea un nuevo usuario.
- **Request Body:**

  ```json
  {
    "name": "string", // (Requerido) Nombre del usuario
    "cellphone": "string", // (Requerido) Teléfono del usuario
    "address": "string", // (Requerido) Dirección del usuario
    "countryId": 1, // (Opcional) ID del país asociado
    "countryName": "string", // (Opcional) Nombre del país asociado
    "departmentId": 1, // (Opcional) ID del departamento asociado
    "departmentName": "string", // (Opcional) Nombre del departamento asociado
    "municipalityId": 1, // (Opcional) ID del municipio asociado
    "municipalityName": "string" // (Opcional) Nombre del municipio asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 2. Obtener Usuarios

- **URL:** `/api/Users/GetUsers`
- **Método:** `GET`
- **Descripción:** Recupera la lista de todos los usuarios.
- **Respuesta Exitosa (200):** Operación exitosa.

#### 3. Actualizar un Usuario

- **URL:** `/api/Users/UpdateUser`
- **Método:** `PUT`
- **Descripción:** Actualiza la información de un usuario existente.
- **Request Body:**

  ```json
  {
    "id": 1, // (Requerido) ID del usuario
    "name": "string", // (Requerido) Nuevo nombre del usuario
    "cellphone": "string", // (Requerido) Nuevo teléfono del usuario
    "address": "string", // (Requerido) Nueva dirección del usuario
    "municipalityId": 1, // (Opcional) ID del municipio asociado
    "municipalityName": "string" // (Opcional) Nombre del municipio asociado
  }
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

#### 4. Eliminar un Usuario

- **URL:** `/api/Users/DeleteUser`
- **Método:** `DELETE`
- **Descripción:** Elimina un usuario por su ID.
- **Query Parameter:**

  ```
  id (int32): (Requerido) ID del usuario a eliminar.
  ```

- **Respuesta Exitosa (200):** Operación exitosa.

### Notas

- Los campos marcados como "opcional" en el cuerpo de la solicitud indican que puede utilizarse uno de los dos, ya sea el id o el name, pero se debe usar alguno de los dos. Sin embargo, aquellos marcados como "requerido" deben ser proporcionados para que la solicitud se ejecute correctamente.

- Los endpoints GET para listar entidades (países, departamentos, municipios, usuarios) no requieren parámetros adicionales.

- Los endpoints DELETE requieren únicamente el parámetro id correspondiente a la entidad a eliminar.
````
