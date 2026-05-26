using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    public class FormEditAbsence : Form
    {
        private readonly Controller _ctrl = Controller.GetInstance();
        private readonly Personnel _personnel;
        private readonly Absence _absenceOrigine;
        private readonly bool _estModification;

        private DateTimePicker dtpDebut;
        private DateTimePicker dtpFin;
        private ComboBox cmbMotif;
        private Label lblErreur;

        public FormEditAbsence(Personnel personnel, Absence absence)
        {
            _personnel = personnel;
            _absenceOrigine = absence;
            _estModification = absence != null;
            InitializeComponent();
            ChargerMotifs();
            if (_estModification)
                PreRemplirFormulaire();
        }

        private void InitializeComponent()
        {
            Text = _estModification ? "Modifier une absence" : "Ajouter une absence";
            Size = new Size(560, 420);
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
                Text = _estModification ? "Modifier l'absence" : "Nouvelle absence",
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
                Size = new Size(500, 210),
                Padding = new Padding(20)
            };
            pnlForm.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(210, 218, 230)),
                    0, 0, pnlForm.Width - 1, pnlForm.Height - 1);
            };

            Label lblPersonnel = new Label
            {
                Text = "Personnel : " + (_personnel != null ? _personnel.NomComplet() : ""),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 54, 93),
                Location = new Point(20, 18),
                AutoSize = true
            };

            Label lblDebut = CreerLabel("Date de debut *", 20, 62);
            dtpDebut = CreerDatePicker(180, 58);

            Label lblFin = CreerLabel("Date de fin *", 20, 102);
            dtpFin = CreerDatePicker(180, 98);

            Label lblMotif = CreerLabel("Motif *", 20, 142);
            cmbMotif = new ComboBox
            {
                Location = new Point(180, 138),
                Size = new Size(280, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };

            pnlForm.Controls.AddRange(new Control[]
                { lblPersonnel, lblDebut, dtpDebut, lblFin, dtpFin, lblMotif, cmbMotif });

            lblErreur = new Label
            {
                Location = new Point(24, 294),
                Size = new Size(500, 24),
                ForeColor = Color.FromArgb(200, 50, 50),
                Font = new Font("Segoe UI", 8.5f),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnAnnuler = new Button
            {
                Text = "Annuler",
                Location = new Point(24, 326),
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
                Location = new Point(404, 326),
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
            Controls.Add(lblErreur);
            Controls.Add(btnAnnuler);
            Controls.Add(btnValider);
            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        private Label CreerLabel(string texte, int x, int y)
        {
            return new Label
            {
                Text = texte,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private DateTimePicker CreerDatePicker(int x, int y)
        {
            return new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(280, 28),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Font = new Font("Segoe UI", 9.5f)
            };
        }

        private void ChargerMotifs()
        {
            List<Motif> motifs = _ctrl.GetAllMotifs();
            cmbMotif.DataSource = motifs;
            cmbMotif.DisplayMember = "Libelle";
            cmbMotif.ValueMember = "IdMotif";
        }

        private void PreRemplirFormulaire()
        {
            dtpDebut.Value = _absenceOrigine.DateDebut;
            dtpFin.Value = _absenceOrigine.DateFin;
            cmbMotif.SelectedValue = _absenceOrigine.IdMotif;
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            if (_personnel == null)
            {
                lblErreur.Text = "Veuillez selectionner un personnel.";
                return;
            }

            DateTime dateDebut = dtpDebut.Value.Date;
            DateTime dateFin = dtpFin.Value.Date;
            int idMotif = cmbMotif.SelectedValue != null ? Convert.ToInt32(cmbMotif.SelectedValue) : 0;

            if (dateFin <= dateDebut)
            {
                lblErreur.Text = "La date de fin doit etre posterieure a la date de debut.";
                return;
            }

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
                ok = _ctrl.ModifierAbsence(_personnel.IdPersonnel, _absenceOrigine.DateDebut, dateDebut, dateFin, idMotif, out erreur);
            else
                ok = _ctrl.AjouterAbsence(_personnel.IdPersonnel, dateDebut, dateFin, idMotif, out erreur);

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
    }
}
