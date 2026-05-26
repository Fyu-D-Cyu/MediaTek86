namespace MediaTek86.modele
{
    /// <summary>
    /// Classe métier représentant un service de l'établissement.
    /// Correspond à la table <c>service</c> de la base de données.
    /// </summary>
    public class Service
    {
        /// <summary>Identifiant unique du service (clé primaire auto-incrémentée)</summary>
        public int IdService { get; set; }

        /// <summary>Nom du service</summary>
        public string Nom { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Service() { }

        /// <summary>
        /// Constructeur avec paramètres.
        /// </summary>
        /// <param name="idService">Identifiant du service</param>
        /// <param name="nom">Nom du service</param>
        public Service(int idService, string nom)
        {
            IdService = idService;
            Nom = nom;
        }

        /// <summary>
        /// Retourne une représentation lisible du service.
        /// </summary>
        /// <returns>Chaîne au format "Nom (ID)"</returns>
        public override string ToString()
        {
            return $"{Nom} (ID: {IdService})";
        }
    }
}
