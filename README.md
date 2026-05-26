# 🧑‍💼 MediaTek86

## 📌 Contexte du projet
MediaTek86 est une application de gestion du personnel et des absences développée en C# avec Windows Forms et MySQL dans le cadre de la formation BTS SIO SLAM.

L’objectif du projet est de permettre la gestion complète du personnel d’une organisation ainsi que le suivi des absences tout en garantissant la sécurité des données.

---

## 🎯 Objectifs de l’application
- Gérer le personnel (ajout, modification, suppression)
- Gérer les absences
- Empêcher le chevauchement des absences
- Authentification sécurisée des utilisateurs
- Interaction avec une base de données MySQL

---

## 🧱 Architecture du projet
- vue → interfaces utilisateur (WinForms)
- controller → logique métier et validations
- dal → accès aux données (requêtes SQL)
- bddmanager → connexion MySQL (singleton)
- modele → classes métier

---

## 🗄️ Base de données
Tables :
- personnel
- absence
- motif
- service
- responsable

Script SQL : MediaTek86.sql

---

## 🔐 Sécurité
- Login / mot de passe
- Hash SHA-256
- Requêtes paramétrées (anti injection SQL)

---

## ⚙️ Installation
1. Importer MediaTek86.sql dans MySQL
2. Ouvrir le projet Visual Studio 2022
3. Vérifier App.config
4. Lancer l’application

---

## 👤 Compte test
- login : admin
- mot de passe : 1234

---

## 🔗 Liens
- GitHub : https://github.com/Fyu-D-Cyu/MediaTek86
