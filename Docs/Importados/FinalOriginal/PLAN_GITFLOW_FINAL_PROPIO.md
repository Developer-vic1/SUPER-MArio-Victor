# GitFlow específico — final propio y sonidos

**Editor:** Unity 6000.3.16f1. **Repositorio objetivo:** Developer-vic1/SUPER-MArio-Victor.  
**Rama exclusiva:** `feature/final-propio` nacida desde `develop` después de que se haya validado la migración a Unity 6.  
**PR:** `feature/final-propio` -> `develop`. Nunca usar `main` para desarrollar este final.

## Orden de commits significativos (crearlos solamente después de probar su funcionalidad)

| Orden | Mensaje del commit | Archivos | Resultado / prueba |
|---|---|---|---|
| 1 | `feat(final): implementar tres sellos y caida de Bowser` | `FinalCastleManager.cs`, `FinalCastleSeal.cs` | Los tres triggers cuentan una sola vez; después del tercero inicia la caída |
| 2 | `feat(victory): crear interfaz original de victoria` | `FinalVictoryScene.cs` | Se ve "EL REINO LIBERADO", botones Jugar de nuevo/Salir |
| 3 | `feat(editor): generar castillo prototipo y escena final` | `CrearFinalMarioVictor.cs`, `FinalDemo*.cs`, escenas Unity y sus `.meta`, `EditorBuildSettings.asset` | Se pueden crear y abrir ambas escenas; el prototipo llega al final |
| 4 | `feat(audio): integrar sonidos de castillo y victoria` | `Assets/Resources/MarioAudio/*.mp3` y `.meta` | Salto, sellos, música y victoria se escuchan; solo versionar audios con licencia adecuada |
| 5 | `docs(final): registrar pruebas y secuencia de victoria` | `Docs/*`, capturas opcionales | Trazabilidad y pasos reproducibles |

**Atención:** este kit se copia completo a Unity para poder probarlo, pero los commits se separan manualmente en el orden anterior. Los BAT del kit GitFlow anterior `03_COMMIT_GUIADO.bat` y `04_SUBIR_FEATURE_Y_ABRIR_PR.bat` permiten escoger archivos por commit.

## Comandos Git equivalentes (ejecutar desde la carpeta raíz del repositorio)

```bat
git status --short
git switch develop
git pull --ff-only origin develop
git switch -c feature/final-propio
```

Después de copiar código y probar:

```bat
git add -- Assets/Scripts/Final/FinalCastleManager.cs Assets/Scripts/Final/FinalCastleSeal.cs
git add -- Assets/Scripts/Final/FinalCastleManager.cs.meta Assets/Scripts/Final/FinalCastleSeal.cs.meta
git diff --cached --stat
git commit -m "feat(final): implementar tres sellos y caida de Bowser"
```

Para las siguientes tareas, preparar solamente los archivos correspondientes. Verificar `git diff --cached` en cada etapa. **Nunca ejecutar `git add .` por costumbre ni `git push --force`**.

```bat
git push -u origin feature/final-propio
```

Crear Pull Request en GitHub hacia `develop`. Tras las pruebas y el merge, integrar más adelante el final en la escena real `8-4` y su sistema de vidas.

## Matriz de aceptación

- [ ] Unity 6000.3.16f1 sin errores de compilación.
- [ ] El generador no reemplaza escenas existentes.
- [ ] En CastilloFinal_Prototipo Mario se mueve y salta.
- [ ] Los tres sellos se pueden activar y no suman doble.
- [ ] El último sello inicia la derrota visual de Bowser.
- [ ] Se muestra VictoriaFinal, con música y estrellas.
- [ ] JUGAR DE NUEVO regresa a 1-1.
- [ ] SALIR termina Play dentro de Unity.
- [ ] `VictoriaFinal` consta en Scene List del Build Profile activo.
- [ ] La integración real en `8-4` se realiza y prueba antes de la versión 1.0.0.
- [ ] No se redistribuyen MP3 sin comprobar derechos de uso.
