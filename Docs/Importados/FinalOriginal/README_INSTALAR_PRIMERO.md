# MARIO VICTOR - FINAL ORIGINAL «EL REINO LIBERADO»

## Estado de esta entrega

Kit fuente para Unity **6000.3.16f1**. Contiene 6 scripts de runtime, 1 generador de escenas para el Editor, 12 sonidos MP3 aportados por el equipo, documentos y 7 BAT para GitFlow. **NO contiene un ejecutable probado ni modifica automáticamente GitHub.** Este paquete no sustituye el resto del videojuego.

## Concepto original (requisito Castillo de Bowser)

Mario recorre un castillo propio. Debe tocar tres sellos (dorado, celeste y rosa). Cada sello desactiva una parte del escudo de Bowser. Al activar el tercero, Bowser cae durante una animación y se abre la escena **VictoriaFinal** con el mensaje «¡EL REINO LIBERADO!». La pantalla reproduce un sonido de victoria, luego música y permite **Jugar de nuevo** o **Salir**.

Esta solución se diferencia de una réplica del castillo clásico: propone una **mecánica de tres sellos**. El modelo de Bowser en la escena de prototipo es **un Koopa agrandado temporal**; sustituirlo por un sprite legalmente reutilizable en la versión final.

## Instalación, exacta y sin sobreescribir la base

1. Guarda una copia local del proyecto y confirma una rama GitFlow `feature/final-propio` creada desde `develop` (o prepara esa rama con el BAT antes de copiar estos archivos).
2. **Cierra Unity** antes de copiar. Abre este kit: copia las carpetas `Assets/Scripts/Final`, `Assets/Editor` y `Assets/Resources/MarioAudio` DENTRO de la carpeta `Assets/` de tu proyecto Unity. No reemplaces `Assets` completo ni copies `ProjectSettings` de otro proyecto.
3. Abre el proyecto con **Unity 6000.3.16f1**. Espera a que Unity importe audios/scripts y genere sus archivos `.meta`.
4. Confirma **Edit > Project Settings > Player > Other Settings > Active Input Handling = Both** o **Input Manager (Old)**, ya que el proyecto original utiliza `Input.GetAxisRaw` y `Input.GetButton`.
5. En la barra superior haz clic en **Super Mario Victor > Generar final original y castillo de prueba**. Confirma el diálogo.
6. El generador crea `Assets/Scenes/VictoriaFinal.unity` y `Assets/Scenes/CastilloFinal_Prototipo.unity` **solo si no existían**. También agrega ambas escenas al Build Profile activo. NO edita `1-1.unity`.
7. Abre la escena **CastilloFinal_Prototipo** desde Project. Presiona Play. Muévete con **A/D o flechas**, salta con **Espacio**, activa los tres sellos y espera la transición a la escena de victoria.
8. Pulsa **JUGAR DE NUEVO** en la pantalla final; debe volver al nivel `1-1` (que ya debe existir). Comprueba también que **SALIR** detenga la reproducción en el Editor.
9. En el castillo definitivo `8-4`, pon un objeto con `FinalCastleManager`, tres objetos con `FinalCastleSeal` (collider en trigger y referencia al manager) y asigna el Transform visual del Bowser real. La escena de victoria se cargará tras activar los tres sellos. El personaje del nivel definitivo usa su prefab normal `Mario` con tag `Player`.
10. Comprueba que `VictoriaFinal.unity` está habilitada en **File > Build Profiles > Scene List** del perfil de Windows. El proyecto debe compilar sin errores antes de integrarlo en `develop`.

## Archivos y responsabilidades

- `Assets/Scripts/Final/FinalCastleManager.cs`: contador de 3 sellos, audio de castillo, animación de caída y cambio a VictoriaFinal.
- `FinalCastleSeal.cs`: interacción mediante trigger con el jugador, no permite duplicados.
- `FinalVictoryScene.cs`: interfaz original, texto final, estrellas, música, reiniciar, salir.
- `FinalDemoPlayer.cs`: movimiento simple de PRUEBA, no reemplaza a `PlayerMovement` original.
- `FinalDemoCamera.cs`: cámara de PRUEBA, no reemplaza a `SideScrolling` original.
- `FinalDemoLava.cs`: peligro de PRUEBA. En el juego real se debe usar el GameManager para descontar vidas.
- `Assets/Editor/CrearFinalMarioVictor.cs`: crea escenas sin editar escenas existentes ni sobrescribir las ya creadas.
- `Assets/Resources/MarioAudio/*.mp3`: 12 archivos de audio aportados por el equipo.

## Límites conocidos y acciones pendientes

- **Sin prueba ejecutada dentro de Unity en este entorno**: hay que compilar y jugar en Windows, resolver cualquier error concreto de consola, validar sonidos y build.
- La demo NO es el castillo 8-4 definitivo. No tiene IA de Bowser, daño real ni vidas: sirve para probar la mecánica y la secuencia de final. Para cumplir el 25% del castillo todavía hay que diseñar el escenario definitivo y su jefe.
- El sistema de sellos **no modifica ni sustituye** la lógica oficial de vidas. Game Over se trabajará por separado.
- El código no incorpora todos los efectos globales: salto funciona en la demo, moneda/vida/castillo/victoria allí mismo. El resto requiere integración con Player, Pipe, BlockCoin y GameManager del proyecto base.
- Si la escena definitiva ya existe, **no uses el generador para sustituirla**; integra los componentes manualmente.

## Derechos de autor y GitHub

Los 12 MP3 son los archivos que subió el equipo; algunos nombres indican música de la franquicia Super Mario. Que sean útiles para una práctica no demuestra que exista licencia para redistribuirlos públicamente. Verifica derechos antes de subirlos a un repositorio público o publicar un ejecutable. De ser necesario, reemplázalos por recursos propios o licenciados; los scripts solo requieren conservar los nombres de los recursos.
