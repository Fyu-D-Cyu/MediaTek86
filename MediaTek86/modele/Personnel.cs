namespace MediaTek86.modele
{
    /// <summary>
    /// Classe métier représentant un membre du personnel.
    /// Correspond à la table <c>personnel</c> de la base de données.
    /// </summary>
    public class Personnel
    {
        /// <summary>Identifiant unique du personnel (clé primaire auto-incrémentée)</summary>
        public int IdPersonnel { get; set; }

        /// <summary>Nom de famille</summary>
        public string Nom { get; set; }

        /// <summary>Prénom</summary>
        public string Prenom { get; set; }

        /// <summary>Numéro de téléphone</summary>
        public string Tel { get; set; }

        /// <summary>Adresse e-mail</summary>
        public string Mail { get; set; }

        /// <summary>Identifiant du service rattaché (clé étrangère)</summary>
        public int IdService { get; set; }

        /// <summary>Objet Service associé (jointure, peut être null si non chargé)</summary>
        public Service Service { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Personnel() { }

        /// <summary>
        /// Constructeur complet avec l'objet service associé.
        /// </summary>
        /// <param name="idPersonnel">Identifiant</param>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="tel">Téléphone</param>
        /// <param name="mail">Email</param>
        /// <param name="service">Objet Service associé</param>
        public Personnel(int idPersonnel, string nom, string prenom, string tel, string mail, Service service)
        {
            IdPersonnel = idPersonnel;
            Nom = nom;
            Prenom = prenom;
            Tel = tel;
            Mail = mail;
            Service = service;
            IdService = service?.IdService ?? 0;
        }

        /// <summary>
        /// Retourne une représentation lisible du personnel.
        /// </summary>
        /// <returns>Chaîne au format "Nom Prénom — Service"</returns>
        public override string ToString()
        {
            string nomService = Service != null ? Service.Nom : $"Service {IdService}";
            return $"{Nom} {Prenom} — {nomService}";
        }

        /// <summary>
        /// Retourne le nom complet (Nom + Prénom).
        /// </summary>
        /// <returns>Nom et prénom concaténés</returns>
        public string NomComplet()
        {
            return $"{Nom} {Prenom}";
        }
    }
}
