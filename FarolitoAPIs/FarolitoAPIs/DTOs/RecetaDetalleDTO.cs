﻿namespace FarolitoAPIs.DTOs
{
    public class RecetaDetalleDTO
    {
        public int Id { get; set; }
        public string? Nombrelampara { get; set; }
        public bool? Estatus { get; set; }
        public decimal CostoProduccion { get; set; }
        public string Imagen { get; set; }

        public List<ComponenteRecetaDTO> Componentes { get; set; } = new List<ComponenteRecetaDTO>();
        

    }
}
