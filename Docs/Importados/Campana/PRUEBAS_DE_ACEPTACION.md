# Pruebas de aceptación manual — No marcar como aprobado sin ejecutar

| ID | Prueba | Esperado |
| --- | --- | --- |
| P01 | Abrir `MV_Menu` y pulsar JUGAR | Carga `MV_1-1`, 3 vidas |
| P02 | Moverse y saltar con teclado | Mario se mueve sin atravesar suelo |
| P03 | Recoger monedas | Suben puntos y monedas, suena efecto |
| P04 | Recoger 1-UP | Aumenta contador de vidas |
| P05 | Caer o colisionar con enemigo | Se pierde exactamente 1 vida |
| P06 | Perder las tres vidas | `MV_GameOver`, no reinicio automático silencioso |
| P07 | Reintentar Game Over | Nueva partida con 3 vidas y 0 monedas |
| P08 | Terminar nivel tierra | Carga `MV_1-2` |
| P09 | Terminar nivel subterráneo | Carga `MV_2-2` |
| P10 | Mantener espacio en agua | Natación funcional, gravedad más baja |
| P11 | Colisionar con pez/medusa | Se pierde exactamente una vida |
| P12 | Terminar nivel acuático | Carga `MV_8-4` |
| P13 | Localizar y activar 3 sellos | Indicador 3/3, escudo del jefe desactivado |
| P14 | Intentar portal sin vencer jefe | Portal bloqueado con indicación |
| P15 | Saltar sobre el jefe tres veces | Jefe derrotado y salida disponible |
| P16 | Entrar portal | Escena `MV_Victoria` y música final |
| P17 | Botón JUGAR DE NUEVO en victoria | Inicio con vidas reiniciadas |
| P18 | Pulsar ESC en niveles | Pausa funcional, botón Continuar |
| P19 | Apagar música al pasar de escena | No se superponen varios temas |
| P20 | Compilar Windows64 | EXE y carpeta data creada correctamente |
| P21 | Abrir EXE fuera de editor | Funciona menú, controles, escenas y audio |
| P22 | Descarga limpia de `main` después del PR | La primera escena de build es MV_Menu y todos los recursos se importan |

**Estado del momento de entrega del ZIP:** todas las pruebas P01-P22 están **pendientes de ejecución real en Unity**. El análisis estático del ZIP se registra por separado.
