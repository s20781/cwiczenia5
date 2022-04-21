using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia5.Models
{
    public class WarehousePost
    {

        

        [Required]
        public int IdProduct { get; set; }

        [Required]
        public int IdWarehouse { get; set; }

       



        [Range(0, int.MaxValue)]
        public int Amount { get; set; }

        
        [Required]
        public DateTime CreatedAt { get; set; }


    }
}
