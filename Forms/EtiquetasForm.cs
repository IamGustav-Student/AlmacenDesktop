using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services; // Usamos nuestro nuevo servicio
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class EtiquetasForm : Form
    {
        private List<PictureBox> _colaEtiquetas = new List<PictureBox>();
        private BarcodeService _barcodeService; // Instancia del servicio

        public EtiquetasForm()
        {
            InitializeComponent();
            _barcodeService = new BarcodeService(); // Inicializamos el servicio

            // Atajos sagrados
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(EtiquetasForm_KeyDown);
        }

        private void EtiquetasForm_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            using (var context = new AlmacenDbContext())
            {
                var lista = context.Productos
                    .Where(p => !string.IsNullOrEmpty(p.CodigoBarras))
                    .OrderBy(p => p.Nombre)
                    .ToList();

                cboProductos.DataSource = lista;
                cboProductos.DisplayMember = "Nombre";
                cboProductos.ValueMember = "Id";
                cboProductos.SelectedIndex = -1;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            AgregarEtiquetas();
        }

        private void AgregarEtiquetas()
        {
            var producto = (Producto)cboProductos.SelectedItem;
            if (producto == null) return;

            int cantidad = (int)numCopias.Value;

            try
            {
                // DELEGAMOS LA RESPONSABILIDAD AL SERVICIO
                // El formulario no sabe cómo se genera, solo pide la imagen.
                using (Bitmap imgBarcode = _barcodeService.GenerarCodigoBarras(producto.CodigoBarras))
                {
                    if (imgBarcode == null) return;

                    for (int i = 0; i < cantidad; i++)
                    {
                        // Pasamos un clon de la imagen para que cada PictureBox tenga su copia
                        PictureBox pb = CrearPanelEtiqueta(producto, (Image)imgBarcode.Clone());
                        pnlPreview.Controls.Add(pb);
                        _colaEtiquetas.Add(pb);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private PictureBox CrearPanelEtiqueta(Producto prod, Image codigoBarras)
        {
            PictureBox pb = new PictureBox();
            pb.Size = new Size(220, 140);
            pb.BackColor = Color.White;
            pb.BorderStyle = BorderStyle.FixedSingle;
            pb.Margin = new Padding(5);

            Bitmap bitmapFinal = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(bitmapFinal))
            {
                g.Clear(Color.White);
                int y = 5;
                StringFormat centro = new StringFormat() { Alignment = StringAlignment.Center };

                if (chkNombre.Checked)
                {
                    string nombreCorto = prod.Nombre.Length > 25 ? prod.Nombre.Substring(0, 25) + "..." : prod.Nombre;
                    g.DrawString(nombreCorto, new Font("Arial", 9, FontStyle.Bold), Brushes.Black, new RectangleF(0, y, pb.Width, 20), centro);
                    y += 20;
                }

                // Centrar imagen
                int xImg = (pb.Width - 180) / 2;
                if (xImg < 0) xImg = 0;
                g.DrawImage(codigoBarras, xImg, y, 180, 60);
                y += 65;

                if (chkPrecio.Checked)
                {
                    g.DrawString($"$ {prod.Precio:N2}", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new RectangleF(0, y, pb.Width, 30), centro);
                }
            }

            pb.Image = bitmapFinal;
            return pb;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCola();
        }

        private void LimpiarCola()
        {
            foreach (Control c in pnlPreview.Controls) c.Dispose();
            pnlPreview.Controls.Clear();
            _colaEtiquetas.Clear();
            cboProductos.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_colaEtiquetas.Count == 0) return;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;

            PrintDialog dialog = new PrintDialog();
            dialog.Document = pd;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private int _indiceImpresion = 0;

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            int x = 50;
            int y = 50;
            int anchoEtiq = 230;
            int altoEtiq = 150;

            int columnas = 3;
            int filas = 6;

            int colActual = 0;
            int filaActual = 0;

            while (_indiceImpresion < _colaEtiquetas.Count)
            {
                var pb = _colaEtiquetas[_indiceImpresion];
                e.Graphics.DrawImage(pb.Image, x + (colActual * anchoEtiq), y + (filaActual * altoEtiq));

                _indiceImpresion++;
                colActual++;

                if (colActual >= columnas)
                {
                    colActual = 0;
                    filaActual++;
                }

                if (filaActual >= filas)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            e.HasMorePages = false;
            _indiceImpresion = 0;
        }

        private void EtiquetasForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnImprimir.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (_colaEtiquetas.Count > 0)
                {
                    if (MessageBox.Show("¿Limpiar cola?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) LimpiarCola();
                }
                else
                {
                    this.Close();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && !btnImprimir.Focused)
            {
                btnAgregar.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}