using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejemplo
{
    public partial class MenuPrincipal : Form
    {
        public List<Proceso> listaProceso = new List<Proceso>(); //Se utiliza para checkbox "Procesos" y "Ambos"
        public List<Referencia> listaReferencia = new List<Referencia>(); //Se utiliza para checkbox "Paginacion"
        public Conexion dbConexion = new Conexion();
        int AlgoProceso = 0, AlgoPaginacion = 0, tipoEmulacion = 0;
        /*AlgoProceso -> Algoritmo Proceso
         * AlgoPaginacion - >Algoritmo Paginacion*/

        public MenuPrincipal()
        {
            InitializeComponent();
            /*Form3 formulario3 = new Form3(listaReferencia, tipoEmulacion);
            formulario3.Show();*/
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            manejarCheckbox(1, checkBox1.Checked);
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            manejarCheckbox(2, checkBox2.Checked);
            dataGridView1.Visible = true;
            dataGridView2.Visible = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            manejarCheckbox(3, checkBox3.Checked);
            dataGridView1.Visible = false;
            dataGridView2.Visible = true;
        }

        private void manejarCheckbox(int x, bool estado)
        {
            if (estado)
            {
                switch (x)
                {
                    case 1:
                        checkBox2.Checked = false;
                        checkBox3.Checked = false;
                        panel3.Visible = true;
                        panel4.Visible = false;
                        tipoEmulacion = 1;
                        break;
                    case 2:
                        checkBox1.Checked = false;
                        checkBox3.Checked = false;
                        panel3.Visible = true;
                        panel4.Visible = false;
                        tipoEmulacion = 2;
                        break;
                    case 3:
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        panel4.Visible = true;
                        panel3.Visible = false;
                        tipoEmulacion = 3;
                        break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(txtID.Text);
                bool encontrado = false;
                encontrado = dbConexion.ValidarCode(id);
                if (encontrado)
                {
                    MessageBox.Show("Proceso ya existe digite uno nuevo!");

                }
                else
                {
                    dbConexion.GuardarProcess(Convert.ToInt32(txtID.Text), txtNombre.Text, Convert.ToInt32(txtCPU.Text),
                        Convert.ToInt32(txtILlegada.Text), Convert.ToInt32(txtPrioridad.Text));
                    MessageBox.Show("Proceso Ingresado a Base de Datos");
                    dataGridView1.DataSource = dbConexion.GetPROCESS();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            listaProceso.Clear();
            List<PROCESS> tempDbLista = new List<PROCESS>();
            tempDbLista = dbConexion.GetPROCESS();

            foreach (PROCESS item in tempDbLista)
                listaProceso.Add(new Proceso(item.ID, item.Nombre, Convert.ToInt32(item.CPU),
                    Convert.ToInt32(item.Prioridad), Convert.ToInt32(item.TiLLegada)));

            /*String nombre = txtID.Text;
            int CPU = int.Parse(txtCPU.Text), Prioridad = int.Parse(txtPrioridad.Text),
                instanciaLlegada = int.Parse(txtILlegada.Text);

            Proceso nuevo = new Proceso(listaProceso.Count, nombre, CPU, Prioridad, instanciaLlegada);

            listaProceso.Add(nuevo);

            txtNombre.Text = "";
            txtCPU.Text = "";
            txtPrioridad.Text = "";
            txtILlegada.Text = "";*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AlgoPaginacion = 1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Referencia nuevo = new Referencia(listaReferencia.Count, textBox6.Text);

            listaReferencia.Add(nuevo);

            textBox6.Text = "";
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
                if(tipoEmulacion == 1 && AlgoProceso != 0)
                {
                    int quantum = int.Parse(textBox5.Text), marco = int.Parse(textBox7.Text);

                    Ambos formulario2 = new Ambos(listaProceso, AlgoProceso,
                        AlgoPaginacion, quantum, marco, tipoEmulacion);
                    formulario2.Show();
                }
                else if(tipoEmulacion == 2 && AlgoProceso != 0 && AlgoPaginacion != 0)
                {
                    int quantum = int.Parse(textBox5.Text), marco = int.Parse(textBox7.Text);

                    Ambos formulario2 = new Ambos(listaProceso, AlgoProceso,
                        AlgoPaginacion, quantum, marco, tipoEmulacion);
                    formulario2.Show();
                }
                else if(tipoEmulacion == 3 && AlgoPaginacion != 0)
                {
                    int marco = int.Parse(textBox7.Text);
                    Paginacion formulario3 = new Paginacion(listaReferencia, marco, AlgoPaginacion);
                    formulario3.Show();
                }
                else
                {
                    MessageBox.Show("Por favor escoja las caracteristicas de la emulacion", "Error");
                }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            //Cargar desde Base de Datos
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            listaReferencia.Add(new Referencia(listaReferencia.Count, textBox6.Text));
            dataGridView2.Rows.Add(listaReferencia[listaReferencia.Count - 1].Id, 
                listaReferencia[listaReferencia.Count - 1].nombre);

            textBox6.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            AlgoPaginacion = 2;
            Btn_Optimo.Focus();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AlgoPaginacion = 3;
            Btn_NRU.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            AlgoPaginacion = 4;
            Btn_SegOport.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            AlgoProceso = 2;
            Btn_RR.Focus();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            AlgoProceso = 3;
            Btn_CPU.Focus();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            AlgoProceso = 4;
            Btn_Prioridad.Focus();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            AlgoProceso = 5;
            Btn_PrioridadTR.Focus();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            AlgoProceso = 6;
            Btn_Garantizado.Focus();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            AlgoProceso = 7;
            Btn_Loteria.Focus();
        }

        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            List<PROCESS> tempDbLista = new List<PROCESS>();
            try
            {
                dataGridView1.DataSource = dbConexion.GetPROCESS();
                tempDbLista = dbConexion.GetPROCESS();
                foreach (PROCESS item in tempDbLista)
                    listaProceso.Add(new Proceso(item.ID, item.Nombre, Convert.ToInt32(item.CPU),
                        Convert.ToInt32(item.Prioridad), Convert.ToInt32(item.TiLLegada)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            dataGridView2.Columns.Add("Id", "Id");
            dataGridView2.Columns.Add("Nombre", "Nombre");
            checkBox1.Checked = true;
            manejarCheckbox(1, checkBox1.Checked);
        }

        private List<PROCESS> Encontrado(int ID)
        {
            List<PROCESS> LU = new List<PROCESS>();
            if (txtID.Text != "")
            {
                LU = dbConexion.Find(ID);
            }
            else
            {
                MessageBox.Show("Digite ID del Proceso");
                txtID.Focus();
            }
            return LU;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<PROCESS> lista_process = new List<PROCESS>();

                lista_process = Encontrado(Convert.ToInt32(txtID.Text));
                if (lista_process.Count > 0)
                {

                    dbConexion.UpdateProcess(Convert.ToInt32(txtID.Text), txtNombre.Text,
                        Convert.ToInt32(txtCPU.Text),
                        Convert.ToInt32(txtILlegada.Text),
                        Convert.ToInt32(txtPrioridad.Text));
                    MessageBox.Show("Proceso Actualizado");
                    dataGridView1.DataSource = dbConexion.GetPROCESS();
                }
                else
                {
                    MessageBox.Show(" No encontrado. Crear Nuevo");
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            listaProceso.Clear();
            List<PROCESS> tempDbLista = new List<PROCESS>();
            tempDbLista = dbConexion.GetPROCESS();

            foreach (PROCESS item in tempDbLista)
                listaProceso.Add(new Proceso(item.ID, item.Nombre, Convert.ToInt32(item.CPU),
                    Convert.ToInt32(item.Prioridad), Convert.ToInt32(item.TiLLegada)));
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                List<PROCESS> lista_process = new List<PROCESS>();

                lista_process = Encontrado(Convert.ToInt32(txtID.Text));
                if (lista_process.Count > 0)
                {
                    MessageBox.Show("Proceso Encontrado");
                    foreach (var li in lista_process)
                    {
                        txtID.Text = li.ID.ToString();
                        txtNombre.Text = li.Nombre.ToString();
                        txtCPU.Text = li.CPU.ToString();
                        txtPrioridad.Text = li.Prioridad.ToString();
                        txtILlegada.Text = li.TiLLegada.ToString();
                    }
                }
                else
                {
                    MessageBox.Show(" No encontrado. Crear Nuevo");
                    txtNombre.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            List<PROCESS> lista_process = new List<PROCESS>();
            lista_process = Encontrado(Convert.ToInt32(txtID.Text));

            if (lista_process.Count > 0)
            {
                dbConexion.DeleteProcesss(Convert.ToInt32(txtID.Text));
                MessageBox.Show("Proceso eliminado");

                txtID.Clear();
                txtNombre.Clear();
                txtCPU.Clear();
                txtPrioridad.Clear();
                txtILlegada.Clear();

                dataGridView1.DataSource = dbConexion.GetPROCESS();
            }
            else
            {
                MessageBox.Show("Digite Codigo de Usuario");
            }

            listaProceso.Clear();
            List<PROCESS> tempDbLista = new List<PROCESS>();
            tempDbLista = dbConexion.GetPROCESS();

            foreach (PROCESS item in tempDbLista)
                listaProceso.Add(new Proceso(item.ID, item.Nombre, Convert.ToInt32(item.CPU),
                    Convert.ToInt32(item.Prioridad), Convert.ToInt32(item.TiLLegada)));
        }

        private void button18_Click(object sender, EventArgs e)
        {
            AlgoPaginacion = 5;
            Btn_Clock.Focus();
        }

        private void Btn_MC_Click(object sender, EventArgs e)
        {
            AlgoProceso = 8;
            Btn_MC.Focus();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            AlgoProceso = 1;
            Btn_PMC.Focus();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            AlgoPaginacion = 1;
            Btn_FIFO.Focus();
        }

    }
}
