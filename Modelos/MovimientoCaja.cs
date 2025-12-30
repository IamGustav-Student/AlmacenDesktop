using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenDesktop.Modelos
{
    public class MovimientoCaja
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [RegularExpression("^(INGRESO|EGRESO)$", ErrorMessage = "El tipo debe ser INGRESO o EGRESO.")]
        public string Tipo { get; set; }

        [Required]
        [Range(0.01, 99999999, ErrorMessage = "El monto debe ser mayor a 0.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(200, ErrorMessage = "La descripción es muy larga.")]
        public string Descripcion { get; set; }

        [Required]
        public int CajaId { get; set; }
        public Caja Caja { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}