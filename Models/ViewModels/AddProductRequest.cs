using System.ComponentModel.DataAnnotations;
using UPINS.Models.Domain;

namespace UPINS.Models.ViewModels
{
    public class AddProductRequest
    {
        
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        //[MinLength(3, ErrorMessage = "El precio no puede ser menor a 3 dígitos.")]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Code { get; set; }

        public string ImageUrl { get; set; }

        public List<Product> Products { get; set; }

        public List<Product>? ProductsInCart { get; set; }

        public int TotalAmountShoppingCart { get; set; }

    }
}
