using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Windows;

namespace Interfaz
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOpenDFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult respuesta = fbd.ShowDialog();
            if ( String.IsNullOrEmpty(respuesta.ToString()) || respuesta.ToString() == "Cancel") {
                System.Windows.Forms.MessageBox.Show("Debe indicar un directorio", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtDestino.Text = fbd.SelectedPath.ToString();
            }
        }

        private void BtnOpenFFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            DialogResult respuesta = fbd.ShowDialog();
            if (String.IsNullOrEmpty(respuesta.ToString()) || respuesta.ToString() == "Cancel")
            {
                System.Windows.Forms.MessageBox.Show("Debe indicar un directorio", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                txtDestino.Text = fbd.SelectedPath.ToString();
            }
        }
    }
}
