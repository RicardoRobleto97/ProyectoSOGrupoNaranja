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
    public partial class Ambos : Form
    {
        private List<GUIProceso> ProcesosVisual = new List<GUIProceso>();
        public List<Registro> Registros = new List<Registro>();
        private List<Proceso> listaProceso;
        private List<Referencia> listaReferencias = new List<Referencia>();
        int AlgoProceso, AlgoPaginacion, marco;
        Thread hiloProcesos;

        public Ambos(List<Proceso> listaProceso, int AlgoProceso,
            int AlgoPaginacion, int quantum, int marco, int tipoEmulacion)
        {
            InitializeComponent();
            this.listaProceso = listaProceso;
            this.AlgoProceso = AlgoProceso;
            this.AlgoPaginacion = AlgoPaginacion;

            if (marco <= 0)
                this.marco = 1;
            else
                this.marco = marco;


            algoritmos(AlgoProceso, AlgoPaginacion, tipoEmulacion, quantum);
        }

        private void crearBarras()
        {
            //Metodo utilizado para la creacion de las barras de progreso para cada proceso
            for (int i = 0; i < listaProceso.Count; i++)
                ProcesosVisual.Add(new GUIProceso(i, listaProceso[i].getID(), listaProceso[i].getNombre(),
                    Controls, listaProceso[i].getCPU()));
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Ambos.CheckForIllegalCrossThreadCalls = false;
        }

        private void algoritmos(int AlgoProceso, int AlgoPaginacion, int tipoEmulacion, int quantum)
        {
            if (tipoEmulacion == 1 || tipoEmulacion == 2) // tipoEmulacion == 3 se ejecuta desde form1
            {
                switch (AlgoProceso)
                {
                    case 1:
                        procesoMasCorto();
                        break;
                    case 2:
                        RoundRobin(quantum);
                        break;
                    case 3:
                        CPU(quantum);
                        break;
                    case 4:
                        Prioridad(quantum);
                        break;
                    case 5:
                        Prioridad_ILlegada_CPU();
                        break;
                    case 6:
                        Garantizado(Convert.ToDouble(quantum));
                        break;
                    case 7:
                        AlgorPorLoteria(quantum);
                        break;
                    case 8:
                        multiplesColas();
                        break;
                }
            }

            if (tipoEmulacion == 1)
            {
                /*hiloProcesos = new Thread(ActualizarBarras);
                hiloProcesos.Start();*/
                dataGridView1.Columns.Add("Nombre", "Nombre");
                dataGridView1.Columns.Add("CPU", "CPU"); //Cantidad de CPU restante
                dataGridView1.Columns.Add("Estado", "Estado");

                for (int i = 0; i < Registros.Count; i++)
                {
                    string[] fila = { Registros[i].nombre, Registros[i].cantCPU.ToString(), Registros[i].estado };

                    dataGridView1.Rows.Add(fila);
                }
            }

            if (tipoEmulacion == 2)
            {
                dataGridView1.Visible = false;

                crearBarras();
                //Meter registros a referencias
                crearReferencias();

                hiloProcesos = new Thread(ActualizarBarras);
                hiloProcesos.Start();

                Paginacion formulario3 = new Paginacion(listaReferencias, marco, AlgoPaginacion);
                formulario3.Show();

                listaReferencias.Clear();
            }

        }

        private void ActualizarBarras()
        {
            int posicion = 0;

            while (posicion != Registros.Count)
            {
                //Primero determinamos el estado
                //Despues usando el codigo determinamos el proceso que se relaciona con su respectivo GUIProceso
                if (Registros[posicion].estado.Equals("Ejecucion"))
                {
                    for (int i = 0; i < ProcesosVisual.Count; i++)
                    {
                        if (Registros[posicion].Id == ProcesosVisual[i].Id)
                        {
                            ProcesosVisual[i].setBarraValue(Registros[posicion].cantCPU);
                            ProcesosVisual[i].estado.BackColor = Color.Green;
                        }
                    }

                    Thread.Sleep(2000);
                }
                else if (Registros[posicion].estado.Equals("Listo"))
                {
                    for (int i = 0; i < ProcesosVisual.Count; i++)
                    {
                        if (Registros[posicion].Id == ProcesosVisual[i].Id)
                            ProcesosVisual[i].estado.BackColor = Color.Yellow;
                    }
                }
                else if (Registros[posicion].estado.Equals("Bloqueado"))
                {
                    for (int i = 0; i < ProcesosVisual.Count; i++)
                    {
                        if (Registros[posicion].Id == ProcesosVisual[i].Id)
                            ProcesosVisual[i].estado.BackColor = Color.Red;
                    }
                }
                else
                {
                    for (int i = 0; i < ProcesosVisual.Count; i++)
                    {
                        if (Registros[posicion].Id == ProcesosVisual[i].Id)
                        {
                            //Esta linea no es explicitamente necesario, pero sirve como garantia
                            ProcesosVisual[i].setBarraValue(ProcesosVisual[i].barra.Maximum); //Esta linea
                            ProcesosVisual[i].estado.BackColor = Color.Gray;
                        }
                    }
                }

                posicion++;
            }
        }

        private void crearReferencias()
        {
            foreach (Registro item in Registros)
            {
                if (item.estado.Equals("Ejecucion"))
                    listaReferencias.Add(new Referencia(item.Id, item.nombre));
            }
        }

        //AlgoritmosProcesos
        private void procesoMasCorto()
        {
            int posProceso;
            bool finIteraciones = false;
            List<Proceso> tempProcesos = new List<Proceso>();

            for (int i = 0; i < listaProceso.Count; i++)
            {
                tempProcesos.Add(new Proceso(listaProceso[i].getID(), listaProceso[i].getNombre(),
                    listaProceso[i].getCPU(), listaProceso[i].getPrioridad(), listaProceso[i].getILlegada()));
                //Agregamos la instancia de llegada como cero, esto es para dejarlo como el inge lo quiere.
            }

            while (!finIteraciones)
            {
                posProceso = tempProcesos.Count - 1;

                for (int i = tempProcesos.Count - 1; i >= 0; i--)
                {
                    if (tempProcesos[i].getCPU() <= tempProcesos[posProceso].getCPU())
                    {
                        posProceso = i;
                    }
                }
                //Console.WriteLine("PosProceso: " + posProceso);
                //Guardamos la cantidad "restada" o "ejecutada" del proceso, como es proceso mas corto
                //la cantidad sera todo lo que hay en CPU
                tempProcesos[posProceso].CPUAcumulado = tempProcesos[posProceso].getCPU();

                //Registramos el estado y el CPUAcumulado
                Registros.Add(new Registro(tempProcesos[posProceso].getID(), tempProcesos[posProceso].getNombre(),
                    tempProcesos[posProceso].CPUAcumulado, "Ejecucion"));

                //Cuando el proceso pase a listo, bloqueado o terminado el CPUAcumulado no importa
                Registros.Add(new Registro(tempProcesos[posProceso].getID(), tempProcesos[posProceso].getNombre(),
                    0, "Terminado"));

                tempProcesos.RemoveAt(posProceso);

                if (tempProcesos.Count == 0)
                    finIteraciones = true;
            }
        }

        private void RoundRobin(int quantum)
        {

            int tiempoGlobal = 0, contadorQ = 0; //contadorQuantum
            bool finIteraciones = false;
            List<Proceso> tempLista = new List<Proceso>();

            while (!finIteraciones)
            {
                for (int i = 0; i < listaProceso.Count; i++)
                {
                    //Mete los procesos cuya instancia de llegada sea acorde al tiempo global
                    if (listaProceso[i].getILlegada() == tiempoGlobal)
                        tempLista.Add(new Proceso(listaProceso[i].getID(), listaProceso[i].getNombre(),
                            listaProceso[i].getCPU(), listaProceso[i].getPrioridad(), listaProceso[i].getILlegada()));
                }

                /*El round robin se aplica con la lista temporal, simplemente pasa a ejecucion
                 * y de ejecucion a terminado, a listo o a bloqueo. Para simular el algoritmo solo
                 * le restamos el cpu y "copiamos" ese objeto al final de la lista, seguidamente
                 * eliminamos el objeto "original".
                 */
                if (contadorQ == tempLista[0].getCPU())
                {
                    Registros.Add(new Registro(tempLista[0].getID(), tempLista[0].getNombre(),
                        tempLista[0].getCPUAcumulado(), "Ejecucion"));
                    Registros.Add(new Registro(tempLista[0].getID(), tempLista[0].getNombre(), 0, "Terminado"));

                    tempLista.RemoveAt(0);
                    contadorQ = 0;
                }
                else if (contadorQ - quantum == 0)
                {
                    tempLista[0].setCPU(tempLista[0].getCPU() - quantum);
                    tempLista[0].agregarCPUAcumulado(quantum);

                    Registros.Add(new Registro(tempLista[0].getID(), tempLista[0].getNombre(),
                        tempLista[0].getCPUAcumulado(), "Ejecucion"));

                    if (tempLista[0].getCPU() > quantum && tempLista[0].getCPU() != 0)
                        Registros.Add(new Registro(tempLista[0].getID(), tempLista[0].getNombre(),
                            tempLista[0].getCPUAcumulado(), "Bloqueado"));
                    else if (tempLista[0].getCPU() != 0)
                        Registros.Add(new Registro(tempLista[0].getID(), tempLista[0].getNombre(),
                            tempLista[0].getCPUAcumulado(), "Listo"));

                    //Pasamos el elemento al final de la cola
                    tempLista.Add(tempLista[0]);
                    tempLista.RemoveAt(0);
                    contadorQ = 0;
                }

                tiempoGlobal++;
                contadorQ++;

                if (tempLista.Count == 0)
                    finIteraciones = true;
            }

        }

        private void CPU(int quantum)
        {
            bool finIteraciones = false;
            Proceso pro = null, proAnterior = null;
            List<Proceso> tempLista = new List<Proceso>();

            foreach (Proceso item in listaProceso)
                tempLista.Add(new Proceso(item.getID(), item.getNombre(),
                    item.getCPU(), item.getPrioridad(), item.getILlegada()));

            //Algoritmo burbuja, ordenamos por CPU de esta forma tratamos de evitar posibles bucles infinitos
            for (int j = 0; j <= tempLista.Count - 2; j++)
            {
                for (int i = 0; i <= tempLista.Count - 2; i++)
                {
                    if (tempLista[i].getCPU() > tempLista[i + 1].getCPU())
                    {
                        Proceso temp = tempLista[i + 1];
                        tempLista[i + 1] = tempLista[i];
                        tempLista[i] = temp;
                    }
                }
            }

            while (!finIteraciones)
            {
                pro = tempLista[tempLista.Count - 1];

                for (int i = tempLista.Count - 1; i >= 0; i--)
                {
                    if (proAnterior == null)
                    {
                        if (tempLista[i].getCPU() <= pro.getCPU())
                            pro = tempLista[i];
                    }
                    else
                    {
                        if (!proAnterior.getNombre().Equals(tempLista[i].getNombre()) && tempLista[i].getCPU() > 0 &&
                            tempLista[i].getCPU() <= pro.getCPU())
                            pro = tempLista[i];
                    }
                }

                proAnterior = pro;

                if (pro.getCPU() > quantum)
                {
                    pro.setCPU(pro.getCPU() - quantum);
                    pro.agregarCPUAcumulado(quantum);
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Ejecucion"));

                    if (pro.getCPU() > quantum)
                        Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Bloqueado"));
                    else
                        Registros.Add(new Registro(pro.getCPUAcumulado(), pro.getNombre(), pro.getCPUAcumulado(), "Listo"));
                }
                else if (pro.getCPU() <= quantum)
                {
                    pro.agregarCPUAcumulado(quantum);
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Ejecucion"));
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), 0, "Terminado"));
                    pro.setCPU(0);
                }

                finIteraciones = true;

                for (int i = 0; i < tempLista.Count; i++)
                    if (tempLista[i].getCPU() > 0)
                        finIteraciones = false;
            }
        }

        private void Prioridad(int quantum)
        {
            bool finIteraciones = false;
            Proceso pro = null, proAnterior = null;
            List<Proceso> tempLista = new List<Proceso>();

            foreach (Proceso item in listaProceso)
                tempLista.Add(new Proceso(item.getID(), item.getNombre(),
                    item.getCPU(), item.getPrioridad(), item.getILlegada()));

            for (int j = 0; j <= tempLista.Count - 2; j++)
            {
                for (int i = 0; i <= tempLista.Count - 2; i++)
                {
                    if (tempLista[i].getPrioridad() > tempLista[i + 1].getPrioridad())
                    {
                        Proceso temp = tempLista[i + 1];
                        tempLista[i + 1] = tempLista[i];
                        tempLista[i] = temp;
                    }
                }
            }

            while (!finIteraciones)
            {
                pro = tempLista[tempLista.Count - 1];

                for (int i = tempLista.Count - 1; i >= 0; i--)
                {
                    if (proAnterior == null)
                    {
                        if (tempLista[i].getPrioridad() <= pro.getPrioridad())
                            pro = tempLista[i];
                    }
                    else
                    {
                        if (!proAnterior.getNombre().Equals(tempLista[i].getNombre()) && tempLista[i].getCPU() > 0 &&
                            tempLista[i].getPrioridad() <= pro.getPrioridad())
                            pro = tempLista[i];
                    }
                }

                proAnterior = pro;

                if (pro.getCPU() > quantum)
                {
                    pro.setCPU(pro.getCPU() - quantum);
                    pro.agregarCPUAcumulado(quantum);
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Ejecucion"));

                    if (pro.getCPU() > quantum)
                        Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Bloqueado"));
                    else
                        Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Listo"));
                }
                else if (pro.getCPU() <= quantum)
                {
                    pro.agregarCPUAcumulado(quantum);
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Ejecucion"));
                    Registros.Add(new Registro(pro.getID(), pro.getNombre(), 0, "Terminado"));
                    pro.setCPU(0);
                }

                finIteraciones = true;

                for (int i = 0; i < tempLista.Count; i++)
                    if (tempLista[i].getCPU() > 0)
                        finIteraciones = false;
            }
        }

        private void Prioridad_ILlegada_CPU()
        {
            /*Evalua primero la prioridad, si dos o mas procesos tiene prioridades iguales,
             * entonces evalua el CPU y si dos o mas procesos tienen el mismo nivel de prioridad y la
             * misma cantidad de CPU entonces, evalua la instancia de llegada.*/

            int tiempoGlobal = 0;
            bool finIteraciones = false;
            List<Proceso> tempLista = new List<Proceso>();

            foreach (Proceso item in listaProceso)
                tempLista.Add(new Proceso(item.getID(), item.getNombre(), item.getCPU(), item.getPrioridad(),
                    item.getILlegada()));

            for (int j = 0; j <= tempLista.Count - 2; j++)
            {
                for (int i = 0; i <= tempLista.Count - 2; i++)
                {
                    if (tempLista[i].getPrioridad() > tempLista[i + 1].getPrioridad())
                    {
                        Proceso temp = tempLista[i + 1];
                        tempLista[i + 1] = tempLista[i];
                        tempLista[i] = temp;
                    }
                }
            }

            Proceso pro = tempLista[tempLista.Count - 1];

            while (!finIteraciones)
            {
                for (int i = tempLista.Count - 1; i >= 0; i--)
                {
                    if (pro.getPrioridad() <= tempLista[i].getPrioridad())
                    {
                        pro = tempLista[i];

                        for (int x = tempLista.Count - 1; x >= 0; x--)
                        {
                            if (pro.getPrioridad() == tempLista[x].getPrioridad() && pro.getCPU() <= tempLista[x].getCPU())
                            {
                                pro = tempLista[x];

                                for (int z = tempLista.Count - 1; z >= 0; z--)
                                {
                                    if (pro.getPrioridad() == tempLista[z].getPrioridad() && pro.getCPU()
                                    == tempLista[z].getCPU() && pro.getILlegada() <= tempLista[z].getILlegada())
                                    {
                                        pro = tempLista[z];
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine(pro.getNombre());

                pro.agregarCPUAcumulado(pro.getCPU());
                Registros.Add(new Registro(pro.getID(), pro.getNombre(), pro.getCPUAcumulado(), "Ejecucion"));
                Registros.Add(new Registro(pro.getID(), pro.getNombre(), 0, "Terminado"));
                tempLista.Remove(pro);

                if (tempLista.Count != 0)
                    pro = tempLista[tempLista.Count - 1];

                if (tempLista.Count == 0)
                    finIteraciones = true;
            }
        }

        private void Garantizado(double quantum)
        {
            //Solo toma en cuenta la rafaga de CPU y la instancia de llegada
            int tiempoGlobal = 0;
            double recibeQuantum; //Es la cantidad de quantum que recibira cada proceso
            bool finIteraciones = false;

            List<Proceso> tempLista = new List<Proceso>();

            for (int i = 0; i < listaProceso.Count; i++)
            {
                tempLista.Add(new Proceso(listaProceso[i].getID(), listaProceso[i].getNombre(),
                    listaProceso[i].getCPU(), listaProceso[i].getPrioridad(), listaProceso[i].getILlegada()));
                tempLista[i].setAlgoPlaniCPU(tempLista[i].getCPU());
            }

            //Algoritmo de burbuja para ordenarlos por instancia de llegada
            for (int j = 0; j <= tempLista.Count - 2; j++)
            {
                for (int i = 0; i <= tempLista.Count - 2; i++)
                {
                    if (tempLista[i].getILlegada() > tempLista[i + 1].getILlegada())
                    {
                        Proceso temp = tempLista[i + 1];
                        tempLista[i + 1] = tempLista[i];
                        tempLista[i] = temp;
                    }
                }
            }


            while (!finIteraciones)
            {
                int n = 0;

                for (int i = 0; i < tempLista.Count; i++)
                {
                    //Console.WriteLine("Nombre: " + tempLista[i].getNombre() + " llegada: " + tempLista[i].getILlegada());
                    if (tempLista[i].getILlegada() <= tiempoGlobal && tempLista[i].getAlgoPlaniCPU() > 0)
                    {
                        n++; //Determinamos entre cuantos procesos se debe repartir el quantum
                        //Console.WriteLine("Entro");
                    }

                }

                //Console.WriteLine("Valor de n: " + n);

                recibeQuantum = ((1.00 / n) * quantum);
                //Console.WriteLine("RecibeQuantum: " + recibeQuantum);
                recibeQuantum = Math.Round(recibeQuantum, 2);
                //Console.WriteLine("RecibeQuantum: " + recibeQuantum);

                for (int i = 0; i < tempLista.Count; i++)
                {
                    if (tempLista[i].getILlegada() <= tiempoGlobal && tempLista[i].getAlgoPlaniCPU() > 0)
                    {
                        if (tempLista[i].getAlgoPlaniCPU() > recibeQuantum)
                        {
                            tempLista[i].setAlgoPlaniCPU(tempLista[i].getAlgoPlaniCPU() - recibeQuantum);
                            tempLista[i].agregarCPUAcumulado(Convert.ToInt32(recibeQuantum) * 100);

                            Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                Convert.ToInt32(tempLista[i].getAlgoPlaniCPU()), "Ejecucion"));
                            Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                Convert.ToInt32(tempLista[i].getAlgoPlaniCPU()), "Listo"));
                            //Console.WriteLine("Se le resto: " + recibeQuantum + " quantum restante: " + tempLista[i].getAlgoPlaniCPU());
                        }
                        else
                        {
                            //Proceso terminado
                            Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                Convert.ToInt32(tempLista[i].getAlgoPlaniCPU()), "Ejecucion"));
                            Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                0, "Terminado"));
                            tempLista[i].setAlgoPlaniCPU(0);
                        }
                    }
                }

                finIteraciones = true;

                for (int i = 0; i < tempLista.Count; i++)
                {
                    if (tempLista[i].getAlgoPlaniCPU() > 0)
                        finIteraciones = false;
                }

                tiempoGlobal++;
            }

        }

        private void AlgorPorLoteria(int quantum)
        {
            bool finIteraciones = false;
            int cantTicket = 0;
            Random rnd = new Random();
            List<Proceso> tempLista = new List<Proceso>();

            foreach (Proceso item in listaProceso)
                tempLista.Add(new Proceso(item.getID(), item.getNombre(), item.getCPU(),
                    item.getPrioridad(), item.getILlegada()));


            while (!finIteraciones)
            {
                Console.WriteLine(tempLista.Count);

                for (int i = 0; i < tempLista.Count; i++)
                {
                    tempLista[i].setTicket(cantTicket + tempLista[i].getPrioridad());
                    cantTicket += tempLista[i].getPrioridad();
                }

                bool procesoTerminado = false;

                while (!procesoTerminado)
                {
                    int loteria = rnd.Next(0, cantTicket);
                    Console.WriteLine(loteria);
                    for (int i = 0; i < tempLista.Count; i++)
                    {
                        if (loteria <= tempLista[i].getTicket())
                        {
                            if (tempLista[i].getCPU() <= quantum && tempLista[i].getCPU() != 0)
                            {
                                Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                    tempLista[i].getCPUAcumulado(), "Ejecucion"));

                                tempLista[i].setCPU(0);
                                Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                    0, "Terminado"));

                                procesoTerminado = true;
                            }
                            else if (tempLista[i].getCPU() != 0)
                            {
                                tempLista[i].setCPU(tempLista[i].getCPU() - quantum);
                                tempLista[i].agregarCPUAcumulado(quantum);

                                Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                    tempLista[i].getCPUAcumulado(), "Ejecucion"));

                                if (tempLista[i].getCPU() > quantum)
                                {
                                    Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                    tempLista[i].getCPUAcumulado(), "Listo"));
                                }
                                else
                                {
                                    Registros.Add(new Registro(tempLista[i].getID(), tempLista[i].getNombre(),
                                    tempLista[i].getCPUAcumulado(), "Bloqueado"));
                                }
                            }
                        }
                    }
                }

                finIteraciones = true;

                for (int i = 0; i < tempLista.Count; i++)
                {
                    if (tempLista[i].getCPU() > 0)
                        finIteraciones = false;
                }
            }

        }

        private void multiplesColas()
        {
            List<listaPrioridad> listaMultiplesColas = new List<listaPrioridad>();

            foreach (Proceso item in listaProceso)
            {
                bool crearCola = true;
                for (int i = 0; i < listaMultiplesColas.Count; i++)
                    if (item.getPrioridad() == listaMultiplesColas[i].numeroPrioridad)
                        crearCola = false;

                if (crearCola)
                    listaMultiplesColas.Add(new listaPrioridad(item.getPrioridad()));
            }

            foreach (Proceso item in listaProceso)
            {
                for (int i = 0; i < listaMultiplesColas.Count; i++)
                {
                    if (item.getPrioridad() == listaMultiplesColas[i].numeroPrioridad)
                        listaMultiplesColas[i].agregarProceso(item.getID(), item.getNombre(), item.getCPU(),
                            item.getPrioridad(), item.getILlegada());
                }
            }

            for (int j = 0; j <= listaMultiplesColas.Count - 2; j++)
            {
                for (int i = 0; i <= listaMultiplesColas.Count - 2; i++)
                {
                    if (listaMultiplesColas[i].numeroPrioridad > listaMultiplesColas[i + 1].numeroPrioridad)
                    {
                        listaPrioridad temp = listaMultiplesColas[i + 1];
                        listaMultiplesColas[i + 1] = listaMultiplesColas[i];
                        listaMultiplesColas[i] = temp;
                    }
                }
            }

            for (int i = 0; i < listaMultiplesColas.Count; i++)
            {
                ejecutarColasFIFO(listaMultiplesColas[i]);
            }
            
        }

        private void ejecutarColasFIFO(listaPrioridad listPrioridad)
        {
            /*Console.WriteLine("Numero de Prioridad: " + listPrioridad.numeroPrioridad + 
                "Cantiadad de elementos: " + listPrioridad.cantidadElementos());*/

            for (int i = 0; i < listPrioridad.listaProcesosPrioridad.Count; i++)
            {
                Registros.Add(new Registro(
                    listPrioridad.listaProcesosPrioridad[i].getID(), listPrioridad.listaProcesosPrioridad[i].getNombre(),
                    listPrioridad.listaProcesosPrioridad[i].getCPU(), "Ejecucion"));

                Registros.Add(new Registro(
                    listPrioridad.listaProcesosPrioridad[i].getID(), listPrioridad.listaProcesosPrioridad[i].getNombre(),
                    0, "Terminado"));
            }
        }

        private class listaPrioridad
        {

            public List<Proceso> listaProcesosPrioridad = new List<Proceso>();
            public int numeroPrioridad { get; set; }
            public listaPrioridad(int numeroPrioridad)
            {
                this.numeroPrioridad = numeroPrioridad;
            }

            public void agregarProceso(int ID, string nombre, int CPU, int Prioridad, int ILlegada)
            {
                listaProcesosPrioridad.Add(new Proceso(ID, nombre, CPU, Prioridad, ILlegada));
            }
        }

    }
}
