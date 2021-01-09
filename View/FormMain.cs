using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;

namespace View
{
    public partial class FormMain : Form
    {
        [Dependency]
        public new IUnityContainer Container { get; set; }
        public FormMain()
        {
            InitializeComponent();
        }

        private void планСчетовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<ChartOfAccounts>();
            form.ShowDialog();
        }

        private void мОЛToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<MOL>();
            form.ShowDialog();
        }
       
        private void складToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Sklad>();
            form.ShowDialog();
        }

        private void поставщикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Providers>();
            form.ShowDialog();
        }

        private void материалыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Materials>();
            form.ShowDialog();
        }

        private void подразделениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Subdivisions>();
            form.ShowDialog();
        }

        private void журналОперацийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Otpusk>();
            form.ShowDialog();
        }

        private void отчетыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<Report>();
            form.ShowDialog();
        }
    }
}
