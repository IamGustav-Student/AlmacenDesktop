using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AlmacenDesktop.Forms
{
    public partial class ReporteFiadosForm : Form
    {
        private class MovimientoCtaCte
        {
            public DateTime Fecha { get; set; }
            public string Tipo { get; set; }
            public string Detalle { get; set; }
            public decimal Debe { get; set; }
            public decimal Haber { get; set; }
            public decimal Saldo { get; set; }
        }

        public ReporteFiadosForm()
        {
            InitializeComponent();
        }

        private void ReporteFiadosForm_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private void CargarClientes()
        {
            using (var context = new AlmacenDbContext())
            {
                var clientes = context.Clientes
                    .Where(c => c.DniCuit != "00000000")
                    .ToList();

                // --- CORRECCIÓN CLAVE AQUÍ ---
                // 1. Desvinculamos eventos temporalmente (opcional pero recomendado)
                cboClientes.SelectedIndexChanged -= cboClientes_SelectedIndexChanged;

                // 2. Configuramos Display y Value ANTES del DataSource
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";

                // 3. Asignamos los datos
                cboClientes.DataSource = clientes;

                // 4. Limpiamos selección inicial
                cboClientes.SelectedIndex = -1;

                // 5. Volvemos a conectar el evento
                cboClientes.SelectedIndexChanged += cboClientes_SelectedIndexChanged;
            }
        }

        private void cboClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // --- BLINDAJE ANTI-CRASH ---
            // Verificamos que haya item y que el valor sea realmente un entero
            if (cboClientes.SelectedItem != null && cboClientes.SelectedValue is int id)
            {
                CalcularEstadoCuenta(id);
                btnRegistrarPago.Enabled = true;
            }
            else
            {
                // Si el valor no es válido, limpiamos la pantalla
                btnRegistrarPago.Enabled = false;
                dgvMovimientos.DataSource = null;
                lblSaldo.Text = "$ 0.00";
            }
        }

        private void CalcularEstadoCuenta(int clienteId)
        {
            using (var context = new AlmacenDbContext())
            {
                var ventasFiadas = context.Ventas
                    .Where(v => v.ClienteId == clienteId && v.MetodoPago == "Fiado")
                    .Select(v => new MovimientoCtaCte
                    {
                        Fecha = v.Fecha,
                        Tipo = "COMPRA FIADA",
                        Detalle = $"Venta #{v.Id}",
                        Debe = v.Total,
                        Haber = 0
                    })
                    .ToList();

                var pagos = context.Pagos
                    .Where(p => p.ClienteId == clienteId)
                    .Select(p => new MovimientoCtaCte
                    {
                        Fecha = p.Fecha,
                        Tipo = "PAGO A CUENTA",
                        Detalle = p.Notas ?? "Pago en efectivo",
                        Debe = 0,
                        Haber = p.Monto
                    })
                    .ToList();

                var movimientos = ventasFiadas.Concat(pagos)
                    .OrderBy(m => m.Fecha)
                    .ToList();

                decimal saldoAcumulado = 0;
                foreach (var mov in movimientos)
                {
                    saldoAcumulado += mov.Debe;
                    saldoAcumulado -= mov.Haber;
                    mov.Saldo = saldoAcumulado;
                }

                dgvMovimientos.DataSource = movimientos;

                // Formato si hay filas
                if (dgvMovimientos.Columns.Count > 0)
                {
                    dgvMovimientos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvMovimientos.Columns["Debe"].DefaultCellStyle.Format = "N2";
                    dgvMovimientos.Columns["Haber"].DefaultCellStyle.Format = "N2";
                    dgvMovimientos.Columns["Saldo"].DefaultCellStyle.Format = "N2";
                    dgvMovimientos.Columns["Saldo"].DefaultCellStyle.Font = new Font(dgvMovimientos.Font, FontStyle.Bold);
                }

                lblSaldo.Text = $"$ {saldoAcumulado:N2}";

                if (saldoAcumulado > 0) lblSaldo.ForeColor = Color.Firebrick;
                else if (saldoAcumulado < 0) lblSaldo.ForeColor = Color.Green;
                else lblSaldo.ForeColor = Color.Black;
            }
        }

        private void btnRegistrarPago_Click(object sender, EventArgs e)
        {
            if (cboClientes.SelectedItem == null) return;

            // Usamos el formulario visual de Pago
            PagoForm form = new PagoForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                decimal monto = form.MontoIngresado;

                if (monto > 0)
                {
                    // Validamos una vez más el ID
                    if (cboClientes.SelectedValue is int clienteId)
                    {
                        GuardarPago(clienteId, monto);
                    }
                }
                else
                {
                    MessageBox.Show("El monto debe ser mayor a cero.");
                }
            }
        }

        private void GuardarPago(int clienteId, decimal monto)
        {
            try
            {
                using (var context = new AlmacenDbContext())
                {
                    var nuevoPago = new Pago
                    {
                        ClienteId = clienteId,
                        Monto = monto,
                        Fecha = DateTime.Now,
                        Notas = "Pago registrado"
                    };

                    context.Pagos.Add(nuevoPago);
                    context.SaveChanges();

                    MessageBox.Show("Pago registrado correctamente.");

                    CalcularEstadoCuenta(clienteId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }
    }
}
