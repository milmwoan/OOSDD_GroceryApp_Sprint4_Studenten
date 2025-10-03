using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestCore
{
    public class TestHelpers
    {
        private Mock<IGroceryListItemsRepository> _groceryListItemsRepoMock;
        private Mock<IProductRepository> _productRepoMock;
        private GroceryListItemsService _service;
        [SetUp]
        public void Setup()
        {
            _groceryListItemsRepoMock = new Mock<IGroceryListItemsRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _service = new GroceryListItemsService(_groceryListItemsRepoMock.Object, _productRepoMock.Object);
        }


        //Happy flow TestPasswordHelper
        [Test]
        public void TestPasswordHelperReturnsTrue()
        {
            string password = "user3";
            string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        public void TestPasswordHelperReturnsTrue(string password, string passwordHash)
        {
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }


        //Unhappy flow TestPasswordHelper
        [Test]
        public void TestPasswordHelperReturnsFalse()
        {
            string password = "user3";
            string passwordHash = "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=";
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        [TestCase("user3", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=")]
        public void TestPasswordHelperReturnsFalse(string password, string passwordHash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        //Happy flow GetBestSellingProducts
        [Test]
        public void GetBestSellingProducts_Returns_TopProducts_ByAmount()
        {
            //  maak mock GroceryListItems aan
            var groceryListItems = new List<GroceryListItem>
            {
                new(0,0,1,3),
                new(0,0,1,1),
                new(0,0,2,5),
                new(0,0,3,1) 
            };

            _groceryListItemsRepoMock.Setup(r => r.GetAll()).Returns(groceryListItems);

            // Setup productgegevens voor alle gebruikte ProductIds
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1, "Product A", 10));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2, "Product B", 20));
            _productRepoMock.Setup(r => r.Get(3)).Returns(new Product(3, "Product C", 30));

            // Act
            var result = _service.GetBestSellingProducts(topX: 2); // vraag top 2 op

            // Assert
            Assert.That(result.Count, Is.EqualTo(2), "TopX moet 2 resultaten teruggeven.");
            Assert.That(result[0].Id, Is.EqualTo(2), "Product 2 moet bovenaan staan (5x verkocht).");
            Assert.That(result[0].NrOfSells, Is.EqualTo(5));
            Assert.That(result[0].Ranking, Is.EqualTo(1));

            Assert.That(result[1].Id, Is.EqualTo(1), "Product 1 moet tweede zijn (3+1=4x verkocht).");
            Assert.That(result[1].NrOfSells, Is.EqualTo(4));
            Assert.That(result[1].Ranking, Is.EqualTo(2));
           
        }
    }
}