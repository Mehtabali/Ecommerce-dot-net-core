using Catalog.Core.Entities;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        // Backward-compatible overload (keeps previous API) - will create its own client.
        public static Task SeedAsync(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var loggerFactory = LoggerFactory.Create(builder => { });
            var logger = loggerFactory.CreateLogger<DatabaseSeeder>();
            return SeedAsync(client, options, logger);
        }

        // Preferred overload: use the application's registered IMongoClient and an ILogger
        public static async Task SeedAsync(IMongoClient client, IOptions<DatabaseSettings> options, ILogger logger)
        {
            try
            {
                var settings = options.Value;
                var db = client.GetDatabase(settings.DatabaseName);
                var brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
                var types = db.GetCollection<ProductType>(settings.TypeCollectionName);
                var products = db.GetCollection<Product>(settings.ProductCollectionName);

                // Use the application's base directory so seed files copied to output are found
                var SeedBasePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");

                // Seed Brands
                List<ProductBrand> brandList = new();

                if ((await brands.CountDocumentsAsync(_ => true) == 0))
                {
                    var brandData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "brands.json"));
                    brandList = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                    if (brandList != null && brandList.Count > 0)
                        await brands.InsertManyAsync(brandList);
                }
                else
                {
                    brandList = await brands.Find(_ => true).ToListAsync();
                }

                // Seed Types
                List<ProductType> typeList = new();
                if ((await types.CountDocumentsAsync(_ => true) == 0))
                {
                    var typeData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "types.json"));
                    typeList = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                    if (typeList != null && typeList.Count > 0)
                        await types.InsertManyAsync(typeList);
                }
                else
                {
                    typeList =  await types.Find(_ => true).ToListAsync();
                }

                // Seed products
                if ((await products.CountDocumentsAsync(_ => true)) == 0)
                {
                    var productData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "products.json"));
                    var productList = JsonSerializer.Deserialize<List<Product>>(productData);

                    if (productList != null)
                    {
                        foreach (var product in productList)
                        {
                            // Reset Id to let Mongo generate one
                            product.Id = null;

                            // Default Created Date if not set
                            if (product.CreatedDate == default)
                                product.CreatedDate = DateTime.UtcNow;
                        }

                        if (productList.Count > 0)
                            await products.InsertManyAsync(productList);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception but do not crash the application during startup seeding.
                logger.LogError(ex, "An error occurred while attempting to seed the database.");
            }
        }

    }
}
