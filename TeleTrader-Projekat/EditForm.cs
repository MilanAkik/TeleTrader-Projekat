﻿using DataAccess;
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

        public enum EditAction { ADD, UPDATE }

        public EditAction editaction { get; set; }
        public String path { get; set; }
        public int Id { get; set; }

        public Action refresh;

        public EditForm(EditAction editaction, String path, int Id, Action refresh)
        {
            InitializeComponent();
            this.editaction = editaction;
            this.path = path;
            this.Id = Id;
            this.refresh = refresh;
        }

        private async void EditForm_Load(object sender, EventArgs e)
        {
            await using var db = new SymbolContext(path);
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
            if (this.editaction == EditAction.ADD) return;

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

        private async void button1_Click(object sender, EventArgs e)
        {
            refresh();
            this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            String message = "";
            if (textBox1.Text.Length == 0) message += "Name field is empty\n";
            if (textBox2.Text.Length == 0) message += "Ticker field is empty\n";
            if (textBox3.Text.Length == 0) message += "Isin field is empty\n";
            if (textBox4.Text.Length == 0) message += "Currency code field is empty\n";
            if (!IsValidDouble(textBox5.Text)) message += "Price field needs to be real number\n";
            if (message.Length > 0)
            {
                MessageBox.Show(message + "This needs to be fixed before saving to the database", "Validation warning");
                return;
            }
            await using var db = new SymbolContext(path);
            if (this.editaction == EditAction.UPDATE)
            {
                var results = from sym in db.symbol where sym.Id == Id select sym;
                var types = from typ in db.type select typ;
                var exchanges = from exc in db.exchange select exc;
                Symbol s = results.First();
                s.Name = textBox1.Text;
                s.Ticker = textBox2.Text;
                s.Isin = textBox3.Text;
                s.CurrencyCode = textBox4.Text;
                s.Price = Double.Parse(textBox5.Text);
                int i = 0;
                await foreach (var t in types.AsAsyncEnumerable()) if (i++ == comboBox1.SelectedIndex) s.TypeId = t.Id;
                i = 0;
                await foreach (var ex in exchanges.AsAsyncEnumerable()) if (i++ == comboBox2.SelectedIndex) s.ExchangeId = ex.Id;
                db.Update(s);
                db.SaveChanges();
                refresh();
                this.Close();
            }
            else
            {
                var types = from typ in db.type select typ;
                var exchanges = from exc in db.exchange select exc;
                Symbol s = new Symbol
                {
                    Name = textBox1.Text,
                    Ticker = textBox2.Text,
                    Isin = textBox3.Text,
                    CurrencyCode = textBox4.Text,
                    Price = Double.Parse(textBox5.Text),
                    TypeId = -1,
                    ExchangeId = -1,
                    DateAdded = DateTime.Now,
                    PriceDate = DateTime.Now,
                };
                int i = 0;
                await foreach (var t in types.AsAsyncEnumerable())
                {
                    if (i++ == comboBox1.SelectedIndex)
                    {
                        s.TypeId = t.Id;
                        s.Type = t;
                    }
                }
                i = 0;
                await foreach (var ex in exchanges.AsAsyncEnumerable())
                {
                    if (i++ == comboBox2.SelectedIndex)
                    {
                        s.ExchangeId = ex.Id;
                        s.Exchange = ex;
                    }
                }
                db.symbol.Add(s);
                db.SaveChanges();
                refresh();
                this.Close();
            }
        }

        public static bool IsValidDouble(string ValueToTest)
        {
            return double.TryParse(ValueToTest, out double d) &&
                   !(double.IsNaN(d) || double.IsInfinity(d));
        }

    }
}
