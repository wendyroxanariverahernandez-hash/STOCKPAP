# STOCKPAP

Sistema de inventario para papeleria.

## Como ejecutar en Visual Studio

1. Abre `stockpap.sln`.
2. Selecciona la plataforma `x64`.
3. Verifica que PostgreSQL este iniciado.
4. Crea la base de datos `stockpap_db`.
5. Ejecuta el script `database_setup.sql` dentro de esa base de datos.
6. Presiona `F5` para iniciar.

Usuarios de prueba:

- Usuario: `administrador` / Contrasena: `wendy123`
- Usuario: `cajero` / Contrasena: `cajero123`
- Usuario: `consulta` / Contrasena: `consulta123`

Roles:

- `Admin`: acceso total y autorizacion para cancelar ventas.
- `Ventas`: puede vender, pero necesita autorizacion de administrador para cancelar.
- `Consulta`: solo puede revisar informacion.

La pantalla `Reportes` junta inventario y movimientos. Permite descargar por separado:

- Inventario en CSV o TXT.
- Movimientos en CSV o TXT.

Si tu usuario o contrasena de PostgreSQL son diferentes, cambia la cadena `StockPapDb` en `App.config`.
