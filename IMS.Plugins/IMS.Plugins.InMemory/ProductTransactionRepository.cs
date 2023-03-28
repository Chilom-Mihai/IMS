using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();

        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionRepository(
            IProductRepository productRepository, 
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository)
        {
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }
        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            var prod = await this.productRepository.GetProductByIdAsync(product.ProductID);
            if (prod != null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    if (pi.Inventory != null)
                    {
                        //add inventory transaction
                        await this.inventoryTransactionRepository.ProduceAsync(productionNumber,
                        pi.Inventory,
                        pi.InventoryQuantity * quantity,
                        doneBy,
                        -1);

                        //decrease the inventories
                        var inv = await this.inventoryRepository.GetInventoryByIdAsync(pi.InventoryID);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await this.inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }
            }

            //add product transaction
            this._productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductID = product.ProductID,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy
            });

        }

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            this._productTransactions.Add(new ProductTransaction
            {
                ActivityType = ProductTransactionType.SellProduct,
                ProductID = product.ProductID,
                QuantityBefore = product.Quantity,
                SONumber = salesOrderNumber,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = unitPrice
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            var products = (await productRepository.GetProductsByNameAsync(string.Empty)).ToList();

            var query = from it in this._productTransactions
                        join inv in products on it.ProductID equals inv.ProductID
                        where
                            (string.IsNullOrWhiteSpace(productName) || inv.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                            &&
                            (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || it.ActivityType == transactionType)
                        select new ProductTransaction
                        {
                            Product = inv,
                            ProductTransactionID = it.ProductTransactionID,
                            SONumber = it.SONumber,
                            ProductionNumber = it.ProductionNumber,
                            ProductID = it.ProductID,
                            QuantityBefore = it.QuantityBefore,
                            ActivityType = it.ActivityType,
                            QuantityAfter = it.QuantityAfter,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy,
                            UnitPrice = it.UnitPrice
                        };

            return query;
        }
    }
}
