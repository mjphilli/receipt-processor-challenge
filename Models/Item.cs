#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace receipt_processor_challenge.Models
{
    public class Item
    {
        //[RegularExpression(@"^[\\w\\s\\-]+$", ErrorMessage = "Invalid description")]
        public string shortDescription { get; set; }

        //[RegularExpression(@"^\\d+\\.\\d{2}$", ErrorMessage = "Invalid price")]
        public string price { get; set; }
    }
}