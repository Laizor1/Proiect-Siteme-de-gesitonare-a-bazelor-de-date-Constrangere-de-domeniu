using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

namespace SGBDProiect
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MySqlDataReader reader;
            MySqlCommand query = SGBD.conn.CreateCommand();

            query.CommandText = "SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA=(SELECT DATABASE());";

            SGBD.conn.Open();

            reader = query.ExecuteReader();

            while (reader.Read())
            {
                listBox1.Items.Add(reader["TABLE_NAME"]);
                System.Console.WriteLine(reader["TABLE_NAME"]);
            }

            SGBD.conn.Close();
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string table = (string) listBox1.SelectedItem;

            MySqlCommand query = SGBD.conn.CreateCommand();

            query.CommandText = "SELECT COLUMN_NAME, DATA_TYPE FROM information_schema.columns WHERE TABLE_NAME='" + table + "'";

            SGBD.conn.Open();

            MySqlDataReader reader = query.ExecuteReader();

            dataGridView1.Rows.Clear();

            while(reader.Read())
            {
                dataGridView1.Rows.Add(new object[] {
                    reader["COLUMN_NAME"],
                    reader["DATA_TYPE"]
                });
            }

            SGBD.conn.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int row = dataGridView1.CurrentCell.RowIndex;

                string table = (string)listBox1.SelectedItem;
                string fieldName = dataGridView1.Rows[row].Cells[0].Value.ToString();
                string dataType = dataGridView1.Rows[row].Cells[1].Value.ToString();

                if (dataType == "datetime" || dataType == "timestamp")
                {
                    MySqlCommand query = SGBD.conn.CreateCommand();
                    query.CommandText = "SELECT MIN(" + fieldName + ") AS min, MAX(" + fieldName + ") AS max FROM " + table + ";";

                    SGBD.conn.Open();

                    MySqlDataReader reader = query.ExecuteReader();

                    reader.Read();

                    txtMin.Text = reader["min"].ToString();
                    txtMax.Text = reader["max"].ToString();

                    SGBD.conn.Close();
                }
            }
        }

        private void btnAplica_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int row = dataGridView1.CurrentCell.RowIndex;

                string table = (string)listBox1.SelectedItem;
                string fieldName = dataGridView1.Rows[row].Cells[0].Value.ToString();
                string dataType = dataGridView1.Rows[row].Cells[1].Value.ToString();

                if (dataType == "datetime" || dataType == "timestamp")
                {
                    // verifica daca exista trigger
                    if (SGBD.HasDomainConstraint(table, fieldName))
                    {
                        MessageBox.Show("Exista deja o constrangere de domeniu pentru aceasta coloana.", "Eroare");
                        return;
                    }

                    // adauga trigger
                    if (SGBD.AddDomainConstraint(table, fieldName, txtMin.Text, txtMax.Text))
                    {
                        MessageBox.Show("Constrangerea de domeniu a fost adaugata cu succes!", "Succes");
                    } else
                    {
                        MessageBox.Show("Nu s-a putut adauga constrangerea de domeniu!", "Eroare");
                    }
                } else
                {
                    MessageBox.Show("Nu se poate aplica constrangerea pe tipul de date selectat!", "Eroare");
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.LoginForm.Visible = true;
        }
    }
}
