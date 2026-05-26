namespace MediaTek86.modele
{
    /// <summary>
    /// Classe métier représentant un responsable pouvant se connecter à l'application.
    /// Correspond à la table <c>responsable</c> de la base de données.
    /// </summary>
    public class Responsable
    {
        /// <summary>Login (identifiant de connexion)</summary>
        public string Login { get; set; }

        /// <summary>Mot de passe hashé en SHA-256</summary>
        public string Pwd { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Responsable() { }

        /// <summary>
        /// Constructeur avec paramètres.
        /// </summary>
        /// <param name="login">Login du responsable</param>
        /// <param name="pwd">Mot de passe hashé</param>
        public Responsable(string login, string pwd)
        {
            Login = login;
            Pwd = pwd;
        }

        /// <summary>
        /// Retourne une représentation lisible du responsable.
        /// </summary>
        /// <returns>Login du responsable</returns>
        public override string ToString()
        {
            return Login;
        }
    }
}
