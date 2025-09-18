using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Application.Dtos.product;
using Ecommerce.Application.Mappings;
using Ecommerce.Application.services;
using Ecommerce.Domain.entities;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.services.ProductServices;
using Ecommerce.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Ecommerce.Tests.Unit.ProductSErviceTests
{
	public class ProductServiceTests
	{
        //in memory database
		private static AppDbContext CreateDbContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;
			return new AppDbContext(options);
		}

		private static IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<ProductMappingProfile>();
			});
			return config.CreateMapper();
		}

		[Fact]
		public async Task GetByIdAsync_Throws_When_NotFound()
		{
			// arrange
			await using var db = CreateDbContext();
			var mapper = CreateMapper();
			var fileStorageMock = new Mock<IFileStorageService>();
			var service = new ProductService(db, fileStorageMock.Object, mapper);

			// act
			Func<Task> act = async () => await service.GetByIdAsync(123);

			// assert
			await act.Should().ThrowAsync<Exception>()
				.WithMessage("Product not found");
		}

		[Fact]
		public async Task GetAllAsync_Returns_Mapped_Dtos()
		{
			// arrange
			await using var db = CreateDbContext();
			var mapper = CreateMapper();
			var fileStorageMock = new Mock<IFileStorageService>();
			var service = new ProductService(db, fileStorageMock.Object, mapper);

            db.Products.AddRange(
			   new Product { Name = "P1", ProductCode = "P01",
					Category = "Cat",Price = 10, MinimumQuantity = 1, DiscountRate = 0,ImagePath = "images/products/a.png"},
			   new Product { Name = "P2", ProductCode = "P02",
					Category = "Cat",Price = 20,MinimumQuantity = 2,DiscountRate = 5,ImagePath = null}
		    );
            await db.SaveChangesAsync();

			// act
			var result = await service.GetAllAsync();

			// assert
			result.Should().HaveCount(2);
			result.Select(r => r.ProductCode).Should().BeEquivalentTo(new[] { "P01", "P02" });
		}

		[Fact]
		public async Task UpdateAsync_Replaces_Image_And_Deletes_Old()
		{
			// arrange
			await using var db = CreateDbContext();
			var mapper = CreateMapper();
			var fileStorageMock = new Mock<IFileStorageService>();
			fileStorageMock.Setup(m => m.DeleteFile(It.IsAny<string>()));
			fileStorageMock.Setup(m => m.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
				.ReturnsAsync("images/products/new.png");
			var service = new ProductService(db, fileStorageMock.Object, mapper);

			var product = new Product
			{
				Name = "OldName",
				ProductCode = "P01",
				Category = "Cat",
				Price = 100,
				MinimumQuantity = 1,
				DiscountRate = 0,
				ImagePath = "images/products/old.png"
			};
			db.Products.Add(product);
			await db.SaveChangesAsync();

			// new image
			await using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
			IFormFile formFile = new FormFile(stream, 0, stream.Length, "Image", "new.png")
			{
				Headers = new HeaderDictionary(),
				ContentType = "image/png"
			};

			var dto = new UpdateProductDto
			{
				Name = "NewName",
				Category = "NewCat",
				Price = 200,
				MinimumQuantity = 2,
				DiscountRate = 10,
				Image = formFile
			};

			// act
			var updated = await service.UpdateAsync(product.Id, dto);

			// assert
			updated.Name.Should().Be("NewName");
			fileStorageMock.Verify(m => m.DeleteFile("images/products/old.png"), Times.Once);
			updated.ImageUrl.Should().Be("images/products/new.png");
		}

		[Fact]
		public async Task DeleteAsync_Removes_Entity_And_Deletes_Image()
		{
			// arrange
			await using var db = CreateDbContext();
			var mapper = CreateMapper();
			var fileStorageMock = new Mock<IFileStorageService>();
			var service = new ProductService(db, fileStorageMock.Object, mapper);

			var product = new Product
			{
				Name = "ToDelete",
				ProductCode = "P03",
				Category = "Cat",
				Price = 50,
				MinimumQuantity = 1,
				DiscountRate = 0,
				ImagePath = "images/products/del.png"
			};
			db.Products.Add(product);
			await db.SaveChangesAsync();

			// act
			await service.DeleteAsync(product.Id);

			// assert
			(db.Products.Count()).Should().Be(0);
			fileStorageMock.Verify(m => m.DeleteFile("images/products/del.png"), Times.Once);
		}
	}
}
