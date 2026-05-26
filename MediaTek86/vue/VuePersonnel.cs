using System;
using System.Collections.Generic;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    /// <summary>
    /// Vue console pour la gestion du personnel (onglet 1).
    /// Gère l'affichage, la recherche, le tri et les formulaires CRUD du personnel.
    /// </summary>
    public class VuePersonnel
    {
        private readonly Controller _ctrl;

        /// <summary>
        /// Constructeur.
        /// </summary>
        public VuePersonnel()
        {
            _ctrl = Controller.GetInstance();
        }

        // ─── Menu principal ───────────────────────────────────────────────────────

        /// <summary>
        /// Affiche et gère le menu de l'onglet Personnel.
        /// </summary>
        public void AfficherMenu()
        {
            bool quitter = false;
            while (!quitter)
            {
                VueHelper.Titre("ONGLET 1 — GESTION DU PERSONNEL");
                VueHelper.Ecrire("  [1] Lister / Rechercher / Trier le personnel", ConsoleColor.Gray);
                VueHelper.Ecrire("  [2] Ajouter un personnel",                      ConsoleColor.Gray);
                VueHelper.Ecrire("  [3] Modifier un personnel",                      ConsoleColor.Gray);
                VueHelper.Ecrire("  [4] Supprimer un personnel",                     ConsoleColor.Gray);
                VueHelper.Ecrire("  [0] Retour au menu principal",                   ConsoleColor.DarkGray);
                VueHelper.Separateur();

                VueHelper.EcrireInline("  Votre choix : ", ConsoleColor.White);
                string choix = Console.ReadLine()?.Trim();

                switch (choix)
                {
                    case "1": ListerPersonnel();   break;
                    case "2": AjouterPersonnel();  break;
                    case "3": ModifierPersonnel(); break;
                    case "4": SupprimerPersonnel();break;
                    case "0": quitter = true;      break;
                    default: VueHelper.Erreur("Choix invalide."); break;
                }
            }
        }

        // ─── Liste / Recherche / Tri ──────────────────────────────────────────────

        /// <summary>
        /// Affiche la liste du personnel avec options de recherche et de tri.
        /// </summary>
        public void ListerPersonnel()
        {
            VueHelper.SousTitre("Liste du personnel");

            VueHelper.EcrireInline("  Rechercher (Entrée pour tous) : ", ConsoleColor.DarkGray);
            string recherche = Console.ReadLine()?.Trim();

            VueHelper.Ecrire("  Trier par : [1] Nom  [2] Prénom  [3] Service", ConsoleColor.DarkGray);
            VueHelper.EcrireInline("  Choix tri (Entrée = Nom) : ", ConsoleColor.DarkGray);
            string triChoix = Console.ReadLine()?.Trim();
            string triPar;
            switch (triChoix)
            {
                case "2":
                    triPar = "prenom";
                    break;
                case "3":
                    triPar = "service";
                    break;
                default:
                    triPar = "nom";
                    break;
            }

            VueHelper.EcrireInline("  Ordre descendant ? (O/N, Entrée = Non) : ", ConsoleColor.DarkGray);
            string descRep = Console.ReadLine()?.Trim().ToUpper();
            bool triDesc = descRep == "O" || descRep == "Y";

            List<Personnel> liste = _ctrl.GetPersonnelFiltreeTrie(recherche, triPar, triDesc);

            Console.WriteLine();
            if (liste.Count == 0)
            {
                VueHelper.Avertissement("Aucun personnel trouvé.");
                VueHelper.AttendreEntree();
                return;
            }

            AfficherTableauPersonnel(liste);
            VueHelper.Info($"{liste.Count} personnel(s) trouvé(s).");
            VueHelper.AttendreEntree();
        }

        // ─── Affichage tableau ────────────────────────────────────────────────────

        /// <summary>
        /// Affiche la liste du personnel sous forme de tableau formaté.
        /// </summary>
        private void AfficherTableauPersonnel(List<Personnel> liste)
        {
            string fmt = "  {0,-4} {1,-18} {2,-15} {3,-16} {4,-28} {5,-15}";
            VueHelper.Ecrire(string.Format(fmt, "ID", "NOM", "PRÉNOM", "TÉLÉPHONE", "EMAIL", "SERVICE"), VueHelper.CouleurEntete);
            VueHelper.Separateur();
            foreach (Personnel p in liste)
            {
                string service = p.Service?.Nom ?? "—";
                Console.WriteLine(string.Format(fmt,
                    p.IdPersonnel,
                    Tronquer(p.Nom, 17),
                    Tronquer(p.Prenom, 14),
                    Tronquer(p.Tel, 15),
                    Tronquer(p.Mail, 27),
                    Tronquer(service, 14)));
            }
            VueHelper.Separateur();
        }

        // ─── Ajouter ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire d'ajout d'un nouveau membre du personnel.
        /// </summary>
        private void AjouterPersonnel()
        {
            VueHelper.SousTitre("Ajouter un personnel");
            string nom    = VueHelper.SaisirObligatoire("Nom");
            string prenom = VueHelper.SaisirObligatoire("Prénom");
            string tel    = VueHelper.SaisirObligatoire("Téléphone");
            string mail   = VueHelper.SaisirObligatoire("Email");
            int idService = ChoisirService();
            if (idService <= 0) return;

            if (!VueHelper.Confirmer("Confirmer l'ajout ?"))
            {
                VueHelper.Avertissement("Ajout annulé.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.AjouterPersonnel(nom, prenom, tel, mail, idService, out string erreur);
            if (ok) VueHelper.Succes("Personnel ajouté avec succès.");
            else    VueHelper.Erreur(erreur);
            VueHelper.AttendreEntree();
        }

        // ─── Modifier ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire de modification d'un membre du personnel.
        /// </summary>
        private void ModifierPersonnel()
        {
            VueHelper.SousTitre("Modifier un personnel");
            List<Personnel> liste = _ctrl.GetAllPersonnel();
            if (liste.Count == 0)
            {
                VueHelper.Avertissement("Aucun personnel à modifier.");
                VueHelper.AttendreEntree();
                return;
            }

            AfficherTableauPersonnel(liste);
            int id = VueHelper.SaisirEntier("ID du personnel à modifier", 1, int.MaxValue);
            Personnel pers = liste.Find(p => p.IdPersonnel == id);
            if (pers == null)
            {
                VueHelper.Erreur("ID introuvable.");
                VueHelper.AttendreEntree();
                return;
            }

            VueHelper.Info($"Modification de : {pers.NomComplet()} — laissez vide pour conserver la valeur");
            string nom    = VueHelper.SaisirAvecDefaut("Nom",       pers.Nom);
            string prenom = VueHelper.SaisirAvecDefaut("Prénom",    pers.Prenom);
            string tel    = VueHelper.SaisirAvecDefaut("Téléphone", pers.Tel);
            string mail   = VueHelper.SaisirAvecDefaut("Email",     pers.Mail);
            int idService = ChoisirService(pers.IdService);
            if (idService <= 0) return;

            if (!VueHelper.Confirmer("Confirmer la modification ?"))
            {
                VueHelper.Avertissement("Modification annulée.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.ModifierPersonnel(id, nom, prenom, tel, mail, idService, out string erreur);
            if (ok) VueHelper.Succes("Personnel modifié avec succès.");
            else    VueHelper.Erreur(erreur);
            VueHelper.AttendreEntree();
        }

        // ─── Supprimer ────────────────────────────────────────────────────────────

        /// <summary>
        /// Formulaire de suppression d'un membre du personnel.
        /// </summary>
        private void SupprimerPersonnel()
        {
            VueHelper.SousTitre("Supprimer un personnel");
            List<Personnel> liste = _ctrl.GetAllPersonnel();
            if (liste.Count == 0)
            {
                VueHelper.Avertissement("Aucun personnel à supprimer.");
                VueHelper.AttendreEntree();
                return;
            }

            AfficherTableauPersonnel(liste);
            int id = VueHelper.SaisirEntier("ID du personnel à supprimer", 1, int.MaxValue);
            Personnel pers = liste.Find(p => p.IdPersonnel == id);
            if (pers == null)
            {
                VueHelper.Erreur("ID introuvable.");
                VueHelper.AttendreEntree();
                return;
            }

            VueHelper.Avertissement($"ATTENTION : cela supprimera aussi toutes les absences de {pers.NomComplet()}.");
            if (!VueHelper.Confirmer($"Supprimer définitivement {pers.NomComplet()} ?"))
            {
                VueHelper.Avertissement("Suppression annulée.");
                VueHelper.AttendreEntree();
                return;
            }

            bool ok = _ctrl.SupprimerPersonnel(id);
            if (ok) VueHelper.Succes("Personnel supprimé.");
            else    VueHelper.Erreur("Erreur lors de la suppression.");
            VueHelper.AttendreEntree();
        }

        // ─── Utilitaires ──────────────────────────────────────────────────────────

        /// <summary>
        /// Affiche les services et demande à l'utilisateur d'en choisir un.
        /// </summary>
        /// <param name="idParDefaut">Service présélectionné (pour modification)</param>
        /// <returns>IdService choisi, ou 0 si annulé</returns>
        private int ChoisirService(int idParDefaut = 0)
        {
            List<Service> services = _ctrl.GetAllServices();
            if (services.Count == 0)
            {
                VueHelper.Erreur("Aucun service disponible dans la base.");
                return 0;
            }
            VueHelper.SousTitre("Services disponibles");
            foreach (Service s in services)
                VueHelper.Ecrire($"    [{s.IdService}] {s.Nom}", ConsoleColor.Gray);

            string defStr = idParDefaut > 0 ? idParDefaut.ToString() : "";
            VueHelper.EcrireInline(
                idParDefaut > 0
                    ? $"  ID du service [{idParDefaut}] : "
                    : "  ID du service : ",
                ConsoleColor.DarkGray);
            string saisie = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(saisie) && idParDefaut > 0) return idParDefaut;
            if (int.TryParse(saisie, out int id) && services.Exists(s => s.IdService == id))
                return id;

            VueHelper.Erreur("Service invalide.");
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
