# Índice
- [Resumen](#resumen-del-proyecto)
- [Terrain](#terrain)
- [Game Objects](#game-objects)
- [Scripts](#scripts)
- [Otros](#otros)
- [Capturas](#capturas)
# Resumen del proyecto
El proyecto busca representar la Gran Tumba de Nazarick (imágenes de ejemplo en [capturas](#capturas)), la cual es uno de los lugares más importantes y reconocibles de la serie de novelas ligeras japonesa Overlord. La novela tiene como protagonista a un no-muerto y uno de los temas más recurrentes es la muerte, de ahí que buscase un ambiente más lúgubre y/o macabro en el proyecto.
# Terrain
En los inicios de la práctica se usaron las herramientas del Terrain de Unity para estudiar su funcionamiento: se crearon montañas, árboles y depresiones, y se probaron distintos materiales y detalles.
- Herramientas: Set Height, Smooth Height, Paint Texture, Paint Trees, Paint Details, Paint Holes.
- Ajustes: se modificaron la altura general (Terrain Height) y el tamaño del terreno. Se partió del tamaño por defecto (1000x1000), se redujo mientras se aprendía a usar las herramientas y se volvió al tamaño inicial al decidir qué construir. 
- Resultado final: el terreno se aplanó para construir la tumba, con una depresión circular con el borde muy elevado.
- Scripts: `TerrainRim` actúa directamente sobre el terreno mediante `GetHeights` y `SetHeights` para crear la depresión circular con un desnivel casi vertical en el borde. Se optó por un script porque resultó muy difícil conseguir ese desnivel con el pincel del Terrain.
## Game Objects
Para realizar el proyecto se han usado una gran cantidad de Game Objects.

| Elemento                                     | Cómo está hecho                                                                      | Origen                                             | Cantidad |
| -------------------------------------------- | ------------------------------------------------------------------------------------ | -------------------------------------------------- | -------- |
| Pilares                                      | Prefab de stone pillars, escalado con scripts                                        | Asset Store (Occult Stone Pillars)                 | ~150     |
| Vigas/Muros                                  | Cubos escalados entre los pilares                                                    | Primitiva de Unity                                 | ~110     |
| Mausoleo central                             | Plataforma, columnas, tejado                                                         | ProBuilder + prefab de pilares                     | 1        |
| Escaleras de la entrada del mausoleo central | Prefab. Se usaron varios objetos para hacer los escalones más bajos.                 | Asset Store (Grid Master)                          | 4        |
| Mausoleos exteriores                         | Prefab                                                                               | Asset Store (Mausoleum)                            | 4        |
| Campamento                                   | Varios prefabs (tiendas de campaña, piedras, hoguera, etc.)                          | Asset Store (Stylized Cozy Camp)                   | 1        |
| Árboles secos                                | Prefab obtenido de la Asset Store, "pintados" con la utilidad Paint Trees de Terrain | Asset Store (Dry Trees)                            | ~20      |
| Lápidas                                      | Prefab descargado de la Asset Store                                                  | Asset Store (Concrete Tombstone Cemetery PBR Prop) | ~10      |
| Fuego en la hoguera del campamento           | Con el sistema de partículas (Particle System) de Unity y una imagen PNG             | Unity                                              | 1        |

Los personajes tienen como base el prefab del asset "First Person + Third Person | Character Controllers", sustituyendo la malla por defecto por modelos descargados de Mixamo. Además, algunos personajes tienen animaciones adicionales descargadas desde la misma web.

La siguiente tabla recoge los personajes usados, su rol y las animaciones asignadas. Se destaca que el nombre del personaje coincide con su nombre en Mixamo.

| Personaje                 | Rol          | Animaciones             | Control                 |
| ------------------------- | ------------ | ----------------------- | ----------------------- |
| Vampire A Lusth           | Jugador      | Idle, walk, run y jump. | Third Person Controller |
| Remy                      | NPC sentado  | Sitting Dazed           | Animator propio         |
| Akai e espiritu           | NPC sentado  | Sitting idle            | Animator propio         |
| Romero (x1)               | NPC sentado  | Sitting Talking         | Animator propio         |
| Romero (x4)               | NPC patrulla | Idle, walk              | NavMeshAgent + `Patrol` |
| Skeletonzombie T Avelange | NPC patrulla | Idle, walk              | NavMeshAgent + `Patrol` |
| Warrok W Kurniawan        | NPC patrulla | Idle, walk              | NavMeshAgent + `Patrol` |

Los NPC de patrulla se mueven sobre la malla de navegación generada con NavMesh Surface (ver [Herramientas adicionales](#herramientas-adicionales)).

# Scripts
- ArcBuilder: coloca pilares y vigas formando arcos (radio y ángulos de inicio y fin ajustables), adaptándose a la altura del Terrain.
- LaneBuilder: igual que ArcBuilder, pero genera líneas rectas (longitud y números de pilares ajustables), orientadas según la rotación del GameObject.
- Patrol: hace que un personaje patrulle entre varios puntos usando un `NavMeshAgent`. Los puntos son GameObjects vacío que se asignan al array `Points` desde el Inspector, y el personaje los recorre en orden y de forma cíclica. El parámetro `Wait Time` define los segundos que espera en cada punto antes de ir al siguiente. Además, también actualiza el parámetro `Speed` del Animator con la velocidad del agente.
- TerrainRim: modifica la altura del terreno para crear la depresión circular con el desnivel casi vertical del borde. Este script fue utilizado porque, con la herramienta de Terrain `SetHeight`, el terreno no quedaba tal y como buscaba.

# Otros
## Materiales y texturas
Se han usado varios materiales y texturas adicionales, siendo la mayoría descargados de la Asset Store. Se destacan:
- **Handpainted Grass & Ground Textures Free**: usado para la arena de la entrada a la tumba.
- **Hand Painted Stone Texture**: para la textura de las escaleras.
- **Marble Design Materials**: usado en la textura del tejado del mausoleo central.

## Herramientas adicionales
- **ProBuilder**: plugin de modelado 3D para Unity. Ha sido usado para crear el "prisma" que sirve de techo para el mausoleo central.
- **AI Navigation**: paquete de Unity usado para la navegación de los NPC. El componente `NavMesh Surface` escanea el terreno y genera la malla de navegación, y los NPC de patrulla se mueven sobre ella con un `NavMeshAgent` controlado por el script `Patrol`.
- **Cinemachine**: cámara que sigue al jugador, usado por el Third Person Controller.

## Iluminación y ambiente
Se modificaron tanto la luz, el cielo y la niebla de la escena para lograr un ambiente más frío y sombrío.
- Luz: se bajó la intensidad y se cambió a un color más desaturado.
- Skybox: se redujo la luz ambiental y se cambió a un skybox nublado.
- Niebla: se activó, se aumentó la densidad para reducir la visibilidad y se cambió el color a un gris verdoso.
- Interior del mausoleo: un cubo con material *Unlit* negro simula una oscuridad total sin depender de la iluminación.

# Capturas

![Referencias: vista aérea de la tumba](screenshots/referencia-aerea.jpg)

*Fig. 1: Referencia de la Gran Tumba de Nazarick. Fuente: anime Overlord.*

![Referencias: fachada del mausoleo](screenshots/referencia-frente.png)

*Fig. 2: Referencia del mausoleo central. Fuente: anime Overlord.*

![Vista aérea de la tumba](screenshots/aerea.png)

*Fig. 3: Vista aérea de la Gran Tumba sin la niebla.*

![Mausoleo central](screenshots/mausoleo.png)

*Fig. 4: Mausoleo central visto desde la entrada.*

![Campamento](screenshots/campamento.png)

*Fig. 5: Campamento junto a la entrada.*

# Referencias
- [Mixamo](https://www.mixamo.com/#/)
- [Unity Asset Store](https://assetstore.unity.com/)
