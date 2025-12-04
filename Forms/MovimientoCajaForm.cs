using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class MovimientoCajaForm : Form
    {
        private Caja _cajaActual;
        private Usuario _usuario;

        public MovimientoCajaForm(Caja caja, Usuario usuario)
        {
            InitializeComponent();
            _cajaActual = caja;
            _usuario = usuario;

            // Por defecto seleccionamos "Salida / Gasto" que es lo más común
            rbEgreso.Checked = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (numMonto.Value <= 0)
            {
                MessageBox.Show("Ingrese un monto válido.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese una descripción (Ej: Pago Luz).");
                return;
            }

            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var mov = new MovimientoCaja
                    {
                        Fecha = DateTime.Now,
                        CajaId = _cajaActual.Id,
                        UsuarioId = _usuario.Id,
                        Monto = numMonto.Value,
                        Descripcion = txtDescripcion.Text,
                        Tipo = rbIngreso.Checked ? "INGRESO" : "EGRESO"
                    };

                    context.MovimientosCaja.Add(mov);
                    context.SaveChanges();

                    MessageBox.Show("Movimiento registrado.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    // --- DISEÑADOR VISUAL AUTOMÁTICO ---
    partial class MovimientoCajaForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblMonto;
        private NumericUpDown numMonto;
        private Label lblDesc;
        private TextBox txtDescripcion;
        private RadioButton rbIngreso;
        private RadioButton rbEgreso;
        private Button btnGuardar;
        private Button btnCancelar;
        private GroupBox grpTipo;

        private void InitializeComponent()
        {
            this.lblMonto = new Label();
            this.numMonto = new NumericUpDown();
            this.lblDesc = new Label();
            this.txtDescripcion = new TextBox();
            this.grpTipo = new GroupBox();
            this.rbIngreso = new RadioButton();
            this.rbEgreso = new RadioButton();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();

            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).BeginInit();
            this.grpTipo.SuspendLayout();
            this.SuspendLayout();

            // Configuración UI
            this.Text = "Registrar Movimiento";
            this.Size = new Size(350, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblMonto.Text = "Monto ($):";
            lblMonto.Location = new Point(20, 20);

            numMonto.Location = new Point(20, 45);
            numMonto.Size = new Size(290, 30);
            numMonto.Font = new Font("Segoe UI", 12);
            numMonto.DecimalPlaces = 2;
            numMonto.Maximum = 10000000;

            grpTipo.Text = "Tipo de Movimiento";
            grpTipo.Location = new Point(20, 90);
            grpTipo.Size = new Size(290, 60);

            rbEgreso.Text = "Salida / Gasto";
            rbEgreso.Location = new Point(15, 25);
            rbEgreso.AutoSize = true;
            rbEgreso.ForeColor = Color.Red;

            rbIngreso.Text = "Entrada / Ingreso";
            rbIngreso.Location = new Point(150, 25);
            rbIngreso.AutoSize = true;
            rbIngreso.ForeColor = Color.Green;

            grpTipo.Controls.Add(rbEgreso);
            grpTipo.Controls.Add(rbIngreso);

            lblDesc.Text = "Descripción / Motivo:";
            lblDesc.Location = new Point(20, 160);

            txtDescripcion.Location = new Point(20, 185);
            txtDescripcion.Size = new Size(290, 25);

            btnGuardar.Text = "GUARDAR";
            btnGuardar.Location = new Point(160, 230);
            btnGuardar.Size = new Size(150, 40);
            btnGuardar.BackColor = Color.Teal;
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Text = "Cancelar";
            btnCancelar.Location = new Point(20, 230);
            btnCancelar.Size = new Size(100, 40);
            btnCancelar.Click += btnCancelar_Click;

            this.Controls.Add(lblMonto);
            this.Controls.Add(numMonto);
            this.Controls.Add(grpTipo);
            this.Controls.Add(lblDesc);
            this.Controls.Add(txtDescripcion);
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);

            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).EndInit();
            this.grpTipo.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
