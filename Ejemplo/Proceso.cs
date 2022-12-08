using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejemplo
{
    //Importante colocar public al inicio de la clase, de lo contrario se volvera private de forma implicita
    public class Proceso
    {
        private int ID; //Agregar ID
        private string nombre;
        private int CPU; //Esta propiedad se va a reduciendo a medida se ejecuta el algoritmo por X proceso
        private int Prioridad;
        private int ILlegada;
        //Cada vez que se reste al CPU, la cantidad restada se acumula en CPUAcumulado
        public int CPUAcumulado;
        
        //private bool agregado;
        private double Ticket;
        private double AlgoPlaniCPU;

        public Proceso(int ID,string nombre, int CPU, int Prioridad, int ILlegada)
        {
            this.ID = ID;
            this.nombre = nombre;
            this.CPU = CPU;
            this.Prioridad = Prioridad;
            this.ILlegada = ILlegada;
            CPUAcumulado = 0;

            AlgoPlaniCPU = 0.00;
        }

        public void agregarCPUAcumulado(int agregar)
        {
            CPUAcumulado += agregar;
        }

        public int getCPUAcumulado()
        {
            return CPUAcumulado;
        }
        public void setID(int ID)
        {
            this.ID = ID;
        }

        public int getID()
        {
            return ID;
        }

        public void setNombre(string nombre)
        {
            this.nombre = nombre;
        }

        public string getNombre()
        {
            return nombre;
        }

        public void setPrioridad(int Prioridad)
        {
            this.Prioridad = Prioridad;
        }

        public int getPrioridad()
        {
            return this.Prioridad;
        }

        public void setILlegada(int ILlegada)
        {
            this.ILlegada = ILlegada;
        }

        public int getILlegada()
        {
            return this.ILlegada;
        }

        public void setCPU(int CPU)
        {
            this.CPU = CPU;
        }

        public int getCPU()
        {
            return this.CPU;
        }

        public void setTicket(double Ticket)
        {
            this.Ticket = Ticket;
        }

        public double getTicket()
        {
            return Ticket;
        }

        public void setAlgoPlaniCPU(double AlgoPlaniCPU)
        {
            this.AlgoPlaniCPU = AlgoPlaniCPU;
        }
        public double getAlgoPlaniCPU()
        {
            return this.AlgoPlaniCPU;
        }

    }
}
