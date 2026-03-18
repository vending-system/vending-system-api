using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiVending.DTO
{
    public class MachineInventoryDto
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }//Количество 
        public int MinStockLevel { get; set; }//Минимальный уровень запасов 
        public bool NeedRestock => Quantity <= MinStockLevel; //Необходимый запас 
    }
}