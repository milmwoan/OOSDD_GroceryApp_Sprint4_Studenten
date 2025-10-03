
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            List<GroceryListItem> groceryListsitems = _groceryListItemsRepository.GetAll().Where(g => g.ProductId == productId).ToList(); // Maakt een lijst met alle verkochte producten met de gegeven productId
            List<BoughtProducts> boughtProducts = new();    

            foreach(GroceryListItem groceryListItem in groceryListsitems) // Haalt informatie op over de klant, de boodschappenlijst en het product en voegt dit toe aan de lijst met BoughtProducts
            {
                GroceryList groceryList = _groceryListRepository.Get(groceryListItem.GroceryListId);
                Client client = _clientRepository.Get(groceryList.ClientId);
                Product product = _productRepository.Get(groceryListItem.ProductId);
                boughtProducts.Add(new BoughtProducts(client, groceryList, product));
            }
            return boughtProducts;  
        }
    }
}
