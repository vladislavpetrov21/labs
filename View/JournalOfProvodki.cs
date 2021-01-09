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
    public partial class JournalOfProvodki : Form
    {
		private string selectTableJourn = "";
		private SQLiteConnection sql_con;
		private SQLiteCommand sql_cmd;
		private DataSet DS = new DataSet();
		private DataTable DT = new DataTable();
		private string sPath = Path.Combine(Application.StartupPath, "C://Users//par55//AppData//Local//Temp//Rar$EXa1416.20756/SQLiteStudio/basa2");
		public JournalOfProvodki(int cur)
		{
			InitializeComponent();
			if (cur != -123)
			{
				selectTableJourn = "Select JournalOfProvodki.id AS '№ перемещения'," +
					" D.NumberOfAccount AS 'Дт', JournalOfProvodki.SubDt1 AS 'СубконтоДт1', " +
					"JournalOfProvodki.SubDt2 AS 'СубконтоДт2'," +
					" JournalOfProvodki.SubDt3 AS 'СубконтоДт3', " +
					" K.NumberOfAccount AS 'Кт', JournalOfProvodki.SubKt1 AS 'СубконтоКт1', " +
					"JournalOfProvodki.SubKt2 AS 'СубконтоКт2'," +
					" JournalOfProvodki.SubKt3 AS 'СубконтоКт3', JournalOfProvodki.Count AS 'Количество'," +
					"JournalOfProvodki.Summ AS 'Сумма', JournalOfProvodki.Date AS 'Дата', JournalOfProvodki.JournalOfOperations AS 'Журнал операций'" +
					" from JournalOfProvodki, ChartOfAccounts D, ChartOfAccounts K " +
					"Where D.id = JournalOfProvodki.DebitAccount and K.id = JournalOfProvodki.KreditAccount and JournalOfProvodki.JournalOfOperations = '" + cur + "'";
			}
			DateTime newDate;
			newDate = DateTime.Today;
			maskedTextBox1.Text = newDate.ToString("yyyy.MM.dd");
		}

		private void JournalOfProvodki_Load(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			selectTable(ConnectionString);
			// выбрать значения из справочников для отображения в comboBox  
			String selectMaterial = "SELECT id, Name, Price FROM Materials";
			selectCombo(ConnectionString, selectMaterial, comboBoxMaterial, "Name", "id");
			String selectMOLRec = "SELECT id, Name FROM MOL";
			selectCombo(ConnectionString, selectMOLRec, comboBoxMOL, "Name", "id");
			String selectStorageRec = "SELECT id, Name FROM Sklad";
			selectCombo(ConnectionString, selectStorageRec, comboBoxStorage, "Name", "id");
			String selectProvider = "SELECT id, Name FROM Providers";
			selectCombo(ConnectionString, selectProvider, comboBoxProvider, "Name", "id");
			comboBoxMaterial.SelectedIndex = 0;
			comboBoxMOL.SelectedIndex = 0;
			comboBoxStorage.SelectedIndex = 0;
			comboBoxProvider.SelectedIndex = 0;
			textBoxCount.Text = "0";
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			// находим максимальное значение кода проводок для записи первичного ключа 
			String mValue = "select MAX(id) from JournalOfProvodki";
			object maxValue = selectValue(ConnectionString, mValue);
			if (Convert.ToString(maxValue) == "")
				maxValue = 0;
			// Обнулить значения переменных 
			string count = "0";
			string Value1 = null;
			string Value2 = null;
			string Value3 = null;
			if (comboBoxMaterial.Text != "")
			{
				Value1 = comboBoxMaterial.SelectedValue.ToString();
			}
			if (comboBoxMOL.Text != "")
			{
				Value2 = comboBoxMOL.SelectedValue.ToString();
			}
			if (comboBoxStorage.Text != "")
			{
				Value3 = comboBoxStorage.SelectedValue.ToString();
			}
			if (textBoxCount.Text != "")
			{
				count = textBoxCount.Text;
			}
			String selectDT = "select id from ChartOfAccounts where NumberOfAccount='10.'";
			object DT = selectValue(ConnectionString, selectDT);
			String selectKT = "select id from ChartOfAccounts where NumberOfAccount='10.'";
			object KT = selectValue(ConnectionString, selectKT);
			String selectCost = "select Price from Materials where id ='" + Value1 + "'";
			object cost = selectValue(ConnectionString, selectCost);
			double Summa = Convert.ToDouble(cost) * Convert.ToInt32(count);
			string add = "INSERT INTO JournalOfProvodki (id, DebitAccount, SubDt1, SubDt2, " +
				"SubDt3, KreditAccount, SubKt1, Count, Summ, Date, JournalOfOperations) " +
				"VALUES (" + (Convert.ToInt32(maxValue) + 1) + ",'" + DT.ToString() + "','" + comboBoxMaterial.Text
				+ "','" + comboBoxStorage.Text + "','" + comboBoxMOL.Text + "','" + KT.ToString() + "','" +
				comboBoxProvider.Text + "','" + textBoxCount.Text + "','" + Summa.ToString() + "','" + maskedTextBox1.Text + "','0')";
			ExecuteQuery(add);
			selectTable(ConnectionString);
		}
		private void ExecuteQuery(string txtQuery)
		{
			sql_con = new SQLiteConnection("Data Source=" + sPath + ";Version=3;New=False;Compress=True;");
			sql_con.Open();
			sql_cmd = sql_con.CreateCommand();
			sql_cmd.CommandText = txtQuery;
			sql_cmd.ExecuteNonQuery();
			sql_con.Close();
		}

		public void selectTable(string ConnectionString)
		{
			try
			{
				SQLiteConnection connect = new SQLiteConnection(ConnectionString);
				connect.Open();
				SQLiteDataAdapter dataAdapter;
				if (selectTableJourn == "")
				{
					dataAdapter = new SQLiteDataAdapter("Select JournalOfProvodki.id AS '№ отпуска'," +
					 " D.NumberOfAccount AS 'Дт', JournalOfProvodki.SubDt1 AS 'СубконтоДт1', " +
					"JournalOfProvodki.SubDt2 AS 'СубконтоДт2'," +
					"JournalOfProvodki.SubDt3 AS 'СубконтоДт3', " + 
					"K.NumberOfAccount AS 'Кт'," +
					"JournalOfProvodki.SubKt1 AS 'СубконтоКт1', " +
					"JournalOfProvodki.SubKt2 AS 'СубконтоКт2'," +
					"JournalOfProvodki.SubKt3 AS 'СубконтоКт3', JournalOfProvodki.Count AS 'Количество'," +
					"JournalOfProvodki.Summ AS 'Сумма',JournalOfProvodki.Date AS 'Дата'," +
					"JournalOfProvodki.JournalOfOperations AS 'Журнал операций'" +
					" from JournalOfProvodki, ChartOfAccounts D, ChartOfAccounts K " +
					"Where D.id = JournalOfProvodki.DebitAccount and K.id = JournalOfProvodki.KreditAccount", connect);
				}
				else
				{
					dataAdapter = new SQLiteDataAdapter(selectTableJourn, connect);
					selectTableJourn = "";
				}
				DataSet ds = new DataSet();
				dataAdapter.Fill(ds);
				dataGridView1.DataSource = ds;
				dataGridView1.DataMember = ds.Tables[0].ToString();
				connect.Close();			
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Oшибка");
			}
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
		public void changeValue(string ConnectionString, String selectCommand)
		{
			SQLiteConnection connect = new SQLiteConnection(ConnectionString);
			connect.Open();
			SQLiteTransaction trans;
			SQLiteCommand cmd = new SQLiteCommand();
			trans = connect.BeginTransaction();
			cmd.Connection = connect;
			cmd.CommandText = selectCommand;
			cmd.ExecuteNonQuery();
			trans.Commit();
			connect.Close();
		}
		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			selectTable(ConnectionString);
		}
	}
}
