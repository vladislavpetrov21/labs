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
            string ConnectionString = @"Data Source=" + sPath +
           ";New=False;Version=3";
            String selectCommand5 = "Select id, Name, Price from Materials";
            comboBoxColumn(ConnectionString, selectCommand5, "id",
           "Name", "SubDt1", "SubDt1");
            String selectCommand6 = "Select id, Name from Subdivisions";
        comboBoxColumn(ConnectionString, selectCommand6,
                "id", "Name", "SubDt2", "SubDt2");
        String selectCommand7 = "Select id, Name from MOL";
            comboBoxColumn(ConnectionString, selectCommand7, "id",
           "Name", "SubDt3", "SubDt3");
            dataGridView1.ColumnHeadersVisible = false;
        }
        // перед новым заполнение очищаем таблицу
        public void Clear(DataGridView dataGridView)
        {
            while (dataGridView.Rows.Count > 1)
                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                    dataGridView.Rows.Remove(dataGridView.Rows[i]);
        }
        // настраиваем поля в таблице для вывода
        public void comboBoxColumn(string ConnectionString, String
       selectCommand, string valueMember, string displayMember, string headerText,
       string dataPropertyName)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectCommand, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            BindingSource licorgSource = new BindingSource();
            licorgSource.DataSource = ds;
            licorgSource.DataMember = ds.Tables[0].ToString();
            connect.Close();
            DataGridViewComboBoxColumn comboColumn = new
           DataGridViewComboBoxColumn();
            comboColumn.DisplayStyle =
       DataGridViewComboBoxDisplayStyle.Nothing;
            comboColumn.FlatStyle = FlatStyle.Popup;
            comboColumn.HeaderText = headerText;
            comboColumn.DataSource = licorgSource;
            comboColumn.DataPropertyName = dataPropertyName;
            comboColumn.DisplayMember = displayMember;
            comboColumn.ValueMember = valueMember;
            dataGridView1.Columns.Add(comboColumn);
        }
        //выбираем источник данных для заполнения таблицы
        public void selectTable(string ConnectionString, String selectdate)
        {
            SQLiteConnection connect = new
           SQLiteConnection(ConnectionString);
            connect.Open();
            SQLiteDataAdapter dataAdapter = new
           SQLiteDataAdapter(selectdate, connect);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = ds.Tables[0].ToString();
            connect.Close();
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.Columns["SubDt1"].DisplayIndex = 0;
            dataGridView1.Columns["SubDt2"].DisplayIndex = 1;
            dataGridView1.Columns["SubDt3"].DisplayIndex = 2;
            dataGridView1.Columns["Count"].DisplayIndex = 3;
            dataGridView1.Columns["Summ"].DisplayIndex = 4;
        }
        //выполняем команду для запроса
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
        //обрабатываем нажатие на кнопку «Сформировать»
        private void buttonOk_Click(object sender, EventArgs e)
        {
            Clear(dataGridView1);
            string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            if (maskedTextBox1.Text != "")
            {
                string selectdate = "select SubDt1, SubDt2, SubDt3, Count, Summ from JournalOfProvodki where Date >= '" + maskedTextBox1.Text + "' and Date <= '" + maskedTextBox2.Text + "'";
                selectTable(ConnectionString, selectdate);
                dataGridView1.Columns[0].HeaderCell.Value = "Материал";
                dataGridView1.Columns[1].HeaderCell.Value = "Подразделение";
                dataGridView1.Columns[2].HeaderCell.Value = "МОЛ";
                dataGridView1.Columns[3].HeaderCell.Value = "Количество";
                dataGridView1.Columns[4].HeaderCell.Value = "Сумма";
                string selectSum = "select SUM (Summ) from JournalOfProvodki where Date = '" + maskedTextBox1.Text + "'";
                object sum = selectValue(ConnectionString, selectSum);
            }
            else
            {
            MessageBox.Show("Введите дату отчета");
            }
        }
    }
}
