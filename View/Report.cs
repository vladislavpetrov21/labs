﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    public partial class Report : Form
    {
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = Path.Combine(Application.StartupPath, "C://Users//par55//AppData//Local//Temp//Rar$EXa1416.20756/SQLiteStudio/basa2");
        public Report()
        {
            InitializeComponent();
        }
        private void Report_Load(object sender, EventArgs e)
        {
            // вместо кодов отображаем наименование полей
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectSub = "SELECT id, Name FROM Subdivisions";
            selectCombo(ConnectionString, selectSub, comboBox1, "Name", "id");
        }
        public void Clear(DataGridView dataGridView)
        {
            while (dataGridView.Rows.Count > 1)
                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    dataGridView.Rows.Remove(dataGridView.Rows[i]);
        }
        public void comboBoxColumn(string ConnectionString, String selectCommand, string valueMember, string displayMember, string headerText, string dataPropertyName)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            BindingSource licorgSource = new BindingSource();
            licorgSource.DataSource = ds;
            licorgSource.DataMember = ds.Tables[0].ToString();
            connect.Close();
            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            comboColumn.FlatStyle = FlatStyle.Popup;
            comboColumn.HeaderText = headerText;
            comboColumn.DataSource = licorgSource;
            comboColumn.DataPropertyName = dataPropertyName;
            comboColumn.DisplayMember = displayMember;
            comboColumn.ValueMember = valueMember;
            dataGridView1.Columns.Add(comboColumn);
        }
        public void selectCombo(string ConnectionString, String selectCommand, ComboBox comboBox, string displayMember, string valueMember)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            comboBox.DataSource = ds.Tables[0];
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
            connect.Close();
        }
        public void selectTable(string ConnectionString, String selectdate)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectdate, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].ToString();
            connect.Close();
            dataGridView1.ColumnHeadersVisible = true;
        }
        public object selectValue(string ConnectionString, String selectCommand)
        {
            SQLiteConnection connect = new SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteCommand command = new SQLiteCommand(selectCommand, connect);
            SQLiteDataReader reader = command.ExecuteReader();
            object value = "";
            while (reader.Read())
            {
                value = reader[0];
            }
            connect.Close();
            return value;
        }
        private void buttonOk_Click(object sender, EventArgs e)
        {
            Clear(dataGridView1);
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if (maskedTextBox1.Text != " - -" && maskedTextBox2.Text != " - -")
            {
                string report = "select JournalOfOperations.Material, Materials.Name, Materials.Price, JournalOfOperations.Count, JournalOfOperations.Summ " +
                "from JournalOfOperations, Materials, Subdivisions where (JournalOfOperations.Date >= '" + maskedTextBox1.Text + "' " +
                "and JournalOfOperations.Date <= '" + maskedTextBox2.Text + "') and Materials.id = JournalOfOperations.Material and Subdivisions.Name = '" + comboBox1.Text + "'" + "and JournalOfOperations.Subdivisions = Subdivisions.id" +
                " GROUP BY JournalOfOperations.id";
                selectTable(ConnectionString, report);
                dataGridView1.Columns[0].HeaderCell.Value = "Код материала";
                dataGridView1.Columns[1].HeaderCell.Value = "Название материала";
                dataGridView1.Columns[2].HeaderCell.Value = "Цена";
                dataGridView1.Columns[3].HeaderCell.Value = "Количество";
                dataGridView1.Columns[4].HeaderCell.Value = "Сумма";
                int sum = 0;
                for (int i = 0; i < Convert.ToInt32(dataGridView1.RowCount); ++i)
                {
                    sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                }
                string sumT = sum.ToString();
                label2.Text = "Итого: " + sumT;
            }
            else
            {
                MessageBox.Show("Введите дату отчета");
            }
        }
    }
}
