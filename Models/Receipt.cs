#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace receipt_processor_challenge.Models
{
    public class Receipt
    {
        [RegularExpression("^[\\w\\s\\-&]+$", ErrorMessage = "Invalid retailer")]
        public string retailer { get; set; }

        [RegularExpression("^\\d{4}-\\d{2}-\\d{2}$", ErrorMessage = "Invalid purchaseDate")]
        public string purchaseDate { get; set; }

        [RegularExpression("^([0-1]\\d|2[0-3]):([0-5]\\d)$", ErrorMessage = "Invalid purchaseTime")]
        public string purchaseTime { get; set; }

        public List<Item> items { get; set; } = new List<Item>();

        [RegularExpression("^\\d+\\.\\d{2}$", ErrorMessage = "Invalid total")]
        public string total { get; set; }
    }
}