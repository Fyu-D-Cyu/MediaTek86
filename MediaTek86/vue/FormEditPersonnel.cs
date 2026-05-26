using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    public class FormEditPersonnel : Form
    {
        private readonly Controller _ctrl = Controller.GetInstance();
        private readonly Personnel _personnelOrigine;
        private readonly bool _estModification;

        private TextBox txtNom;
        private TextBox txtPrenom;
        private TextBox txtTel;
        private TextBox txtMail;
        private ComboBox cmbService;
        private Label lblErreur;

        public FormEditPersonnel(Personnel personnel)
        {
            _personnelOrigine = personnel;
            _estModification = personnel != null;
            InitializeComponent();
            ChargerServices();
            if (_estModification)
                PreRemplirFormulaire();
        }

        private void InitializeComponent()
        {
            Text = _estModification ? "Modifier un personnel" : "Ajouter un personnel";
            Size = new Size(560, 530);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 9.5f);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(26, 54, 93)
            };
            Label lblTitre = new Label
            {
                Text = _estModification ? "Modifier le personnel" : "Nouveau personnel",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Location = new Point(16, 14),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblTitre);

            Panel pnlForm = new Panel
            {
                BackColor = Color.White,
                Location = new Point(24, 76),
                Size = new Size(500, 330),
                Padding = new Padding(20)
            };
            pnlForm.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(210, 218, 230)),
                    0, 0, pnlForm.Width - 1, pnlForm.Height - 1);
            };

            int y = 18;
            txtNom = AjouterChamp(pnlForm, "Nom *", ref y, 440);
            txtPrenom = AjouterChamp(pnlForm, "Prenom *", ref y, 440);
            txtTel = AjouterChamp(pnlForm, "Telephone *", ref y, 440);
            txtMail = AjouterChamp(pnlForm, "Email *", ref y, 440);

            Label lblService = new Label
            {
                Text = "Service *",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                Location = new Point(20, y),
                AutoSize = true
            };
            y += 20;
            cmbService = new ComboBox
            {
                Location = new Point(20, y),
                Size = new Size(440, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlForm.Controls.Add(lblService);
            pnlForm.Controls.Add(cmbService);

            lblErreur = new Label
            {
                Location = new Point(24, 414),
                Size = new Size(500, 24),
                ForeColor = Color.FromArgb(200, 50, 50),
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(24, 446),
                Size = new Size(120, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(200, 205, 215),
                ForeColor = Color.FromArgb(40, 50, 70),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnAnnuler.FlatAppearance.BorderSize = 0;

            Button btnValider = new Button
            {
                Text = _estModification ? "Enregistrer" : "Ajouter",
                Location = new Point(404, 446),
                Size = new Size(120, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(26, 54, 93),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnValider.FlatAppearance.BorderSize = 0;
            btnValider.Click += BtnValider_Click;

            Controls.Add(pnlHeader);
            Controls.Add(pnlForm);
            Controls.Add(btnAnnuler);
            Controls.Add(btnValider);
            Controls.Add(lblErreur);
            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        private void ChargerServices()
        {
            List<Service> services = _ctrl.GetAllServices();
            cmbService.DataSource = services;
            cmbService.DisplayMember = "Nom";
            cmbService.ValueMember = "IdService";
        }

        private void PreRemplirFormulaire()
        {
            txtNom.Text = _personnelOrigine.Nom;
            txtPrenom.Text = _personnelOrigine.Prenom;
            txtTel.Text = _personnelOrigine.Tel;
            txtMail.Text = _personnelOrigine.Mail;
            cmbService.SelectedValue = _personnelOrigine.IdService;
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            string nom = txtNom.Text.Trim();
            string prenom = txtPrenom.Text.Trim();
            string tel = txtTel.Text.Trim();
            string mail = txtMail.Text.Trim();
            int idService = cmbService.SelectedValue != null ? Convert.ToInt32(cmbService.SelectedValue) : 0;

            DialogResult confirm = MessageBox.Show(
                _estModification ? "Confirmer la modification ?" : "Confirmer l'ajout ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            bool ok;
            string erreur;
            if (_estModification)
                ok = _ctrl.ModifierPersonnel(_personnelOrigine.IdPersonnel, nom, prenom, tel, mail, idService, out erreur);
            else
                ok = _ctrl.AjouterPersonnel(nom, prenom, tel, mail, idService, out erreur);

            if (ok)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblErreur.Text = erreur;
            }
        }

        private TextBox AjouterChamp(Panel parent, string label, ref int y, int largeur)
        {
            Label lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                Location = new Point(20, y),
                AutoSize = true
            };
            y += 20;

            TextBox txt = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(largeur, 26),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 252)
            };
            y += 36;

            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
            return txt;
        }
    }
}
