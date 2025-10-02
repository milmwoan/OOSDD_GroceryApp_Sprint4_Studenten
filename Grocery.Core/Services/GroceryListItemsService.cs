using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)

        {
            var allBoughtItems = _groceriesRepository.GetAll(); // haalt alle verkochte producten op

            var groupedItems = allBoughtItems
                .GroupBy(item => item.ProductId) // verkochte producten worden gegroepeerd op productID
                .Select(group =>
                {
                    var product = _productRepository.Get(group.Key);

                    return new
                    {
                        ProductId = group.Key,
                        ProductName = product?.Name ?? "Onbekend product", // als Product.Name null is, wordt deze string ingevoerd
                        Stock = product?.Stock ?? 0, // als product.Stock null is, wordt de stock 0
                        NrOfSells = group.Sum(item => item.Amount) // per groep(product) worden de amounts bij elkaar opgeteld
                    };
                })
                .OrderByDescending(p => p.NrOfSells) //van hoog naar laag sorteren
                .Take(topX)
                .ToList();

            var bestSellingProducts = groupedItems // zet de gegroepeerde items om in bestsellingproducts en geeft elk item een ranking mee
                .Select((item, index) => new BestSellingProducts(
                    item.ProductId,
                    item.ProductName,
                    item.Stock,
                    item.NrOfSells,
                    index + 1 // zodat de ranking start op 1
                ))
                .ToList();

            return bestSellingProducts;
        }

        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
