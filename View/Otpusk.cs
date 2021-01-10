using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;

namespace View
{
    public partial class Otpusk : Form
    {
		private SQLiteConnection sql_con;
		private SQLiteCommand sql_cmd;
		private DataSet DS = new DataSet();
		private DataTable DT = new DataTable();
		private string sPath = Path.Combine(Application.StartupPath, "C://Users//par55//AppData//Local//Temp//Rar$EXa1416.20756/SQLiteStudio/basa2");
		public Otpusk()
		{
			InitializeComponent();
			DateTime newDate;
			newDate = DateTime.Today;
			maskedTextBox1.Text = newDate.ToString("yyyy.MM.dd");
		}
		private void Otpusk_Load(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			selectTable(ConnectionString);
			// выбрать значения из справочников для отображения в comboBox  
			String selectMaterial = "SELECT id, Name, Price FROM Materials";
			selectCombo(ConnectionString, selectMaterial, comboBoxMaterial, "Name", "id");
			String selectSub = "SELECT id, Name FROM Subdivisions";
			selectCombo(ConnectionString, selectSub, comboBoxSub, "Name", "id");
			String selectMOLRec = "SELECT id, Name FROM MOL";
			selectCombo(ConnectionString, selectMOLRec, comboBoxMOL, "Name", "id");			
			String selectStorageRec = "SELECT id, Name FROM Sklad";
			selectCombo(ConnectionString, selectStorageRec, comboBoxStorage, "Name", "id");			
			String mValue = "select MAX(id) from JournalOfOperations";
			object maxValue = selectValue(ConnectionString, mValue);
			if (Convert.ToString(maxValue) == "")
				maxValue = 0;
			textBoxNum.Text = (Convert.ToInt32(maxValue) + 1).ToString();
			comboBoxMaterial.SelectedIndex = 0;
			comboBoxMOL.SelectedIndex = -1;
			comboBoxStorage.SelectedIndex = -1;
			textBoxCount.Text = "0";
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			// находим максимальное значение кода проводок для записи первичного ключа 
			String mValue = "select MAX(id) from JournalOfOperations";
			object maxValue = selectValue(ConnectionString, mValue);
			String mValuej = "select MAX(id) from JournalOfProvodki";
			object maxValuej = selectValue(ConnectionString, mValuej);
			if (Convert.ToString(maxValue) == "")
				maxValue = 0;
			// Обнулить значения переменных 
			string sum = "0";
			string count = "0";
			string Value1 = null;
			string Value2 = null;
			string Value3 = null;
			string Value4 = null;
			if (comboBoxMaterial.Text != "")
			{
				//ОС 
				Value1 = comboBoxMaterial.SelectedValue.ToString();
			}
			if (comboBoxMOL.Text != "")
			{
				//Подразделение 
				Value2 = comboBoxMOL.SelectedValue.ToString();
			}
			if (comboBoxSub.Text != "")
			{
				//Подразделение 
				Value3 = comboBoxSub.SelectedValue.ToString();
			}
			if (comboBoxStorage.Text != "")
			{
				//Подразделение 
				Value4 = comboBoxStorage.SelectedValue.ToString();
			}
			//Поле количество 
			if (textBoxCount.Text != "")
			{
				count = textBoxCount.Text;
			}
			if (textBoxCount.Text == "0")
			{
				MessageBox.Show("Количество материалов не может быть 0!");
				return;
			}
			String selectCost = "select Price from Materials where id ='" + Value1 + "'";
			object cost = selectValue(ConnectionString, selectCost);
			double Summa = Convert.ToDouble(cost) * Convert.ToInt32(count);
			String selectDT = "select id from ChartOfAccounts where NumberOfAccount='10.'";
			object DT = selectValue(ConnectionString, selectDT);
			String selectKT = "select id from ChartOfAccounts where NumberOfAccount='60.'";
			object KT = selectValue(ConnectionString, selectKT);
			string selectcount = "select SUM(count) from JournalOfProvodki where DebitAccount = '" + DT.ToString()
				+ "' and SubDt1 = '"
				+ comboBoxMaterial.Text + "' and SubDt2 = '" + comboBoxStorage.Text + "' and SubDt3 = '"
				+ comboBoxMOL.Text + "'";
			object countMat = selectValue(ConnectionString, selectcount);
			selectcount = "select SUM(count) from JournalOfProvodki where DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + DT.ToString() + "' and SubKt1 = '"
				+ comboBoxMaterial.Text + "' and SubKt2 = '" + comboBoxStorage.Text + "' and SubKt3 = '"
				+ comboBoxMOL.Text + "'";
			string selectDate = "select COUNT(*) from JournalOfProvodki where ((DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + DT.ToString() + "' and SubKt1 = '"
				+ comboBoxMaterial.Text + "' and SubKt2 = '" + comboBoxStorage.Text + "' and SubKt3 = '"
				+ comboBoxMOL.Text + "') or (DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + KT.ToString()
				+ "' and SubDt1 = '"
				+ comboBoxMaterial.Text + "' and SubDt2 = '" + comboBoxStorage.Text + "' and SubDt3 = '"
				+ comboBoxMOL.Text + "')) and Date > '" + maskedTextBox1.Text + "'";
			object countDate = selectValue(ConnectionString, selectDate);
			if (countDate.ToString() != "0")
			{
				MessageBox.Show("Неверная дата! Есть документ с более поздней датой");
				return;
			}
			if (countMat.ToString() == "")
			{
				countMat = "0";
			}
			object countMater = selectValue(ConnectionString, selectcount);
			if (countMater.ToString() == "")
			{
				if (Convert.ToInt32(countMat) < Convert.ToInt32(count))
				{
					MessageBox.Show("Недостаточно материалов! На складе осталось " + countMat + " единиц");
					return;
				}
			}
			else if (Convert.ToInt32(countMat) - Convert.ToInt32(countMater) < Convert.ToInt32(count))
			{
				MessageBox.Show("Недостаточно материалов! На складе осталось " + (Convert.ToInt32(countMat) - Convert.ToInt32(countMater)) + " единиц");
				return;
			}
			string add = "INSERT INTO JournalOfOperations (id, Date, Summ, Count, MOL, Sklad, Material, Subdivisions) " +
				"VALUES (" + (Convert.ToInt32(maxValue) + 1) + ",'" + maskedTextBox1.Text + "','" + Summa.ToString() + "','" + textBoxCount.Text + "','" + Convert.ToInt32(Value2) + "','" + Convert.ToInt32(Value4) + "','" +
				Convert.ToInt32(Value1) + "','" + Convert.ToInt32(Value3) + "')";
			string addjourn = "INSERT INTO JournalOfProvodki (id, DebitAccount, SubDt1, SubDt2, " +
				"SubDt3, KreditAccount, SubKt1, Count, Summ, Date, JournalOfOperations) " +
				"VALUES (" + (Convert.ToInt32(maxValuej) + 1) + ",'" + DT.ToString() + "','" + comboBoxMaterial.Text
				+ "','" + comboBoxStorage.Text + "','" + comboBoxMOL.Text + "','" + KT.ToString() + "','" +
				comboBoxStorage.Text + "','" + textBoxCount.Text + "','" + Summa.ToString() + "','" + maskedTextBox1.Text + "'," + (Convert.ToInt32(maxValue) + 1) + ")";
			ExecuteQuery(add);
			ExecuteQuery(addjourn);
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
				SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter("Select JournalOfOperations.id AS '№ отпуска', JournalOfOperations.Date AS 'Дата', JournalOfOperations.Summ AS 'Сумма', Materials.Name AS 'Материал', JournalOfOperations.Count AS 'Количество' from JournalOfOperations JOIN Materials ON JournalOfOperations.Material = Materials.id", connect);
				DataSet ds = new DataSet();
				dataAdapter.Fill(ds);
				dataGridView1.DataSource = ds;
				dataGridView1.DataMember = ds.Tables[0].ToString();
				connect.Close();
				dataGridView1.Columns["№ отпуска"].DisplayIndex = 0;
				dataGridView1.Columns["Дата"].DisplayIndex = 1;
				dataGridView1.Columns["Сумма"].DisplayIndex = 2;
				dataGridView1.Columns["Материал"].DisplayIndex = 3;
				dataGridView1.Columns["Количество"].DisplayIndex = 4;
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
		
		private void buttonChange_Click(object sender, EventArgs e)
		{
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
			string valueId = dataGridView1[0, CurrentRow].Value.ToString();
			string id = comboBoxMaterial.SelectedValue.ToString();
			string select = "select Price from Materials where id = '" + id + "'";
			object str = selectValue(ConnectionString, select);
			double sum = Convert.ToDouble(str) * Convert.ToInt32(textBoxCount.Text);
			string count = "0";			
			string Value1 = null;
			string Value2 = null;
			string Value3 = null;
			string Value4 = null;
			string Value5 = null;
			if (comboBoxMaterial.Text != "")
			{
				//ОС 
				Value1 = comboBoxMaterial.SelectedValue.ToString();
			}
			if (comboBoxMOL.Text != "")
			{
				//Подразделение 
				Value2 = comboBoxMOL.SelectedValue.ToString();
			}			
			if (comboBoxStorage.Text != "")
			{
				//Подразделение 
				Value4 = comboBoxStorage.SelectedValue.ToString();
			}			
			//Поле количество 
			if (textBoxCount.Text != "")
			{
				count = textBoxCount.Text;
			}
			if (textBoxCount.Text == "0")
			{
				MessageBox.Show("Количество материалов не может быть 0!");
				return;
			}//Поиск по базе данных значений 
			String selectCost = "select Price from Materials where id ='" + Value1 + "'";
			object cost = selectValue(ConnectionString, selectCost);
			double Summa = Convert.ToDouble(cost) * Convert.ToInt32(count);
			String selectDT = "select id from ChartOfAccounts where NumberOfAccount='10." + Value1 + "'";
			object DT = selectValue(ConnectionString, selectDT);
			String selectKT = "select id from ChartOfAccounts where NumberOfAccount='10." + Value1 + "'";
			object KT = selectValue(ConnectionString, selectKT);
			string selectcount = "select Count from JournalOfProvodki where DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + KT.ToString()
				+ "' and SubDt1 = '"
				+ comboBoxMaterial.Text + "' and SubDt2 = '" + comboBoxStorage.Text + "' and SubDt3 = '"
				+ comboBoxMOL.Text + "'";
			object countMat = selectValue(ConnectionString, selectcount);
			selectcount = "select SUM(Count) from JournalOfProvodki where DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + DT.ToString() + "' and SubKt1 = '"
				+ comboBoxMaterial.Text + "' and SubKt2 = '" + comboBoxStorage.Text + "' and SubKt3 = '"
				+ comboBoxMOL.Text + "'";
			string selectDate = "select Count(*) from JournalOfProvodki where ((DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + KT.ToString() + "' and SubKt1 = '"
				+ comboBoxMaterial.Text + "' and SubKt2 = '" + comboBoxStorage.Text + "' and SubKt3 = '"
				+ comboBoxMOL.Text + "') or (DebitAccount = '" + DT.ToString()
				+ "' and KreditAccount = '" + KT.ToString()
				+ "' and SubDt1 = '"
				+ comboBoxMaterial.Text + "' and SubDt2 = '" + comboBoxStorage.Text + "' and SubDt3 = '"
				+ comboBoxMOL.Text + "')) and Date > '" + maskedTextBox1.Text + "'";
			object countM = selectValue(ConnectionString, selectcount);
			object countDate = selectValue(ConnectionString, selectDate);
			if (countDate.ToString() != "0")
			{
				MessageBox.Show("Неверная дата! Есть документ с более поздней датой");
				return;
			}
			if (countM.ToString() == "")
			{
				countM = "0";
			}
			object countMater = selectValue(ConnectionString, selectcount);
			if (countMater.ToString() == "")
			{
				if (Convert.ToInt32(countM) < Convert.ToInt32(count))
				{
					MessageBox.Show("Недостаточно материалов! На складе осталось " + countM + " единиц");
					return;
				}
			}
			else if (Convert.ToInt32(countM) - (Convert.ToInt32(countMater) - Convert.ToInt32(dataGridView1[4, CurrentRow].Value.ToString())) < Convert.ToInt32(count))
			{
				MessageBox.Show("Недостаточно материалов! На складе осталось " + (Convert.ToInt32(countMat) - Convert.ToInt32(countMater)) + " единиц");
				return;
			}
			String selectCommand = "update JournalOfOperations set Date='" + maskedTextBox1.Text + "', Summ='"
				+ sum.ToString() + "', Count = '" + textBoxCount.Text
				+ "', MOL ='" + comboBoxMOL.SelectedValue.ToString() + "', Sklad = '"
				+ comboBoxStorage.SelectedValue.ToString() + "', Material = '" + comboBoxMaterial.SelectedValue.ToString() + "' where id = " + valueId +
				";" +
				"update JournalOfProvodki set DebitAccount = '" + DT.ToString() + "', SubDt1 = '" +
				comboBoxMaterial.Text + "',  SubDt2 = '" + comboBoxStorage.Text + "', SubDt3 = '" +
				comboBoxMOL.Text + "', KreditAccount = '" + KT.ToString() + "', SubKt1 = '" +
				comboBoxMaterial.Text + "',SubKt2 = '" + comboBoxStorage.Text + "', SubKt3 = '" +
				comboBoxMOL.Text + "', Count = '" + textBoxCount.Text + "', Summ = '" + sum.ToString() +
				"', Date = '" + maskedTextBox1.Text + "' where JournalOfOperations = " + valueId;
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
			//обновление 
			selectTable(ConnectionString);
			dataGridView1.Update();
			dataGridView1.Refresh();
		}

		private void buttonDel_Click(object sender, EventArgs e)
		{
			int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
			//получить значение IDвыбранной строки
			string valueId = dataGridView1[0, CurrentRow].Value.ToString();
			String selectCommand = "delete from JournalOfOperations where id=" + valueId + "; " + "delete from JournalOfProvodki where JournalOfOperations=" + valueId;			
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
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
			//обновление 
			selectTable(ConnectionString);
			dataGridView1.Update();
			dataGridView1.Refresh();
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

		private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			string id = dataGridView1[0, e.RowIndex].Value.ToString();
			string ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
			String select = "select Material from JournalOfOperations where id = '" + id + "'";
			object str = selectValue(ConnectionString, select);
			comboBoxMOL.SelectedValue = str.ToString();
			select = "select Sklad from JournalOfOperations where id = '" + id + "'";
			str = selectValue(ConnectionString, select);
			comboBoxStorage.SelectedValue = str.ToString();
			DateTime newDate;
			newDate = DateTime.Today;
			textBoxNum.Text = id;
			maskedTextBox1.Text = newDate.ToString("yyyy.MM.dd");
			textBoxCount.Text = dataGridView1[4, e.RowIndex].Value.ToString();
			comboBoxMaterial.Text = dataGridView1[3, e.RowIndex].Value.ToString();
		}
		private void buttonPostJourn_Click(object sender, EventArgs e)
		{
			int CurrentRow = dataGridView1.SelectedCells[0].RowIndex;
			string valueId = dataGridView1[0, CurrentRow].Value.ToString();
			JournalOfProvodki form = new JournalOfProvodki(Convert.ToInt32(valueId));
			form.Show();	
		}
	}
}