using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using AlmacenDesktop.Services;
using AlmacenDesktop.Helpers;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ConfiguracionForm : Form
    {
        private BackupService _backupService;

        public ConfiguracionForm()
        {
            InitializeComponent();
            _backupService = new BackupService();
        }

        private void ConfiguracionForm_Load(object sender, EventArgs e)
        {
            // --- SEGURIDAD: VERIFICACIÓN FINAL ---
            // Si por alguna razón un usuario no Admin llega aquí, lo sacamos.
            if (Program.UsuarioActualGlobal != null && Program.UsuarioActualGlobal.Rol != RolUsuario.Admin)
            {
                MessageBox.Show("Intento de acceso no autorizado registrado.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Close();
                return;
            }
            // -------------------------------------

            CargarImpresoras();
            CargarDatos();
        }

        private void CargarImpresoras()
        {
            cboImpresoras.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cboImpresoras.Items.Add(printer);
            }

            if (cboImpresoras.Items.Count == 0) cboImpresoras.Items.Add("Microsoft Print to PDF");
        }

        private void CargarDatos()
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var datos = context.DatosNegocio.FirstOrDefault();
                    if (datos != null)
                    {
                        txtNombreNegocio.Text = datos.NombreFantasia;
                        txtDireccion.Text = datos.Direccion;
                        txtTelefono.Text = datos.Telefono;
                        txtCuit.Text = datos.CUIT;
                        txtEmail.Text = datos.RazonSocial;
                        txtMensajeTicket.Text = datos.MensajeTicket;

                        if (!string.IsNullOrEmpty(datos.NombreImpresora) && cboImpresoras.Items.Contains(datos.NombreImpresora))
                            cboImpresoras.SelectedItem = datos.NombreImpresora;
                        else if (cboImpresoras.Items.Count > 0)
                            cboImpresoras.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var datos = context.DatosNegocio.FirstOrDefault();
                    if (datos == null)
                    {
                        datos = new DatosNegocio();
                        context.DatosNegocio.Add(datos);
                    }

                    datos.NombreFantasia = txtNombreNegocio.Text;
                    datos.Direccion = txtDireccion.Text;
                    datos.Telefono = txtTelefono.Text;
                    datos.CUIT = txtCuit.Text;
                    datos.RazonSocial = txtEmail.Text;
                    datos.MensajeTicket = txtMensajeTicket.Text;

                    if (cboImpresoras.SelectedItem != null)
                        datos.NombreImpresora = cboImpresoras.SelectedItem.ToString();

                    context.SaveChanges();
                    AudioHelper.PlayOk();
                    MessageBox.Show("Datos guardados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                AudioHelper.PlayError();
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccione dónde guardar la copia de seguridad";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string archivoGenerado = _backupService.RealizarBackupManual(fbd.SelectedPath);
                        AudioHelper.PlayOk();
                        MessageBox.Show($"Respaldo creado con éxito en:\n{archivoGenerado}", "Backup OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        AudioHelper.PlayError();
                        MessageBox.Show(ex.Message, "Error Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnConfigurarAfip_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show(
                "¿Desea iniciar el Asistente Fiscal 1-Click (Recomendado) para generar claves y CSR automáticamente?\n\n" +
                "[Sí] = Abrir Asistente 1-Click\n" +
                "[No] = Abrir Configuración Manual Avanzada",
                "Asistente Fiscal VENDEMAX",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                var frm = new AfipAsistenteForm();
                frm.ShowDialog();
            }
            else if (res == DialogResult.No)
            {
                var frm = new ConfiguracionAfipForm();
                frm.ShowDialog();
            }
        }
    }
}