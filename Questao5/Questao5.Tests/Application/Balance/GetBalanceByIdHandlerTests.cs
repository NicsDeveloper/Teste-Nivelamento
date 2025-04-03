using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Questao5.Application.Balance.Queries;
using Questao5.Application.Balance.Queries.Handlers;
using Questao5.Application.Interfaces;
using Questao5.Domain.Entities;
using Questao5.Domain.Errors;

namespace Questao5.Tests.Application.Balance
{
    public class GetBalanceByIdHandlerTests
    {
        private readonly Mock<ICurrentAccountRepository> _mockAccountRepository;
        private readonly Mock<IMovementRepository> _mockMovementRepository;
        private readonly GetBalanceByIdHandler _sut;

        public GetBalanceByIdHandlerTests()
        {
            _mockAccountRepository = new Mock<ICurrentAccountRepository>();
            _mockMovementRepository = new Mock<IMovementRepository>();

            _sut = new GetBalanceByIdHandler(
                _mockAccountRepository.Object,
                _mockMovementRepository.Object
            );
        }

        private GetBalanceByIdQuery CreateQuery(string currentAccountId = "123")
        {
            return new GetBalanceByIdQuery
            {
                CurrentAccountId = currentAccountId
            };
        }

        #region Account Validation Tests

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAccountIsNotFound()
        {
            var query = CreateQuery();

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((CurrentAccount?)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(query, CancellationToken.None));

            Assert.Equal("INVALID_ACCOUNT: Current account not found.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAccountIsInactive()
        {
            var query = CreateQuery();

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new CurrentAccount { Active = false });

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(query, CancellationToken.None));

            Assert.Equal("INACTIVE_ACCOUNT: Current account is inactive.", exception.Message);
        }

        #endregion

        #region Balance Retrieval Tests

        [Fact]
        public async Task Handle_ShouldReturnBalance_WhenAccountIsValid()
        {
            var query = CreateQuery();
            var account = new CurrentAccount
            {
                Number = 12345,
                Name = "Test Account",
                Active = true
            };

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(account);

            _mockMovementRepository
                .Setup(repo => repo.GetBalance(It.IsAny<string>()))
                .ReturnsAsync(1000.50m);

            var result = await _sut.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(account.Number, result.Data.AccountNumber);
            Assert.Equal(account.Name, result.Data.AccountName);
            Assert.Equal(1000.50m, result.Data.Balance);
            Assert.False(string.IsNullOrEmpty(result.Data.ResponseDateTime));
        }

        #endregion
    }
}