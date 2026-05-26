using System;
using System.Windows.Forms;
using MediaTek86.vue;

namespace MediaTek86
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (FormLogin login = new FormLogin())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new FormMain());
        }
    }
}
