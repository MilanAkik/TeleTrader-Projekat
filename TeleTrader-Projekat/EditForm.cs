using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeleTrader_Projekat
{
    public partial class EditForm : Form
    {

        public enum Action { ADD, UPDATE }

        public Action action { get; set; }
        public SymbolContext db { get; set; }
        public int Id { get; set; }

        public EditForm(Action action, SymbolContext db, int Id)
        {
            InitializeComponent();
            this.action = action;
            this.db = db;
            this.Id = Id;
        }

        private async void EditForm_Load(object sender, EventArgs e)
        {
            var results = from typ in db.type select typ;
            comboBox1.Items.Clear();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Id";
            List<DataAccess.Type> types = new List<DataAccess.Type>();
            await foreach (var t in results.AsAsyncEnumerable())
            {
                comboBox1.Items.Add(t);
                types.Add(t);
            }
            comboBox1.SelectedIndex = 0;

            var exchanges = from ex in db.exchange select ex;
            comboBox2.Items.Clear();
            comboBox2.DisplayMember = "Name";
            comboBox2.ValueMember = "Id";
            List<Exchange> exc = new List<Exchange>();
            await foreach (var ex in exchanges.AsAsyncEnumerable())
            {
                comboBox2.Items.Add(ex);
                exc.Add(ex);
            }
            comboBox2.SelectedIndex = 0;
            if (this.action == Action.ADD) return;

            var symbols = from s in db.symbol where s.Id == this.Id select s;
            await foreach (var s in symbols.AsAsyncEnumerable())
            {
                textBox1.Text = s.Name;
                textBox2.Text = s.Ticker;
                textBox3.Text = s.Isin;
                textBox4.Text = s.CurrencyCode;
                textBox5.Text = s.Price.ToString();
                comboBox1.SelectedIndex = types.FindIndex(x => x.Id == s.TypeId);
                comboBox2.SelectedIndex = exc.FindIndex(x => x.Id == s.ExchangeId);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
