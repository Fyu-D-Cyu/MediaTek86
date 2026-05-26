using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MediaTek86.dal;
using MediaTek86.modele;

namespace MediaTek86.controller
{
    /// <summary>
    /// Contrôleur principal de l'application MediaTek86.
    /// Coordonne les échanges entre la vue (console) et la couche d'accès aux données (DAL).
    /// Gère les validations métier avant persistance.
    /// </summary>
    public class Controller
    {
        /// <summary>Instance unique du contrôleur (Singleton)</summary>
        private static Controller _instance = null;

        /// <summary>Accès aux données</summary>
        private readonly Access _access;

        /// <summary>Responsable connecté (null si déconnecté)</summary>
        private Responsable _responsableConnecte = null;

        // ─── Singleton ────────────────────────────────────────────────────────────

        /// <summary>
        /// Constructeur privé.
        /// </summary>
        private Controller()
        {
            _access = Access.GetInstance();
        }

        /// <summary>
        /// Retourne l'instance unique du contrôleur.
        /// </summary>
        /// <returns>Instance unique de <see cref="Controller"/></returns>
        public static Controller GetInstance()
        {
            if (_instance == null)
                _instance = new Controller();
            return _instance;
        }

        // ─── Propriétés ───────────────────────────────────────────────────────────

        /// <summary>Indique si un responsable est connecté</summary>
        public bool EstConnecte => _responsableConnecte != null;

        /// <summary>Login du responsable connecté</summary>
        public string LoginConnecte => _responsableConnecte?.Login ?? "";

        // ═══════════════════════════════════════════════════════════════════════════
        // UC1 – CONNEXION
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tente de connecter un responsable après hashage SHA-256 du mot de passe.
        /// </summary>
        /// <param name="login">Login saisi</param>
        /// <param name="motDePasse">Mot de passe en clair</param>
        /// <returns>True si connexion réussie, False sinon</returns>
        public bool Connecter(string login, string motDePasse)
        {
            string hash = HashSHA256(motDePasse);
            _responsableConnecte = _access.ChercheResponsable(login, hash);
            return EstConnecte;
        }

        /// <summary>
        /// Déconnecte le responsable actuel.
        /// </summary>
        public void Deconnecter()
        {
            _responsableConnecte = null;
        }

        /// <summary>
        /// Calcule le hash SHA-256 d'une chaîne.
        /// </summary>
        /// <param name="texte">Texte à hasher</param>
        /// <returns>Hash en hexadécimal minuscule</returns>
        private string HashSHA256(string texte)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texte));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // RÉFÉRENTIELS
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Retourne la liste de tous les services.
        /// </summary>
        /// <returns>Liste de <see cref="Service"/></returns>
        public List<Service> GetAllServices() => _access.GetAllServices();

        /// <summary>
        /// Retourne la liste de tous les motifs d'absence.
        /// </summary>
        /// <returns>Liste de <see cref="Motif"/></returns>
        public List<Motif> GetAllMotifs() => _access.GetAllMotifs();

        // ═══════════════════════════════════════════════════════════════════════════
        // UC2/3/4 – GESTION DU PERSONNEL
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère tous les membres du personnel.
        /// </summary>
        /// <returns>Liste de <see cref="Personnel"/></returns>
        public List<Personnel> GetAllPersonnel() => _access.GetAllPersonnel();

        /// <summary>
        /// Récupère le personnel filtré et/ou trié.
        /// </summary>
        /// <param name="recherche">Texte recherché dans nom/prénom/service (null = tous)</param>
        /// <param name="triPar">Champ de tri : "nom", "prenom", "service" (null = nom)</param>
        /// <param name="triDesc">True pour tri descendant</param>
        /// <returns>Liste filtrée et triée de <see cref="Personnel"/></returns>
        public List<Personnel> GetPersonnelFiltreeTrie(string recherche = null, string triPar = "nom", bool triDesc = false)
        {
            List<Personnel> liste = _access.GetAllPersonnel();

            // Filtre
            if (!string.IsNullOrWhiteSpace(recherche))
            {
                string r = recherche.ToLower();
                liste = liste.FindAll(p =>
                    p.Nom.ToLower().Contains(r) ||
                    p.Prenom.ToLower().Contains(r) ||
                    p.Mail.ToLower().Contains(r) ||
                    p.Tel.Contains(r) ||
                    (p.Service?.Nom.ToLower().Contains(r) ?? false)
                );
            }

            // Tri
            switch (triPar?.ToLower())
            {
                case "prenom":
                    liste.Sort((a, b) => triDesc
                        ? string.Compare(b.Prenom, a.Prenom, StringComparison.OrdinalIgnoreCase)
                        : string.Compare(a.Prenom, b.Prenom, StringComparison.OrdinalIgnoreCase));
                    break;
                case "service":
                    liste.Sort((a, b) => triDesc
                        ? string.Compare(b.Service?.Nom, a.Service?.Nom, StringComparison.OrdinalIgnoreCase)
                        : string.Compare(a.Service?.Nom, b.Service?.Nom, StringComparison.OrdinalIgnoreCase));
                    break;
                default: // nom
                    liste.Sort((a, b) => triDesc
                        ? string.Compare(b.Nom, a.Nom, StringComparison.OrdinalIgnoreCase)
                        : string.Compare(a.Nom, b.Nom, StringComparison.OrdinalIgnoreCase));
                    break;
            }

            return liste;
        }

        /// <summary>
        /// Ajoute un nouveau membre du personnel après validation des champs.
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="tel">Téléphone</param>
        /// <param name="mail">Email</param>
        /// <param name="idService">Identifiant du service</param>
        /// <param name="erreur">Message d'erreur si validation échoue</param>
        /// <returns>True si ajout réussi, False sinon</returns>
        public bool AjouterPersonnel(string nom, string prenom, string tel, string mail, int idService, out string erreur)
        {
            erreur = ValiderChampsPersonnel(nom, prenom, tel, mail, idService);
            if (erreur != null) return false;

            Personnel p = new Personnel
            {
                Nom = nom.Trim(),
                Prenom = prenom.Trim(),
                Tel = tel.Trim(),
                Mail = mail.Trim(),
                IdService = idService
            };
            bool ok = _access.AjouterPersonnel(p);
            if (!ok) erreur = "Erreur lors de l'insertion en base de données.";
            return ok;
        }

        /// <summary>
        /// Modifie un membre du personnel après validation.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="nom">Nouveau nom</param>
        /// <param name="prenom">Nouveau prénom</param>
        /// <param name="tel">Nouveau téléphone</param>
        /// <param name="mail">Nouveau mail</param>
        /// <param name="idService">Nouvel identifiant de service</param>
        /// <param name="erreur">Message d'erreur si validation échoue</param>
        /// <returns>True si modification réussie, False sinon</returns>
        public bool ModifierPersonnel(int idPersonnel, string nom, string prenom, string tel, string mail, int idService, out string erreur)
        {
            erreur = ValiderChampsPersonnel(nom, prenom, tel, mail, idService);
            if (erreur != null) return false;

            Personnel p = new Personnel
            {
                IdPersonnel = idPersonnel,
                Nom = nom.Trim(),
                Prenom = prenom.Trim(),
                Tel = tel.Trim(),
                Mail = mail.Trim(),
                IdService = idService
            };
            bool ok = _access.ModifierPersonnel(p);
            if (!ok) erreur = "Erreur lors de la mise à jour en base de données.";
            return ok;
        }

        /// <summary>
        /// Supprime un membre du personnel et toutes ses absences.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel à supprimer</param>
        /// <returns>True si suppression réussie</returns>
        public bool SupprimerPersonnel(int idPersonnel)
        {
            return _access.SupprimerPersonnel(idPersonnel);
        }

        // ─── Validation Personnel ─────────────────────────────────────────────────

        /// <summary>
        /// Valide les champs d'un formulaire personnel.
        /// </summary>
        /// <returns>Message d'erreur ou null si valide</returns>
        private string ValiderChampsPersonnel(string nom, string prenom, string tel, string mail, int idService)
        {
            if (string.IsNullOrWhiteSpace(nom))    return "Le nom est obligatoire.";
            if (string.IsNullOrWhiteSpace(prenom)) return "Le prénom est obligatoire.";
            if (string.IsNullOrWhiteSpace(tel))    return "Le téléphone est obligatoire.";
            if (string.IsNullOrWhiteSpace(mail))   return "L'email est obligatoire.";
            if (idService <= 0)                    return "Veuillez sélectionner un service.";
            return null;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // UC5/6/7/8 – GESTION DES ABSENCES
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère les absences d'un personnel, triées de la plus récente à la plus ancienne.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <returns>Liste de <see cref="Absence"/></returns>
        public List<Absence> GetAbsencesPersonnel(int idPersonnel)
            => _access.GetAbsencesPersonnel(idPersonnel);

        /// <summary>
        /// Récupère les absences filtrées et/ou triées.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="recherche">Texte recherché dans le motif</param>
        /// <param name="triPar">"datedebut", "datefin" ou "motif"</param>
        /// <param name="triDesc">True pour tri descendant</param>
        /// <returns>Liste filtrée et triée de <see cref="Absence"/></returns>
        public List<Absence> GetAbsencesFiltreesTries(int idPersonnel, string recherche = null, string triPar = "datedebut", bool triDesc = true)
        {
            List<Absence> liste = _access.GetAbsencesPersonnel(idPersonnel);

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                string r = recherche.ToLower();
                liste = liste.FindAll(a =>
                    (a.Motif?.Libelle.ToLower().Contains(r) ?? false) ||
                    a.DateDebut.ToString("dd/MM/yyyy").Contains(r) ||
                    a.DateFin.ToString("dd/MM/yyyy").Contains(r)
                );
            }

            switch (triPar?.ToLower())
            {
                case "datefin":
                    liste.Sort((a, b) => triDesc
                        ? DateTime.Compare(b.DateFin, a.DateFin)
                        : DateTime.Compare(a.DateFin, b.DateFin));
                    break;
                case "motif":
                    liste.Sort((a, b) => triDesc
                        ? string.Compare(b.Motif?.Libelle, a.Motif?.Libelle, StringComparison.OrdinalIgnoreCase)
                        : string.Compare(a.Motif?.Libelle, b.Motif?.Libelle, StringComparison.OrdinalIgnoreCase));
                    break;
                default: // datedebut
                    liste.Sort((a, b) => triDesc
                        ? DateTime.Compare(b.DateDebut, a.DateDebut)
                        : DateTime.Compare(a.DateDebut, b.DateDebut));
                    break;
            }

            return liste;
        }

        /// <summary>
        /// Ajoute une absence après toutes les validations métier.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebut">Date de début</param>
        /// <param name="dateFin">Date de fin</param>
        /// <param name="idMotif">Identifiant du motif</param>
        /// <param name="erreur">Message d'erreur si validation échoue</param>
        /// <returns>True si ajout réussi</returns>
        public bool AjouterAbsence(int idPersonnel, DateTime dateDebut, DateTime dateFin, int idMotif, out string erreur)
        {
            erreur = ValiderChampsAbsence(dateDebut, dateFin, idMotif);
            if (erreur != null) return false;

            if (_access.ExisteChevauchement(idPersonnel, dateDebut, dateFin))
            {
                erreur = "Cette période chevauche une absence existante.";
                return false;
            }

            Absence a = new Absence
            {
                IdPersonnel = idPersonnel,
                DateDebut = dateDebut,
                DateFin = dateFin,
                IdMotif = idMotif
            };
            bool ok = _access.AjouterAbsence(a);
            if (!ok) erreur = "Erreur lors de l'insertion en base de données.";
            return ok;
        }

        /// <summary>
        /// Modifie une absence après toutes les validations métier.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebutOrigine">Date de début originale (clé primaire)</param>
        /// <param name="dateDebut">Nouvelle date de début</param>
        /// <param name="dateFin">Nouvelle date de fin</param>
        /// <param name="idMotif">Nouvel identifiant de motif</param>
        /// <param name="erreur">Message d'erreur si validation échoue</param>
        /// <returns>True si modification réussie</returns>
        public bool ModifierAbsence(int idPersonnel, DateTime dateDebutOrigine, DateTime dateDebut, DateTime dateFin, int idMotif, out string erreur)
        {
            erreur = ValiderChampsAbsence(dateDebut, dateFin, idMotif);
            if (erreur != null) return false;

            if (_access.ExisteChevauchement(idPersonnel, dateDebut, dateFin, dateDebutOrigine))
            {
                erreur = "Cette période chevauche une autre absence existante.";
                return false;
            }

            Absence a = new Absence
            {
                IdPersonnel = idPersonnel,
                DateDebut = dateDebut,
                DateFin = dateFin,
                IdMotif = idMotif
            };
            bool ok = _access.ModifierAbsence(a, dateDebutOrigine);
            if (!ok) erreur = "Erreur lors de la mise à jour en base de données.";
            return ok;
        }

        /// <summary>
        /// Supprime une absence identifiée par le personnel et la date de début.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebut">Date de début de l'absence</param>
        /// <returns>True si suppression réussie</returns>
        public bool SupprimerAbsence(int idPersonnel, DateTime dateDebut)
            => _access.SupprimerAbsence(idPersonnel, dateDebut);

        // ─── Validation Absences ──────────────────────────────────────────────────

        /// <summary>
        /// Valide les champs d'un formulaire d'absence.
        /// </summary>
        /// <returns>Message d'erreur ou null si valide</returns>
        private string ValiderChampsAbsence(DateTime dateDebut, DateTime dateFin, int idMotif)
        {
            if (dateDebut == default)  return "La date de début est obligatoire.";
            if (dateFin == default)    return "La date de fin est obligatoire.";
            if (idMotif <= 0)          return "Veuillez sélectionner un motif.";
            if (dateFin <= dateDebut)  return "La date de fin doit être postérieure à la date de début.";
            return null;
        }
    }
}
