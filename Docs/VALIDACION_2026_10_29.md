# Validación real — 29 de octubre de 2026

## Entorno

Unity **6000.3.16f1**, Windows de 64 bits. Unity CLI **1.0.0-beta.15**, descargada del CDN oficial y verificada por SHA-256. Unity Pipeline **0.9.1-exp.1**.

## Resultado de las comprobaciones

- Compilación de C#: **0 errores y 0 advertencias de compilador**.
- Suite de aceptación de campaña: **36 comprobaciones aprobadas**, sin excepciones ni errores de runtime durante la suite.
- Incluye conservación del mapa original con bloques y tuberías, controles simulados de movimiento/salto, pausa, monedas y vidas del sistema original, bandera y cambios de mundo por colisión física, natación, bloqueo del portal, escudo, tres sellos físicos, tres golpes al jefe, animación del final, victoria, reinicio y Game Over.
- MCP: handshake real `initialize` y `tools/list` correcto; **169 herramientas** ofrecidas por el Editor de Mario.
- Menú: captura de la pantalla real con Canvas y créditos, revisada visualmente. Se corrigieron la superposición de sprites, el contraste de botones y la falta de espacio para el título.

Evidencias en `Docs/Evidencias`: resultado JSON de la suite, comprobación MCP y captura del menú. Los logs de trabajo permanecen fuera de Git en `Logs/MarioQA`.

## Correcciones verificadas

- `EditorApplication.isBatchMode` fue reemplazado por `Application.isBatchMode`.
- Se actualizaron usos obsoletos de `Rigidbody2D.isKinematic` a `bodyType`.
- Se corrigió la carga de escenas y se incluyó el nivel original completo en la lista de compilación.
- Los contadores, muertes y bandera originales están conectados a la campaña. La muerte y la bandera tienen protección contra activaciones duplicadas.
- Se evita iniciar varias transiciones de tubería al mantener la tecla presionada.
- Los doce audios se comparten entre los dos paquetes; la música de victoria respeta la preferencia del menú.

## Alcance

Las entradas de la suite se simularon dentro de Unity y varias interacciones se verificaron con física real mediante colocación controlada del personaje. Esto valida las conexiones y regresiones probadas; **no equivale a una partida manual completa de todos los recorridos**. La dificultad de todas las plataformas, la percepción de los sonidos y las preferencias artísticas requieren revisión humana.

La advertencia del Editor sobre futura deprecación de Input Manager es informativa. Se conserva porque el personaje, las tuberías y la interfaz originales dependen de él; cambiar de sistema de entrada está fuera de esta integración.

## Compilación de Windows

Compilación **Succeeded** para Windows de 64 bits, con **0 errores y 0 advertencias**. El informe registra 94.599.264 bytes y se conserva en `Docs/Evidencias/build.json`.

Se abrió `Builds/Windows64/SuperMarioVictor.exe` fuera del Editor y se revisó visualmente el menú a **800×600**: título, botones, ilustración y créditos permanecen visibles sin superposición. La comprobación interactiva adicional de teclado en el ejecutable fue interrumpida mediante Escape; no se registra como aprobada. Las comprobaciones de entrada simulada dentro de Unity sí están incluidas en la suite de 36 pruebas.
