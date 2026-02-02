# MyTracker

## Objectif V1
Application web pour **suivre et planifier des séances de sport** :
- Suivi séance par séance : exercices → séries → reps + poids, avec **récupération du dernier poids** utilisé
- **Base d’exercices** : nom + groupe musculaire
- **Planification** : séances planifiées, avec ou sans **récurrence**
- Bonus V1 (gardé) : **templates/routines** + **historique rapide par exercice**

---

## Stack technique (proposée)
- .NET 8+
- ASP.NET Core + Blazor (recommandé : **Blazor Server** pour aller vite)
- EF Core + SQLite (dev/local)
- Auth : **pas en V1** (option V2)

---

## Fonctionnalités V1 (scope)
### 1) Gestion des exercices (catalogue)
- CRUD exercice : `Nom`, `GroupeMusculaire`
- Recherche / filtre par groupe
- Validation : nom unique (optionnel mais pratique)

### 2) Gestion des séances (réel)
- Créer une séance (date/heure, titre optionnel)
- Ajouter des exercices à la séance
- Pour chaque exercice : gérer des séries
  - `Reps`, `Poids` (et éventuellement `Ordre`)
- UX clé : lors de l’ajout d’un exercice à une séance,
  - proposer **les dernières séries** connues (reps/poids) de cet exercice
  - bouton “Copier dernière séance” / “Pré-remplir”

### 3) Templates / routines
- CRUD template : nom + liste d’exercices + séries “par défaut” (reps/poids vides ou valeurs)
- “Créer séance depuis template” en 1 clic

### 4) Planification (avec récurrence)
- Créer une séance **planifiée** (date/heure + template optionnel)
- Récurrence : quotidienne/hebdo + jours de semaine (ex: Lun/Jeu)
- Stratégie :
  - stocker une **règle de récurrence**
  - générer des occurrences “à l’avance” (ex: 30 jours)
- Action : “Démarrer séance” → transforme une séance planifiée en séance réelle (et pré-remplit via template si dispo)

### 5) Historique rapide par exercice
- Page exercice : afficher les **N dernières performances**
  - ex: dernière séance, date, séries/reps/poids
- Utilisé aussi pour la pré-remplissage

---

## Écrans (pages)
- **Dashboard / Accueil**
  - Prochaines séances planifiées (7–14 jours)
  - Dernières séances réalisées
- **Planning**
  - liste (V1) ou mini-calendrier (V1+)
  - création/édition des planifiées + récurrences
- **Séances**
  - liste des séances réalisées (filtre date)
  - détail séance : exercices + séries, ajout rapide, pré-remplissage
- **Exercices**
  - catalogue + création/édition
  - détail exercice : historique
- **Templates**
  - liste + CRUD
  - bouton “Créer une séance à partir du template”

---

## Modèle de données (proposition)
### Entités
- `Exercise`
  - `Id`, `Name`, `MuscleGroup`
- `WorkoutSession` (séance réalisée)
  - `Id`, `StartDateTime`, `Title?`
- `SessionExercise`
  - `Id`, `WorkoutSessionId`, `ExerciseId`, `Order`
- `SetEntry`
  - `Id`, `SessionExerciseId`, `Order`, `Reps`, `Weight`
- `Template`
  - `Id`, `Name`
- `TemplateExercise`
  - `Id`, `TemplateId`, `ExerciseId`, `Order`
- `TemplateSet`
  - `Id`, `TemplateExerciseId`, `Order`, `Reps?`, `Weight?`
- `PlannedSession`
  - `Id`, `PlannedDateTime`, `Title?`, `TemplateId?`, `RecurrenceRuleId?`
  - `Status` (Planned/Done/Skipped) ou `DoneSessionId?`
- `RecurrenceRule`
  - `Id`, `Frequency` (Daily/Weekly), `Interval` (ex: 1), `DaysOfWeekMask` (pour Weekly),
  - `StartDate`, `EndDate?`

### Règles importantes
- “Dernier poids” = chercher la dernière `SetEntry` de l’exercice (via `SessionExercise → ExerciseId`) triée par date desc
- Génération d’occurrences planifiées :
  - job au lancement (ou action manuelle) : “Generate upcoming planned sessions until +30 days”
