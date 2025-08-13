using System;
using System.Collections.Generic;
using System.Linq;
using FinanceWallet.Application.DTOs;
using FinanceWallet.Application.Services;
using FinanceWallet.Domain.Entities;
using FinanceWallet.Domain.Interfaces;
using Moq;
using Xunit;

namespace FinanceWallet.Tests
{
    public class SpendingServiceTests
    {
        [Fact]
        public void AddSpending_ShouldAddSpendingToRepository()
        {
            // Arrange
            var mockCommandRepo = new Mock<ISpendingCommandRepository>();
            var mockQueryRepo = new Mock<ISpendingQueryRepository>();

            var service = new SpendingService(mockCommandRepo.Object, mockQueryRepo.Object);

            var dto = new CreateSpendingDto(50m, "Test Item", "TestCategory", DateTime.UtcNow);

            // Act
            service.AddSpending(dto);

            // Assert
            mockCommandRepo.Verify(
                repo => repo.Add(It.Is<Spending>(s =>
                    s.Amount == dto.Amount &&
                    s.Description == dto.Description &&
                    s.Category == dto.Category
                )), Times.Once);
        }
    }
}
