namespace AlmacenDesktop.Forms
{
    partial class ClientesForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvClientes = new DataGridView();
            panelFormulario = new Panel();
            btnLimpiar = new Button();
            btnEliminar = new Button();
            btnGuardar = new Button();
            txtDireccion = new TextBox();
            label6 = new Label();
            txtTelefono = new TextBox();
            label5 = new Label();
            txtEmail = new TextBox();
            label4 = new Label();
            txtDni = new TextBox();
            label3 = new Label();
            txtApellido = new TextBox();
            label2 = new Label();
            txtNombre = new TextBox();
            label1 = new Label();
            lblTitulo = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvClientes).BeginInit();
            panelFormulario.SuspendLayout();
            SuspendLayout();
            // 
            // dgvClientes
            // 
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClientes.Dock = DockStyle.Right;
            dgvClientes.Location = new Point(320, 0);
            dgvClientes.MultiSelect = false;
            dgvClientes.Name = "dgvClientes";
            dgvClientes.ReadOnly = true;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.Size = new Size(580, 500);
            dgvClientes.TabIndex = 1;
            dgvClientes.SelectionChanged += dgvClientes_SelectionChanged;
            // 
            // panelFormulario
            // 
            panelFormulario.BackColor = Color.WhiteSmoke;
            panelFormulario.Controls.Add(btnLimpiar);
            panelFormulario.Controls.Add(btnEliminar);
            panelFormulario.Controls.Add(btnGuardar);
            panelFormulario.Controls.Add(txtDireccion);
            panelFormulario.Controls.Add(label6);
            panelFormulario.Controls.Add(txtTelefono);
            panelFormulario.Controls.Add(label5);
            panelFormulario.Controls.Add(txtEmail);
            panelFormulario.Controls.Add(label4);
            panelFormulario.Controls.Add(txtDni);
            panelFormulario.Controls.Add(label3);
            panelFormulario.Controls.Add(txtApellido);
            panelFormulario.Controls.Add(label2);
            panelFormulario.Controls.Add(txtNombre);
            panelFormulario.Controls.Add(label1);
            panelFormulario.Controls.Add(lblTitulo);
            panelFormulario.Dock = DockStyle.Left;
            panelFormulario.Location = new Point(0, 0);
            panelFormulario.Name = "panelFormulario";
            panelFormulario.Size = new Size(320, 500);
            panelFormulario.TabIndex = 0;
            // 
            // btnLimpiar
            // 
            btnLimpiar.Location = new Point(15, 450);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(290, 30);
            btnLimpiar.TabIndex = 0;
            btnLimpiar.Text = "Limpiar / Nuevo";
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.Firebrick;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.ForeColor = Color.White;
            btnEliminar.Location = new Point(15, 400);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(290, 40);
            btnEliminar.TabIndex = 1;
            btnEliminar.Text = "ELIMINAR";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.ForestGreen;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(15, 350);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(290, 40);
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "GUARDAR CLIENTE";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(15, 300);
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(290, 23);
            txtDireccion.TabIndex = 3;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(15, 280);
            label6.Name = "label6";
            label6.Size = new Size(60, 15);
            label6.TabIndex = 4;
            label6.Text = "Dirección:";
            // 
            // txtTelefono
            // 
            txtTelefono.Location = new Point(15, 245);
            txtTelefono.Name = "txtTelefono";
            txtTelefono.Size = new Size(290, 23);
            txtTelefono.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(15, 225);
            label5.Name = "label5";
            label5.Size = new Size(56, 15);
            label5.TabIndex = 6;
            label5.Text = "Teléfono:";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(15, 190);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(290, 23);
            txtEmail.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(15, 170);
            label4.Name = "label4";
            label4.Size = new Size(39, 15);
            label4.TabIndex = 8;
            label4.Text = "Email:";
            // 
            // txtDni
            // 
            txtDni.Location = new Point(15, 135);
            txtDni.MaxLength = 11;
            txtDni.Name = "txtDni";
            txtDni.Size = new Size(290, 23);
            txtDni.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 115);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 10;
            label3.Text = "DNI / CUIT:";
            // 
            // txtApellido
            // 
            txtApellido.Location = new Point(165, 80);
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(140, 23);
            txtApellido.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(165, 60);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 12;
            label2.Text = "Apellido:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(15, 80);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(140, 23);
            txtNombre.TabIndex = 13;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 60);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 14;
            label1.Text = "Nombre:";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 15);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(181, 25);
            lblTitulo.TabIndex = 15;
            lblTitulo.Text = "Gestión de Clientes";
            // 
            // ClientesForm
            // 
            ClientSize = new Size(900, 500);
            Controls.Add(dgvClientes);
            Controls.Add(panelFormulario);
            Name = "ClientesForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Administración de Clientes";
            Load += ClientesForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvClientes).EndInit();
            panelFormulario.ResumeLayout(false);
            panelFormulario.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.Panel panelFormulario;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDni;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnLimpiar;
    }
}