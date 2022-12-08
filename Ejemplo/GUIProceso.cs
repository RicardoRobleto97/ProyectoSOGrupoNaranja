using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejemplo
{
    class GUIProceso
    {
        public int Id { get; set; }
        public string nombreProceso { get; set; }
        public Label etiqueta;
        public ProgressBar barra;
        public Label estado;

        public GUIProceso(int posicion, int Id, string nombre, Control.ControlCollection controls, int valorCPU)
        {
            this.Id = Id;
            nombreProceso = nombre;

            etiqueta = new Label();
            etiqueta.Text = nombre;
            etiqueta.Location = new Point(20, 40 + (35 * posicion));
            etiqueta.Width = 130;
            etiqueta.Height = 22;
            etiqueta.TextAlign = ContentAlignment.MiddleCenter;
            controls.Add(etiqueta);

            barra = new ProgressBar();
            barra.Location = new Point(180, 40 + (35 * posicion));
            barra.Name = nombre;
            barra.Width = 200;
            barra.Height = 22;
            barra.Maximum = valorCPU;
            controls.Add(barra);

            estado = new Label();
            estado.Location = new Point(425, 40 + (35 * posicion));
            estado.Width = 22;
            estado.Height = 22;
            estado.BackColor = Color.Yellow;
            estado.TextAlign = ContentAlignment.MiddleCenter;
            controls.Add(estado);
        }

        public void setBarraValue(int value)
        {
            if (value > barra.Maximum)
                barra.Value = barra.Maximum;
            else
                barra.Value = value;
        }
    }
}
