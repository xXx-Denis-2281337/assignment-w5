using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace KmaOoad18.Assignments.Week5
{
    [Route("api/customer")]
    public class LoyaltyCustomerController : ControllerBase
    {
        // Gets customer's loyalty balance
        // loyaltyId can be either loyalty card number OR customer phone number, method must support both cases
        // Returns amount of loyalty points on customer's account
        [Route("{loyaltyId}/balance"), HttpGet]
        public ActionResult<decimal> LoyaltyBalance([FromRoute] string loyaltyId)
        {
            // Add your implementation
            var balance = -500; // Assign actual balance here


            return Ok(balance);
        }


        // Calculates bonus for purchase and adds to customer's account
        //    a. Normal loyalty bonus for product is 10% of paid total amount 
        //    b. If useLoyaltyPoints=true, up to 50% of total amount can be covered with loyalty points; //       in this case loyalty bonus is calculated only for actually paid amount
        [Route("{loyaltyId}/purchase"), HttpPost]
        public IActionResult ProcessPurchase([FromBody] List<OrderItemDto> order, [FromRoute] string loyaltyId, [FromBody] bool useLoyaltyPoints = false)
        {
            // Add implementation

            return Ok();
        }

        public struct OrderItemDto
        {
            public string Sku;
            public int Qty;
        }
    }


}
