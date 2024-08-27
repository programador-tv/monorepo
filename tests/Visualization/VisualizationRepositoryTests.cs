using System;
using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests
{
	public class VisualizationRepositoryTests
	{
		private readonly IVisualizationsRepository _repository;
		private readonly ApplicationDbContext _context;

		public VisualizationRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
			_context = new(options);
			_repository = new VisualizationRepository(_context);

			var visualization = Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), "");

			if (!_context.Visualizations.Any())
			{
				_context.Visualizations.AddRange(new List<Visualization> { visualization });
				_context.SaveChanges();
			}
		}

		[Fact]
		public async Task GetVisualizationsByLiveIds_ShouldReturnLiveVisualizations()
		{
			List<Guid> liveId = [Guid.NewGuid(), Guid.NewGuid()];

			await _context.AddRangeAsync(
				Visualization.Create(liveId[0], Guid.NewGuid(), ""),
				Visualization.Create(liveId[1], Guid.NewGuid(), ""),
				Visualization.Create(liveId[0], Guid.NewGuid(), ""),
				Visualization.Create(Guid.NewGuid(), Guid.NewGuid(), "")
			);
			await _context.SaveChangesAsync();

			var result = await _repository.GetVisualizationsByLiveIds(liveId);
			Assert.NotNull(result);
			Assert.Equal(3, result.Count);
		}
	}
}