# SUPER MARIO VICTOR — LA ÚLTIMA ESTRELLA

**Entrega de código fuente + instalador GitFlow**, preparada para **Unity 6000.3.16f1** (Windows 64-bit). Fecha: 29/10/2026.

## Dos formas de abrir el juego

**Opción A (recomendada, conserva el fork):**
1. Instalar Git for Windows y Unity 6000.3.16f1 con módulo Windows Build Support.
2. Extraer el ZIP fuera de `Assets` o de cualquier proyecto Unity.
3. Con Unity cerrado, ejecutar `BAT/00_INSTALAR_Y_CREAR_RAMA.bat`. Clona `Developer-vic1/SUPER-MArio-Victor` si hace falta, crea `develop` y `feature/campana-completa-unity6`, conserva los archivos del repositorio e incorpora la carpeta nueva `Assets/MarioVictor`.
4. Abrir el proyecto `SUPER-MArio-Victor` creado por el BAT desde Unity Hub, con **Unity 6000.3.16f1**. Esperar a que termine la migración/importación. No editar con dos instancias del editor simultáneamente.
5. Abrir `Assets/MarioVictor/Scenes/MV_Menu.unity` y presionar **Play**. Todas las escenas de la campaña están registradas con el menú primero.
6. Ejecutar las validaciones `BAT/01_VALIDAR_ARCHIVOS.bat` y `BAT/02_VALIDAR_CON_UNITY6.bat` (con editor cerrado). Compilar con `BAT/03_COMPILAR_WINDOWS64.bat`.
7. Probar manualmente todo el recorrido, crear commits con el BAT 05, subir feature con el 06, integrar PR a develop y después hacer release/1.0.0 hacia main.

**Opción B (standalone):**
Abre con Unity Hub la carpeta extraída de `PROYECTO_AUTONOMO_UNITY6.zip` (se entrega también). Es una campaña independiente con las mismas escenas y assets, pero **no contiene los prefabs ni la escena original del docente**. Para cumplir el requisito de fork, usa la opción A.

## Controles

- **A/D o flechas izquierda/derecha:** desplazarse.
- **Espacio, W o flecha arriba:** salto; en el nivel acuático se mantiene para nadar.
- **Escape:** pausa, reiniciar o regresar al menú.
- **Mouse:** botones de menú, Game Over y victoria.

## Escenas incluidas

| Escena | Contenido |
| --- | --- |
| `MV_Menu` | Menú principal, controles, música, iniciar, salir |
| `MV_1-1` | Nivel terrestre, goombas, koopas, monedas, extra life, meta |
| `MV_1-2` | Subterráneo con bloques, obstáculos, tuberías, enemigos y salida |
| `MV_2-2` | Acuático con natación, peces, medusas, coral, obstáculos y salida |
| `MV_8-4` | Castillo original, lava, tres sellos, Tiko, guardián de cenizas y portal final |
| `MV_GameOver` | Fin de vidas, puntuación y reintentar |
| `MV_Victoria` | Final «La última estrella», Tiko, liberación del reino y reiniciar |

`Assets/Scenes/1-1.unity` **permanece intacta** en el fork, pero la campaña usa una nueva escena `MV_1-1` diseñada para integrarse con el sistema de vidas.

## Sistema de vidas

Tres vidas iniciales; perder por contacto con enemigo, lava, caída o agotar el tiempo; después de perder vida se recarga el mismo nivel. Con 0 vidas se carga `MV_GameOver`. Cada 100 monedas da una vida adicional, también aparecen hongos 1-UP. Las monedas y los puntos persisten entre escenas; las tres estrellas/sellos del castillo se restablecen al reintentar el castillo.

## Integración y rutas

`Assets/MarioVictor/Resources/MarioVictor/Sprites` contiene sprites aportados por el usuario y recursos nuevos originales (Tiko, peces, medusas, sello, estrella, jefe, lava). 
`Assets/MarioVictor/Resources/MarioVictor/Audio` contiene los 12 MP3 provistos. 
`Assets/MarioVictor/Scripts` contiene el sistema de juego; `Assets/MarioVictor/Scenes` las 7 escenas; `Assets/MarioVictor/Editor/MVBuildValidator.cs` la verificación y compilación.

## Precauciones de compatibilidad

- Unity 6 dispone de **File > Build Profiles > Scene List**. El instalador actualiza el *global* `ProjectSettings/EditorBuildSettings.asset` para poner `MV_Menu` primero. Si un build profile personal sobrescribe su propia lista, corrígela desde Unity.
- Las entradas usan `MVKeys` para aceptar el Input Manager antiguo o el nuevo Input System si está habilitado e instalado. Si el movimiento no responde, usa **Edit > Project Settings > Player > Active Input Handling = Input Manager (Old)** y reinicia Unity.
- Si al abrir las `.unity` aparecen referencias rotas, usa el menú **Mario Victor > 0. Reparar escenas generadas**; esto regenera únicamente las escenas `MV_*` y registra el menú de arranque, no toca la escena `1-1` del docente.
- La validación PowerShell y la validación del editor son pruebas de **integridad**, no sustituyen jugar todas las escenas.
- El entorno que generó este ZIP no tiene Unity instalado: **no es posible afirmar que el proyecto ha compilado o que se ha probado el ejecutable aquí**. Ejecuta los BAT 02/03 y las pruebas manuales antes de hacer merge a main.

## Derechos de los recursos

Los sprites y MP3 originales fueron enviados por quien solicitó el proyecto. La presencia de esos recursos en un ZIP **no acredita autorización** para su redistribución pública; revisa sus licencias antes de subirlos a GitHub. Tiko, peces, medusas, sellos, jefe y otros sprites nuevos fueron dibujados originalmente para esta entrega. No se copia el sprite oficial de Yoshi.

## Alcance respecto a la rúbrica

Incluye representaciones jugables de niveles subterráneo y acuático, sin certificación de réplica exacta de los mapas NES. Si el docente exige réplica geométrica fiel, todavía se requiere comparar el trazado y ajustar plataformas/obstáculos a referencias del original. El castillo final y el final narrativo son propuestas originales.
