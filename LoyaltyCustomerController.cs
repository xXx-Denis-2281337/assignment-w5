using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using KmaOoad18.Assignments.Week5.Data;
using KmaOoad18.Assignments.Week5.Extensions;

namespace KmaOoad18.Assignments.Week5
{
    [Route("api/customer")]
    [ApiController]
    public class LoyaltyCustomerController : ControllerBase
    {
        private readonly LoyaltyContext _db;
        private const decimal StdBonus = 10m; // %

        public LoyaltyCustomerController(LoyaltyContext db) => _db = db;

        // Gets customer's loyalty balance
        // loyaltyId can be either loyalty card number OR customer phone number, method must support both cases
        // Returns amount of loyalty points on customer's account
        [Route("{loyaltyId}/balance"), HttpGet]
        public ActionResult<decimal> LoyaltyBalance([FromRoute] string loyaltyId)
            => Ok(CustomerExt.GetByLoyaltyId(_db, loyaltyId).LoyaltyBalance);

        // Calculates bonus for purchase and adds to customer's account
        //    a. Normal loyalty bonus for product is 10% of paid total amount 
        //    b. If useLoyaltyPoints=true, up to 50% of total amount can be covered with loyalty points;
        //       in this case loyalty bonus is calculated only for actually paid amount
        [Route("{loyaltyId}/purchase"), HttpPost]
        public IActionResult ProcessPurchase([FromRoute] string loyaltyId, [FromBody] OrderDto order)
        {
            var cust = CustomerExt.GetByLoyaltyId(_db, loyaltyId);

            var bill = order.OrderItems.Select(
                o => new { o.Sku, Total = ProductExt.GetBySku(_db, o.Sku).Price * o.Qty }
            );

            var totalBonus = 0m;

            foreach (var pos in bill)
            {
                var spending = pos.Total;

                if (order.UseLoyaltyPoints)
                    spending = cust.UseLoyaltyPoints(spending);

                var offerings = SpecialOfferingExt.ForProduct(_db, pos.Sku);

                var bonus = StdBonus;

                foreach (var off in offerings)
                    bonus = off.ApplyPromotion(bonus);

                bonus /= 100;

                totalBonus += spending * bonus;
            }

            cust.UpdateLoyaltyBalance(totalBonus);

            _db.SaveChanges();

            return Ok();
        }
    }

    public class OrderDto
    {
        public List<OrderItemDto> OrderItems { get; set; }

        public bool UseLoyaltyPoints { get; set; }
    }

    public class OrderItemDto
    {
        public string Sku { get; set; }

        public int Qty { get; set; }
    }
}
