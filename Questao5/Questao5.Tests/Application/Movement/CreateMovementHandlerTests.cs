using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Questao5.Application.Interfaces;
using Questao5.Application.Movement;
using Questao5.Domain.Entities;
using Questao5.Domain.Errors;

namespace Questao5.Tests.Application.Movement
{
    public class CreateMovementHandlerTests
    {
        private readonly Mock<ICurrentAccountRepository> _mockAccountRepository;
        private readonly Mock<IMovementRepository> _mockMovementRepository;
        private readonly Mock<IIdempotenciaRepository> _mockIdempotenciaRepository;
        private readonly CreateMovementHandler _sut;

        public CreateMovementHandlerTests()
        {
            _mockAccountRepository = new Mock<ICurrentAccountRepository>();
            _mockMovementRepository = new Mock<IMovementRepository>();
            _mockIdempotenciaRepository = new Mock<IIdempotenciaRepository>();

            _sut = new CreateMovementHandler(
                _mockAccountRepository.Object,
                _mockMovementRepository.Object,
                _mockIdempotenciaRepository.Object
            );
        }

        private CreateMovementCommand CreateCommand(
            decimal value = 100,
            string movementType = "C",
            string currentAccountId = "123",
            string idempotencyKey = "abc")
        {
            return new CreateMovementCommand
            {
                Value = value,
                MovementType = movementType,
                CurrentAccountId = currentAccountId,
                Idempotencia = idempotencyKey
            };
        }

        #region Input Validation Tests

        [Fact]
        public async Task Handle_ShouldThrowException_WhenValueIsNonPositive()
        {
            var command = CreateCommand(value: -10);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_VALUE: Value must be positive.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenMovementTypeIsInvalid()
        {
            var command = CreateCommand(movementType: "X");

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_TYPE: Invalid type. Must be 'C' or 'D'.", exception.Message);
        }

        #endregion

        #region Account Validation Tests

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAccountIsNotFound()
        {
            var command = CreateCommand();

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((CurrentAccount?)null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(command, CancellationToken.None));

            Assert.Equal("INVALID_ACCOUNT: Account not found.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAccountIsInactive()
        {
            var command = CreateCommand();

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new CurrentAccount { Active = false });

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.Handle(command, CancellationToken.None));

            Assert.Equal("INACTIVE_ACCOUNT: Account is inactive.", exception.Message);
        }

        #endregion

        #region Idempotency Tests

        [Fact]
        public async Task Handle_ShouldReturnExistingResult_WhenIdempotencyKeyExists()
        {
            var command = CreateCommand();
            var existingResultJson = "{\"MovementId\":\"existing-id\"}";

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new CurrentAccount { Active = true });

            _mockIdempotenciaRepository
                .Setup(repo => repo.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockIdempotenciaRepository
                .Setup(repo => repo.GetResultadoAsync(It.IsAny<string>()))
                .ReturnsAsync(existingResultJson);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("existing-id", result.Data.MovementId);
        }

        #endregion

        #region Movement Creation Tests

        [Fact]
        public async Task Handle_ShouldCreateMovement_WhenAllConditionsAreMet()
        {
            var command = CreateCommand();

            _mockAccountRepository
                .Setup(repo => repo.GetCurrentAccountByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new CurrentAccount { Active = true });

            _mockIdempotenciaRepository
                .Setup(repo => repo.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data.MovementId);

            _mockMovementRepository.Verify(repo => repo.InsertMovementAsync(It.IsAny<Domain.Entities.Movement>()), Times.Once);
            _mockIdempotenciaRepository.Verify(repo => repo.SaveAsync(It.IsAny<string>(), It.IsAny<CreateMovementResponse>()), Times.Once);
        }

        #endregion
    }
}