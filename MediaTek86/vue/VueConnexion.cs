using System;
using MediaTek86.controller;

namespace MediaTek86.vue
{
    /// <summary>
    /// Vue console pour l'écran de connexion (UC1).
    /// Gère la saisie des identifiants et le retour visuel.
    /// </summary>
    public class VueConnexion
    {
        private readonly Controller _ctrl;
        private const int MAX_TENTATIVES = 3;

        /// <summary>
        /// Constructeur.
        /// </summary>
        public VueConnexion()
        {
            _ctrl = Controller.GetInstance();
        }

        /// <summary>
        /// Affiche l'écran de connexion et tente l'authentification.
        /// Bloque l'accès après <see cref="MAX_TENTATIVES"/> tentatives échouées.
        /// </summary>
        /// <returns>True si connexion réussie, False si abandon ou verrouillage</returns>
        public bool AfficherConnexion()
        {
            VueHelper.AfficherBandeau();
            VueHelper.Titre("CONNEXION");

            int tentatives = 0;
            while (tentatives < MAX_TENTATIVES)
            {
                VueHelper.EcrireInline("  Login    : ", ConsoleColor.White);
                string login = Console.ReadLine()?.Trim() ?? "";
                string pwd   = VueHelper.SaisirMotDePasse("Mot de passe");

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pwd))
                {
                    VueHelper.Erreur("Login et mot de passe obligatoires.");
                    tentatives++;
                    Console.WriteLine();
                    continue;
                }

                if (_ctrl.Connecter(login, pwd))
                {
                    VueHelper.Succes($"Bienvenue, {login} !");
                    System.Threading.Thread.Sleep(800);
                    return true;
                }
                else
                {
                    tentatives++;
                    int restantes = MAX_TENTATIVES - tentatives;
                    VueHelper.Erreur($"Identifiants incorrects. {restantes} tentative(s) restante(s).");
                    Console.WriteLine();
                }
            }

            VueHelper.Avertissement("Accès bloqué après 3 tentatives échouées.");
            VueHelper.AttendreEntree("Appuyez sur Entrée pour quitter...");
            return false;
        }
    }
}
