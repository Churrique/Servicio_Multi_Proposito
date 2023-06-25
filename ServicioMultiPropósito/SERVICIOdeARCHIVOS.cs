using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.VisualBasic.Devices;
using System.Security.Principal;
using System.Reflection;
using System.Collections.Specialized;

namespace ServicioMultiPropósito
{
    partial class SERVICIOdeARCHIVOS : ServiceBase
    {
        Computer MiPc = new Computer();
        bool blProcesoEnCurso = false;
        static StringCollection log = new StringCollection();

        public SERVICIOdeARCHIVOS()
        {
            InitializeComponent();
        }

        protected override void OnStart( string [] args )
        {
            StReloj.Start();
        }

        protected override void OnStop()
        {
            StReloj.Stop();
        }

        private void StReloj_Elapsed( object sender, System.Timers.ElapsedEventArgs e )
        {
            if (blProcesoEnCurso) return;
            try
            {
                blProcesoEnCurso = true;
                //
                Escribe_al_Visor_de_Eventos("La aplicación se ha iniciado con éxito...!", EventLogEntryType.Information);
                //
                string strProceso = ConfigurationManager.AppSettings[ "strAccion" ].ToString();
                //
                switch (strProceso)
                {
                    case "EliminarUnDirectorio":
                        string strCarpeta = ConfigurationManager.AppSettings[ "strEDirectorio" ].ToString();
                        string strDirectorioInicial = ConfigurationManager.AppSettings[ "strEDFuente" ].ToString();
                        EventLog.WriteEntry("El proceso a ejecutar es Eliminar un directorio desde esta ubicación [" + strCarpeta + "] ...!", EventLogEntryType.Information);
                        DirectoryInfo DirectorioRaiz = new DirectoryInfo(strDirectorioInicial);
                        WalkDirectoryTree(DirectorioRaiz, strProceso);
                        break;
                    case "CopiarArchivos":
                        string strRutaOrigen = ConfigurationManager.AppSettings["strCDFuente"].ToString();
                        DirectoryInfo CopiarDirRaiz = new DirectoryInfo(strRutaOrigen);
                        //MiPc.FileSystem.CopyDirectory(ConfigurationManager.AppSettings["strCDFuente"].ToString(), ConfigurationManager.AppSettings["strCDDestino"].ToString());
                        WalkDirectoryTree(CopiarDirRaiz, strProceso);
                        break;
                    case "MoverArchivos":
                        string strMoverRutaOrigen = ConfigurationManager.AppSettings["strMDFuente"].ToString();
                        DirectoryInfo MoverDirRaiz = new DirectoryInfo(strMoverRutaOrigen);
                        //MiPc.FileSystem.MoveDirectory(ConfigurationManager.AppSettings["strMDFuente"].ToString(), ConfigurationManager.AppSettings["strMDDestino"].ToString());
                        WalkDirectoryTree(MoverDirRaiz, strProceso);
                        break;
                    default:
                        break;
                }
                blProcesoEnCurso = false;
                Escribe_al_Visor_de_Eventos("Ha finalizado el proceso...!", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Escribe_al_Visor_de_Eventos( "Ha ocurrido una Excepción...!" + Environment.NewLine + ex.Message, EventLogEntryType.Error );
            }
        }

        public void WalkDirectoryTree( System.IO.DirectoryInfo root, string strProcesoEnCurso )
        {
            try
            {
                FileInfo[] MisArchivos = null;
                DirectoryInfo[] MisSubDirectorios = null;
                // Primero se procesan todos los archivos que estan bajo esta carpeta
                MisArchivos = root.GetFiles("*.*");
                if (MisArchivos != null)
                {
                    foreach (FileInfo Archivo in MisArchivos)
                    {
                        if (strProcesoEnCurso != "EliminarUnDirectorio")
                        {
                            string strDirDes = ConfigurationManager.AppSettings["strCDDestino"].ToString();
                            if (strProcesoEnCurso == "CopiarArchivos")
                            {
                                //< add key = "strCDFuente" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                                //< add key = "strCDDestino" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                                string Directorio_Destino = ConfigurationManager.AppSettings["strCDDestino"].ToString();
                                string Archivo_Destino = Path.Combine(Directorio_Destino, Archivo.Name);
                                string Archivo_Fuente = Path.Combine(root.FullName, Archivo.Name);
                                File.Copy(Archivo_Fuente, Archivo_Destino, true);
                            }
                            else
                            {
                                //<add key="strMDFuente" value="D:\Dev\Proyectos3W\jesusacosta.cz"/>
                                //< add key = "strMDDestino" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                                string Directorio_Destino = ConfigurationManager.AppSettings["strMDDestino"].ToString();
                                string Archivo_Destino = Path.Combine(Directorio_Destino, Archivo.Name);
                                string Archivo_Fuente = Path.Combine(root.FullName, Archivo.Name);
                                File.Move(Archivo_Fuente, Archivo_Destino);
                            }
                        }
                    }
                    // Después encontrar todos los sub-directorios que estan bajo el actual directorio
                    MisSubDirectorios = root.GetDirectories();
                    foreach (DirectoryInfo Directorio in MisSubDirectorios)
                    {
                        // Llamada recursiva para cada sub-directorio
                        if (strProcesoEnCurso == "EliminarUnDirectorio")
                        {
                            if (Directorio.Name == ConfigurationManager.AppSettings["strEDirectorio"].ToString())
                            {
                                Directory.Delete(Directorio.Name, true);
                            }
                        }
                        else if (strProcesoEnCurso == "CopiarArchivos")
                        {
                            //< add key = "strCDFuente" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                            //< add key = "strCDDestino" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                            string Directorio_Inicial = ConfigurationManager.AppSettings["strCDDestino"].ToString();
                            string Directorio_Destino = Path.Combine(Directorio_Inicial, root.FullName);
                            Directory.CreateDirectory(Directorio_Destino);
                        }
                        else
                        {
                            // MoverArchivos
                            //<add key="strMDFuente" value="D:\Dev\Proyectos3W\jesusacosta.cz"/>
                            //< add key = "strMDDestino" value = "D:\Dev\Proyectos3W\jesusacosta.cz" />
                            string Directorio_Inicial = ConfigurationManager.AppSettings["strMDDestino"].ToString();
                            string Directorio_Destino = Path.Combine(Directorio_Inicial, root.FullName);
                            Directory.Move(root.FullName, Directorio_Destino);
                        }
                        WalkDirectoryTree(Directorio, strProcesoEnCurso);
                    }
                }
            }
            catch (PathTooLongException eP)
            {
                log.Add("Excepción que se produce cuando una ruta o un nombre de archivo completo es más largo que la longitud máxima definida por el sistema." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("Excepción que se produce cuando una ruta o un nombre de archivo completo es más largo que la longitud máxima definida por el sistema." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (DirectoryNotFoundException eP)
            {
                log.Add("Excepción que se produce cuando no se puede encontrar parte de un archivo o directorio." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("Excepción que se produce cuando no se puede encontrar parte de un archivo o directorio." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (FileNotFoundException eP)
            {
                log.Add("La excepción que se lanza cuando falla un intento de acceder a un archivo que no existe en el disco." +
                    Environment.NewLine + "Excepción del tipo DIRECTORYNOTFOUNDEXCEPTION" + Environment.NewLine + eP.Message);
            }
            catch (IOException eP)
            {
                log.Add("La excepción que se lanza cuando se produce un error de E / S." +
                    Environment.NewLine + "Excepción del tipo ANAUTHORIZEDACCESSEXCEPTION" + Environment.NewLine + eP.Message);
            }
            catch (UnauthorizedAccessException eP)
            {
                log.Add("La excepción que se lanza cuando el sistema operativo deniega el acceso debido a un error de E / S o un tipo específico de error de seguridad." +
                    Environment.NewLine + "Excepción del tipo ANAUTHORIZEDACCESSEXCEPTION" + Environment.NewLine + eP.Message);
            }
            catch (Exception eP)
            {
                log.Add("Excepción genérica." + Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("Excepción genérica." + Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
        }

        public void Escribe_al_Visor_de_Eventos(string txtLinea, EventLogEntryType evtTipo)
        {
            WindowsIdentity Usuario = WindowsIdentity.GetCurrent();
            string fullName = Assembly.GetEntryAssembly().Location;
            string myAppName = Path.GetFileNameWithoutExtension(fullName);
            string txtDescriptivo = "Aplicación [ " + myAppName + " ]" + Environment.NewLine + "Usuario [ " + Usuario.Name + " ]" + Environment.NewLine + txtLinea;
            if (!EventLog.SourceExists("SERVICIOdeARCHIVOS"))
            {
                EventLog.CreateEventSource("SERVICIOdeARCHIVOS", "Application");
            }
            using (EventLog evtLog = new EventLog("Application"))
            {
                evtLog.Source = "SERVICIOdeARCHIVOS";
                evtLog.WriteEntry(txtDescriptivo, evtTipo, 1825);
            }
        }
    }
}
