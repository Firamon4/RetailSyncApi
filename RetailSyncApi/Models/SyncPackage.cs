using System.ComponentModel.DataAnnotations;

namespace RetailSyncApi.Models
{
    public class SyncPackage
    {
        [Key]
        public int Id { get; set; }

        // Хто відправив? 
        [Required]
        public string Source { get; set; }

        // Кому призначено?
        [Required]
        public string Target { get; set; }

        // Тип даних (Наприклад: "Products", "Orders") - щоб отримувач знав, як це читати
        public string DataType { get; set; }

        // Самі дані. 1С сюди покладе XML рядок, а C# покладе JSON.
        [Required]
        public string Payload { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}