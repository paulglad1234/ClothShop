﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Catalog.Requests;
using Services.DbConnection;

namespace Services.Catalog
{
    internal class CatalogService : ICatalog
    {
        private readonly ShopContext _dbContext;

        public CatalogService(IShopConnection dbContext)
        {
            _dbContext = dbContext.Context;
        }

        public async Task<List<Product>> GetWithFilters(CatalogFilters filters)
        {
            var products = filters.SortByPrice
                ? _dbContext.Product.OrderBy(product => product.Price).AsQueryable()
                : _dbContext.Product.OrderByDescending(product => product.Price).AsQueryable();

            if (!string.IsNullOrEmpty(filters.NameContains))
                products = products.Where(product => product.Name.Contains(filters.NameContains));

            if (filters.PriceHigherThan != null)
                products = products.Where(product => product.Price >= filters.PriceHigherThan);

            if (filters.PriceLowerThan != null)
                products = products.Where(product => product.Price <= filters.PriceLowerThan);

            if (filters.Genders != null)
                products = products.Where(product => filters.Genders.Contains(product.Gender));

            if (filters.Categories != null)
                products = products.Where(product => filters.Categories.Contains(product.Category));

            if (filters.Brands != null)
                products = products.Where(product => filters.Brands.Contains(product.Brand));

            if (filters.Colors != null)
                products = products.Where(product => filters.Colors.Contains(product.Color));

            if (filters.Sizes != null)
            {
                var ids = _dbContext.ProductSize.Where(ps => filters.Sizes.Contains(ps.Size))
                    .Select(ps => ps.ProductId);
                products = products.Where(product => ids.Contains(product.Id));
            }

            return await products.Where(product => _dbContext.ProductSize.Any(ps => ps.ProductId == product.Id && ps.Quantity > 0)).ToListAsync();
        }

        public async Task<Product> GetProduct(int productId)
        {
            var product = await _dbContext.Product.FindAsync(productId);
            product.ProductSize.Clear();
            foreach (var productSize in _dbContext.ProductSize.Where(ps => ps.ProductId == productId))
            {
                product.ProductSize.Add(productSize);
            }
            return product;
        }

        public async Task AddProduct(AddItem request, byte[] image)
        {
            //if (request.Sizes.Length != request.Quantities.Length)
                //throw new Exception("Массивы размеров и их количеств не совпадают!");
            var product = await _dbContext.Product.AddAsync(new Product
            {
                VendorCode = request.VendorCode,
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                Gender = request.Gender,
                Category = request.Category,
                Brand = request.Brand,
                Color = request.Color,
                Image = image
            });
            await _dbContext.SaveChangesAsync();
            for (var i = 0; i < request.Sizes.Length; i++)
            {
                if (request.Quantities[i] > 0)
                    await _dbContext.ProductSize.AddAsync(new ProductSize
                    {
                        ProductId = product.Entity.Id,
                        Size = request.Sizes[i],
                        Quantity = request.Quantities[i]
                    });
            }

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task AddToBag(int productId, string size, int userId)
        {
            await _dbContext.Bag.AddAsync(new Bag
            {
                UserId = userId,
                ProductId = productId,
                Size = size
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFromBag(int productId, string size, int userId)
        {
            _dbContext.Remove(await _dbContext.Bag.FindAsync(userId, productId, size));
            await _dbContext.SaveChangesAsync();
        }
    }
}