using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modelo;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public static string ConexionPruebas()
        {
            bool Persist_Security = false;
            string userName = "Ricardo";
            string pass = "rhvjinzo101212";
            string db = "PruebasRicardo";
            string server = "Localhost";

            string conexionDB =
                $"Persist Security Info={Persist_Security};" +
                $"User ID={userName};" +
                $"Password={pass};" +
                $"Initial Catalog={db};" +
                $"Server={server};";



            return conexionDB;
        }

        SqlConnection con = new SqlConnection(ConexionPruebas());

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {

            Persona p1 = new Persona();
            List<Persona> list = new List<Persona>();

            p1.nombre = textBox1.Text;


            list.Add(p1);

            foreach (var item in list)
            {
                ListaDatos.Items.Add(item.nombre);
            }




            
        }

        private void ListaDatos_DoubleClick(object sender, EventArgs e)
        {
            foreach (var item in ListaDatos.SelectedItems)
            {
                MessageBox.Show(item.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
    
            string nombre = "";

            for (int i = 0; i < ListaDatos.Items.Count; i++)
            {
                try
                {
                    string consulta = "INSERT INTO nombre VALUES (@nombre,@estatus)";
                    SqlCommand cmd = new SqlCommand(consulta, con);
                    nombre = ListaDatos.Items[i].ToString();
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@estatus", checkBox1.Checked);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }

            ListaDatos.Items.Clear();
            MessageBox.Show("datos eliminados","Eliminados",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            mostrarDatos();
     
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {


                    SqlCommand cmd = new SqlCommand("SELECT * FROM nombre WHERE nombre=@nombre", con);
                    cmd.Parameters.AddWithValue("@nombre", textBox1.Text);
                    con.Open();
                    SqlDataReader rs = cmd.ExecuteReader();

                    if (rs.Read())
                    {
                        textBox3.Text = rs.GetInt32(0).ToString();
                        textBox1.Text = rs.GetString(1);
                        textBox2.Text = rs.GetString(2);
                        return;
                    }
                    else
                    {

                        Persona p1 = new Persona();
                        List<Persona> list = new List<Persona>();

                        p1.nombre = textBox1.Text;


                        list.Add(p1);

                        foreach (var item in list)
                        {
                            ListaDatos.Items.Add(item.nombre);
                        }

                        textBox1.Text = "";
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    con.Close();
                }



            }
        }

        void mostrarDatos()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("Select id ,nombre, CASE WHEN estatus = '1' THEN 'Activo' ELSE 'Inactivo' END EstatusLetra from nombre", con);
            SqlDataAdapter adaptador = new SqlDataAdapter(cmd);

            adaptador.Fill(dt);
            con.Close();
            TablaDatos.DataSource = dt;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //DataGridViewColumn columan1 = new DataGridViewColumn();
            //DataGridViewCell cell = new DataGridViewTextBoxCell();
            //columan1.CellTemplate = cell;
            //columan1.HeaderText = "Folio";
            //columan1.Width = 200;

           
            try
            {
                 mostrarDatos();
                //TablaDatos.Columns.Add(columan1);
                TablaDatos.Columns[0].HeaderText = "ID";
                TablaDatos.Columns[1].HeaderText = "NOMBRE";
                TablaDatos.Columns[2].HeaderText = "ESTATUS";

                //Establecemos el tamano de las columas

                TablaDatos.Columns[0].Width = 50;
                TablaDatos.Columns[1].Width = 150;
                TablaDatos.Columns[2].Width = 550;

                if (TablaDatos.Rows.Count != 0)
                {
                    TablaDatos.Rows[0].DefaultCellStyle.BackColor = Color.Red;
                    TablaDatos.Rows[0].DefaultCellStyle.ForeColor = Color.White;
                    TablaDatos.Rows[0].DefaultCellStyle.Font = new Font(Font.FontFamily, 14, FontStyle.Bold);

                }




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }




        }

        private void TablaDatos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = TablaDatos.CurrentCell.RowIndex;

            textBox1.Text = TablaDatos[0, fila].Value.ToString();
            textBox2.Text = TablaDatos[1, fila].Value.ToString();
            textBox3.Text = TablaDatos[2, fila].Value.ToString();



            ListaDatos.Items.Add(textBox1.Text);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Deseas eliminar el registro", "Eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                for (int i = 0; i < ListaDatos.Items.Count; i++)
                {

                    SqlCommand cmd = new SqlCommand("DELETE FROM nombre WHERE id=@id", con);
                    cmd.Parameters.AddWithValue("@id", int.Parse(ListaDatos.Items[i].ToString()));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                ListaDatos.Items.Clear();
                mostrarDatos();
                limpiarTexto();
            }
            void limpiarTexto()
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }

        }
    }
}
