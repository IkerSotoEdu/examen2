# Sistema de reservas de talleres

Aplicación de escritorio en C# con Windows Forms para registrar y consultar reservas de talleres. La interfaz inicial está en tema oscuro y se encuentra en `Interfaz.cs`.

## Estado actual

La aplicación contiene únicamente la interfaz. Los controles permiten preparar estas operaciones, pero todavía no guardan información ni ejecutan acciones:

- Seleccionar un taller.
- Capturar el nombre de la persona responsable y la cantidad de lugares.
- Elegir el turno matutino o vespertino e indicar si se requiere material.
- Mostrar las reservas en una tabla.
- Mostrar lugares disponibles, cantidad de reservas y lugares ocupados.
- Presentar los botones para registrar, cancelar y limpiar.

Los talleres cargados en el selector son ejemplos visuales. Los indicadores muestran guiones porque aún no están conectados a datos.

## Requisitos funcionales propuestos

### RF-01. Catálogo y disponibilidad de talleres

- Cargar el catálogo desde una fuente de datos en lugar de depender de los elementos de ejemplo de la interfaz.
- Cada taller debe tener un identificador, nombre y capacidad.
- Mostrar la disponibilidad del taller seleccionado.
- Calcular los lugares ocupados sumando los lugares de sus reservas activas.
- Calcular los disponibles como capacidad menos lugares ocupados.
- No permitir que la disponibilidad sea negativa.

### RF-02. Registrar una reserva

Al pulsar **Registrar reserva**, validar la captura y, si es correcta:

1. Crear una reserva con taller, responsable, cantidad de lugares, turno y requisito de material.
2. Guardarla en la colección o almacenamiento elegido.
3. Actualizar la tabla y los indicadores.
4. Informar claramente si se registró o si ocurrió un error.

Validaciones mínimas:

- Debe seleccionarse un taller válido.
- El nombre del responsable no puede estar vacío; eliminar espacios al inicio y al final.
- La cantidad debe ser mayor que cero.
- La cantidad solicitada no debe exceder los lugares disponibles.
- El turno debe corresponder a una de las opciones disponibles.

### RF-03. Cancelar una reserva

- Requerir que haya una fila seleccionada.
- Pedir confirmación antes de cancelar.
- Cancelar únicamente la reserva seleccionada.
- Actualizar la tabla y los indicadores para liberar sus lugares.
- Informar si no hay una selección o si la cancelación falla.

### RF-04. Limpiar la captura

Al pulsar **Limpiar captura**:

- Limpiar el responsable.
- Restablecer la cantidad a su valor inicial.
- Restablecer el turno predeterminado y desmarcar el requisito de material.
- Mantener las reservas existentes sin cambios.
- Definir si también se conserva el taller seleccionado; se recomienda conservarlo para facilitar capturas consecutivas.

### RF-05. Consultar y resumir reservas

- Mostrar una fila por reserva activa.
- Mantener las columnas actuales: responsable, taller, lugares, turno y material.
- Mostrar el número de reservas registradas y la suma de lugares ocupados.
- Actualizar los datos después de registrar o cancelar una reserva.
- Presentar la tabla vacía de forma clara cuando aún no existan reservas.

## Datos sugeridos

Una reserva debería conservar, como mínimo:

| Campo | Tipo sugerido | Descripción |
|---|---|---|
| `Id` | `int` o `Guid` | Identificador único para seleccionar y cancelar la reserva. |
| `TallerId` | `int` o `Guid` | Referencia al taller reservado. |
| `Responsable` | `string` | Nombre capturado para la reserva. |
| `CantidadLugares` | `int` | Número de lugares solicitados. |
| `Turno` | enumeración | `Matutino` o `Vespertino`. |
| `RequiereMaterial` | `bool` | Indica si la reserva requiere material. |
| `Activa` | `bool` | Permite excluir reservas canceladas de los totales si se conserva su historial. |

Un taller debería incluir `Id`, `Nombre` y `Capacidad`. La capacidad y el catálogo real de talleres todavía deben definirse.

## Guía de implementación

Para mantener la interfaz independiente de las reglas del negocio:

1. Crear modelos de dominio para taller y reserva.
2. Definir un servicio que registre, cancele y calcule disponibilidad.
3. Conectar los eventos de los botones y controles en `Interfaz.cs`; dejar el diseño visual en sus métodos de construcción.
4. Enlazar la tabla a una lista de reservas mediante `BindingSource` o el mecanismo de enlace elegido, en vez de modificar celdas desde distintas partes del formulario.
5. Actualizar los indicadores desde los datos calculados por el servicio.
6. Empezar con almacenamiento en memoria para validar el flujo. Incorporar persistencia solo cuando se decida el formato o base de datos.
7. Añadir pruebas para las reglas del servicio antes de conectar persistencia.

Los campos de controles de `Interfaz` son privados. Si las reglas se implementan fuera del formulario, es preferible que el formulario llame a un servicio y no que otras clases accedan directamente a esos controles.

## Criterios de aceptación

- Una captura válida crea una reserva visible en la tabla.
- Una captura inválida muestra un mensaje comprensible y no cambia los datos.
- La cantidad de lugares disponibles cambia correctamente al registrar o cancelar.
- No se puede superar la capacidad del taller.
- Cancelar modifica únicamente la reserva seleccionada.
- Limpiar no borra ni cancela reservas existentes.
- Los indicadores coinciden con las reservas activas mostradas.

## Ejecutar

Requiere el SDK de .NET 10 y Windows, ya que el proyecto usa Windows Forms.

```powershell
dotnet run --project .\examen2.csproj
```
