using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejemplo
{
    public class Referencia
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        
        //Marco es la posicion al marco de referencia en donde se asigna la referencia
        public int marco { get; set; }

        public bool fallo { get; set; }

        public Referencia(int Id, string nombre)
        {
            this.Id = Id;
            this.nombre = nombre;
            marco = -1;
            fallo = false;
        }

    }
}
