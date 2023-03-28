using IMS.CoreBusiness;
using IMS.WebApp.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class SellViewModel
    {
        [Required]
        public string SalesOrderNumber { get; set; } = string.Empty;

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Quantity has to be grater or equal to 1.")]
        [SellEnsureEnoughProductQuantity]
        public int QuantityToSell { get; set; }

        [Required]
        [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Price has to be gtaer or equal to 0.")]
        public double UnitPrice { get; set; }

        public Product? Product { get; set; } = null;
    }
}