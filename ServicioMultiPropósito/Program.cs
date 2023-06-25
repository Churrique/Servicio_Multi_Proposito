using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServicioMultiPropósito
{
    public class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            ServiceBase [] ServicesToRun;
            ServicesToRun = new ServiceBase []
            {
                new SERVICIOdeARCHIVOS()
            };
            ServiceBase.Run( ServicesToRun );
        }
    }
}
