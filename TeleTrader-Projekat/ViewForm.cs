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
                comboBox1.Items.Clear();
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Id";
                DataAccess.Type all = new DataAccess.Type { Id = -1, Name = "All" };
                comboBox1.Items.Add(all);
                await foreach (var t in results.AsAsyncEnumerable()) comboBox1.Items.Add(t);
                comboBox1.SelectedIndex = 0;

                var exchanges = from ex in db.exchange select ex;
                comboBox2.Items.Clear();
                comboBox2.DisplayMember = "Name";
                comboBox2.ValueMember = "Id";
                Exchange all_e = new Exchange { Id = -1, Name = "All" };
                comboBox2.Items.Add(all_e);
                await foreach (var ex in exchanges.AsAsyncEnumerable()) comboBox2.Items.Add(ex);
                comboBox2.SelectedIndex = 0;

                List<Symbol> list_s = new List<Symbol>();
                var symbols = from s in db.symbol select s;
                await foreach (var s in symbols.AsAsyncEnumerable()) list_s.Add(s);
                dataGridView1.DataSource = list_s;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataAccess.Type t = comboBox1.SelectedItem as DataAccess.Type;
            this.type = t;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Exchange ex = comboBox2.SelectedItem as Exchange;
            this.exchange = ex;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            reloadFiltered();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await using var db = new SymbolContext(openFileDialog1.FileName);
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete!", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    DataGridViewRow sel = dataGridView1.SelectedRows[0];
                    int i = (int)sel.Cells[0].Value;
                    db.Remove(db.symbol.Single(x => x.Id == i));
                    db.SaveChanges();
                    reloadFiltered();
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            EditForm ef = new EditForm(EditForm.EditAction.ADD, openFileDialog1.FileName, -1, reloadFiltered);
            ef.Show();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                EditForm ef = new EditForm(EditForm.EditAction.UPDATE, openFileDialog1.FileName, (int)dataGridView1.SelectedRows[0].Cells[0].Value, reloadFiltered);
                ef.Show();
            }
        }

        private async void reloadFiltered()
        {
            List<Symbol> list_s = new List<Symbol>();
            await using SymbolContext db = new SymbolContext(openFileDialog1.FileName);
            if (this.exchange.Id == -1)
            {
                if (this.type.Id == -1)
                {
                    var symbols = from s in db.symbol select s;
                    await foreach (var s in symbols.AsAsyncEnumerable()) list_s.Add(s);
                }
                else
                {
                    var symbols = from s in db.symbol where s.TypeId == type.Id select s;
                    await foreach (var s in symbols.AsAsyncEnumerable()) list_s.Add(s);
                }
            }
            else
            {
                if (this.type.Id == -1)
                {
                    var symbols = from s in db.symbol where s.ExchangeId == exchange.Id select s;
                    await foreach (var s in symbols.AsAsyncEnumerable()) list_s.Add(s);
                }
                else
                {
                    var symbols = from s in db.symbol where (s.TypeId == type.Id) && (s.ExchangeId == exchange.Id) select s;
                    await foreach (var s in symbols.AsAsyncEnumerable()) list_s.Add(s);
                }
            }
            dataGridView1.DataSource = list_s;
        }

        private DataAccess.Type type = new DataAccess.Type { Name = "All", Id = -1 };
        private Exchange exchange = new Exchange { Name = "All", Id = -1 };

    }
}