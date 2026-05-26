namespace MediaTek86.modele
{
    /// <summary>
    /// Classe métier représentant un motif d'absence.
    /// Correspond à la table <c>motif</c> de la base de données.
    /// </summary>
    public class Motif
    {
        /// <summary>Identifiant unique du motif (clé primaire auto-incrémentée)</summary>
        public int IdMotif { get; set; }

        /// <summary>Libellé descriptif du motif d'absence</summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Motif() { }

        /// <summary>
        /// Constructeur avec paramètres.
        /// </summary>
        /// <param name="idMotif">Identifiant du motif</param>
        /// <param name="libelle">Libellé du motif</param>
        public Motif(int idMotif, string libelle)
        {
            IdMotif = idMotif;
            Libelle = libelle;
        }

        /// <summary>
        /// Retourne une représentation lisible du motif.
        /// </summary>
        /// <returns>Libellé du motif</returns>
        public override string ToString()
        {
            return Libelle;
        }
    }
}
