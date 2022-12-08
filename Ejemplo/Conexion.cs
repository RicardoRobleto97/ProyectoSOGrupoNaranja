using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejemplo
{

    public  class Conexion
    {
        ProcesosEntities db = new ProcesosEntities();

        public Conexion()
        {

        }
        public bool ValidarCode(int id)
        {
            var result = db.PROCESS.Where(e => e.ID == id);
            int count = result.ToList().Count();
            if (count > 0)
                return true;
            else
                return false;
        }
        public List<PROCESS> GetPROCESS()
        {
            var Query = from emple in db.PROCESS
                        select emple;
            return Query.ToList();
        }

        public void GuardarProcess(int id, string nombre, int cpu, int tillegada, int prioridad)
        {
            PROCESS np = new PROCESS()
            {
                ID = id,
                Nombre = nombre,
                CPU = cpu,
                TiLLegada = tillegada,
                Prioridad = prioridad
            };
            db.PROCESS.Add(np);
            db.SaveChanges();
        }
        public void UpdateProcess(int id, string nombre, int cpu, int tillegada, int prioridad)
        {
            PROCESS emp = (from e in db.PROCESS
                           where e.ID == id
                           select e).FirstOrDefault();

            emp.ID = id;
            emp.Nombre = nombre;
            emp.CPU = cpu;
            emp.TiLLegada = tillegada;
            emp.Prioridad = prioridad;

            db.SaveChanges();
        }
        public void DeleteProcesss(int ID)
        {
            PROCESS emple = (from e in db.PROCESS
                             where e.ID == ID
                             select e).FirstOrDefault();
            db.PROCESS.Remove(emple);//entity framework
            db.SaveChanges(); // commit [dele from
        }
        public List<PROCESS> Find(int ID)
        {
            var Query = from emple in db.PROCESS
                        select emple;
            return Query.Where(e => e.ID == ID).ToList();
        }
    }
}