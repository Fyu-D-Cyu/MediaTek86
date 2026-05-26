using System;

namespace MediaTek86.modele
{
    /// <summary>
    /// Classe métier représentant une absence d'un membre du personnel.
    /// Correspond à la table <c>absence</c> de la base de données.
    /// Clé primaire composite : (idpersonnel, datedebut).
    /// </summary>
    public class Absence
    {
        /// <summary>Identifiant du personnel concerné (partie de la clé primaire)</summary>
        public int IdPersonnel { get; set; }

        /// <summary>Date et heure de début de l'absence (partie de la clé primaire)</summary>
        public DateTime DateDebut { get; set; }

        /// <summary>Date et heure de fin de l'absence</summary>
        public DateTime DateFin { get; set; }

        /// <summary>Identifiant du motif d'absence (clé étrangère)</summary>
        public int IdMotif { get; set; }

        /// <summary>Objet Motif associé (jointure, peut être null si non chargé)</summary>
        public Motif Motif { get; set; }

        /// <summary>Objet Personnel associé (optionnel, pour affichage)</summary>
        public Personnel Personnel { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Absence() { }

        /// <summary>
        /// Constructeur complet avec les objets associés.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebut">Date de début</param>
        /// <param name="dateFin">Date de fin</param>
        /// <param name="motif">Objet Motif associé</param>
        public Absence(int idPersonnel, DateTime dateDebut, DateTime dateFin, Motif motif)
        {
            IdPersonnel = idPersonnel;
            DateDebut = dateDebut;
            DateFin = dateFin;
            Motif = motif;
            IdMotif = motif?.IdMotif ?? 0;
        }

        /// <summary>
        /// Calcule la durée de l'absence en jours.
        /// </summary>
        /// <returns>Nombre de jours d'absence (arrondi à l'entier supérieur)</returns>
        public int DureeJours()
        {
            return (int)Math.Ceiling((DateFin - DateDebut).TotalDays);
        }

        /// <summary>
        /// Retourne une représentation lisible de l'absence.
        /// </summary>
        /// <returns>Chaîne décrivant la période et le motif</returns>
        public override string ToString()
        {
            string libMotif = Motif != null ? Motif.Libelle : $"Motif {IdMotif}";
            return $"Du {DateDebut:dd/MM/yyyy} au {DateFin:dd/MM/yyyy} — {libMotif} ({DureeJours()} jour(s))";
        }

        /// <summary>
        /// Vérifie si cette absence chevauche une autre période donnée.
        /// </summary>
        /// <param name="debut">Début de la période à tester</param>
        /// <param name="fin">Fin de la période à tester</param>
        /// <returns>True si chevauchement détecté, False sinon</returns>
        public bool Chevauche(DateTime debut, DateTime fin)
        {
            return DateDebut < fin && DateFin > debut;
        }
    }
}
