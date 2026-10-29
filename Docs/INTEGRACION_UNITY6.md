# Integración de Mario Víctor — 29 de octubre de 2026

Autor: **Víctor · VictorDev**. Base original del tutorial de Zigurous, con el historial del fork conservado.

## Juego y fuentes

Se integraron los dos paquetes entregados en el repositorio existente. El punto de partida jugable es `Assets/Scenes/1-1.unity`, con su recorrido original completo, prefab Mario, enemigos, bloques, poderes y tuberías. El controlador de campaña conecta sus monedas, vidas y bandera con los siguientes mundos; no sustituye el mapa por el nivel corto del ZIP.

La secuencia es menú → nivel original 1-1 → subterráneo 1-2 → acuático 2-2 → castillo 8-4 → final original. Los tres mundos añadidos se ampliaron a 195, 207 y 190 unidades. La construcción de esos mundos se realiza con los sprites existentes en `MVGameController`; el mapa original sigue siendo una escena de autoría.

En el castillo se conservan tres sellos, escudo del jefe, tres impactos desde arriba, animación de derrota y portal. `FinalCastleManager` y `FinalVictoryScene` reutilizan el final del segundo ZIP. El prototipo `CastilloFinal_Prototipo` se conserva como herramienta adicional; no sustituye el castillo de campaña.

Todo el juego usa sprites, cámara ortográfica y física 2D. El menú utiliza Canvas, botones con navegación de teclado, control de música, movimiento reducido y el crédito del autor. Los sonidos de ambos ZIP se comparten en `Assets/MarioVictor/Resources/MarioVictor/Audio`, evitando copias duplicadas.

Los documentos de `Docs/Importados` son el material original de los ZIP: describen su entrega anterior y pueden contener rutas o procedimientos que ya no corresponden a esta integración. No se ejecutaron sus BAT de commits, mezclas o publicación.

## Abrir, probar y compilar

1. Abrir este proyecto con **Unity 6000.3.16f1** y esperar la importación.
2. Play entra por `MV_Menu`. Jugar carga el nivel original completo.
3. Mantener **Input Manager (Old)** en Active Input Handling: el movimiento original lo utiliza. La advertencia de futura deprecación no significa que el proyecto sea 3D.
4. Menú **Mario Victor → 1. Verificar proyecto completo** revisa escenas y recursos.
5. Menú **Mario Victor → 3. Ejecutar pruebas de aceptación 2D**, fuera de Play, ejecuta comprobaciones de entrada simulada y física, transiciones y final. El resultado real se escribe en `Logs/MarioQA/acceptance.json`.
6. Salir de Play y usar **Mario Victor → 2. Verificar y compilar Windows 64-bit** o `Tools/Build-Windows.ps1`.
7. Entregar la carpeta completa `Builds/Windows64`, incluidos el EXE, `UnityPlayer.dll` y la carpeta de datos. El EXE solo no es una distribución completa.

Las pruebas automatizadas de navegación no sustituyen una partida completa de una persona ni la revisión auditiva. Los resultados verificados se registran por separado en `Docs/VALIDACION_2026_10_29.md`.

## MCP de este proyecto

El paquete Unity Pipeline está fijado en `Packages/manifest.json`. La configuración del servidor de este proyecto está en `.vscode/mcp.json` y usa `Tools/MCP/Start-MarioMcp.ps1`, que calcula la ruta de Mario a partir de su propia carpeta. No depende de CAMPUS_GLITCH.

El lanzador prefiere la CLI local en `Logs/Tools/unity.exe`, cuando está disponible, y usa la CLI instalada en otras máquinas. Para utilizar el MCP, Unity debe estar abierto y haber terminado de compilar. VS Code utiliza la configuración de `.vscode/mcp.json` al abrir esta carpeta de proyecto.

## GitFlow

Se preservó primero la migración local a Unity 6 en `develop`. Las mejoras se desarrollan en `feature/mario-unity6-integracion`, con commits significativos en español separados por migración, integración, interfaz, jugabilidad, compilación, herramientas y validación. El cierre sigue `feature/mario-unity6-integracion` → `develop` → `release/mario-unity6` → `main`, con fusiones que conservan los commits de cada trabajo. El historial original anterior a esta entrega se mantiene intacto.
