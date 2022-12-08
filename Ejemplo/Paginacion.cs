using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejemplo
{
    public partial class Paginacion : Form
    {
        List<Referencia> listaReferencias;
        List<Referencia> tempLista = new List<Referencia>();
        List<string[]> datosFilas = new List<string[]>();
        int marcos, AlgoPaginacion;
        public Paginacion(List<Referencia> listaReferencias, int marcos, int AlgoPaginacion)
        {
            InitializeComponent();
            this.listaReferencias = listaReferencias;
            this.marcos = marcos;
            this.AlgoPaginacion = AlgoPaginacion;

            algoritmos();
            crearFilas();
            actualizarTabla();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void algoritmos()
        {
            switch (AlgoPaginacion)
            {
                case 1:
                    FIFO();
                    break;
                case 2:
                    Optimo();
                    break;
                case 3:
                    NRU();
                    break;
                case 4:
                    SegundasOportunidades();
                    break;
                case 5:
                    Clock();
                    break;
            }
        }

        //Este metodo sirve para crear las filas que se colocaran en la tabla
        public void crearFilas()
        {
            
            for (int i = 0; i < marcos; i++)
            {
                string[] nuevoArreglo = new string[tempLista.Count];

                for (int x = 0; x < tempLista.Count; x++)
                    nuevoArreglo[x] = " - ";

                for (int x = 0; x < tempLista.Count; x++)
                    if (tempLista[x].marco == i)
                    {
                        //Console.WriteLine("Agregando: " + tempLista[x].nombre);
                        nuevoArreglo[x] = tempLista[x].nombre;
                    }
                        
                
                datosFilas.Add(nuevoArreglo);
            }

            string[] filaFallos = new string[tempLista.Count];
            int cantFallos = 0;
            for (int i = 0; i < tempLista.Count; i++)
            {
                if (tempLista[i].fallo)
                {
                    filaFallos[i] = " X ";
                    cantFallos++;
                }
            }
            datosFilas.Add(filaFallos);

            /*for (int i = 0; i < datosFilas.Count; i++)
            {
                mostrarArreglo(datosFilas[i]);
            }*/

            //Creamos las columnas
            for (int i = 0; i < tempLista.Count; i++)
            {
                dataGridView1.Columns.Add(tempLista[i].nombre, tempLista[i].nombre);
                dataGridView1.Columns[i].Visible = false; //seteamos las columnas como "no visibles"
            }

            //Insertamos las filas para las columnas
            for (int i = 0; i < datosFilas.Count; i++)
            {
                dataGridView1.Rows.Add(datosFilas[i]);
            }

            //Estadistica
            textBox1.Text = cantFallos.ToString();
            textBox2.Text = tempLista.Count.ToString();
            double FreRen = cantFallos;
            FreRen *= 1.00;
            FreRen /= tempLista.Count;
            textBox3.Text = Math.Round(FreRen, 2).ToString();
            FreRen = 1 - FreRen;
            textBox4.Text = Math.Round(FreRen, 2).ToString();

        }

        public async void actualizarTabla()
        {
            dataGridView1.Columns[0].Visible = true;

            for(int i=1; i<tempLista.Count; i++)
            {
                await Task.Delay(2000);
                dataGridView1.Columns[i].Visible = true;
            }
        }

        //AlgoritmosPaginacion
        public void FIFO()
        {
            /*  1. Evaluamos si la referencia ya esta dentro del marco 
                    (Puede darse el caso en donde hayan marcos vacios pero la referencia este repetida)
                2. Evaluamos si hay marcos vacios
                3. Si esta vacio lo metemos en esa posicion
                4. Si no esta vacio evaluamos si la referencia ya esta dentro del marco
                5. Si ya esta dentro del marco lo dejamos en esa posicion
                6. Si no esta dentro del marco evaluamos el proceso mas antiguo
                7. Se saca el proceso mas antiguo y se coloca en esa posicion   */

            int posListaR = 0, tiempo = 0;
            MarcoReferencia[] marcosReferencias = new MarcoReferencia[marcos];

            for (int i = 0; i < marcos; i++)
            {
                marcosReferencias[i] = new MarcoReferencia();
            }

            foreach (Referencia refs in listaReferencias)
                tempLista.Add(new Referencia(refs.Id, refs.nombre));

            while (posListaR != tempLista.Count)
            {
                bool vacio_Repetido = false; //
                //Evaluamos si hay marcos vacios y si no esta repetido
                for (int i = 0; i < marcosReferencias.Length; i++)
                {
                    //Referencias ya dentro de uno de los marcos
                    if (marcosReferencias[i].getOcupadoPor().Equals(tempLista[posListaR].nombre))
                    {
                        tempLista[posListaR].marco = i;
                        vacio_Repetido = true;
                        break;
                    }
                    //marcos de referencias si estan vacios
                    else if (marcosReferencias[i].vacio)
                    {
                        marcosReferencias[i].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                        tempLista[posListaR].marco = i;
                        marcosReferencias[i].vacio = false;
                        tempLista[posListaR].fallo = true;
                        vacio_Repetido = true;
                        break;
                    }
                }

                //Al llegar aqui implica que la referencia que se esta evaluando no esta repetida y tampoco hay espacio
                if (!vacio_Repetido)
                {
                    int refMasAntiguo = 0;

                    for (int i = 0; i < marcosReferencias.Length; i++)
                        if(marcosReferencias[i].tiempo < marcosReferencias[refMasAntiguo].tiempo)
                        {
                            refMasAntiguo = i;
                            break;
                        }

                    marcosReferencias[refMasAntiguo].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                    tempLista[posListaR].marco = refMasAntiguo;
                    tempLista[posListaR].fallo = true;
                }

                posListaR++;
                tiempo++;
            }

            /*foreach (Referencia item in tempLista)
            {
                Console.WriteLine(item.nombre + " - " + item.marco);
            }*/
        }

        private void Optimo()
        {
            /*  Evaluamos si la referencia ya ocupa un marco
                Si no ocupa un marco evaluamos si hay un marco vacio
                Si no hay marcos vacios, evaluamos las referencias futuras y 
                    las comparamos con aquellas referencias que ya ocupan un marco
                Evaluamos las que se repitan y aplicamos fifo en las que no se repitan
                Si todos se repiten aplicamos "FIFO inverso" es decir sacamos aquella 
                    referencia 'repetida' (que se encuentra en el marco) que tardara mas 
                    tiempo en volver a ocupar un marco */
            int posListaR = 0, tiempo = 0;
            MarcoReferencia[] marcosReferencias = new MarcoReferencia[marcos];

            for (int i = 0; i < marcos; i++)
            {
                marcosReferencias[i] = new MarcoReferencia();
            }

            foreach (Referencia refs in listaReferencias)
                tempLista.Add(new Referencia(refs.Id, refs.nombre));

            while (posListaR != tempLista.Count)
            {
                bool vacio_Repetido = false; //
                //Evaluamos si hay marcos vacios y si no esta repetido
                for (int i = 0; i < marcosReferencias.Length; i++)
                {
                    //Referencias ya dentro de uno de los marcos
                    if (marcosReferencias[i].getOcupadoPor().Equals(tempLista[posListaR].nombre))
                    {
                        tempLista[posListaR].marco = i;
                        vacio_Repetido = true;
                        break;
                    }
                    //Marcos de referencias si estan vacios
                    else if (marcosReferencias[i].vacio)
                    {
                        marcosReferencias[i].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                        tempLista[posListaR].marco = i;
                        marcosReferencias[i].vacio = false;
                        tempLista[posListaR].fallo = true;
                        vacio_Repetido = true;
                        break;
                    }
                }

                if (!vacio_Repetido)
                {
                    //Evaluamos las referencias futuras si estas se repiten y si ocupan un marco en el futuro
                    for (int i = posListaR; i < tempLista.Count; i++)
                    {
                        for (int x = 0; x < marcosReferencias.Length; x++)
                        {
                            if (tempLista[i].nombre.Equals(marcosReferencias[x].getOcupadoPor()))
                                marcosReferencias[x].permanecer = true;
                        }
                    }
                    
                    int posSacar = -1;

                    //De las referencias que no se repiten sacamos el mas antiguo (FIFO)
                    for (int x = 0; x < marcosReferencias.Length; x++)
                    {
                        if (!marcosReferencias[x].permanecer)
                        {
                            if (posSacar == -1) //Solo se entra en este bloque en el primer marco.permanecer == false
                                posSacar = x;
                            else if (marcosReferencias[x].tiempo < marcosReferencias[posSacar].tiempo)
                                posSacar = x;
                        }
                    }

                    //Console.WriteLine("PosSacar: " + posSacar);

                    //Si posSacar queda igual a -1 implica que todas las referencias volveran a ocupar un marco
                    if (posSacar == -1) //Sacamos la referencia que va a tardar mas tiempo en volver a ocuparse
                    {
                        int contMarcosTemp = marcosReferencias.Length;

                        for (int i = posListaR; i < tempLista.Count; i++)
                        {
                            for (int x = 0; x < marcosReferencias.Length; x++)
                            {
                                if (marcosReferencias[x].getOcupadoPor().Equals(tempLista[i].nombre)
                                    && !marcosReferencias[x].yaEvaluado && contMarcosTemp != 0)
                                {
                                    posSacar = x;
                                    marcosReferencias[x].yaEvaluado = true;
                                    contMarcosTemp--;
                                    break;
                                }
                            }
                            if (contMarcosTemp == 0)
                                break;
                        }
                    }

                    tempLista[posListaR].marco = posSacar;
                    tempLista[posListaR].fallo = true;
                    marcosReferencias[posSacar].setOcupadoPor(tempLista[posListaR].nombre, tiempo);

                    //Resetear permanecer y yaEvaluado
                    for (int i = 0; i < marcosReferencias.Length; i++)
                    {
                        marcosReferencias[i].permanecer = false;
                        marcosReferencias[i].yaEvaluado = false;
                    }
                }

                posListaR++;
                tiempo++;
            }
        }

        private void NRU()
        {
            int posListaR = 0, tiempo = 0;
            MarcoReferencia[] marcosReferencias = new MarcoReferencia[marcos];

            for (int i = 0; i < marcos; i++)
            {
                marcosReferencias[i] = new MarcoReferencia();
            }

            foreach (Referencia refs in listaReferencias)
                tempLista.Add(new Referencia(refs.Id, refs.nombre));

            while(posListaR != tempLista.Count)
            {
                bool vacio_Repetido = false; //
                //Evaluamos si hay marcos vacios y si no esta repetido
                for (int i = 0; i < marcosReferencias.Length; i++)
                {
                    //Referencias ya dentro de uno de los marcos
                    if (marcosReferencias[i].getOcupadoPor().Equals(tempLista[posListaR].nombre))
                    {
                        tempLista[posListaR].marco = i;
                        vacio_Repetido = true;
                        break;
                    }
                    //marcos de referencias si estan vacios
                    else if (marcosReferencias[i].vacio)
                    {
                        marcosReferencias[i].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                        tempLista[posListaR].marco = i;
                        marcosReferencias[i].vacio = false;
                        tempLista[posListaR].fallo = true;
                        vacio_Repetido = true;
                        break;
                    }
                }

                if (!vacio_Repetido)
                {
                    int contMarcosTemp = marcosReferencias.Length, posSacar = 0;

                    for (int i = posListaR - 1; i >= 0; i--)
                    {
                        for (int x = 0; x < marcosReferencias.Length; x++)
                        {
                            if(marcosReferencias[x].getOcupadoPor().Equals(tempLista[i].nombre) &&
                                !marcosReferencias[x].yaEvaluado && contMarcosTemp != 0)
                            {
                                posSacar = x;
                                marcosReferencias[x].yaEvaluado = true;
                                contMarcosTemp--;
                                break;
                            }
                            if (contMarcosTemp == 0)
                                break;
                        }
                    }

                    tempLista[posListaR].marco = posSacar;
                    tempLista[posListaR].fallo = true;
                    marcosReferencias[posSacar].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                    
                    //Resetear yaEvaluado
                    for (int i = 0; i < marcosReferencias.Length; i++)
                        marcosReferencias[i].yaEvaluado = false;
                }

                posListaR++;
                tiempo++;
            }
        }

        private void SegundasOportunidades()
        {
            int posListaR = 0, tiempo = 0;
            MarcoReferencia[] marcosReferencias = new MarcoReferencia[marcos];

            for (int i = 0; i < marcos; i++)
            {
                marcosReferencias[i] = new MarcoReferencia();
            }

            foreach (Referencia refs in listaReferencias)
                tempLista.Add(new Referencia(refs.Id, refs.nombre));

            while (posListaR != tempLista.Count)
            {
                bool vacio_Repetido = false; //

                //Evaluamos si hay marcos vacios y si no esta repetido
                for (int i = 0; i < marcosReferencias.Length; i++)
                {
                    //Referencias ya dentro de uno de los marcos
                    if (marcosReferencias[i].getOcupadoPor().Equals(tempLista[posListaR].nombre))
                    {
                        tempLista[posListaR].marco = i;
                        marcosReferencias[i].permanecer = true;
                        vacio_Repetido = true;
                        break;
                    }
                    //marcos de referencias si estan vacios
                    else if (marcosReferencias[i].vacio)
                    {
                        marcosReferencias[i].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                        marcosReferencias[i].vacio = false;
                        marcosReferencias[i].permanecer = true;
                        tempLista[posListaR].marco = i;
                        tempLista[posListaR].fallo = true;
                        vacio_Repetido = true;
                        break;
                    }
                }

                if (!vacio_Repetido)
                {

                    int posSacar = -1;

                    //Evaluamos si hay marcos inactivos
                    for (int i = 0; i < marcosReferencias.Length; i++)
                    {
                        if(!marcosReferencias[i].permanecer)
                        {
                            if(posSacar == -1)
                                posSacar = i;
                            else if(marcosReferencias[i].tiempo < marcosReferencias[posSacar].tiempo)
                                posSacar = i;
                        }
                    }

                    if(posSacar == -1) //Si no hay marcos inactivos entramos en este bloque
                    {
                        posSacar = 0;

                        for (int i = 0; i < marcosReferencias.Length; i++)
                        {
                            //Console.WriteLine(marcosReferencias[i].getOcupadoPor() + " - " + marcosReferencias[posSacar].getOcupadoPor());
                            if (marcosReferencias[i].tiempo < marcosReferencias[posSacar].tiempo)
                                posSacar = i;
                        }
                    }

                    for (int i = 0; i < marcosReferencias.Length; i++)
                        marcosReferencias[i].permanecer = false;

                    //Asignar los marcos y referencias
                    tempLista[posListaR].marco = posSacar;
                    tempLista[posListaR].fallo = true;
                    marcosReferencias[posSacar].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                    marcosReferencias[posSacar].permanecer = true;
                }

                posListaR++;
                tiempo++;
            }
            }

        private void Clock()
        {
            int posListaR = 0, tiempo = 0, clockMarco = 0;
            MarcoReferencia[] marcosReferencias = new MarcoReferencia[marcos];

            for (int i = 0; i < marcos; i++)
                marcosReferencias[i] = new MarcoReferencia();

            foreach (Referencia refs in listaReferencias)
                tempLista.Add(new Referencia(refs.Id, refs.nombre));

            while (posListaR != tempLista.Count)
            {
                bool vacio_Repetido = false;

                //Evaluamos si hay marcos vacios y si no esta repetido
                for (int i = 0; i < marcosReferencias.Length; i++)
                {
                    //Referencias ya dentro de uno de los marcos
                    if (marcosReferencias[i].getOcupadoPor().Equals(tempLista[posListaR].nombre))
                    {
                        tempLista[posListaR].marco = i;
                        marcosReferencias[i].permanecer = true;
                        vacio_Repetido = true;
                        break;
                    }
                    //marcos de referencias si estan vacios
                    else if (marcosReferencias[i].vacio)
                    {
                        marcosReferencias[i].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                        marcosReferencias[i].vacio = false;
                        marcosReferencias[i].permanecer = true;
                        tempLista[posListaR].marco = i;
                        tempLista[posListaR].fallo = true;
                        vacio_Repetido = true;
                        break;
                    }
                }


                if (!vacio_Repetido)
                {

                    int posSacar = -1;

                    //Evaluamos si hay marcos inactivos
                    for (int i = 0; i < marcosReferencias.Length; i++)
                    {
                        if (!marcosReferencias[i].permanecer)
                        {
                            if (posSacar == -1)
                                posSacar = i;
                            else if (marcosReferencias[i].tiempo < marcosReferencias[posSacar].tiempo)
                                posSacar = clockMarco;
                        }
                    }

                    if (posSacar == -1) //Si no hay marcos inactivos entramos en este bloque
                    {
                        posSacar = 0;

                        for (int i = 0; i < marcosReferencias.Length; i++)
                        {
                            //Console.WriteLine(marcosReferencias[i].getOcupadoPor() + " - " + marcosReferencias[posSacar].getOcupadoPor());
                            if (marcosReferencias[i].tiempo < marcosReferencias[posSacar].tiempo)
                                posSacar = i;
                        }
                    }

                    for (int i = 0; i < marcosReferencias.Length; i++)
                        marcosReferencias[i].permanecer = false;

                    //Asignar los marcos y referencias
                    tempLista[posListaR].marco = posSacar;
                    tempLista[posListaR].fallo = true;
                    marcosReferencias[posSacar].setOcupadoPor(tempLista[posListaR].nombre, tiempo);
                    marcosReferencias[posSacar].permanecer = true;
                }
                if (clockMarco == marcosReferencias.Length)
                {
                    clockMarco = 0;
                }
                clockMarco++;
                posListaR++;
                tiempo++;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Paginacion.CheckForIllegalCrossThreadCalls = false;
        }


    }
}
