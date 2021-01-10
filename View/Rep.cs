using System;
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
    public partial class Rep : Form
    {
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private string sPath = Path.Combine(Application.StartupPath, "C://Users//par55//AppData//Local//Temp//Rar$EXa1416.20756/SQLiteStudio/basa2");
        public Rep()
        {
            InitializeComponent();
        }
        private void Rep_Load(object sender, EventArgs e)
        {
            // вместо кодов отображаем наименование полей
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            String selectStorageRec = "SELECT ID, Name FROM Storage";
           // selectCombo(ConnectionString, selectStorageRec, comboBox1, "Name", "ID");
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
          /*  Clear(dataGridView1);
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if (maskedTextBox1.Text != " - -")
            {
                string report = "select Material.ID As 'Код Материала', A.SubkontoDebit1 As 'Название материала'," +
                " Material.Cost As 'Цена материала',((SELECT SUM(Count) " +
                "FROM PostingJournal A WHERE A.SubkontoDebit2 = '" + comboBox1.Text + "' and " +
                "Material.Name = A.SubkontoDebit1 GROUP BY A.SubkontoDebit1) " +
                "- IFNULL((SELECT SUM(Count) FROM PostingJournal A WHERE A.SubkontoKredit2 = '" + comboBox1.Text + "' and" +
                " Material.Name = A.SubkontoKredit1 GROUP BY A.SubkontoKredit1),0))" +
                " AS 'Количество остатка', ((SELECT SUM(Count) FROM PostingJournal A WHERE A.SubkontoDebit2 = '" + comboBox1.Text + "' and " +
                "Material.Name = A.SubkontoDebit1 GROUP BY A.SubkontoDebit1) " +
                "- IFNULL((SELECT SUM(Count) FROM PostingJournal A WHERE A.SubkontoKredit2 = '" + comboBox1.Text + "' and" +
                " Material.Name = A.SubkontoKredit1 GROUP BY A.SubkontoKredit1),0)) * Material.Cost AS 'Сумма остатка'" +
                " FROM Material JOIN PostingJournal A ON " +
                "(A.SubkontoDebit2 = '" + comboBox1.Text + "') and" +
                " Material.Name = A.SubkontoDebit1 and A.date <= '" + maskedTextBox1.Text + "' GROUP BY A.SubkontoDebit1";
                selectTable(ConnectionString, report);
                int sum = 0;
                for (int i = 0; i < Convert.ToInt32(dataGridView1.RowCount); ++i)
                {
                    sum += Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                }
                string sumT = sum.ToString();
                label2.Text = "Итого:" + sumT;
            }
            else
            {
                MessageBox.Show("Введите дату отчета");
            }*/
        }
    }
}
