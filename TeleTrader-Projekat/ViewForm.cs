using DataAccess;
using Microsoft.EntityFrameworkCore;

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

        private async void ViewForm_Load(object sender, EventArgs e)
        {
            await using var db = new SymbolContext();
            var results = from typ in db.type select typ;
            String s = "Types";
            await foreach( var t in results.AsAsyncEnumerable())
            {
                s += "\n" + t.Name;
            }
            MessageBox.Show(s);
        }
    }
}