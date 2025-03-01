using System.Net;
using System.Net.Http.Json;
using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace EnglishDraughtsProjectTests;

public class UnitTest1
{
    private readonly Mock<ILogger<AiService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly AiService _aiService;

    public UnitTest1()
    {
        _mockLogger = new Mock<ILogger<AiService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };

        _aiService = new AiService("OPENAI_API_KEY", _mockLogger.Object, _httpClient);
    }

    [Fact]
    public async Task Test1()
    {
        var fakeResponse = new
        {
            choices = new[]
            {
                new
                {
                    message = new { content = "Move: B2 to C3" }
                }
            }
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(fakeResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var result = await _aiService.GetHintAsync(new Board(), true);

        Assert.Equal("Move: B2 to C3", result);
    }

    [Fact]
    public async Task Test2()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var result = await _aiService.GetHintAsync(new Board(), true);

        Assert.Contains("Error", result);
    }
}