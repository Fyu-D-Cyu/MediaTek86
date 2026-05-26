using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    public class FormAbsence : UserControl
    {
        private readonly Controller _ctrl = Controller.GetInstance();
        private bool _chargementPersonnel;

        private ComboBox cmbPersonnel;
        private TextBox txtRecherche;
        private ComboBox cmbTri;
        private CheckBox chkDesc;
        private DataGridView dgvAbsences;
        private Label lblResultat;

        public FormAbsence()
        {
            InitializeComponent();
            ChargerPersonnel();
            ChargerAbsences();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 246, 250);

            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.White,
                Padding = new Padding(12, 0, 12, 0)
            };
            pnlToolbar.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 225, 235)), 0,
                    pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            Label lblPersonnel = new Label
            {
                Text = "Personnel",
                Location = new Point(12, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };

            cmbPersonnel = new ComboBox
            {
                Location = new Point(82, 18),
                Size = new Size(210, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbPersonnel.SelectedIndexChanged += delegate { ChargerAbsences(); };

            Label lblRecherche = new Label
            {
                Text = "Recherche",
                Location = new Point(306, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };

            txtRecherche = new TextBox
            {
                Location = new Point(382, 18),
                Size = new Size(150, 26),
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtRecherche.TextChanged += delegate { ChargerAbsences(); };
            txtRecherche.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    ChargerAbsences();
            };

            Label lblTri = new Label
            {
                Text = "Trier par :",
                Location = new Point(546, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };

            cmbTri = new ComboBox
            {
                Location = new Point(620, 18),
                Size = new Size(115, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbTri.Items.AddRange(new object[] { "Date debut", "Date fin", "Motif" });
            cmbTri.SelectedIndex = 0;
            cmbTri.SelectedIndexChanged += delegate { ChargerAbsences(); };

            chkDesc = new CheckBox
            {
                Text = "Desc",
                Location = new Point(745, 21),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };
            chkDesc.CheckedChanged += delegate { ChargerAbsences(); };

            Button btnRechercher = CreerBouton("Actualiser", Color.FromArgb(26, 54, 93), 800, 16, 92, 30);
            btnRechercher.Click += delegate { ChargerAbsences(); };

            Button btnAjouter = CreerBouton("+ Ajouter", Color.FromArgb(34, 139, 87), 906, 16, 90, 30);
            Button btnModifier = CreerBouton("Modifier", Color.FromArgb(200, 130, 20), 1002, 16, 90, 30);
            Button btnSupprimer = CreerBouton("Supprimer", Color.FromArgb(180, 45, 45), 1098, 16, 100, 30);
            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;
            btnSupprimer.Click += BtnSupprimer_Click;

            pnlToolbar.Controls.AddRange(new Control[]
            {
                lblPersonnel, cmbPersonnel, lblRecherche, txtRecherche, lblTri, cmbTri, chkDesc,
                btnRechercher, btnAjouter, btnModifier, btnSupprimer
            });

            dgvAbsences = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                GridColor = Color.FromArgb(230, 233, 240),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f)
            };
            dgvAbsences.RowTemplate.Height = 34;
            dgvAbsences.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 54, 93);
            dgvAbsences.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAbsences.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvAbsences.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 54, 93);
            dgvAbsences.ColumnHeadersHeight = 36;
            dgvAbsences.EnableHeadersVisualStyles = false;
            dgvAbsences.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 252);
            dgvAbsences.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 54, 93);
            dgvAbsences.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvAbsences.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            dgvAbsences.CellDoubleClick += delegate { BtnModifier_Click(this, EventArgs.Empty); };

            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdPersonnel", HeaderText = "ID Personnel", Visible = false });
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "Personnel", HeaderText = "Personnel", FillWeight = 25 });
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "DateDebut", HeaderText = "Date debut", FillWeight = 16 });
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "DateFin", HeaderText = "Date fin", FillWeight = 16 });
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "Jours", HeaderText = "Jours", FillWeight = 8 });
            dgvAbsences.Columns.Add(new DataGridViewTextBoxColumn { Name = "Motif", HeaderText = "Motif", FillWeight = 35 });

            lblResultat = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 110, 130),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                BackColor = Color.FromArgb(240, 242, 248)
            };

            Panel pnlGrid = new Panel { Dock = DockStyle.Fill };
            pnlGrid.Controls.Add(dgvAbsences);

            Controls.Add(pnlGrid);
            Controls.Add(pnlToolbar);
            Controls.Add(lblResultat);
        }

        private void ChargerPersonnel()
        {
            _chargementPersonnel = true;
            List<Personnel> personnel = _ctrl.GetPersonnelFiltreeTrie();
            List<PersonnelOption> options = new List<PersonnelOption>();
            options.Add(new PersonnelOption(null));
            foreach (Personnel p in personnel)
                options.Add(new PersonnelOption(p));

            cmbPersonnel.DataSource = options;
            cmbPersonnel.DisplayMember = "Libelle";
            cmbPersonnel.ValueMember = "IdPersonnel";
            cmbPersonnel.Format += delegate(object sender, ListControlConvertEventArgs e)
            {
                PersonnelOption option = e.ListItem as PersonnelOption;
                if (option != null)
                    e.Value = option.Libelle;
            };
            _chargementPersonnel = false;
        }

        public void ChargerAbsences()
        {
            if (_chargementPersonnel || dgvAbsences == null)
                return;

            Personnel personnel = GetPersonnelSelectionne();
            dgvAbsences.Rows.Clear();

            string triPar;
            switch (cmbTri.SelectedIndex)
            {
                case 1:
                    triPar = "datefin";
                    break;
                case 2:
                    triPar = "motif";
                    break;
                default:
                    triPar = "datedebut";
                    break;
            }

            List<Absence> absences;
            if (personnel == null)
                absences = GetToutesAbsencesFiltreesTriees(txtRecherche.Text.Trim(), triPar, chkDesc.Checked);
            else
                absences = _ctrl.GetAbsencesFiltreesTries(
                    personnel.IdPersonnel,
                    txtRecherche.Text.Trim(),
                    triPar,
                    chkDesc.Checked);

            foreach (Absence a in absences)
            {
                dgvAbsences.Rows.Add(
                    a.IdPersonnel,
                    a.Personnel != null ? a.Personnel.NomComplet() : GetNomPersonnel(a.IdPersonnel),
                    a.DateDebut.ToString("dd/MM/yyyy"),
                    a.DateFin.ToString("dd/MM/yyyy"),
                    a.DureeJours(),
                    a.Motif != null ? a.Motif.Libelle : "-");
            }

            lblResultat.Text = personnel == null
                ? "  " + absences.Count + " absence(s) affichee(s) pour tous les personnels"
                : "  " + absences.Count + " absence(s) affichee(s) pour " + personnel.NomComplet();
        }

        private Personnel GetPersonnelSelectionne()
        {
            PersonnelOption option = cmbPersonnel.SelectedItem as PersonnelOption;
            return option != null ? option.Personnel : null;
        }

        private Absence GetAbsenceSelectionnee()
        {
            if (dgvAbsences.SelectedRows.Count == 0)
                return null;

            DataGridViewRow row = dgvAbsences.SelectedRows[0];
            int idPersonnel = Convert.ToInt32(row.Cells["IdPersonnel"].Value);
            DateTime dateDebut = DateTime.ParseExact(
                row.Cells["DateDebut"].Value.ToString(),
                "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            return _ctrl.GetAbsencesPersonnel(idPersonnel)
                .Find(a => a.DateDebut.Date == dateDebut.Date);
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            Personnel personnel = GetPersonnelSelectionne();
            if (personnel == null)
            {
                MessageBox.Show("Veuillez selectionner un personnel precis avant d'ajouter une absence.", "Selection requise",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FormEditAbsence form = new FormEditAbsence(personnel, null))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    ChargerAbsences();
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            Personnel personnel = GetPersonnelSelectionne();
            Absence absence = GetAbsenceSelectionnee();
            if (absence == null)
            {
                MessageBox.Show("Veuillez selectionner une absence.", "Aucune selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (personnel == null)
                personnel = _ctrl.GetAllPersonnel().Find(p => p.IdPersonnel == absence.IdPersonnel);

            using (FormEditAbsence form = new FormEditAbsence(personnel, absence))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    ChargerAbsences();
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            Personnel personnel = GetPersonnelSelectionne();
            Absence absence = GetAbsenceSelectionnee();
            if (absence == null)
            {
                MessageBox.Show("Veuillez selectionner une absence.", "Aucune selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (personnel == null)
                personnel = _ctrl.GetAllPersonnel().Find(p => p.IdPersonnel == absence.IdPersonnel);

            DialogResult confirm = MessageBox.Show(
                "Supprimer l'absence du " + absence.DateDebut.ToString("dd/MM/yyyy") +
                " au " + absence.DateFin.ToString("dd/MM/yyyy") + " ?",
                "Confirmer la suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            if (_ctrl.SupprimerAbsence(personnel.IdPersonnel, absence.DateDebut))
            {
                MessageBox.Show("Absence supprimee.", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerAbsences();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Absence> GetToutesAbsencesFiltreesTriees(string recherche, string triPar, bool desc)
        {
            List<Absence> absences = new List<Absence>();
            List<Personnel> personnels = _ctrl.GetAllPersonnel();

            foreach (Personnel p in personnels)
            {
                List<Absence> liste = _ctrl.GetAbsencesPersonnel(p.IdPersonnel);
                foreach (Absence absence in liste)
                {
                    absence.Personnel = p;
                    absences.Add(absence);
                }
            }

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                string r = recherche.ToLower();
                absences = absences.FindAll(a =>
                    (a.Personnel != null && a.Personnel.NomComplet().ToLower().Contains(r)) ||
                    (a.Motif != null && a.Motif.Libelle.ToLower().Contains(r)) ||
                    a.DateDebut.ToString("dd/MM/yyyy").Contains(r) ||
                    a.DateFin.ToString("dd/MM/yyyy").Contains(r));
            }

            switch (triPar)
            {
                case "datefin":
                    absences.Sort((a, b) => desc ? DateTime.Compare(b.DateFin, a.DateFin) : DateTime.Compare(a.DateFin, b.DateFin));
                    break;
                case "motif":
                    absences.Sort((a, b) => desc
                        ? string.Compare(GetMotif(a), GetMotif(b), StringComparison.OrdinalIgnoreCase) * -1
                        : string.Compare(GetMotif(a), GetMotif(b), StringComparison.OrdinalIgnoreCase));
                    break;
                default:
                    absences.Sort((a, b) => desc ? DateTime.Compare(b.DateDebut, a.DateDebut) : DateTime.Compare(a.DateDebut, b.DateDebut));
                    break;
            }

            return absences;
        }

        private string GetMotif(Absence absence)
        {
            return absence.Motif != null ? absence.Motif.Libelle : "";
        }

        private string GetNomPersonnel(int idPersonnel)
        {
            Personnel personnel = _ctrl.GetAllPersonnel().Find(p => p.IdPersonnel == idPersonnel);
            return personnel != null ? personnel.NomComplet() : "";
        }

        private class PersonnelOption
        {
            public PersonnelOption(Personnel personnel)
            {
                Personnel = personnel;
            }

            public Personnel Personnel { get; private set; }

            public int IdPersonnel
            {
                get { return Personnel != null ? Personnel.IdPersonnel : 0; }
            }

            public string Libelle
            {
                get { return Personnel != null ? Personnel.NomComplet() : "Tous les personnels"; }
            }
        }

        private Button CreerBouton(string texte, Color couleur, int x, int y, int w, int h)
        {
            Button btn = new Button
            {
                Text = texte,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = couleur,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += delegate { btn.BackColor = ControlPaint.Light(couleur, 0.2f); };
            btn.MouseLeave += delegate { btn.BackColor = couleur; };
            return btn;
        }
    }
}
