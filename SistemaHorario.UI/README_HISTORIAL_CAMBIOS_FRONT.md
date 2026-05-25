# Front historial de cambios

Este paquete actualiza la pantalla de historial de cambios para consumir los endpoints reales del backend.

## Archivos incluidos

- `SistemaHorario.UI/Services/HistorialCambiosApiService.cs`
- `SistemaHorario.UI/Models/UI/HistorialCambioItem.cs`
- `SistemaHorario.UI/ViewModels/HistorialCambios/HistorialCambiosViewModel.cs`
- `SistemaHorario.UI/ViewModels/HistorialCambios/HistorialCambiosView.xaml`
- `SistemaHorario.UI/ViewModels/HistorialCambios/HistorialCambiosView.xaml.cs`

## Funcionalidades

- Consulta historial real desde la API.
- Filtra por usuario.
- Filtra por módulo.
- Filtra por acción.
- Filtra por fechas.
- Busca por texto.
- Muestra tabla con fecha, hora, usuario, rol, módulo, acción y descripción.
- Permite ver detalle del cambio seleccionado.

## Importante

La vista del proyecto ya estaba ubicada en `ViewModels/HistorialCambios`, aunque su namespace es `SistemaHorario.UI.Views.HistorialCambios`. Por eso se mantiene esa ruta para no romper la navegación actual.
