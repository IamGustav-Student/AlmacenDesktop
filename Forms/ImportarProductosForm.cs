using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
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
    }
}