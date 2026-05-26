using System;

namespace MediaTek86.vue
{
    /// <summary>
    /// Classe utilitaire pour l'affichage console de l'application MediaTek86.
    /// Fournit les méthodes d'affichage stylisées (couleurs, tableaux, messages).
    /// </summary>
    public static class VueHelper
    {
        // ─── Couleurs par rôle ────────────────────────────────────────────────────

        public static ConsoleColor CouleurTitre      = ConsoleColor.Cyan;
        public static ConsoleColor CouleurSucces     = ConsoleColor.Green;
        public static ConsoleColor CouleurErreur     = ConsoleColor.Red;
        public static ConsoleColor CouleurAvertissement = ConsoleColor.Yellow;
        public static ConsoleColor CouleurInfo       = ConsoleColor.White;
        public static ConsoleColor CouleurEntete     = ConsoleColor.DarkCyan;
        public static ConsoleColor CouleurMenu       = ConsoleColor.Gray;

        // ─── Affichage texte coloré ───────────────────────────────────────────────

        /// <summary>Écrit une ligne colorée dans la console.</summary>
        public static void Ecrire(string texte, ConsoleColor couleur = ConsoleColor.Gray)
        {
            ConsoleColor orig = Console.ForegroundColor;
            Console.ForegroundColor = couleur;
            Console.WriteLine(texte);
            Console.ForegroundColor = orig;
        }

        /// <summary>Écrit sans saut de ligne.</summary>
        public static void EcrireInline(string texte, ConsoleColor couleur = ConsoleColor.Gray)
        {
            ConsoleColor orig = Console.ForegroundColor;
            Console.ForegroundColor = couleur;
            Console.Write(texte);
            Console.ForegroundColor = orig;
        }

        // ─── Messages sémantiques ─────────────────────────────────────────────────

        /// <summary>Affiche un message de succès (vert) avec icône ✔</summary>
        public static void Succes(string message) => Ecrire("  ✔  " + message, CouleurSucces);

        /// <summary>Affiche un message d'erreur (rouge) avec icône ✘</summary>
        public static void Erreur(string message) => Ecrire("  ✘  " + message, CouleurErreur);

        /// <summary>Affiche un avertissement (jaune) avec icône ⚠</summary>
        public static void Avertissement(string message) => Ecrire("  ⚠  " + message, CouleurAvertissement);

        /// <summary>Affiche une info (blanc)</summary>
        public static void Info(string message) => Ecrire("  ℹ  " + message, CouleurInfo);

        // ─── Séparateurs et titres ────────────────────────────────────────────────

        /// <summary>Affiche une ligne de séparation horizontale.</summary>
        public static void Separateur(char c = '─', int largeur = 70)
        {
            Ecrire(new string(c, largeur), ConsoleColor.DarkGray);
        }

        /// <summary>Affiche un titre encadré.</summary>
        public static void Titre(string texte)
        {
            Console.WriteLine();
            Separateur('═');
            Ecrire($"  {texte}", CouleurTitre);
            Separateur('═');
        }

        /// <summary>Affiche un sous-titre.</summary>
        public static void SousTitre(string texte)
        {
            Console.WriteLine();
            Ecrire($"  ── {texte} ──", CouleurEntete);
        }

        // ─── Entête application ───────────────────────────────────────────────────

        /// <summary>Affiche le bandeau de démarrage de l'application.</summary>
        public static void AfficherBandeau()
        {
            Console.Clear();
            Ecrire(@"
  ╔═══════════════════════════════════════════════════════════╗
  ║              M E D I A T E K 8 6                         ║
  ║        Gestion du Personnel et des Absences              ║
  ╚═══════════════════════════════════════════════════════════╝", CouleurTitre);
            Console.WriteLine();
        }

        // ─── Saisies ──────────────────────────────────────────────────────────────

        /// <summary>Demande une saisie obligatoire (répète si vide).</summary>
        public static string SaisirObligatoire(string libelle)
        {
            string valeur;
            do
            {
                EcrireInline($"  {libelle} : ", CouleurMenu);
                valeur = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(valeur))
                    Erreur("Ce champ est obligatoire.");
            } while (string.IsNullOrWhiteSpace(valeur));
            return valeur;
        }

        /// <summary>Demande une saisie avec valeur par défaut (Entrée = conserver).</summary>
        public static string SaisirAvecDefaut(string libelle, string valeurDefaut)
        {
            EcrireInline($"  {libelle} [{valeurDefaut}] : ", CouleurMenu);
            string saisie = Console.ReadLine()?.Trim() ?? "";
            return string.IsNullOrWhiteSpace(saisie) ? valeurDefaut : saisie;
        }

        /// <summary>Saisit une date au format dd/MM/yyyy (répète si invalide).</summary>
        public static DateTime SaisirDate(string libelle, DateTime? defaut = null)
        {
            DateTime date;
            string defautStr = defaut.HasValue ? defaut.Value.ToString("dd/MM/yyyy") : "";
            while (true)
            {
                string label = defaut.HasValue ? $"{libelle} [{defautStr}]" : libelle;
                EcrireInline($"  {label} (dd/MM/yyyy) : ", CouleurMenu);
                string saisie = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(saisie) && defaut.HasValue)
                    return defaut.Value;
                if (DateTime.TryParseExact(saisie, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out date))
                    return date;
                Erreur("Format invalide. Utilisez dd/MM/yyyy (ex: 15/03/2024)");
            }
        }

        /// <summary>Demande un entier dans une plage donnée.</summary>
        public static int SaisirEntier(string libelle, int min, int max)
        {
            int valeur;
            while (true)
            {
                EcrireInline($"  {libelle} ({min}-{max}) : ", CouleurMenu);
                string saisie = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(saisie, out valeur) && valeur >= min && valeur <= max)
                    return valeur;
                Erreur($"Veuillez entrer un nombre entre {min} et {max}.");
            }
        }

        /// <summary>
        /// Demande une confirmation oui/non.
        /// </summary>
        /// <param name="message">Question posée</param>
        /// <returns>True si l'utilisateur confirme (O/o/Y/y)</returns>
        public static bool Confirmer(string message)
        {
            EcrireInline($"  {message} (O/N) : ", CouleurAvertissement);
            string rep = Console.ReadLine()?.Trim().ToUpper() ?? "N";
            return rep == "O" || rep == "Y";
        }

        /// <summary>Attend l'appui sur Entrée avec un message.</summary>
        public static void AttendreEntree(string message = "Appuyez sur Entrée pour continuer...")
        {
            Console.WriteLine();
            EcrireInline($"  {message}", ConsoleColor.DarkGray);
            Console.ReadLine();
        }

        /// <summary>Saisit le mot de passe en masquant les caractères.</summary>
        public static string SaisirMotDePasse(string libelle = "Mot de passe")
        {
            EcrireInline($"  {libelle} : ", CouleurMenu);
            string pwd = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd.Substring(0, pwd.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    pwd += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pwd;
        }
    }
}
