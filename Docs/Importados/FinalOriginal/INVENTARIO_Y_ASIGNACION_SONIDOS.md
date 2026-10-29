# Inventario de 12 MP3 suministrados

Las duraciones proceden del análisis del ZIP aportado. El uso exacto de cada pista es una propuesta de integración; el nombre no verifica su licencia ni su contenido musical.

| Archivo | Duración aprox. | Asignación prevista | Estado en este kit |
|---|---:|---|---|
| `mario-bros-salto.mp3` | 0.39 s | salto de Mario | **Conectado en prototipo** |
| `mario-moneda.mp3` | 0.81 s | coleccionar monedas / activar sellos | **Conectado a sellos** |
| `mario-bros-vida.mp3` | 0.84 s | vida extra / inicio de victoria | **Conectado a victoria y derrota de Bowser** |
| `mario-bros-vida-1.mp3` | 0.94 s | variante de 1UP | Pendiente de integrar en nivel real |
| `mario-bros-tuberia.mp3` | 0.86 s | entrada y salida de tubería | Pendiente de conectar a Pipe.cs |
| `mario-bros-game-over-1.mp3` | 3.86 s | pantalla de derrota | Pendiente de conectar a escena Game Over |
| `mario-monedas-varias-veces.mp3` | 12.04 s | bonus de monedas | Pendiente de mecánica bonus |
| `mario-bros-1.mp3` | 11.09 s | música de ambiente alternativa | Reservada |
| `super-mario-bros.mp3` | 29.07 s | música para nivel terrestre | Pendiente de integración global |
| `super-mario-bros-castillo.mp3` | 7.24 s | música en castillo | **Conectado al prototipo** |
| `mario-bros-castillo.mp3` | 10.74 s | alternativa de música del castillo | Reservada |
| `super-mario.mp3` | 40.66 s | música durante créditos de victoria | **Conectado a VictoriaFinal** |

Los audios se guardan en `Assets/Resources/MarioAudio/` para que los scripts puedan cargarlos con `Resources.Load<AudioClip>("MarioAudio/nombre-sin-extension")`.

## Integración recomendada por fases

1. Final original: castillo, sellos, caída y victoria (este kit).
2. Game Over: disparar `mario-bros-game-over-1` cuando vidas = 0, sin confundir muerte temporal con Game Over.
3. Tubería: disparar `mario-bros-tuberia` al iniciar transición válida, una sola vez.
4. Moneda: disparar `mario-moneda` después de contar la moneda, sin ejecutarlo cada frame.
5. Nivel acuático: seleccionar música adecuada o sustituir por sonido ambiente/licenciado.
6. Revisión: comprobar que no se reproduzcan dos músicas simultáneas al cargar una escena.
