using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MediaTek86.bddmanager
{
    /// <summary>
    /// Classe technique singleton gérant la connexion à la base de données MySQL.
    /// Utilise le pattern Singleton pour garantir une instance unique.
    /// </summary>
    public class BddManager
    {
        /// <summary>Instance unique du singleton</summary>
        private static BddManager _instance = null;

        /// <summary>Connexion MySQL active</summary>
        private MySqlConnection _connection = null;

        /// <summary>
        /// Constructeur privé : initialise et ouvre la connexion à la base de données.
        /// </summary>
        /// <param name="stringConnect">Chaîne de connexion MySQL</param>
        private BddManager(string stringConnect)
        {
            try
            {
                _connection = new MySqlConnection(stringConnect);
                _connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur de connexion à la base de données : " + e.Message);
                _connection = null;
            }
        }

        /// <summary>
        /// Retourne l'instance unique du BddManager (crée si inexistante).
        /// </summary>
        /// <param name="stringConnect">Chaîne de connexion MySQL</param>
        /// <returns>Instance unique de BddManager</returns>
        public static BddManager GetInstance(string stringConnect)
        {
            if (_instance == null)
            {
                _instance = new BddManager(stringConnect);
            }
            return _instance;
        }

        /// <summary>
        /// Retourne l'instance existante sans recréer de connexion.
        /// </summary>
        /// <returns>Instance courante ou null si non initialisée</returns>
        public static BddManager GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Réinitialise le singleton (utile pour les tests ou reconnexion).
        /// </summary>
        public static void ResetInstance()
        {
            if (_instance != null)
            {
                _instance.CloseConnection();
                _instance = null;
            }
        }

        /// <summary>
        /// Exécute une requête SELECT et retourne le résultat sous forme de MySqlDataReader.
        /// </summary>
        /// <param name="requete">Requête SQL SELECT</param>
        /// <param name="parametres">Paramètres nommés de la requête</param>
        /// <returns>MySqlDataReader avec les résultats, ou null en cas d'erreur</returns>
        public MySqlDataReader ExecuteSelect(string requete, Dictionary<string, object> parametres = null)
        {
            if (_connection == null) return null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(requete, _connection);
                if (parametres != null)
                {
                    foreach (var p in parametres)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur SELECT : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Exécute une requête d'action (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="requete">Requête SQL d'action</param>
        /// <param name="parametres">Paramètres nommés de la requête</param>
        /// <returns>Nombre de lignes affectées, -1 en cas d'erreur</returns>
        public int ExecuteAction(string requete, Dictionary<string, object> parametres = null)
        {
            if (_connection == null) return -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand(requete, _connection);
                if (parametres != null)
                {
                    foreach (var p in parametres)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur action SQL : " + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Exécute une requête et retourne une valeur scalaire unique.
        /// </summary>
        /// <param name="requete">Requête SQL</param>
        /// <param name="parametres">Paramètres nommés</param>
        /// <returns>Valeur scalaire ou null</returns>
        public object ExecuteScalar(string requete, Dictionary<string, object> parametres = null)
        {
            if (_connection == null) return null;
            try
            {
                MySqlCommand cmd = new MySqlCommand(requete, _connection);
                if (parametres != null)
                {
                    foreach (var p in parametres)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                return cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur scalaire SQL : " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Ferme la connexion MySQL si elle est ouverte.
        /// </summary>
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Indique si la connexion est active et ouverte.
        /// </summary>
        /// <returns>True si connecté, False sinon</returns>
        public bool IsConnected()
        {
            return _connection != null && _connection.State == System.Data.ConnectionState.Open;
        }
    }
}
