using System;

namespace _242516036.DbModels // Namespace adını kendi projene göre düzelt
{
    public class Reward
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int PointsCost { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}