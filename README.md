# GOAP an AI Experiment

Sur la scène il y a deux IA : Jean et Robert

Par défaut, les deux ont pour goal de tuer l'ennemi tout en se sentant relaxé (il faut fumer une cigarette pour se sentir relaxé...)

Robert commence avec 30 HP, donc automatiquement lui est attribué le goal déffensif (ne pas être blessé, se sentir relaxé et ne plus fuire)

Donc au début les deux IA vont surement ne pas bouger :
Robert va se soigner et fumer une cigarette tandis que Jean va fumer une cigarette
Puis Jean va aller chercher l'arme avant d'essayer de tuer Robert avec

Robert, après s'être soigné / avoir fumé, va voir que Jean a pris l'arme
Donc il va commencer à fuire (se déplacer aléatoirement)

De temps en temps, aléatoirement, le worldstate va les faire se sentir anxieux et donc quand ils vont replanifier automatiquement il est possible qu'ils aient de nouveau besoin de fumer un peu...

![GOAP Exemple](Assets/5_RESSOURCES/GOAP.gif)

# Fait :

Implémentation de GOAP avec actions : MoveToWeapon, MoveToEnnemy, PickUpWeapon, KillEnnemy, HealSelf

Système de préconditions et effets pour chaque action

Recherche Forward (depuis état initial vers le but)

Coût d’exécution pour chaque action

Planificateur avec mesure de performance (Stopwatch)

Action de fuite dynamique (FleeFromEnemy)

Changement de but en fonction du contexte (offensif/défensif)

Replanification automatique

Multi-agents concurrents

Ajout d’action annexe (SmokeCigarette) avec probabilité

Passage à un BitArray (Dictionnaire -> WorldStateBit)

![GOAP Exemple](Assets/5_RESSOURCES/GOAP.mp4)
