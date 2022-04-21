using cwiczenia5.Models;
using cwiczenia5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WareHousesController : ControllerBase
    {
        private readonly IWarehouseService _service;
        
        public WareHousesController(IWarehouseService warehouseService)
        {
            _service = warehouseService;
        }




        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _service.GetWarehouse();

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWarehouse(WarehousePost warehousePost)
        {
            int warehousePostID = 0;

            try
            {
                await _service.DoesProductExist(warehousePost.IdProduct);
                await _service.DoesWarehouseExist(warehousePost.IdWarehouse);
                int orderId = await _service.GetTheValidOrderId(warehousePost.IdWarehouse,warehousePost.IdProduct,warehousePost.Amount,warehousePost.CreatedAt);
                warehousePostID = await _service.CompeleteTheOrder(orderId, warehousePost.IdWarehouse, warehousePost.IdProduct, warehousePost.Amount, warehousePost.CreatedAt);
                
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           


            return Ok(warehousePostID);
        }
            




    }
}
