using System;
using System.Collections.Generic;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    /// <summary>
    /// Vue console pour la gestion des absences (onglet 2).
    /// Gère l'affichage, la recherche, le tri et les formulaires CRUD des absences.
    /// </summary>
    public class VueAbsence
    {
        private readonly Controller _ctrl;

        /// <summary>
        /// Constructeur.
        /// </summary>
        public VueAbsence()
        {
            _ctrl = Controller.GetInstance();
        }

        // ─── Menu principal ───────────────────────────────────────────────────────

        /// <summary>
        /// Affiche et gère le menu de l'onglet Absences.
        /// </summary>
        public void AfficherMenu()
        {
            bool quitter = false;
            while (!quitter)
            {
                VueHelper.Titre("ONGLET 2 — GESTION DES ABSENCES");
                VueHelper.Ecrire("  [1] Afficher les absences d'un personnel", ConsoleColor.Gray);
                VueHelper.Ecrire("  [2] Ajouter une absence",                   ConsoleColor.Gray);
                VueHelper.Ecrire("  [3] Modifier une absence",                   ConsoleColor.Gray);
                VueHelper.Ecrire("  [4] Supprimer une absence",                  ConsoleColor.Gray);
                VueHelper.Ecrire("  [0] Retour au menu principal",               ConsoleColor.DarkGray);
                VueHelper.Separateur();

                VueHelper.EcrireInline("  Votre choix : ", ConsoleColor.White);
                string choix = Console.ReadLine()?.Trim();

                switch (choix)
                {
                    case "1": AfficherAbsences();  break;
                    case "2": AjouterAbsence();    break;
                    case "3": ModifierAbsence();   break;
                    case "4": SupprimerAbsence();  break;
                    case "0": quitter = true;      break;
                    default: VueHelper.Erreur("Choix invalide."); break;
                }
            }
        }

        // ─── Afficher absences ────────────────────────────────────────────────────

        /// <summary>
        /// Sélectionne un personnel et affiche ses absences avec options de recherche et tri.
        /// </summary>
        private void AfficherAbsences()
        {
            VueHelper.SousTitre("Absences d'un personnel");
            Personnel pers = ChoisirPersonnel();
            if (pers == null) return;

            VueHelper.EcrireInline("  Rechercher dans les motifs (Entrée pour tous) : ", ConsoleColor.DarkGray);
            string recherche = Console.ReadLine()?.Trim();

            VueHelper.Ecrire("  Trier par : [1] Date début  [2] Date fin  [3] Motif", ConsoleColor.DarkGray);
            VueHelper.EcrireInline("  Choix tri (Entrée = Date début) : ", ConsoleColor.DarkGray);
            string triChoix = Console.ReadLine()?.Trim();
            string triPar;
            switch (triChoix)
            {
                case "2":
                    triPar = "datefin";
                    break;
                case "3":
                    triPar = "motif";
                    break;
                default:
                    triPar = "datedebut";
                    break;
            }

            VueHelper.EcrireInline("  Ordre descendant ? (O/N, Entrée = Oui) : ", ConsoleColor.DarkGray);
            string descRep = Console.ReadLine()?.Trim().ToUpper();
            bool triDesc = !(descRep == "N");

            List<Absence> absences = _ctrl.GetAbsencesFiltreesTries(pers.IdPersonnel, recherche, triPar, triDesc);

            VueHelper.SousTitre($"Absences de {pers.NomComplet()}");
            if (absences.Count == 0)
            {
                VueHelper.Avertissement("Aucune absence trouvée pour ce personnel.");
                VueHelper.AttendreEntree();
                return;
            }

            AfficherTableauAbsences(absences);
            VueHelper.Info($"{absences.Count} absence(s) trouvée(s).");
            VueHelper.AttendreEntree();
        }

        // ─── Affichage tableau ────────────────────────────────────────────────────

        /// <summary>
        /// Affiche la liste des absences sous forme de tableau formaté.
        /// </summary>
        private void AfficherTableauAbsences(List<Absence> absences)
        {
            string fmt = "  {0,-12} {1,-12} {2,-6} {3,-30}";
            VueHelper.Ecrire(string.Format(fmt, "DÉB.", "FIN", "JOURS", "MOTIF"), VueHelper.CouleurEntete);
            VueHelper.Separateur();
            foreach (Absence a in absences)
            {
                Console.WriteLine(string.Format(fmt,
                    a.DateDebut.ToString("dd/MM/yyyy"),
                    a.DateFin.ToString("dd/MM/yyyy"),
                    a.DureeJours() + "j",
                    Tronquer(a.Motif?.Libelle ?? "—", 29)));
            }
            VueHelper.Separateur();
        }

        // ─── Ajouter ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire d'ajout d'une absence.
        /// </summary>
        private void AjouterAbsence()
        {
            VueHelper.SousTitre("Ajouter une absence");
            Personnel pers = ChoisirPersonnel();
            if (pers == null) return;

            DateTime dateDebut = VueHelper.SaisirDate("Date de début");
            DateTime dateFin   = VueHelper.SaisirDate("Date de fin");
            int idMotif        = ChoisirMotif();
            if (idMotif <= 0) return;

            if (!VueHelper.Confirmer("Confirmer l'ajout de cette absence ?"))
            {
                VueHelper.Avertissement("Ajout annulé.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.AjouterAbsence(pers.IdPersonnel, dateDebut, dateFin, idMotif, out string erreur);
            if (ok) VueHelper.Succes("Absence ajoutée avec succès.");
            else    VueHelper.Erreur(erreur);
            VueHelper.AttendreEntree();
        }

        // ─── Modifier ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire de modification d'une absence.
        /// </summary>
        private void ModifierAbsence()
        {
            VueHelper.SousTitre("Modifier une absence");
            Personnel pers = ChoisirPersonnel();
            if (pers == null) return;

            List<Absence> absences = _ctrl.GetAbsencesPersonnel(pers.IdPersonnel);
            if (absences.Count == 0)
            {
                VueHelper.Avertissement($"Aucune absence pour {pers.NomComplet()}.");
                VueHelper.AttendreEntree();
                return;
            }

            // Affichage numéroté pour sélection
            VueHelper.SousTitre($"Absences de {pers.NomComplet()}");
            for (int i = 0; i < absences.Count; i++)
                VueHelper.Ecrire($"  [{i + 1}] {absences[i]}", ConsoleColor.Gray);

            int idx = VueHelper.SaisirEntier("Numéro de l'absence à modifier", 1, absences.Count);
            Absence abs = absences[idx - 1];
            DateTime dateDebutOrigine = abs.DateDebut;

            VueHelper.Info("Laissez vide pour conserver la date actuelle.");
            DateTime dateDebut = VueHelper.SaisirDate("Date de début", abs.DateDebut);
            DateTime dateFin   = VueHelper.SaisirDate("Date de fin",   abs.DateFin);
            int idMotif        = ChoisirMotif(abs.IdMotif);
            if (idMotif <= 0) return;

            if (!VueHelper.Confirmer("Confirmer la modification ?"))
            {
                VueHelper.Avertissement("Modification annulée.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.ModifierAbsence(pers.IdPersonnel, dateDebutOrigine, dateDebut, dateFin, idMotif, out string erreur);
            if (ok) VueHelper.Succes("Absence modifiée avec succès.");
            else    VueHelper.Erreur(erreur);
            VueHelper.AttendreEntree();
        }

        // ─── Supprimer ────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire de suppression d'une absence.
        /// </summary>
        private void SupprimerAbsence()
        {
            VueHelper.SousTitre("Supprimer une absence");
            Personnel pers = ChoisirPersonnel();
            if (pers == null) return;

            List<Absence> absences = _ctrl.GetAbsencesPersonnel(pers.IdPersonnel);
            if (absences.Count == 0)
            {
                VueHelper.Avertissement($"Aucune absence pour {pers.NomComplet()}.");
                VueHelper.AttendreEntree();
                return;
            }

            VueHelper.SousTitre($"Absences de {pers.NomComplet()}");
            for (int i = 0; i < absences.Count; i++)
                VueHelper.Ecrire($"  [{i + 1}] {absences[i]}", ConsoleColor.Gray);

            int idx = VueHelper.SaisirEntier("Numéro de l'absence à supprimer", 1, absences.Count);
            Absence abs = absences[idx - 1];

            if (!VueHelper.Confirmer($"Supprimer l'absence du {abs.DateDebut:dd/MM/yyyy} au {abs.DateFin:dd/MM/yyyy} ?"))
            {
                VueHelper.Avertissement("Suppression annulée.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.SupprimerAbsence(pers.IdPersonnel, abs.DateDebut);
            if (ok) VueHelper.Succes("Absence supprimée.");
            else    VueHelper.Erreur("Erreur lors de la suppression.");
            VueHelper.AttendreEntree();
        }

        // ─── Utilitaires ──────────────────────────────────────────────────────────

        /// <summary>
        /// Affiche la liste du personnel et demande d'en choisir un.
        /// </summary>
        /// <returns>Objet <see cref="Personnel"/> sélectionné ou null si annulé</returns>
        private Personnel ChoisirPersonnel()
        {
            List<Personnel> liste = _ctrl.GetPersonnelFiltreeTrie();
            if (liste.Count == 0)
            {
                VueHelper.Erreur("Aucun personnel disponible.");
                VueHelper.AttendreEntree();
                return null;
            }

            VueHelper.EcrireInline("  Rechercher un personnel (Entrée pour lister tous) : ", ConsoleColor.DarkGray);
            string recherche = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(recherche))
                liste = _ctrl.GetPersonnelFiltreeTrie(recherche);

            if (liste.Count == 0)
            {
                VueHelper.Avertissement("Aucun personnel trouvé.");
                VueHelper.AttendreEntree();
                return null;
            }

            string fmt = "  {0,-4} {1,-20} {2,-15}";
            VueHelper.Ecrire(string.Format(fmt, "ID", "NOM COMPLET", "SERVICE"), VueHelper.CouleurEntete);
            VueHelper.Separateur('-');
            foreach (Personnel p in liste)
                Console.WriteLine(string.Format(fmt, p.IdPersonnel, p.NomComplet(), p.Service?.Nom ?? "—"));
            VueHelper.Separateur('-');

            int id = VueHelper.SaisirEntier("ID du personnel", 1, int.MaxValue);
            Personnel pers = liste.Find(p => p.IdPersonnel == id);
            if (pers == null)
            {
                VueHelper.Erreur("ID introuvable dans la sélection.");
                return null;
            }
            return pers;
        }

        /// <summary>
        /// Affiche les motifs disponibles et demande d'en choisir un.
        /// </summary>
        /// <param name="idParDefaut">Motif présélectionné (pour modification)</param>
        /// <returns>IdMotif choisi, ou 0 si invalide</returns>
        private int ChoisirMotif(int idParDefaut = 0)
        {
            List<Motif> motifs = _ctrl.GetAllMotifs();
            if (motifs.Count == 0)
            {
                VueHelper.Erreur("Aucun motif disponible dans la base.");
                return 0;
            }
            VueHelper.SousTitre("Motifs disponibles");
            foreach (Motif m in motifs)
                VueHelper.Ecrire($"    [{m.IdMotif}] {m.Libelle}", ConsoleColor.Gray);

            VueHelper.EcrireInline(
                idParDefaut > 0
                    ? $"  ID du motif [{idParDefaut}] : "
                    : "  ID du motif : ",
                ConsoleColor.DarkGray);
            string saisie = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(saisie) && idParDefaut > 0) return idParDefaut;
            if (int.TryParse(saisie, out int id) && motifs.Exists(m => m.IdMotif == id))
                return id;

            VueHelper.Erreur("Motif invalide.");
            return 0;
        }

        /// <summary>Tronque un texte à la longueur maximale.</summary>
        private static string Tronquer(string texte, int max)
        {
            if (string.IsNullOrEmpty(texte)) return "";
            return texte.Length <= max ? texte : texte.Substring(0, max - 1) + "…";
        }
    }
}
