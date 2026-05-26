using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using MediaTek86.bddmanager;
using MediaTek86.modele;

namespace MediaTek86.dal
{
    /// <summary>
    /// Classe d'accès aux données (DAL) pour l'application MediaTek86.
    /// Gère toutes les interactions avec la base de données MySQL via <see cref="BddManager"/>.
    /// </summary>
    public class Access
    {
        // ─── Chaîne de connexion ──────────────────────────────────────────────────

        /// <summary>
        /// Chaîne de connexion récupérée depuis le fichier App.config (clé "MediaTek86").
        /// </summary>
        private static readonly string _connectionString = GetConnectionString();

        /// <summary>
        /// Instance unique de la DAL (pattern Singleton).
        /// </summary>
        private static Access _instance = null;

        /// <summary>
        /// Instance du gestionnaire de base de données.
        /// </summary>
        private readonly BddManager _bdd;

        // ─── Singleton ────────────────────────────────────────────────────────────

        /// <summary>
        /// Constructeur privé : initialise la connexion BDD.
        /// </summary>
        private Access()
        {
            _bdd = BddManager.GetInstance(_connectionString);
        }

        /// <summary>
        /// Retourne l'instance unique de la DAL.
        /// </summary>
        /// <returns>Instance unique d'Access</returns>
        public static Access GetInstance()
        {
            if (_instance == null)
                _instance = new Access();
            return _instance;
        }

        /// <summary>
        /// Réinitialise l'instance (utile pour les tests).
        /// </summary>
        public static void ResetInstance()
        {
            BddManager.ResetInstance();
            _instance = null;
        }

        // ─── Chaîne de connexion ──────────────────────────────────────────────────

        /// <summary>
        /// Récupère la chaîne de connexion depuis App.config.
        /// Si absente, retourne une chaîne par défaut (localhost/mediatek86).
        /// </summary>
        /// <returns>Chaîne de connexion MySQL</returns>
        private static string GetConnectionString()
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["MediaTek86"]?.ConnectionString;
                if (!string.IsNullOrEmpty(cs))
                    return cs;
            }
            catch { }
            // Chaîne par défaut si App.config absent
            return "server=localhost;port=3306;database=media tek86;uid=root;pwd=;SslMode=Disabled;AllowPublicKeyRetrieval=True;";
        }

        /// <summary>
        /// Expose la chaîne de connexion (lecture seule).
        /// </summary>
        public string ConnectionString => _connectionString;

        // ═══════════════════════════════════════════════════════════════════════════
        // UC1 – CONNEXION
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Vérifie les identifiants d'un responsable en comparant le hash SHA-256 du mot de passe.
        /// </summary>
        /// <param name="login">Login saisi</param>
        /// <param name="pwdHash">Mot de passe hashé en SHA-256</param>
        /// <returns>Objet <see cref="Responsable"/> si authentifié, null sinon</returns>
        public Responsable ChercheResponsable(string login, string pwdHash)
        {
            string req = "SELECT login, pwd FROM responsable WHERE login=@login AND pwd=SHA2(@pwd256, 256)";
            // On passe le hash directement pour comparaison — SHA2 appliqué côté BD
            string req2 = "SELECT login, pwd FROM responsable WHERE login=@login AND pwd=@pwdHash";
            var parametres = new Dictionary<string, object>
            {
                { "@login", login },
                { "@pwdHash", pwdHash }
            };
            MySqlDataReader reader = _bdd.ExecuteSelect(req2, parametres);
            Responsable responsable = null;
            if (reader != null && reader.Read())
            {
                responsable = new Responsable(reader["login"].ToString(), reader["pwd"].ToString());
            }
            reader?.Close();
            return responsable;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // SERVICES
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère tous les services de la base de données.
        /// </summary>
        /// <returns>Liste de tous les <see cref="Service"/></returns>
        public List<Service> GetAllServices()
        {
            string req = "SELECT idservice, nom FROM service ORDER BY nom";
            MySqlDataReader reader = _bdd.ExecuteSelect(req);
            List<Service> services = new List<Service>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    services.Add(new Service(
                        Convert.ToInt32(reader["idservice"]),
                        reader["nom"].ToString()
                    ));
                }
                reader.Close();
            }
            return services;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // MOTIFS
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère tous les motifs d'absence.
        /// </summary>
        /// <returns>Liste de tous les <see cref="Motif"/></returns>
        public List<Motif> GetAllMotifs()
        {
            string req = "SELECT idmotif, libelle FROM motif ORDER BY libelle";
            MySqlDataReader reader = _bdd.ExecuteSelect(req);
            List<Motif> motifs = new List<Motif>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    motifs.Add(new Motif(
                        Convert.ToInt32(reader["idmotif"]),
                        reader["libelle"].ToString()
                    ));
                }
                reader.Close();
            }
            return motifs;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // UC2/3/4 – GESTION DU PERSONNEL
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère tous les membres du personnel avec leur service associé.
        /// </summary>
        /// <returns>Liste de tous les <see cref="Personnel"/></returns>
        public List<Personnel> GetAllPersonnel()
        {
            string req = @"
                SELECT p.idpersonnel, p.nom, p.prenom, p.tel, p.mail,
                       s.idservice, s.nom AS nomservice
                FROM personnel p
                JOIN service s ON p.idservice = s.idservice
                ORDER BY p.nom, p.prenom";
            MySqlDataReader reader = _bdd.ExecuteSelect(req);
            List<Personnel> liste = new List<Personnel>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    Service service = new Service(
                        Convert.ToInt32(reader["idservice"]),
                        reader["nomservice"].ToString()
                    );
                    Personnel pers = new Personnel(
                        Convert.ToInt32(reader["idpersonnel"]),
                        reader["nom"].ToString(),
                        reader["prenom"].ToString(),
                        reader["tel"].ToString(),
                        reader["mail"].ToString(),
                        service
                    );
                    liste.Add(pers);
                }
                reader.Close();
            }
            return liste;
        }

        /// <summary>
        /// Ajoute un nouveau membre du personnel en base.
        /// </summary>
        /// <param name="personnel">Objet <see cref="Personnel"/> à insérer</param>
        /// <returns>True si l'insertion a réussi, False sinon</returns>
        public bool AjouterPersonnel(Personnel personnel)
        {
            string req = @"INSERT INTO personnel (nom, prenom, tel, mail, idservice)
                           VALUES (@nom, @prenom, @tel, @mail, @idservice)";
            var parametres = new Dictionary<string, object>
            {
                { "@nom",       personnel.Nom },
                { "@prenom",    personnel.Prenom },
                { "@tel",       personnel.Tel },
                { "@mail",      personnel.Mail },
                { "@idservice", personnel.IdService }
            };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }

        /// <summary>
        /// Modifie un membre du personnel existant.
        /// </summary>
        /// <param name="personnel">Objet <see cref="Personnel"/> avec les nouvelles valeurs</param>
        /// <returns>True si la mise à jour a réussi, False sinon</returns>
        public bool ModifierPersonnel(Personnel personnel)
        {
            string req = @"UPDATE personnel
                           SET nom=@nom, prenom=@prenom, tel=@tel, mail=@mail, idservice=@idservice
                           WHERE idpersonnel=@id";
            var parametres = new Dictionary<string, object>
            {
                { "@nom",       personnel.Nom },
                { "@prenom",    personnel.Prenom },
                { "@tel",       personnel.Tel },
                { "@mail",      personnel.Mail },
                { "@idservice", personnel.IdService },
                { "@id",        personnel.IdPersonnel }
            };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }

        /// <summary>
        /// Supprime un membre du personnel (et ses absences associées via CASCADE ou préalablement supprimées).
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel à supprimer</param>
        /// <returns>True si la suppression a réussi, False sinon</returns>
        public bool SupprimerPersonnel(int idPersonnel)
        {
            // Suppression des absences d'abord pour respecter les FK
            string reqAbsences = "DELETE FROM absence WHERE idpersonnel=@id";
            _bdd.ExecuteAction(reqAbsences, new Dictionary<string, object> { { "@id", idPersonnel } });

            string req = "DELETE FROM personnel WHERE idpersonnel=@id";
            var parametres = new Dictionary<string, object> { { "@id", idPersonnel } };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // UC5/6/7/8 – GESTION DES ABSENCES
        // ═══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Récupère les absences d'un personnel trié de la plus récente à la plus ancienne.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <returns>Liste des <see cref="Absence"/> triées par date décroissante</returns>
        public List<Absence> GetAbsencesPersonnel(int idPersonnel)
        {
            string req = @"
                SELECT a.idpersonnel, a.datedebut, a.datefin,
                       m.idmotif, m.libelle
                FROM absence a
                JOIN motif m ON a.idmotif = m.idmotif
                WHERE a.idpersonnel = @id
                ORDER BY a.datedebut DESC";
            var parametres = new Dictionary<string, object> { { "@id", idPersonnel } };
            MySqlDataReader reader = _bdd.ExecuteSelect(req, parametres);
            List<Absence> absences = new List<Absence>();
            if (reader != null)
            {
                while (reader.Read())
                {
                    Motif motif = new Motif(
                        Convert.ToInt32(reader["idmotif"]),
                        reader["libelle"].ToString()
                    );
                    Absence absence = new Absence(
                        Convert.ToInt32(reader["idpersonnel"]),
                        Convert.ToDateTime(reader["datedebut"]),
                        Convert.ToDateTime(reader["datefin"]),
                        motif
                    );
                    absences.Add(absence);
                }
                reader.Close();
            }
            return absences;
        }

        /// <summary>
        /// Vérifie s'il existe un chevauchement d'absence pour un personnel donné,
        /// en excluant optionnellement une absence existante (pour la modification).
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebut">Date de début à tester</param>
        /// <param name="dateFin">Date de fin à tester</param>
        /// <param name="dateDebutExclue">Date de début de l'absence à exclure (modification), null pour ajout</param>
        /// <returns>True si chevauchement détecté, False sinon</returns>
        public bool ExisteChevauchement(int idPersonnel, DateTime dateDebut, DateTime dateFin, DateTime? dateDebutExclue = null)
        {
            string req;
            Dictionary<string, object> parametres;

            if (dateDebutExclue.HasValue)
            {
                req = @"SELECT COUNT(*) FROM absence
                        WHERE idpersonnel=@id
                          AND datedebut != @exclu
                          AND datedebut < @fin
                          AND datefin > @debut";
                parametres = new Dictionary<string, object>
                {
                    { "@id",    idPersonnel },
                    { "@debut", dateDebut },
                    { "@fin",   dateFin },
                    { "@exclu", dateDebutExclue.Value }
                };
            }
            else
            {
                req = @"SELECT COUNT(*) FROM absence
                        WHERE idpersonnel=@id
                          AND datedebut < @fin
                          AND datefin > @debut";
                parametres = new Dictionary<string, object>
                {
                    { "@id",    idPersonnel },
                    { "@debut", dateDebut },
                    { "@fin",   dateFin }
                };
            }

            object result = _bdd.ExecuteScalar(req, parametres);
            return result != null && Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Ajoute une nouvelle absence en base.
        /// </summary>
        /// <param name="absence">Objet <see cref="Absence"/> à insérer</param>
        /// <returns>True si l'insertion a réussi, False sinon</returns>
        public bool AjouterAbsence(Absence absence)
        {
            string req = @"INSERT INTO absence (idpersonnel, datedebut, datefin, idmotif)
                           VALUES (@idpersonnel, @datedebut, @datefin, @idmotif)";
            var parametres = new Dictionary<string, object>
            {
                { "@idpersonnel", absence.IdPersonnel },
                { "@datedebut",   absence.DateDebut },
                { "@datefin",     absence.DateFin },
                { "@idmotif",     absence.IdMotif }
            };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }

        /// <summary>
        /// Modifie une absence existante (identifiée par idpersonnel + datedebut originale).
        /// </summary>
        /// <param name="absence">Objet <see cref="Absence"/> avec les nouvelles valeurs</param>
        /// <param name="dateDebutOrigine">Date de début originale (clé primaire)</param>
        /// <returns>True si la mise à jour a réussi, False sinon</returns>
        public bool ModifierAbsence(Absence absence, DateTime dateDebutOrigine)
        {
            string req = @"UPDATE absence
                           SET datedebut=@datedebut, datefin=@datefin, idmotif=@idmotif
                           WHERE idpersonnel=@idpersonnel AND datedebut=@origine";
            var parametres = new Dictionary<string, object>
            {
                { "@idpersonnel", absence.IdPersonnel },
                { "@datedebut",   absence.DateDebut },
                { "@datefin",     absence.DateFin },
                { "@idmotif",     absence.IdMotif },
                { "@origine",     dateDebutOrigine }
            };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }

        /// <summary>
        /// Supprime une absence identifiée par le personnel et la date de début.
        /// </summary>
        /// <param name="idPersonnel">Identifiant du personnel</param>
        /// <param name="dateDebut">Date de début de l'absence (clé primaire)</param>
        /// <returns>True si la suppression a réussi, False sinon</returns>
        public bool SupprimerAbsence(int idPersonnel, DateTime dateDebut)
        {
            string req = "DELETE FROM absence WHERE idpersonnel=@id AND datedebut=@datedebut";
            var parametres = new Dictionary<string, object>
            {
                { "@id",        idPersonnel },
                { "@datedebut", dateDebut }
            };
            return _bdd.ExecuteAction(req, parametres) > 0;
        }
    }
}
