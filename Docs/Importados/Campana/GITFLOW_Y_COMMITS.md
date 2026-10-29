# Flujo GitFlow para integración completa

Repositorio: `https://github.com/Developer-vic1/SUPER-MArio-Victor.git`

1. `main`: base del profesor y releases aceptadas.
2. `develop`: integración y pruebas; se crea desde main si aún no existe.
3. `feature/campana-completa-unity6`: rama generada por instalador con todas las nuevas piezas del proyecto. Se publican commits separados por recursos, gameplay, escenas, pruebas.
4. PR `feature/campana-completa-unity6` -> `develop`, tras compilar y verificar el recorrido completo.
5. `release/1.0.0` nace de develop ya integrado; prueba en Windows, correcciones y PR a `main`.
6. Se puede etiquetar `v1.0.0` sobre el commit mergeado de main, una vez aprobado.

Comandos de referencia:

```bat
git status
git log --oneline --decorate -10
git branch -vv
git push -u origin feature/campana-completa-unity6
```

Mensajes preparados por BAT 05:

- `chore(assets): incorporar sprites originales, Tiko y efectos de audio`
- `feat(gameplay): integrar movimiento, natacion, enemigos, vidas y jefe final`
- `feat(scenes): incorporar menu, cuatro niveles, game over y victoria`
- `test(unity): agregar validacion del editor y compilacion Windows64`
- `chore(unity): registrar migracion a Unity 6000.3.16f1` (si Unity realmente cambia archivos de configuración).

**No se realizaron estos commits en GitHub durante la generación del paquete.** Los ejecutan los archivos BAT en el clon local del propietario. No usar `push --force`, no saltar la revisión de PR, no fusionar a main si las pruebas fallan.

Para futuros ajustes pequeños, crear una nueva rama `feature/ajustar-nivel-acuatico`, `feature/animaciones-castillo` o `fix/colision-bowser` desde develop; no trabajar directamente sobre main.
