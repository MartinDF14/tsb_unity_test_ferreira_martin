Pixowl Test

Nombre: Ferreira Martin
Juego Base: Asteroids
Sistema principal: Unity DOTS

Aclaraciones sobre el juego:
-Pueden editar casi cualquier cosa desde el inspector:
    .Velocidad max: WorldData (gameobject) - Max Speed
    .Nivel: WorldData (gameobject) - Level
    .Nro de balas, cadencia de disparo, velocidad de bala y si daña o no al jugador: Player (gameobject) - WeaponComponent (component)
    .Puede editar el arma tanto del jugador como del UFO (para editar el arma del UFO editan el PREFAB al igual que el player)
    .Tiempos de powerups: Cada powerup respectivamente (prefab)) - componente respectivo
    .Tiempos de autodestruccion de objetos: Prefab respectivo - SelfDestructComponent
    .Velocidades, rotaciones, tiempos, la mayoria de las cosas es editable desde su componente respectivo

Power Ups:
Hay 3 tipos de power ups:
    .MadShot: Disparo estilo laser de particulas, gran alcance, buena duracion de balas, pero dura unos segundos. Aprovechen para disparar mucho!
    .Invulnerability: Basicamente eso.. invulnerabilidad por unos segundos. Pueden chocar contra cosas y destruirlas al impactarlas.
    .Weapon Imporvement: Mejora el arma de forma permanente hasta que pierdas la partida actual.
        .Incrementa en 1 las cantidad de balas al disparar (comienzan a disparar en forma de arco)
        .Incrementa la cadencia de disparo (tiene un cap)

Segun el documento que me pasaron: http://www.retrogamedeconstructionzone.com/2019/10/asteroids-by-numbers.html
-Similar en tamaños de pantalla y escala de objetos
-Similar en velocidades de todo (objetos, jugador, meteoritos, balas, etc) y aceleraciones
-Implementado el sistema de balas:
    .Aumenta o disminuye su velocidad al disparar segun la velocidad actual
    .Tiene en cuenta el forward y tu relativa
    .Permite disparar mas lejos hacia adelante si vas en esa direccion y dejar como "minas" si miras hacia el lado contrario

Notas:
Unity DOTS esta todavia muy "verde". 
Hay varios patrones que no tuve el tiempo para investigar como hacerlas con Unity DOTS, pero que habría implementado si usara Unity de forma normal, como por ejemplo:
    -Factory para la creacion de balas y enemigos
    -Pool de objetos para balas (super importante)
    -Observer para hookear datos en tiempo real para la UI (no puedo tomar datos de los jobs)

Por otro lado nunca había desarrollado nada en DOTS por completo.
Me gusta muchísimo el poder manejar tantos objetos en trabajos en paralelo y poder tener miles de balas sin que caigan los fps 
    -Pueden probarlo subiendo la cantidad de balas del jugador (20 por ejemplo) y bajandole la cadencia de disparo (a 0 po ejemplo)
    -Pruebenlo, se ve genial ! 


Bueno, creo que nada mas. Dejo el adjunto y el git.
Cualquier consulta no duden en hablarme: ferreira.martind@gmail.com

Gracias por la oportunidad.

Martín.