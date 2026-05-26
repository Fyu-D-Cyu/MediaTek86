using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;
using MediaTek86.modele;

namespace MediaTek86.vue
{
    public class FormPersonnel : UserControl
    {
        private readonly Controller _ctrl = Controller.GetInstance();

        private TextBox txtRecherche;
        private ComboBox cmbTri;
        private CheckBox chkDesc;
        private DataGridView dgvPersonnel;
        private Label lblResultat;

        public FormPersonnel()
        {
            InitializeComponent();
            ChargerPersonnel();
        }

        private void InitializeComponent()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 246, 250);

            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(12, 0, 12, 0)
            };
            pnlToolbar.Paint += delegate(object sender, PaintEventArgs e)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 225, 235)), 0,
                    pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
            };

            Label lblR = new Label
            {
                Text = "Recherche",
                Location = new Point(12, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };

            txtRecherche = new TextBox
            {
                Location = new Point(88, 16),
                Size = new Size(180, 26),
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtRecherche.TextChanged += delegate { ChargerPersonnel(); };
            txtRecherche.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                    ChargerPersonnel();
            };

            Label lblTri = new Label
            {
                Text = "Trier par :",
                Location = new Point(282, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };

            cmbTri = new ComboBox
            {
                Location = new Point(356, 16),
                Size = new Size(110, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbTri.Items.AddRange(new object[] { "Nom", "Prenom", "Service" });
            cmbTri.SelectedIndex = 0;
            cmbTri.SelectedIndexChanged += delegate { ChargerPersonnel(); };

            chkDesc = new CheckBox
            {
                Text = "Desc",
                Location = new Point(476, 19),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(80, 90, 110)
            };
            chkDesc.CheckedChanged += delegate { ChargerPersonnel(); };

            Button btnRechercher = CreerBouton("Actualiser", Color.FromArgb(26, 54, 93), 540, 14, 100, 28);
            btnRechercher.Click += delegate { ChargerPersonnel(); };

            Button btnAjouter = CreerBouton("+ Ajouter", Color.FromArgb(34, 139, 87), 654, 14, 100, 28);
            Button btnModifier = CreerBouton("Modifier", Color.FromArgb(200, 130, 20), 760, 14, 100, 28);
            Button btnSupprimer = CreerBouton("Supprimer", Color.FromArgb(180, 45, 45), 866, 14, 110, 28);
            btnAjouter.Click += BtnAjouter_Click;
            btnModifier.Click += BtnModifier_Click;
            btnSupprimer.Click += BtnSupprimer_Click;

            pnlToolbar.Controls.AddRange(new Control[]
                { lblR, txtRecherche, lblTri, cmbTri, chkDesc, btnRechercher, btnAjouter, btnModifier, btnSupprimer });

            dgvPersonnel = new DataGridView
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
            dgvPersonnel.RowTemplate.Height = 34;
            dgvPersonnel.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(26, 54, 93);
            dgvPersonnel.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPersonnel.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvPersonnel.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 54, 93);
            dgvPersonnel.ColumnHeadersHeight = 36;
            dgvPersonnel.EnableHeadersVisualStyles = false;
            dgvPersonnel.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 252);
            dgvPersonnel.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 54, 93);
            dgvPersonnel.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvPersonnel.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            dgvPersonnel.CellDoubleClick += delegate { BtnModifier_Click(this, EventArgs.Empty); };

            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdPersonnel", HeaderText = "ID", FillWeight = 5, Visible = false });
            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nom", HeaderText = "Nom", FillWeight = 15 });
            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "Prenom", HeaderText = "Prenom", FillWeight = 15 });
            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tel", HeaderText = "Telephone", FillWeight = 14 });
            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "Mail", HeaderText = "Email", FillWeight = 30 });
            dgvPersonnel.Columns.Add(new DataGridViewTextBoxColumn { Name = "Service", HeaderText = "Service", FillWeight = 21 });

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
            pnlGrid.Controls.Add(dgvPersonnel);

            Controls.Add(pnlGrid);
            Controls.Add(pnlToolbar);
            Controls.Add(lblResultat);
        }

        public void ChargerPersonnel()
        {
            string recherche = txtRecherche.Text.Trim();
            string triPar;
            switch (cmbTri.SelectedIndex)
            {
                case 1:
                    triPar = "prenom";
                    break;
                case 2:
                    triPar = "service";
                    break;
                default:
                    triPar = "nom";
                    break;
            }

            List<Personnel> liste = _ctrl.GetPersonnelFiltreeTrie(recherche, triPar, chkDesc.Checked);
            dgvPersonnel.Rows.Clear();
            foreach (Personnel p in liste)
            {
                dgvPersonnel.Rows.Add(
                    p.IdPersonnel,
                    p.Nom,
                    p.Prenom,
                    p.Tel,
                    p.Mail,
                    p.Service != null ? p.Service.Nom : "-");
            }
            lblResultat.Text = "  " + liste.Count + " personnel(s) affiche(s)";
        }

        private Personnel GetPersonnelSelectionne()
        {
            if (dgvPersonnel.SelectedRows.Count == 0)
                return null;

            DataGridViewRow row = dgvPersonnel.SelectedRows[0];
            int id = Convert.ToInt32(row.Cells["IdPersonnel"].Value);
            return _ctrl.GetAllPersonnel().Find(p => p.IdPersonnel == id);
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            using (FormEditPersonnel form = new FormEditPersonnel(null))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    ChargerPersonnel();
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            Personnel pers = GetPersonnelSelectionne();
            if (pers == null)
            {
                MessageBox.Show("Veuillez selectionner un personnel.", "Aucune selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FormEditPersonnel form = new FormEditPersonnel(pers))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    ChargerPersonnel();
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            Personnel pers = GetPersonnelSelectionne();
            if (pers == null)
            {
                MessageBox.Show("Veuillez selectionner un personnel.", "Aucune selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Supprimer definitivement " + pers.NomComplet() + " ?\n\nToutes ses absences seront egalement supprimees.",
                "Confirmer la suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            if (_ctrl.SupprimerPersonnel(pers.IdPersonnel))
            {
                MessageBox.Show("Personnel supprime.", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChargerPersonnel();
            }
            else
            {
                MessageBox.Show("Erreur lors de la suppression.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
