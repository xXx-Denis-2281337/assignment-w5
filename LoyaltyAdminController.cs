using Microsoft.AspNetCore.Mvc;

namespace KmaOoad18.Assignments.Week5
{
    [Route("api/admin")]
    [ApiController]
    public class LoyaltyAdminController : ControllerBase
    {

        // Adds product with SKU, name, and price
        // SKU is stock keeping unit - unique identifier of product inventory
        [Route("products"), HttpPost]
        public IActionResult AddProduct([FromBody] ProductDto product)
        {
            // Add implementation

            return Ok();
        }


        // Sets special offering for a given product
        // Special offering is either adding extra points, or multiplying normal points by some coefficient
        // extra param is amount to add to normal bonus or coeff to multiply normal bonus by
        [Route("special-offerings"), HttpPost]
        public IActionResult AddSpecialOffering([FromBody] PromotionDto promo)
        {
            // Add implementation   

            return Ok();
        }


        // Removes all special offerings for the given product
        [Route("special-offerings/{sku}"), HttpDelete]

        public IActionResult RemoveSpecialOffering([FromRoute] string sku)
        {
            // Add implementation    

            return Ok();
        }


        // Adds customer to loyalty program
        // Returns new loyalty card ID
        [Route("loyalties"), HttpPost]
        public ActionResult<string> LaunchLoyalty([FromBody] NewLoyaltyDto newLoyalty)
        {
            // Add implementation  

            var loyaltyId = string.Empty; // assign ID of created loyalty

            return Ok(loyaltyId);
        }
    }

    public class ProductDto
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }

    public class PromotionDto
    {
        public string Sku { get; set; }
        public string PromotionType { get; set; }
        public int PromotionValue { get; set; }
    }

    public static class PromotionType
    {
        public const string AddPoints = nameof(AddPoints);
        public const string Multiplier = nameof(Multiplier);
    }

    public class NewLoyaltyDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
    }


}
