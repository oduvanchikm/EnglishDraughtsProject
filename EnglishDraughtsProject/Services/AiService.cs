using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EnglishDraughtsProject.Models;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject.Services;

public class AiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<AiService> _logger;

    public AiService(string apiKey, ILogger<AiService> logger)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        _logger = logger;
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    private string SerializeBoardState(Board board, bool isWhiteTurn)
    {
        StringBuilder serializedString = new StringBuilder();
        
        serializedString.AppendLine($"Current Turn: {(isWhiteTurn ? "White" : "Black")}");

        for (int j = 0; j < 8; ++j)
        {
            for (int i = 0; i < 8; ++i)
            {
                var cell = board.Cells[i, j];
                
                switch (cell.Value)
                {
                    case CellValueEnum.CellValue.Empty:
                        serializedString.Append(".");
                        break;
                    case CellValueEnum.CellValue.WhiteChecker:
                        serializedString.Append("W");
                        break;
                    case CellValueEnum.CellValue.BlackChecker:
                        serializedString.Append("B");
                        break;
                    case CellValueEnum.CellValue.WhiteKing:
                        serializedString.Append("WK");
                        break;
                    case CellValueEnum.CellValue.BlackKing:
                        serializedString.Append("BK");
                        break;
                }
                
                serializedString.Append(" ");
                
            }
            
            serializedString.AppendLine();
        }
        
        return serializedString.ToString();
    }

    public async Task<string> GetHintAsync(Board board, bool isWhiteTurn)
    {
        try
        {
            var boardState = SerializeBoardState(board, isWhiteTurn);
            Console.WriteLine($"Sending board state to AI:\n{boardState}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant for checkers. Provide the best move in standard notation." },
                    new { role = "user", content = $"Current position:\n{boardState}\nBest move?" }
                },
                max_tokens = 50
            };

            Console.WriteLine("Sending request to AI...");
            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

            Console.WriteLine($"Response status: {response.StatusCode}");
            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response JSON: {responseJson}");

            response.EnsureSuccessStatusCode();

            var responseObject = JsonSerializer.Deserialize<OpenAiResponse>(responseJson);
            return responseObject?.Choices[0]?.Message?.Content ?? "No hint available.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            _logger.LogError(ex, "Failed to get hint.");
            return $"Error: {ex.Message}";
        }
    }
}