using Microsoft.AspNetCore.Mvc;

using KmaOoad18.Assignments.Week5.Data;
using KmaOoad18.Assignments.Week5.Extensions;

namespace KmaOoad18.Assignments.Week5
{
    [Route("api/admin")]
    [ApiController]
    public class LoyaltyAdminController : ControllerBase
    {
        private readonly LoyaltyContext _db;

        public LoyaltyAdminController(LoyaltyContext db) => _db = db;

        // Adds product with SKU, name, and price
        // SKU is stock keeping unit - unique identifier of product inventory
        [Route("products"), HttpPost]
        public IActionResult AddProduct([FromBody] ProductDto product)
        {
            _db.Products.Add(ProductExt.Construct(product.Sku, product.Name, product.Price));

            _db.SaveChanges();

            return Ok();
        }

        // Sets special offering for a given product
        // Special offering is either adding extra points, or multiplying normal points by some coefficient
        // extra param is amount to add to normal bonus or coeff to multiply normal bonus by
        [Route("products/{sku}/special-offerings"), HttpPost]
        public IActionResult AddSpecialOffering([FromRoute] string sku, [FromBody] PromotionDto promo)
        {
            var prod = ProductExt.GetBySku(_db, sku);

            _db.SpecialOfferings.Add(
                SpecialOfferingExt.Construct(prod, promo.PromotionType, promo.PromotionValue)
            );

            _db.SaveChanges();

            return Ok();
        }

        // Removes all special offerings for the given product
        [Route("products/{sku}/special-offerings"), HttpDelete]
        public IActionResult RemoveSpecialOffering([FromRoute] string sku)
        {
            _db.SpecialOfferings.RemoveRange(SpecialOfferingExt.ForProduct(_db, sku));

            _db.SaveChanges();

            return Ok();
        }

        // Adds customer to loyalty program
        // Returns new loyalty card ID
        [Route("loyalties"), HttpPost]
        public ActionResult<string> LaunchLoyalty([FromBody] NewLoyaltyDto newLoyalty)
        {
            var cust = _db.Customers.Add(
                CustomerExt.Construct(newLoyalty.CustomerName, newLoyalty.CustomerPhone)
            ).Entity;

            _db.SaveChanges();

            return Ok(cust.LoyaltyCardId);
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
