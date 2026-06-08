using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Data;
using System;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ImportarProductosForm : Form
    {
        public ImportarProductosForm()
        {
            InitializeComponent();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtArchivo.Text = ofd.FileName;
                }
            }
        }

        private void btnDescargarPlantilla_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                sfd.FileName = "PlantillaProductos.xlsx";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var service = new ExcelService();
                        service.GenerarPlantilla(sfd.FileName);
                        MessageBox.Show("Plantilla guardada con éxito.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtArchivo.Text))
            {
                MessageBox.Show("Seleccione un archivo primero.");
                return;
            }

            btnImportar.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            panelResultados.Visible = false;

            try
            {
                var service = new ExcelService();
                var resultado = service.ImportarProductosInteligente(txtArchivo.Text);

                // Mostrar Resultados
                panelResultados.Visible = true;
                string resumen = $"PROCESO COMPLETADO\r\n" +
                                 $"------------------\r\n" +
                                 $"Total Procesados: {resultado.Procesados}\r\n" +
                                 $"Nuevos (Creados): {resultado.Nuevos}\r\n" +
                                 $"Actualizados:    {resultado.Actualizados}\r\n" +
                                 $"Errores:         {resultado.Errores}";

                txtLog.Text = resumen + "\r\n\r\nDETALLE DE ERRORES:\r\n" + string.Join("\r\n", resultado.MensajesError);

                AudioHelper.PlayOk();
                MessageBox.Show($"Importación finalizada.\nNuevos: {resultado.Nuevos}, Actualizados: {resultado.Actualizados}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error crítico en importación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnImportar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCargarSemilla_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "¿Desea precargar el catálogo con más de 110 productos reales del rubro almacén (con códigos de barras y marcas oficiales de Argentina)?\n" +
                "Si los productos ya existen por código de barras, se actualizarán.",
                "Confirmar Precarga", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            btnCargarSemilla.Enabled = false;
            btnImportar.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            panelResultados.Visible = false;

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var (creados, actualizados) = AlmacenSeedData.SembrarProductos(context);

                    panelResultados.Visible = true;
                    string resumen = $"PROCESO DE SIEMBRA COMPLETADO\r\n" +
                                     $"----------------------------\r\n" +
                                     $"Productos Creados:     {creados}\r\n" +
                                     $"Productos Actualizados: {actualizados}";

                    txtLog.Text = resumen + "\r\n\r\n¡El catálogo inicial de almacén fue cargado con éxito en la base de datos!";

                    AudioHelper.PlayOk();
                    MessageBox.Show($"Carga finalizada con éxito.\nProductos Nuevos: {creados}\nActualizados: {actualizados}", 
                        "Catálogo Precargado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error crítico al sembrar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCargarSemilla.Enabled = true;
                btnImportar.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }
    }
}