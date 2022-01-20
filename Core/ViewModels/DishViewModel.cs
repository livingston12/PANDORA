using System;
using System.Collections.Generic;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.ViewModels
{
    public class DishViewModel
    {
        public int DishId { get; set; }
        public string Dish { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public CategoryEntity Category { get; set; }
        public IEnumerable<DishesDetailViewModel> Ingredients { get; set; }
    }

    public class DishViewModelCreate
    {
        public int DishId { get; set; }
        public string Dish { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public IEnumerable<DishDetailViewModel> Ingredients { get; set; }
    }

    public class DishesDetailViewModel
    {
        public int DishDetailId { get; set; }
    }

    public class DishDetailViewModel
    {
        public int DishDetailId { get; set; }
        public IngredientViewModel Ingredient { get; set; }
        public int QuantityRequired { get; set; }
    }

    public class DishDetailViewModelCreate
    {
        public int DishId { get; set; }
        public int IngredientId { get; set; }
        public int QuantityRequired { get; set; }
    }
}