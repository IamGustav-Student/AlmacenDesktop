using System;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class PagoForm : Form
    {
        public PagoForm()
        {
            InitializeComponent();
        }

        // Propiedad pública para sacar el dato fácil
        public decimal MontoIngresado
        {
            get { return numMonto.Value; }
        }
    }
}