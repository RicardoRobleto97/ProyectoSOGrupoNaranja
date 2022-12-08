using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejemplo
{
    public class Registro //Los registros son los que nos van a ayudar a hacer las referencias
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public int cantCPU { get; set; }
        public string estado { get; set; }
        /* Estados:
         *  Ejecucion, Listo, Bloqueado, Terminado */
        
        

        public Registro(int codigo, string nombre, int cantCPU, string estado)
        {
            this.Id = codigo;
            this.nombre = nombre;
            this.cantCPU = cantCPU;
            this.estado = estado;
        }

        public Registro(int codigo, string nombre) //Este es para las referencias
        {
            this.Id = codigo;
            this.nombre = nombre;
        }

    }
}
