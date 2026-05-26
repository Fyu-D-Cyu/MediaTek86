using System;
using System.Drawing;
using System.Windows.Forms;
using MediaTek86.controller;

namespace MediaTek86.vue
{
    public class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "MediaTek86 - Gestion Personnel et Absences";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(980, 600);
            Font = new Font("Segoe UI", 9.5f);

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(26, 54, 93)
            };

            Label lblTitle = new Label
            {
                Text = "MediaTek86",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                Location = new Point(18, 14),
                AutoSize = true
            };

            Label lblUser = new Label
            {
                Text = "Connecte : " + Controller.GetInstance().LoginConnecte,
                ForeColor = Color.FromArgb(190, 210, 235),
                Font = new Font("Segoe UI", 9f),
                Dock = DockStyle.Right,
                Width = 220,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblUser);

            TabControl tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f)
            };

            TabPage tabPersonnel = new TabPage("Personnel");
            tabPersonnel.Controls.Add(new FormPersonnel());

            TabPage tabAbsences = new TabPage("Absences");
            tabAbsences.Controls.Add(new FormAbsence());

            tabs.TabPages.Add(tabPersonnel);
            tabs.TabPages.Add(tabAbsences);

            Controls.Add(tabs);
            Controls.Add(pnlHeader);
            FormClosed += delegate { Controller.GetInstance().Deconnecter(); };
        }
    }
}
