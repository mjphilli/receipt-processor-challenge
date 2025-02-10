using Microsoft.AspNetCore.Mvc;
using receipt_processor_challenge.Models;
using System.Linq;

namespace receipt_processor_challenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(ILogger<ReceiptsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "hello from default page";
        }

        [HttpGet("{id}/points")]
        public ActionResult GetJsonData(string id)
        {
            var data = new
            {
                points = HttpContext.Session.GetInt32(id)
            };

            if (data.points == null)
            {
                return NotFound("No receipt found for that ID.");
            }

            return new JsonResult(data);
        }

        [HttpPost("process")]
        public ActionResult Process(Receipt receipt)
        {
            int points = 0;

            //technically, only purchaseDate and purchaseTime will throw an exception here
            //everything else is handled in the model
            try
            {
                //counts the alphanumeric characters in retailer name
                points += receipt.retailer.Count(char.IsLetterOrDigit);

                //checks if the total is a round dollar amount
                if (double.Parse(receipt.total) % 1 == 0)
                {
                    points += 50;
                }

                //checks if the total is a multiple of 0.25
                if (double.Parse(receipt.total) % 0.25 == 0)
                {
                    points += 25;
                }

                //5 points for every 2 items on the receipt
                points += (receipt.items.Count / 2) * 5;

                //if trimmed length of each item desc is multiple of 3
                //multiply price by 0.2 rounded up
                //it's a little sloppy with all the conversions going on, there might be a better way
                for (int i = 0; i < receipt.items.Count; i++)
                {
                    if (receipt.items[i].shortDescription.Trim().Length % 3 == 0)
                    {
                        points += (int)Math.Ceiling(double.Parse(receipt.items[i].price) * 0.2);
                    }
                }

                //if and only if this was generated using an LLM, 5 points if total is greater than 10.00
                //nothing in this project was generated using an LLM so this part is left commented out
                //if (double.Parse(receipt.total) > 10.00)
                //{
                //    points += 5;
                //}

                //6 points if the purchase day is odd
                if (DateTime.Parse(receipt.purchaseDate).Day % 2 == 1)
                {
                    points += 6;
                }

                //10 points if the time of purchase is after 2:00pm and before 4:00pm
                if (DateTime.Parse(receipt.purchaseTime).TimeOfDay > DateTime.Parse("2:00 PM").TimeOfDay &&
                    DateTime.Parse(receipt.purchaseTime).TimeOfDay < DateTime.Parse("4:00 PM").TimeOfDay)
                {
                    points += 10;
                }
            }
            catch (Exception ex)
            {
                //I added the exception message to be a little more detailed in where the receipt was invalid
                //This should only trigger when purchaseDate or purchaseTime match the regex, but are out of bounds
                return BadRequest("The receipt is invalid.\n" + ex.Message);
            }

            string newId = GenerateNewId();

            HttpContext.Session.SetInt32(newId, points);

            var data = new
            {
                id = newId
            };

            return new JsonResult(data);
        }

        public string GenerateNewId()
        {
            //there might be an easier to generate an alphanumeric id like below using regex, but I'd have to look more into it
            //example format: adb6b560-0eef-42bc-9d16-df48f30e89b2
            //same format as the sessionID, but I didn't want to apply the same ID to multiple receipts
            char[] characters = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
            Random r = new Random();
            string id = "";
            for (int i = 0; i < 28; i++)
            {
                //could also write it as (i > 4 && i < 21 && i % 4 == 0)
                if (i == 8 || i == 12 || i == 16 || i == 20)
                {
                    id += "-";
                }

                id += characters[r.Next(0, 36)].ToString();
            }

            return id;
        }
    }
}