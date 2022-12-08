using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejemplo
{
    public class MarcoReferencia
    {
        public bool vacio { get; set; }
        private string ocupadoPor;
        public int tiempo; //Se utiliza para determinar aquel proceso mas viejo
        public bool permanecer {get; set;} //Determinamos si la referencia ocupada en el marco, permanece en el mismo
        public bool yaEvaluado { get; set; }


        public MarcoReferencia()
        {
            vacio = true;
            ocupadoPor = "-";
            permanecer = false; //false = que no permanece
            yaEvaluado = false;
        }

        public void setOcupadoPor(string referencia, int tiempo)
        {
            if(ocupadoPor == null)
            {
                ocupadoPor = referencia;
                this.tiempo = tiempo;
            }
            else if (ocupadoPor.Equals(referencia))
            {
                ocupadoPor = referencia;
            }
            else
            {
                ocupadoPor = referencia;
                this.tiempo = tiempo;
            }
        }

        public string getOcupadoPor()
        {
            return ocupadoPor;
        }
    }
}
