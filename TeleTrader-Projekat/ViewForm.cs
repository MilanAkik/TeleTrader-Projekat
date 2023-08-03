using DataAccess;
using Microsoft.EntityFrameworkCore;
using DataAccess;

namespace TeleTrader_Projekat
{
    public partial class ViewForm : Form
    {
        public ViewForm()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String description = "This is a project in the selection process for the position of Software Developer – C# at TeleTrader";
            String title = "About the app";
            MessageBox.Show(description, title);
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "SQLite files|*.sqlite; *.sqlite3; *.db; *.db3; *.s3db; *.sl3|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                await using var db = new SymbolContext(openFileDialog1.FileName);
                var results = from typ in db.type select typ;
                List<DataAccess.Type> types = new List<DataAccess.Type>();
                await foreach (var t in results.AsAsyncEnumerable())
                {
                    types.Add(t);
                }
                dataGridView1.DataSource = types;
            }
        }
    }
}