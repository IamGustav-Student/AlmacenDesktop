using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
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
        // Cola de objetos puros (datos), no controles visuales
        private List<Producto> _colaImpresion = new List<Producto>();

        // Cache de imágenes de códigos de barra para no regenerarlos en cada repintado (Performance)
        private Dictionary<int, Bitmap> _cacheBarcodes = new Dictionary<int, Bitmap>();

        private BarcodeService _barcodeService;

        // --- CONFIGURACIÓN DE LA HOJA (Ajustable) ---
        // Tamaño típico de etiqueta adhesiva o celda en hoja A4 troquelada
        private int _anchoEtiqueta = 220;
        private int _altoEtiqueta = 140;
        private int _margenX = 30; // Margen izquierdo de la hoja
        private int _margenY = 30; // Margen superior de la hoja
        private int _espacioEntre = 10; // Separación entre etiquetas

        public EtiquetasForm()
        {
            InitializeComponent();
            _barcodeService = new BarcodeService();

            // Configuración visual del panel de vista previa
            pnlPreview.AutoScroll = true;
            pnlPreview.BackColor = Color.LightGray;

            // Habilitar KeyPreview para atajos globales en el form
            this.KeyPreview = true;
            this.KeyDown += EtiquetasForm_KeyDown;
        }

        private void EtiquetasForm_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            using (var context = new AlmacenDbContext())
            {
                // Solo traemos productos que tengan código de barras cargado
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
            var producto = (Producto)cboProductos.SelectedItem;
            if (producto == null) return;

            int cantidad = (int)numCopias.Value;

            // 1. Generar código de barras una sola vez y cachearlo en memoria
            if (!_cacheBarcodes.ContainsKey(producto.Id))
            {
                var bmp = _barcodeService.GenerarCodigoBarras(producto.CodigoBarras);
                if (bmp != null) _cacheBarcodes[producto.Id] = bmp;
            }

            // 2. Agregar a la cola lógica (datos)
            for (int i = 0; i < cantidad; i++)
            {
                _colaImpresion.Add(producto);
            }

            // 3. Reflejar cambios en pantalla
            ActualizarPrevisualizacion();
        }

        private void ActualizarPrevisualizacion()
        {
            // Limpiamos visualmente el panel
            // NOTA: Borrar controles es lento, en una app muy grande optimizaríamos esto,
            // pero para < 50 etiquetas está bien.
            pnlPreview.Controls.Clear();

            foreach (var prod in _colaImpresion)
            {
                // Creamos un PictureBox simple solo para mostrar en pantalla
                PictureBox pb = new PictureBox();
                pb.Size = new Size(_anchoEtiqueta, _altoEtiqueta);
                pb.BackColor = Color.White;
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.Margin = new Padding(5);

                // Dibujamos la etiqueta en un Bitmap para asignarlo al PictureBox
                Bitmap previewBmp = new Bitmap(_anchoEtiqueta, _altoEtiqueta);
                using (Graphics g = Graphics.FromImage(previewBmp))
                {
                    g.Clear(Color.White);
                    // IMPORTANTE: Reutilizamos la misma lógica que usará la impresora
                    Bitmap barcode = _cacheBarcodes.ContainsKey(prod.Id) ? _cacheBarcodes[prod.Id] : null;

                    // Renderizado antialias para que se vea lindo en pantalla
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    EtiquetaRenderer.Dibujar(g, new RectangleF(0, 0, _anchoEtiqueta, _altoEtiqueta), prod, barcode, chkPrecio.Checked, chkNombre.Checked);
                }

                pb.Image = previewBmp;
                pnlPreview.Controls.Add(pb);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            _colaImpresion.Clear();
            pnlPreview.Controls.Clear();
            cboProductos.Focus();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_colaImpresion.Count == 0)
            {
                MessageBox.Show("No hay etiquetas en la cola para imprimir.", "Cola Vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Pd_PrintPage;

            // Diálogo estándar de Windows para elegir impresora
            PrintDialog dialog = new PrintDialog();
            dialog.Document = pd;
            dialog.UseEXDialog = true; // Recomendado para Windows modernos

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _indiceImpresion = 0; // Reiniciamos el contador global antes de empezar
                    pd.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar imprimir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Variable de estado para controlar qué etiqueta toca imprimir
        private int _indiceImpresion = 0;

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Coordenadas iniciales
            float xActual = _margenX;
            float yActual = _margenY;

            // Calculamos cuántas etiquetas entran por fila en esta hoja
            int anchoPaginaUtil = e.PageBounds.Width - (_margenX * 2);
            int columnasPosibles = (int)(anchoPaginaUtil / (_anchoEtiqueta + _espacioEntre));

            if (columnasPosibles < 1) columnasPosibles = 1; // Seguridad por si la etiqueta es gigante

            int colCount = 0;

            // Iteramos mientras queden etiquetas en la cola
            while (_indiceImpresion < _colaImpresion.Count)
            {
                var prod = _colaImpresion[_indiceImpresion];
                Bitmap barcode = _cacheBarcodes.ContainsKey(prod.Id) ? _cacheBarcodes[prod.Id] : null;

                // Definimos el área exacta donde se dibujará ESTA etiqueta
                RectangleF rectEtiqueta = new RectangleF(xActual, yActual, _anchoEtiqueta, _altoEtiqueta);

                // --- MAGIA: DIBUJAR VECTORES DIRECTO A LA IMPRESORA ---
                EtiquetaRenderer.Dibujar(e.Graphics, rectEtiqueta, prod, barcode, chkPrecio.Checked, chkNombre.Checked);

                // Avanzamos posición X para la siguiente etiqueta
                xActual += _anchoEtiqueta + _espacioEntre;
                colCount++;

                // Si llegamos al límite de columnas, saltamos de línea
                if (colCount >= columnasPosibles)
                {
                    colCount = 0;
                    xActual = _margenX; // Reset X
                    yActual += _altoEtiqueta + _espacioEntre; // Avanzar Y
                }

                _indiceImpresion++;

                // Chequeamos si la SIGUIENTE etiqueta se saldría de la hoja verticalmente
                if (yActual + _altoEtiqueta > e.PageBounds.Height - _margenY)
                {
                    e.HasMorePages = true; // Pide una hoja nueva al sistema
                    return; // Salimos de la función; el evento se volverá a disparar para la nueva hoja
                }
            }

            // Si el while termina, significa que imprimimos todo
            e.HasMorePages = false;
            _indiceImpresion = 0; // Reset para la próxima vez
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
                this.Close();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && !btnImprimir.Focused && !btnLimpiar.Focused)
            {
                // Si presionan Enter en los controles de arriba, agregamos
                btnAgregar.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}