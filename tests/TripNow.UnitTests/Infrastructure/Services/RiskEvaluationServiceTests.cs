using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TripNow.Domain.Enums;
using TripNow.Domain.Services;
using TripNow.Infrastructure.Extensions;
using TripNow.Infrastructure.Services;
using Xunit;

namespace TripNow.UnitTests.Infrastructure.Services;

public class RiskEvaluationServiceTests
{
    [Fact]
    public void Result_ShouldDeserializeCorrectlyHasCamelCaseAndNumericScore()
    {
        var jsonResponse = "{\"riskScore\":82.29,\"status\":\"REJECTED\"}";
        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<RiskEvaluationServiceResult>(jsonResponse, jsonOptions);
        Assert.NotNull(response);
        Assert.Equal(82.29, response.RiskScore);
        Assert.Equal("REJECTED", response.Status);
    }


    [Fact]
    public void Result_ShouldDeserializeCorrectlyHasCamelCaseAndNumericScore_AsReservationStatus()
    {
        var jsonResponse = "{\"riskScore\":82.29,\"status\":\"REJECTED\"}";
        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        var response = JsonSerializer.Deserialize<RiskEvaluationServiceResult>(jsonResponse, jsonOptions);
        Assert.NotNull(response);
        Assert.Equal(82.29, response.RiskScore);
        Assert.Equal(ReservationStatus.Rejected, response.Status.AsReservationStatus());
    }

    [Fact]
    public async Task EvaluateAsync_ShouldDeserializeCorrecty_WhenResponseHasCamelCaseAndNumericScore()
    {
        // Arrange
        var jsonResponse = "{\"riskScore\":82.29,\"status\":\"REJECTED\"}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://test-service")
        };

        var loggerMock = new Mock<ILogger<RiskEvaluationService>>();
        var service = new RiskEvaluationService(httpClient, loggerMock.Object);
        var request = new RiskEvaluationRequest 
        {
            CustomerEmail = "test@example.com",
            Amount = 12,
            TripCountry = "USA"
        };

        // Act
        var result = await service.EvaluateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(82.29, result.RiskScore);
        Assert.Equal(ReservationStatus.Rejected, result.Status);
    }
}
