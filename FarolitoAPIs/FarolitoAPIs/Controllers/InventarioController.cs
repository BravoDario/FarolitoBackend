﻿using FarolitoAPIs.Data;
using FarolitoAPIs.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarolitoAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly FarolitoDbContext _baseDatos;
        public InventarioController(FarolitoDbContext baseDatos)
        {
            _baseDatos = baseDatos;
        }

        [HttpGet("inventario-componentes")]
        public async Task<IActionResult> ObtenerComponentesConDetalles()
        {
            var componentes = await _baseDatos.Componentes
                .Include(c => c.Inventariocomponentes)
                    .ThenInclude(ic => ic.Proveedor)
                .Include(c => c.Inventariocomponentes)
                    .ThenInclude(ic => ic.Detallecompra)
                        .ThenInclude(dc => dc.Compra)
                .ToListAsync();

            if (componentes == null || !componentes.Any())
            {
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "No se encontraron componentes"
                });
            }

            var componentesConDetallesDTO = componentes.Select(c => new ComponenteConDetallesDTO
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Existencia = c.Inventariocomponentes.Sum(ic => ic.Cantidad ?? 0),
                Detalles = c.Inventariocomponentes.Select(ic => new InventarioComponenteDetalleDTO
                {
                    Id = ic.Id,
                    Cantidad = ic.Cantidad,
                    ProveedorNombre = ic.Proveedor.NombreEmpresa,
                    FechaCompra = ic.Detallecompra.Compra.Fecha ?? DateOnly.MinValue
                }).ToList()
            }).ToList();

            return Ok(componentesConDetallesDTO);
        }

        [HttpGet("inventario-lamparas")]
        public async Task<IActionResult> ObtenerRecetasConDetalles()
        {
            var recetas = await _baseDatos.Receta
                .Include(r => r.Inventariolamparas)
                    .ThenInclude(il => il.Produccion)
                        .ThenInclude(p => p.Usuario)
                .ToListAsync();

            if (recetas == null || !recetas.Any())
            {
                return NotFound(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "No se encontraron recetas"
                });
            }

            var recetasConDetallesDTO = recetas.Select(r => new RecetaConDetallesDTO
            {
                Id = r.Id,
                Nombrelampara = r.Nombrelampara,
                Existencias = r.Inventariolamparas.Sum(il => il.Cantidad ?? 0),
                Costo = r.Inventariolamparas.Any(il => il.Precio.HasValue)
                ? r.Inventariolamparas.Where(il => il.Precio.HasValue).Average(il => il.Precio.Value) * 1.2
                : 0,
                Detalles = r.Inventariolamparas.Select(il => new InventarioLamparaDetalleDTO
                {
                    Id = il.Id,
                    FechaProduccion = il.Produccion.Fecha,
                    Usuario = il.Produccion.Usuario.UserName,
                    Cantidad = il.Cantidad,
                    Precio = il.Precio
                }).ToList()
            }).ToList();

            return Ok(recetasConDetallesDTO);
        }


    }
}