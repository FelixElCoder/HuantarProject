# Huántar: La mirada del forastero

> Título provisional · Versión **0.1.0-beta**

Videojuego de terror y suspenso en primera persona para PC, con estética inspirada en la primera PlayStation. Está ambientado en el valle de Chavín de Huántar (Áncash, Perú) a inicios del siglo XVII.

El jugador controla a un joven investigador español que huye de noche por una quebrada cubierta de niebla. Lo acechan criaturas del folclore andino: el **condenado** y las **umas**. Mientras avanza, registra en su **cuaderno de campo** datos reales sobre Chavín, la sociedad colonial y las creencias andinas.

Este repositorio contiene el producto de la tesis **"Desarrollo de un videojuego serio para fomentar el turismo en zonas arqueológicas del Perú"**, de la Facultad de Ciencias e Ingeniería de la Pontificia Universidad Católica del Perú (PUCP).

---

## Estado del proyecto

La versión actual es una **beta jugable** de la sección de la quebrada del Acto 1. Incluye el núcleo de mecánicas del juego:

| Incluido en la beta | Pendiente |
|---|---|
| Movimiento en primera persona con resistencia, carrera y postura agachada | Zona del pueblo (exposición y huida de la turba) |
| Farol con riesgo y recompensa frente al condenado | Zona del templo y cinemática del guardián felino-chamán |
| Escondites y vegetación que bloquea la visión | Arte final (modelos, texturas y audio) |
| Condenado con máquina de estados, percepción por vista, oído y proximidad | Menú principal y guardado entre sesiones |
| Persecución final de las umas | |
| Diálogos, monólogos internos y cuaderno de campo con 6 entradas | |
| Puntos de control, estado de fallo y menú de pausa | |
| Registro automático y anónimo de partidas para la evaluación con jugadores | |

Las zonas del pueblo y del templo existen en el proyecto en estado de bloqueo (*greybox*).

---

## Requisitos

- **Unity 6000.6.10f1** (Unity 6), instalado desde Unity Hub. Se recomienda usar exactamente esta versión.
- **Git** y **Git LFS**. Las texturas, los modelos y el audio se almacenan con Git LFS.
- Windows 10/11 de 64 bits para compilar el ejecutable.

Paquetes de Unity utilizados: Universal Render Pipeline (URP), AI Navigation, Input System y TextMesh Pro.

---

## Cómo abrir el proyecto

1. Instala Git LFS una sola vez en tu equipo:
   ```bash
   git lfs install
   ```
2. Clona el repositorio. Los archivos LFS se descargan automáticamente:
   ```bash
   git clone https://github.com/FelixElCoder/HuantarProject.git
   ```
3. Abre la carpeta clonada desde **Unity Hub → Add → Add project from disk**.
4. Abre la escena `Persistent` y pulsa **Play**. Esta escena carga las zonas del juego de forma aditiva.

> Si al abrir el proyecto las texturas aparecen rosadas o faltan modelos, probablemente Git LFS no estaba instalado al clonar. Ejecuta `git lfs pull` dentro de la carpeta del proyecto.

---

## Controles

| Acción | Tecla |
|---|---|
| Moverse | W A S D |
| Mirar | Ratón |
| Correr | Shift izquierdo |
| Agacharse / esconderse | Ctrl o C |
| Encender / apagar el farol | F |
| Interactuar / examinar | E |
| Avanzar diálogo | E, Espacio o clic izquierdo |
| Cuaderno de campo | Tab |
| Pausa (sensibilidad, volumen, salir) | Esc |

---

## Compilar el ejecutable

1. **File → Build Profiles → Windows**.
2. En **Scene List**, verifica este orden:
   | Índice | Escena |
   |---|---|
   | 0 | `Persistent` |
   | 1 | `Zone_Pueblo` |
   | 2 | `Zone_Quebrada` |
   | 3 | `Zone_Templo` |
3. Arquitectura **Intel 64-bit** y **Development Build** desmarcado.
4. **Build** y elige una carpeta `Builds/` en la raíz del proyecto. Esa carpeta no se sube al repositorio.

Para distribuir el juego se comparte la carpeta generada completa (`Huantar.exe`, `Huantar_Data/`, etc.), no solo el `.exe`.

---

## Estructura del proyecto

```
Assets/_Project/
├── Art/          Materiales, texturas y modelos
├── Audio/        Efectos y ambiente
├── Data/         Contenido en ScriptableObjects
│   ├── Dialogos/   Diálogos y monólogos
│   └── Cuaderno/   Entradas del cuaderno de campo
├── Prefabs/
├── Scenes/       Persistent, Zone_Pueblo, Zone_Quebrada, Zone_Templo
└── Scripts/
    ├── Core/        Arranque, carga de zonas, eventos globales, puntos de control
    ├── Player/      Controlador en primera persona, farol, escondites, interacción
    ├── AI/          Máquina de estados, condenado, umas, percepción
    ├── Narrative/   Diálogos, monólogos, objetos examinables, cuaderno
    ├── UI/          Interfaz, pausa, cuaderno, fallo y pantalla final
    ├── Atmosphere/  Efectos de ambiente
    └── Telemetry/   Registro de partidas
```

### Arquitectura en breve

- **Escenas aditivas:** `Persistent` contiene al jugador, la cámara, los gestores y la interfaz. Las zonas se cargan y descargan con disparadores ubicados entre ellas.
- **Eventos globales** (`GameEvents`): captura, reaparición, punto de control y fin de la beta se comunican sin acoplar los sistemas.
- **Máquina de estados** (`CreatureFSM`) para las criaturas que usan navegación: Deambular, Investigar, Perseguir, Buscar y Capturar.
- **Contenido en ScriptableObjects:** los diálogos y las entradas del cuaderno se editan desde el editor sin tocar código.

La arquitectura completa está documentada en el *Documento de Arquitectura de Software* (Anexo E de la tesis).

---

## Registro de partidas

Para la evaluación con jugadores, el juego guarda un archivo JSON anónimo por sesión. El archivo incluye el tiempo de juego, las capturas con su ubicación, los puntos de control y las entradas del cuaderno descubiertas. En Windows se guarda en:

```
%USERPROFILE%\AppData\LocalLow\<CompanyName>\Huantar\Playtests\
```

Cada archivo se identifica con el código de sesión que aparece en la pantalla final. No se registran datos personales.

---

## Créditos y licencias de recursos externos

| Recurso | Autor | Fuente | Licencia |
|---|---|---|---|
| *(completar con cada modelo, textura o audio de terceros)* | | | |

---

## Autor

**Franz Javier Vilcapoma Madueño**: diseño, programación y documentación.
Asesor: **Johan Paul Baldeón Medrano**.
Pontificia Universidad Católica del Perú, Facultad de Ciencias e Ingeniería, 2026.

### Cómo citar

Vilcapoma Madueño, F. J. (2026). *Desarrollo de un videojuego serio para fomentar el turismo en zonas arqueológicas del Perú* [Tesis de licenciatura en preparación, Pontificia Universidad Católica del Perú].

---

## Licencia

Código fuente © 2026 Franz Javier Vilcapoma Madueño. Todos los derechos reservados, salvo que se indique otra licencia. Los recursos de terceros conservan las licencias indicadas en la sección de créditos.
