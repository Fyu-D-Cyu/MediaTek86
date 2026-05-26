using System;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;

namespace MediaTek86.vue
{
    public class FormLogin : Form
    {
        private TextBox txtLogin;
        private TextBox txtPwd;
        private Button btnConnexion;
        private Label lblErreur;

        private int _tentatives;
        private const int MaxTentatives = 3;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "MediaTek86 - Connexion";
            Size = new Size(480, 540);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 9.5f);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.FromArgb(26, 54, 93)
            };

            Label lblAppName = new Label
            {
                Text = "MediaTek86",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                Location = new Point(24, 20),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Gestion du personnel et des absences",
                ForeColor = Color.FromArgb(160, 190, 230),
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(26, 66),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblAppName);
            pnlHeader.Controls.Add(lblSubtitle);

            Panel pnlCard = new Panel
            {
                BackColor = Color.White,
                Size = new Size(400, 300),
                Location = new Point(40, 140)
            };
            pnlCard.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(210, 218, 230), 1),
                    0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            };

            Label lblTitre = new Label
            {
                Text = "Connexion",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 54, 93),
                Location = new Point(28, 24),
                AutoSize = true
            };

            Label lblLogin = new Label
            {
                Text = "Identifiant",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                Location = new Point(28, 72),
                AutoSize = true
            };

            txtLogin = new TextBox
            {
                Location = new Point(28, 92),
                Size = new Size(344, 30),
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 252)
            };
            txtLogin.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    txtPwd.Focus();
            };

            Label lblPwd = new Label
            {
                Text = "Mot de passe",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                Location = new Point(28, 138),
                AutoSize = true
            };

            txtPwd = new TextBox
            {
                Location = new Point(28, 158),
                Size = new Size(344, 30),
                Font = new Font("Segoe UI", 10f),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 252)
            };
            txtPwd.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    BtnConnexion_Click(sender, e);
            };

            btnConnexion = new Button
            {
                Text = "SE CONNECTER",
                Location = new Point(28, 210),
                Size = new Size(344, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(26, 54, 93),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConnexion.FlatAppearance.BorderSize = 0;
            btnConnexion.Click += BtnConnexion_Click;
            btnConnexion.MouseEnter += delegate { btnConnexion.BackColor = Color.FromArgb(41, 82, 128); };
            btnConnexion.MouseLeave += delegate { btnConnexion.BackColor = Color.FromArgb(26, 54, 93); };

            lblErreur = new Label
            {
                ForeColor = Color.FromArgb(200, 50, 50),
                Font = new Font("Segoe UI", 9f),
                Location = new Point(28, 262),
                Size = new Size(344, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlCard.Controls.AddRange(new Control[]
                { lblTitre, lblLogin, txtLogin, lblPwd, txtPwd, btnConnexion, lblErreur });

            Label lblVersion = new Label
            {
                Text = "v1.0 - .NET Framework",
                ForeColor = Color.FromArgb(160, 170, 185),
                Font = new Font("Segoe UI", 8f),
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(pnlHeader);
            Controls.Add(pnlCard);
            Controls.Add(lblVersion);

            AcceptButton = btnConnexion;
            ActiveControl = txtLogin;
        }

        private void BtnConnexion_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string pwd = txtPwd.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pwd))
            {
                AfficherErreur("Veuillez remplir tous les champs.");
                return;
            }

            if (Controller.GetInstance().Connecter(login, pwd))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            _tentatives++;
            int restantes = MaxTentatives - _tentatives;
            if (_tentatives >= MaxTentatives)
            {
                AfficherErreur("Acces bloque apres 3 tentatives echouees.");
                btnConnexion.Enabled = false;
                txtLogin.Enabled = false;
                txtPwd.Enabled = false;
            }
            else
            {
                AfficherErreur("Identifiants incorrects. " + restantes + " tentative(s) restante(s).");
            }

            txtPwd.Clear();
            txtPwd.Focus();
        }

        private void AfficherErreur(string msg)
        {
            lblErreur.Text = msg;
        }
    }
}
