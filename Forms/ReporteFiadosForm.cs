using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos; // Usamos el modelo unificado
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
        // Se ELIMINÓ la clase interna "MovimientoCtaCte" que causaba duplicación.

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

                cboClientes.SelectedIndexChanged -= cboClientes_SelectedIndexChanged;
                cboClientes.DisplayMember = "NombreCompleto";
                cboClientes.ValueMember = "Id";
                cboClientes.DataSource = clientes;
                cboClientes.SelectedIndex = -1;
                cboClientes.SelectedIndexChanged += cboClientes_SelectedIndexChanged;
            }
        }

        private void cboClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboClientes.SelectedItem != null && cboClientes.SelectedValue is int id)
            {
                CalcularEstadoCuenta(id);
                btnRegistrarPago.Enabled = true;
            }
            else
            {
                btnRegistrarPago.Enabled = false;
                dgvMovimientos.DataSource = null;
                lblSaldo.Text = "$ 0.00";
            }
        }

        private void CalcularEstadoCuenta(int clienteId)
        {
            using (var context = new AlmacenDbContext())
            {
                // Mapeamos a la clase unificada MovimientoCtaCte
                var ventasFiadas = context.Ventas
                    .Where(v => v.ClienteId == clienteId && v.MetodoPago == "Fiado")
                    .Select(v => new MovimientoCtaCte
                    {
                        Fecha = v.Fecha,
                        Tipo = "COMPRA FIADA",
                        Descripcion = $"Venta #{v.Id}", // CAMBIO: Detalle -> Descripcion
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
                        Descripcion = "Entrega de Dinero", // CAMBIO: Detalle -> Descripcion
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

                // Ajustamos nombres de columnas si es necesario (Descripcion en vez de Detalle)
                if (dgvMovimientos.Columns.Count > 0)
                {
                    if (dgvMovimientos.Columns["Fecha"] != null)
                        dgvMovimientos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";

                    if (dgvMovimientos.Columns["Debe"] != null)
                        dgvMovimientos.Columns["Debe"].DefaultCellStyle.Format = "N2";

                    if (dgvMovimientos.Columns["Haber"] != null)
                        dgvMovimientos.Columns["Haber"].DefaultCellStyle.Format = "N2";

                    if (dgvMovimientos.Columns["Saldo"] != null)
                    {
                        dgvMovimientos.Columns["Saldo"].DefaultCellStyle.Format = "N2";
                        dgvMovimientos.Columns["Saldo"].DefaultCellStyle.Font = new Font(dgvMovimientos.Font, FontStyle.Bold);
                    }

                    // Ocultamos VentaId si aparece
                    if (dgvMovimientos.Columns["VentaId"] != null)
                        dgvMovimientos.Columns["VentaId"].Visible = false;
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

            PagoForm form = new PagoForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                decimal monto = form.MontoIngresado;

                if (monto > 0)
                {
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
                        UsuarioId = Program.UsuarioActualGlobal.Id // Asignamos usuario actual
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
