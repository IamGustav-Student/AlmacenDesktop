using AlmacenDesktop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AlmacenDesktop.Services; // Para reimprimir

namespace AlmacenDesktop.Forms
{
    public partial class HistorialCajasForm : Form
    {
        // UI
        private DataGridView dgvCajas;
        private Button btnReimprimir;

        public HistorialCajasForm()
        {
            InitializeComponent();
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            using (var context = new AlmacenDbContext())
            {
                var cajas = context.Cajas
                    .Include(c => c.Usuario)
                    .Where(c => !c.EstaAbierta) // Solo cajas cerradas
                    .OrderByDescending(c => c.FechaCierre)
                    .Select(c => new
                    {
                        Id = c.Id,
                        Apertura = c.FechaApertura,
                        Cierre = c.FechaCierre,
                        Usuario = c.Usuario.NombreUsuario,
                        Sistema = c.SaldoFinalSistema,
                        Real = c.SaldoFinalReal,
                        Diferencia = c.Diferencia
                    })
                    .ToList();

                dgvCajas.DataSource = cajas;

                // Formato condicional para faltantes
                dgvCajas.CellFormatting += (s, e) =>
                {
                    if (dgvCajas.Columns[e.ColumnIndex].Name == "Diferencia" && e.Value != null)
                    {
                        decimal val = (decimal)e.Value;
                        if (val < 0) e.CellStyle.ForeColor = Color.Red;
                        else if (val > 0) e.CellStyle.ForeColor = Color.Blue;
                    }
                };
            }
        }

        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            if (dgvCajas.SelectedRows.Count == 0) return;

            int idCaja = (int)dgvCajas.SelectedRows[0].Cells["Id"].Value;

            using (var context = new AlmacenDbContext())
            {
                var cajaFull = context.Cajas.Find(idCaja);
                if (cajaFull != null)
                {
                    new TicketService().ImprimirCierreCaja(cajaFull);
                }
            }
        }

        private void InitializeComponent()
        {
            this.dgvCajas = new DataGridView();
            this.btnReimprimir = new Button();

            this.ClientSize = new Size(800, 500);
            this.Text = "Historial de Cierres de Caja";
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvCajas.Dock = DockStyle.Top;
            dgvCajas.Height = 400;
            dgvCajas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCajas.AllowUserToAddRows = false;
            dgvCajas.ReadOnly = true;
            dgvCajas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            btnReimprimir.Text = "🖨️ Reimprimir Ticket Z";
            btnReimprimir.Location = new Point(20, 420);
            btnReimprimir.Size = new Size(200, 40);
            btnReimprimir.Click += btnReimprimir_Click;

            this.Controls.Add(dgvCajas);
            this.Controls.Add(btnReimprimir);
        }
    }
}