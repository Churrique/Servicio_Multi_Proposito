using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.VisualBasic.Devices;
using System.Security.Principal;
using System.Reflection;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;


namespace InterfazMejorada
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Computer MiPc = new Computer();
        bool blProcesoEnCurso = false, blEstoyEnRaiz = true, blChequeaValorEnLlave = true;
        static StringCollection log = new StringCollection();
        string strPuntoDePartida = "";
        #region TEST
        int intCiclos = 0;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            txtDAEliminar.Text = ConfigurationManager.AppSettings["strDAEliminar"].ToString();
            txtDestino.Text = ConfigurationManager.AppSettings["strDDestino"].ToString();
            txtFuente.Text = ConfigurationManager.AppSettings["strDFuente"].ToString();
        }

        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            #region BLOQUE DE PRUEBA EXPORTAR DESPUÉS...!
            if (blProcesoEnCurso) return;
            try
            {
                blProcesoEnCurso = true;
                //
                Escribe_al_Visor_de_Eventos("La aplicación se ha iniciado con éxito...!", EventLogEntryType.Information);
                //
                //string strProceso = ConfigurationManager.AppSettings["strAccion"].ToString();
                string strProceso = ProcesoAEjecutar(blChequeaValorEnLlave);
                //
                switch (strProceso)
                {
                    case "EliminarUnDirectorio":
                        Escribe_al_Visor_de_Eventos("El proceso a ejecutar es Eliminar un Directorio desde esta ubicación [" + txtFuente.Text + "] ...!", EventLogEntryType.Information);
                        DirectoryInfo DirectorioRaiz = new DirectoryInfo(txtFuente.Text);
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso va a empezar.", "Eliminar Directorio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        WalkDirectoryTree(DirectorioRaiz, strProceso);
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso ha terminado.", "Eliminar Directorio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        break;
                    case "CopiarArchivos":
                        Escribe_al_Visor_de_Eventos("El proceso a ejecutar es Copiar un Directorio desde esta ubicación [" + txtFuente.Text + "] ...!", EventLogEntryType.Information);
                        DirectoryInfo CopiarDirRaiz = new DirectoryInfo(txtFuente.Text);
                        //MiPc.FileSystem.CopyDirectory(ConfigurationManager.AppSettings["strCDFuente"].ToString(), ConfigurationManager.AppSettings["strCDDestino"].ToString());
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso va a empezar.", "Copiar Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        WalkDirectoryTree(CopiarDirRaiz, strProceso);
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso ha terminado.", "Copiar Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        break;
                    case "MoverArchivos":
                        Escribe_al_Visor_de_Eventos("El proceso a ejecutar es Mover un Directorio desde esta ubicación [" + txtFuente.Text + "] ...!", EventLogEntryType.Information);
                        DirectoryInfo MoverDirRaiz = new DirectoryInfo(txtFuente.Text);
                        //MiPc.FileSystem.MoveDirectory(ConfigurationManager.AppSettings["strMDFuente"].ToString(), ConfigurationManager.AppSettings["strMDDestino"].ToString());
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso va a empezar.", "Mover Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        WalkDirectoryTree(MoverDirRaiz, strProceso);
                        #region ...TEST...
                        System.Windows.Forms.MessageBox.Show("El proceso ha terminado.", "Mover Archivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        #endregion
                        break;
                    default:
                        #region ...TEST...
                        DialogResult dlgRespondio = System.Windows.Forms.MessageBox.Show("No se tiene ningun caso para el [switch] que se plantea...!" + Environment.NewLine + 
                            "Si desea parar la ejecución pulse en CANCELAR", "Cuidado...!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        //
                        if (dlgRespondio == System.Windows.Forms.DialogResult.Cancel)
                        {
                            Environment.Exit(0);
                        }
                        #endregion
                        break;
                }
                blProcesoEnCurso = false;
                Escribe_al_Visor_de_Eventos("Ha finalizado el proceso...!", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Escribe_al_Visor_de_Eventos("Ha ocurrido una Excepción...!" + Environment.NewLine + ex.Message, EventLogEntryType.Error);
            }
            #endregion
            #region Código para cuando este listo el servicio e instalado
            // Pendiente por codificar
            #endregion
        }

        private void BtnOpenFFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult respuesta = fbd.ShowDialog();
            if (String.IsNullOrEmpty(respuesta.ToString()) || respuesta.ToString() == "Cancel")
            {
                System.Windows.Forms.MessageBox.Show("Debe indicar un directorio...!", "Advertencia...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtFuente.Text = fbd.SelectedPath.ToString();
                string strCaracter = "\\";
                string strCadena = fbd.SelectedPath.ToString();
                int numPosicion = strCadena.LastIndexOf(strCaracter) + 1;
                int numLongitud = strCadena.Length;
                int numNumeroDeCaracteres = numLongitud - numPosicion;
                strPuntoDePartida = strCadena.Substring(numPosicion, numNumeroDeCaracteres);
            }
        }

        private void BtnOpenDFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult respuesta = fbd.ShowDialog();
            if (String.IsNullOrEmpty(respuesta.ToString()) || respuesta.ToString() == "Cancel")
            {
                System.Windows.Forms.MessageBox.Show("Debe indicar un directorio...!", "Advertencia...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtDestino.Text = fbd.SelectedPath.ToString();
            }
        }

        private void BtnDAEliminar_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult respuesta = fbd.ShowDialog();
            if (String.IsNullOrEmpty(respuesta.ToString()) || respuesta.ToString() == "Cancel")
            {
                System.Windows.Forms.MessageBox.Show("Debe indicar un directorio", "Advertencia...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string strCaracter = "\\";
                string strCadena = fbd.SelectedPath.ToString();
                int numPosicion = strCadena.LastIndexOf(strCaracter) + 1;
                int numLongitud = strCadena.Length;
                int numNumeroDeCaracteres = numLongitud - numPosicion;
                txtDAEliminar.Text = strCadena.Substring(numPosicion, numNumeroDeCaracteres);
            }
        }

        public void WalkDirectoryTree(DirectoryInfo root, string strProcesoEnCurso)
        {
            #region ...CUERPO CENTRAL...
            try
            {
                FileInfo[] MisArchivos = null;
                DirectoryInfo[] MisSubDirectorios = null;
                #region ...TEST...
                intCiclos++;
                System.Windows.Forms.MessageBox.Show("Ciclo["+intCiclos.ToString()+"] Voy a procesar desde aquí " + root.FullName, "Información - WalkDirectoryTree", MessageBoxButtons.OK, MessageBoxIcon.Information);
                #endregion
                #region ...Procesando los Archivos...
                // Primero se procesan todos los archivos que estan bajo esta carpeta
                MisArchivos = root.GetFiles("*.*");
                if (MisArchivos != null)
                {
                    foreach (FileInfo Archivo in MisArchivos)
                    {
                        intCiclos++;
                        if (strProcesoEnCurso != "EliminarUnDirectorio")
                        {
                            if (strProcesoEnCurso == "CopiarArchivos")
                            {
                                if (blEstoyEnRaiz)
                                {
                                    string Archivo_Destino = System.IO.Path.Combine(txtDestino.Text, Archivo.Name);
                                    string Archivo_Fuente = System.IO.Path.Combine(root.FullName, Archivo.Name);
                                    #region ...TEST...
                                    System.Windows.Forms.MessageBox.Show("Voy a copiar de:" + Environment.NewLine +
                                        Archivo_Fuente + Environment.NewLine +
                                        "a" + Environment.NewLine +
                                        Archivo_Destino + Environment.NewLine,
                                        "[Raiz] Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    #endregion
                                    File.Copy(Archivo_Fuente, Archivo_Destino, true);
                                }
                                else   // no estoy en blEstoyEnRaiz
                                {
                                    int numLongitudDePuntoDePartida = strPuntoDePartida.Length;
                                    int numPosicionDeCoincidencia = root.FullName.LastIndexOf(strPuntoDePartida) + numLongitudDePuntoDePartida;
                                    int numCaracteresTotales = root.FullName.Length - (numPosicionDeCoincidencia + 1);
                                    string strSegmento = root.FullName.Substring(numPosicionDeCoincidencia + 1 , numCaracteresTotales);
                                    #region ...TEST...
                                    DialogResult dlgRespuesta = System.Windows.Forms.MessageBox.Show("Valores: " + Environment.NewLine +
                                        "root.FullName = " + root.FullName + Environment.NewLine +
                                        "numLongitudDePuntoDePartida = " + Convert.ToString(numLongitudDePuntoDePartida) + Environment.NewLine +
                                        "numPosicionDeCoincidencia = " + Convert.ToString(numPosicionDeCoincidencia) + Environment.NewLine +
                                        "numCaracteresTotales = " + Convert.ToString(numCaracteresTotales) + Environment.NewLine +
                                        "strSegmento = " + strSegmento + Environment.NewLine +
                                        "Combine(txtDestino.Text, strSegmento)" + System.IO.Path.Combine(txtDestino.Text, strSegmento) + Environment.NewLine +
                                        "Archivo_Fuente = " + System.IO.Path.Combine(root.FullName, Archivo.Name) + Environment.NewLine +
                                        "Archivo_Destino = " + System.IO.Path.Combine(txtDestino.Text, strSegmento, Archivo.Name) + Environment.NewLine +
                                        "¿Desea continuar...?", "Comprovación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (dlgRespuesta.ToString() == "No")
                                    {
                                        Environment.Exit(0);
                                    }
                                    #endregion
                                    string Archivo_Fuente = System.IO.Path.Combine(root.FullName, Archivo.Name);
                                    string Archivo_Destino = System.IO.Path.Combine(txtDestino.Text, strSegmento, Archivo.Name);
                                    if (!Directory.Exists(System.IO.Path.Combine(txtDestino.Text, strSegmento)))
                                    {
                                        #region ...TEST...
                                        System.Windows.Forms.MessageBox.Show("Se ha a crear el directorio" + Environment.NewLine + System.IO.Path.Combine(txtDestino.Text, strSegmento), "Acción", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        #endregion
                                        Directory.CreateDirectory(System.IO.Path.Combine(txtDestino.Text, strSegmento));
                                    }
                                    File.Copy(Archivo_Fuente, Archivo_Destino, true);
                                }
                            }   // if -> CopiarArchivos
                            else   // MoverArchivos
                            {
                                string ADestino = System.IO.Path.Combine(txtDestino.Text, Archivo.Name);
                                string AFuente = System.IO.Path.Combine(root.FullName, Archivo.Name);
                                if (!File.Exists(ADestino))
                                {
                                    #region
                                    System.Windows.Forms.MessageBox.Show("Estoy en la raiz y el archivo no existe en el destino" + Environment.NewLine +
                                        "AFuente= " + AFuente + Environment.NewLine +
                                        "ADestino= " + ADestino, "Información - MoverArchivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    #endregion
                                    File.Move(AFuente, ADestino);
                                }
                                else
                                {
                                    #region
                                    System.Windows.Forms.MessageBox.Show("Estoy en la raiz y el archivo si existe en el destino" + Environment.NewLine +
                                        "AFuente= " + AFuente + Environment.NewLine +
                                        "ADestino= " + ADestino, "Información - MoverArchivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    #endregion
                                    File.Delete(ADestino);
                                    File.Move(AFuente, ADestino);
                                }
                            }   // else -> MoverArchivos
                        }   // if -> strProcesoEnCurso != EliminarUnDirectorio
                    }   // foreach -> Archivo
                }   // if -> MisArchivos != null
                #endregion
                #region ...Procesando los Directorios...
                // Después encontrar todos los sub-directorios que estan bajo el actual directorio
                MisSubDirectorios = root.GetDirectories();
                if (MisSubDirectorios != null)
                {
                    foreach (DirectoryInfo Directorio in MisSubDirectorios)
                    {
                        #region ...TEST...
                        intCiclos++;
                        #endregion
                        // Llamada recursiva para cada sub-directorio
                        if (strProcesoEnCurso == "EliminarUnDirectorio")
                        {
                            if (Directorio.Name == txtDAEliminar.Text)
                            {
                                #region ...TEST...
                                System.Windows.Forms.MessageBox.Show("Me dispongo a eliminar el directorio " + Directorio.Name + Environment.NewLine +
                                    "Directorio [Directorio.FullName])" + Environment.NewLine + 
                                    Directorio.FullName, "Información - EliminarUnDirectorio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                #endregion
                                Directory.Delete(Directorio.FullName, true);
                            }
                        }
                        else if (strProcesoEnCurso == "CopiarArchivos")
                        {
                            // En la rutina anterior (Archivos), el Directorio es creado.
                            // Para este saco no se hace nada.
                        }
                        else // MoverArchivos
                        {
                            string Directorio_Destino = System.IO.Path.Combine(txtDestino.Text , Directorio.Name);
                            #region ...TEST...
                            System.Windows.Forms.MessageBox.Show("Me dispongo a mover el directorio [" + Directorio.Name + "]" + Environment.NewLine + 
                                "Ubicado en la ruta (FullName)" + Environment.NewLine + Directorio.FullName + Environment.NewLine +
                                "Directorio_Destino= " + Directorio_Destino + Environment.NewLine,
                                "Información - MoverArchivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            #endregion
                            if (!Directory.Exists(Directorio_Destino))
                            {
                                #region ...TEST...
                                System.Windows.Forms.MessageBox.Show("El directorio [" + Directorio.Name + "] no existe en la ruta:" + Environment.NewLine +
                                    "Directorio_Destino = " + Directorio_Destino + Environment.NewLine + 
                                    "root.FullName =" + Environment.NewLine + root.FullName + Environment.NewLine + 
                                    "Directorio.FullName =" + Environment.NewLine + Directorio.FullName, "Información - MoverArchivos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                #endregion
                                Directory.Move(Directorio.FullName, Directorio_Destino);
                            }
                        }
                        #region ...Se llama RECURSIVAMENTE si es EliminarUnDirectorio ó CopiarArchivos...
                        if (strProcesoEnCurso != "MoverArchivos")
                        {
                            blEstoyEnRaiz = false;
                            WalkDirectoryTree(Directorio, strProcesoEnCurso);
                        }
                        #endregion
                    }   // foreach -> Directorio
                }   // if -> MisSubDirectorios != null
                #endregion
            }
            #endregion
            #region ...EXCEPCIONES...
            catch (PathTooLongException eP)
            {
                log.Add("<PathTooLongException> Excepción que se produce cuando una ruta o un nombre de archivo completo es más largo que la longitud máxima definida por el sistema." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<PathTooLongException> Excepción que se produce cuando una ruta o un nombre de archivo completo es más largo que la longitud máxima definida por el sistema." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (DirectoryNotFoundException eP)
            {
                log.Add("<DirectoryNotFoundException> Excepción que se produce cuando no se puede encontrar parte de un archivo o directorio." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<DirectoryNotFoundException> Excepción que se produce cuando no se puede encontrar parte de un archivo o directorio." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (FileNotFoundException eP)
            {
                log.Add("<FileNotFoundException> La excepción que se lanza cuando falla un intento de acceder a un archivo que no existe en el disco." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<FileNotFoundException> La excepción que se lanza cuando falla un intento de acceder a un archivo que no existe en el disco." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (IOException eP)
            {
                log.Add("<IOException> La excepción que se lanza cuando se produce un error de E / S." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<IOException> La excepción que se lanza cuando se produce un error de E / S." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (UnauthorizedAccessException eP)
            {
                log.Add("<UnauthorizedAccessException> La excepción que se lanza cuando el sistema operativo deniega el acceso debido a un error de E / S o un tipo específico de error de seguridad." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<UnauthorizedAccessException> La excepción que se lanza cuando el sistema operativo deniega el acceso debido a un error de E / S o un tipo específico de error de seguridad." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (NotSupportedException eP)
            {
                log.Add("<NotSupportedException> La excepción que se lanza cuando el nombre del Archivo/Direcorio fuente o el nombre del Archivo/Direcorio destino tiene un formato no válido." +
                    Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<NotSupportedException> La excepción que se lanza cuando el nombre del Archivo/Direcorio fuente o el nombre del Archivo/Direcorio destino tiene un formato no válido." +
                    Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            catch (Exception eP)
            {
                log.Add("<Exception> Excepción genérica." + Environment.NewLine + eP.Message);
                Escribe_al_Visor_de_Eventos("<Exception> Excepción genérica." + Environment.NewLine + eP.Message, EventLogEntryType.Error);
            }
            #endregion
        }

        public void Escribe_al_Visor_de_Eventos(string txtLinea, EventLogEntryType evtTipo)
        {
            WindowsIdentity Usuario = WindowsIdentity.GetCurrent();
            string fullName = Assembly.GetEntryAssembly().Location;
            string myAppName = System.IO.Path.GetFileNameWithoutExtension(fullName);
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

        public string ProcesoAEjecutar(bool blChequeoLlave)
        {
            if (blChequeoLlave)
            {
                return (ConfigurationManager.AppSettings["strAccion"].ToString());
            }
            else
            {
                if (rdbEliminar.IsChecked == true)
                {
                    return "EliminarUnDirectorio";
                }
                else if (rdbCopiar.IsChecked == true)
                {
                    return "CopiarArchivos";
                }
                else if (rdbMover.IsChecked == true)
                {
                    return "MoverArchivos";
                }
                else
                {
                    return "NoHayValor";
                }
            }
        }
    }
}
