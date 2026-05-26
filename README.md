# MediaTek86

## Présentation du projet

MediaTek86 est une application console développée en C# avec .NET Framework 4.8 permettant la gestion du personnel et des absences d’une entreprise.

Le projet a été réalisé dans le cadre d’un travail scolaire afin de mettre en pratique :

- la programmation orientée objet ;
- l’architecture en couches ;
- l’utilisation d’une base de données MySQL ;
- la gestion sécurisée des accès utilisateurs ;
- les opérations CRUD (Create, Read, Update, Delete).

L’application permet à un responsable authentifié d’administrer le personnel et les absences via une interface console.

---

# Technologies utilisées

- Langage : C#
- Framework : .NET Framework 4.8
- Base de données : MySQL
- IDE : Visual Studio 2022
- Gestionnaire SQL : phpMyAdmin / WampServer
- Package NuGet : MySql.Data

---

# Fonctionnalités principales

## Authentification

- Connexion sécurisée via login et mot de passe
- Vérification des identifiants dans la table `responsable`
- Mot de passe stocké sous forme hashée en SHA-256
- Limitation du nombre de tentatives de connexion

## Gestion du personnel

- Ajouter un personnel
- Modifier un personnel
- Supprimer un personnel
- Rechercher un personnel
- Trier les personnels
- Afficher la liste des personnels

## Gestion des absences

- Ajouter une absence
- Modifier une absence
- Supprimer une absence
- Afficher les absences
- Vérification automatique des chevauchements de dates

---

# Architecture du projet

Le projet est organisé selon une architecture en couches afin de séparer les responsabilités.

## Structure des dossiers

```text
MediaTek86
│
├── bddmanager
│   └── BddManager.cs
│
├── dal
│   └── Access.cs
│
├── modele
│   ├── Personnel.cs
│   ├── Service.cs
│   ├── Motif.cs
│   ├── Absence.cs
│   └── Responsable.cs
│
├── controller
│   └── Controller.cs
│
├── vue
│   ├── VueConnexion.cs
│   ├── VuePersonnel.cs
│   ├── VueAbsence.cs
│   └── VueHelper.cs
│
├── Program.cs
└── App.config
```

## Description des couches

### BddManager

Gère la connexion MySQL à l’aide du pattern Singleton.

### DAL (Data Access Layer)

Contient toutes les requêtes SQL nécessaires à l’application.

### Modèle

Contient les classes métier représentant les tables de la base de données.

### Controller

Assure la logique métier et les validations.

### Vue

Gère l’affichage console et les interactions utilisateur.

---

# Sécurité

Les mots de passe sont sécurisés grâce à un hash SHA-256.

Toutes les requêtes SQL utilisent des paramètres nommés afin d’éviter les injections SQL.

Exemple :

```sql
SELECT * FROM responsable WHERE login = @login;
```

---

# Base de données

La base de données MySQL contient plusieurs tables :

- personnel
- absence
- motif
- service
- responsable

La table `responsable` permet l’authentification des utilisateurs.

---

# Diagramme de paquetages

Le diagramme de paquetages montre l’organisation générale du projet.

```text
vue --> controller --> dal --> bddmanager
                |
              modele
```

---

# Étapes de construction du projet

## Étape 1 : Création de la base de données

- Création des tables
- Mise en place des relations
- Insertion des données de test

## Étape 2 : Mise en place de la connexion MySQL

- Création du BddManager
- Configuration du App.config
- Installation du package MySql.Data

## Étape 3 : Développement des classes métier

- Personnel
- Service
- Motif
- Absence
- Responsable

## Étape 4 : Développement de la DAL

- Requêtes CRUD
- Requêtes de recherche
- Gestion des absences

## Étape 5 : Développement du contrôleur

- Vérifications métier
- Validation des données
- Gestion des connexions

## Étape 6 : Développement de l’interface console

- Menus
- Navigation
- Affichage des données

## Étape 7 : Sécurisation

- Hashage SHA-256
- Requêtes paramétrées
- Gestion des erreurs

---

# Exemple de commits Git

```text
Initialisation du projet
Ajout de la connexion MySQL
Création des classes métier
Ajout du CRUD personnel
Ajout du CRUD absences
Ajout de l’authentification
Ajout de la sécurité SHA-256
Correction des bugs et finalisation
```

---

# Installation du projet

## Pré-requis

- Visual Studio 2022
- .NET Framework 4.8
- WampServer ou MySQL Server
- phpMyAdmin

## Étapes d’installation

1. Cloner ou télécharger le projet
2. Ouvrir la solution dans Visual Studio 2022
3. Installer le package NuGet `MySql.Data`
4. Importer le script SQL dans phpMyAdmin
5. Configurer la chaîne de connexion dans `App.config`
6. Lancer le projet

---

# Exemple de chaîne de connexion

```xml
<connectionStrings>
  <add name="MediaTek86"
       connectionString="server=localhost;port=3306;database=mediatek86;uid=root;pwd=;"
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

---

# Conclusion

Ce projet a permis de développer une application complète de gestion du personnel et des absences en utilisant une architecture organisée, une base de données MySQL et des mécanismes de sécurité adaptés.

Il met en pratique les compétences liées au développement orienté objet, à l’accès aux données et à la sécurisation d’une application.

