namespace ServicioMultiPropósito
{
    partial class SERVICIOdeARCHIVOS
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.StReloj = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.StReloj)).BeginInit();
            // 
            // StReloj
            // 
            this.StReloj.Enabled = true;
            this.StReloj.Interval = 30000D;
            this.StReloj.Elapsed += new System.Timers.ElapsedEventHandler(this.StReloj_Elapsed);
            // 
            // EliminaDirectorios
            // 
            this.ServiceName = "SERVICIOdeARCHIVOS";
            ((System.ComponentModel.ISupportInitialize)(this.StReloj)).EndInit();
        }

        #endregion

        private System.Timers.Timer StReloj;
    }
}
