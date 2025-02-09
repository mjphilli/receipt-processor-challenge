#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace receipt_processor_challenge.Models
{
    public class Receipt
    {
        [RegularExpression("^[\\w\\s\\-&]+$", ErrorMessage = "Invalid retailer")]
        public string retailer { get; set; }

        public string purchaseDate { get; set; }

        public string purchaseTime { get; set; }

        public List<Item> items { get; set; } = new List<Item>();

        [RegularExpression("^\\d+\\.\\d{2}$", ErrorMessage = "Invalid total")]
        public string total { get; set; }
    }
}