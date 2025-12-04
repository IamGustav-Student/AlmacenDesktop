using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace AlmacenDesktop.Forms
{
    public partial class ConfiguracionForm : Form
    {
        public ConfiguracionForm()
        {
            InitializeComponent();
        }

        private void ConfiguracionForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (var context = new AlmacenDbContext())
            {
                // Buscamos el primer registro, si no existe no cargamos nada
                var datos = context.DatosNegocio.FirstOrDefault();
                if (datos != null)
                {
                    txtNombre.Text = datos.NombreFantasia;
                    txtRazon.Text = datos.RazonSocial;
                    txtCuit.Text = datos.CUIT;
                    txtDireccion.Text = datos.Direccion;
                    txtTelefono.Text = datos.Telefono;
                    txtMensaje.Text = datos.MensajeTicket;
                }
                else
                {
                    // Valores por defecto sugeridos
                    txtMensaje.Text = "¡Gracias por su compra!";
                }
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
                        // CREAR (Primera vez)
                        datos = new DatosNegocio();
                        context.DatosNegocio.Add(datos);
                    }

                    // ACTUALIZAR DATOS
                    datos.NombreFantasia = txtNombre.Text;
                    datos.RazonSocial = txtRazon.Text;
                    datos.CUIT = txtCuit.Text;
                    datos.Direccion = txtDireccion.Text;
                    datos.Telefono = txtTelefono.Text;
                    datos.MensajeTicket = txtMensaje.Text;

                    context.SaveChanges();
                    MessageBox.Show("Datos del negocio actualizados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    // --- DISEÑADOR VISUAL ---
    partial class ConfiguracionForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitulo;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblRazon;
        private TextBox txtRazon;
        private Label lblCuit;
        private TextBox txtCuit;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Label lblTelefono;
        private TextBox txtTelefono;
        private Label lblMensaje;
        private TextBox txtMensaje;
        private Button btnGuardar;
        private Button btnCancelar;
        private GroupBox grpDatos;

        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.grpDatos = new GroupBox();
            this.lblNombre = new Label();
            this.txtNombre = new TextBox();
            this.lblRazon = new Label();
            this.txtRazon = new TextBox();
            this.lblCuit = new Label();
            this.txtCuit = new TextBox();
            this.lblDireccion = new Label();
            this.txtDireccion = new TextBox();
            this.lblTelefono = new Label();
            this.txtTelefono = new TextBox();
            this.lblMensaje = new Label();
            this.txtMensaje = new TextBox();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();

            this.grpDatos.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Configuración del Negocio";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Titulo
            this.lblTitulo.Text = "Datos para el Ticket";
            this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitulo.ForeColor = Color.DimGray;
            this.lblTitulo.Location = new Point(20, 15);
            this.lblTitulo.AutoSize = true;

            // GroupBox
            this.grpDatos.Location = new Point(20, 50);
            this.grpDatos.Size = new Size(390, 330);
            this.grpDatos.Text = "Información General";

            // Campos
            int y = 30;
            int esp = 50;

            // Nombre Fantasia
            this.lblNombre.Text = "Nombre del Negocio:";
            this.lblNombre.Location = new Point(20, y);
            this.lblNombre.AutoSize = true;
            this.txtNombre.Location = new Point(20, y + 20);
            this.txtNombre.Size = new Size(350, 23);
            y += esp;

            // Razón Social
            this.lblRazon.Text = "Razón Social (Dueño):";
            this.lblRazon.Location = new Point(20, y);
            this.lblRazon.AutoSize = true;
            this.txtRazon.Location = new Point(20, y + 20);
            this.txtRazon.Size = new Size(350, 23);
            y += esp;

            // CUIT
            this.lblCuit.Text = "CUIT / DNI:";
            this.lblCuit.Location = new Point(20, y);
            this.lblCuit.AutoSize = true;
            this.txtCuit.Location = new Point(20, y + 20);
            this.txtCuit.Size = new Size(150, 23);

            // Teléfono (al lado del CUIT)
            this.lblTelefono.Text = "Teléfono:";
            this.lblTelefono.Location = new Point(190, y);
            this.lblTelefono.AutoSize = true;
            this.txtTelefono.Location = new Point(190, y + 20);
            this.txtTelefono.Size = new Size(180, 23);
            y += esp;

            // Dirección
            this.lblDireccion.Text = "Dirección:";
            this.lblDireccion.Location = new Point(20, y);
            this.lblDireccion.AutoSize = true;
            this.txtDireccion.Location = new Point(20, y + 20);
            this.txtDireccion.Size = new Size(350, 23);
            y += esp;

            // Mensaje Ticket
            this.lblMensaje.Text = "Mensaje al pie del Ticket:";
            this.lblMensaje.Location = new Point(20, y);
            this.lblMensaje.AutoSize = true;
            this.txtMensaje.Location = new Point(20, y + 20);
            this.txtMensaje.Size = new Size(350, 23);

            this.grpDatos.Controls.Add(lblNombre);
            this.grpDatos.Controls.Add(txtNombre);
            this.grpDatos.Controls.Add(lblRazon);
            this.grpDatos.Controls.Add(txtRazon);
            this.grpDatos.Controls.Add(lblCuit);
            this.grpDatos.Controls.Add(txtCuit);
            this.grpDatos.Controls.Add(lblTelefono);
            this.grpDatos.Controls.Add(txtTelefono);
            this.grpDatos.Controls.Add(lblDireccion);
            this.grpDatos.Controls.Add(txtDireccion);
            this.grpDatos.Controls.Add(lblMensaje);
            this.grpDatos.Controls.Add(txtMensaje);

            // Botones
            this.btnGuardar.Text = "GUARDAR CAMBIOS";
            this.btnGuardar.Location = new Point(210, 400);
            this.btnGuardar.Size = new Size(200, 45);
            this.btnGuardar.BackColor = Color.SteelBlue;
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.Click += btnGuardar_Click;

            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Location = new Point(20, 400);
            this.btnCancelar.Size = new Size(100, 45);
            this.btnCancelar.Click += btnCancelar_Click;

            this.Controls.Add(lblTitulo);
            this.Controls.Add(grpDatos);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }
    }
}