using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Enums;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;


namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _globalViewModel;

        [ObservableProperty]
        string clientName = "";
        
        public GroceryListViewModel(IGroceryListService groceryListService,GlobalViewModel globalViewModel) 
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
           _globalViewModel = globalViewModel;
            GroceryLists = new(_groceryListService.GetAll());
            clientName = _globalViewModel.Client.Name; // Client naam wordt opgehaald uit de globalviewmodal
            
        }
        

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }
        [RelayCommand]
        public async Task ShowBoughtProducts()
        {
            // Controleer of de Client admin is
            if (_globalViewModel.Client.UserRole == Role.Admin)
            {
                await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
            }
            else
            {
                // Niet admin → doe niets, of toon optioneel een waarschuwing
                return;
            }
        }
    }
}
