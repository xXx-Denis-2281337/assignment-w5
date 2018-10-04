using Microsoft.AspNetCore.Mvc;

namespace KmaOoad18.Assignments.Week5
{

    [Route("api/admin")]
    public class LoyaltyAdminController : ControllerBase
    {

        // Adds product with SKU, name, and price
        // SKU is stock keeping unit - unique identifier of product inventory
        [Route("products"), HttpPost]
        public IActionResult AddProduct([FromBody] string sku, [FromBody] string name, [FromBody] decimal price)
        {
            // Add implementation

            return Ok();
        }


        // Sets special offering for a given product
        // Special offering is either adding extra points, or multiplying normal points by some coefficient
        // extra param is amount to add to normal bonus or coeff to multiply normal bonus by
        [Route("special-offerings"), HttpPost]
        public IActionResult AddSpecialOffering([FromBody] string sku, [FromBody] Promotion promotion, [FromBody] int extra)
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
        [Route("customers"), HttpPost]
        public IActionResult LaunchLoyalty([FromBody] string customerName, [FromBody] string customerPhone)
        {
            // Add implementation    

            return Ok();
        }


        #region Config
        // This is to simplify config for testing purposes in this educational project only. Normaly you should avoid such public fields in real life!
        public static string Db = "loyalty.db";
        #endregion
    }

    public enum Promotion
    {
        AddPoints,
        MultiplyPoints
    }


}
