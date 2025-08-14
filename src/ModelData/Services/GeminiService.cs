using ModelData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Services
{
    public class GeminiService
    {
        private readonly Setting _settings;

        public GeminiService(Setting settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<string> GenerateGeminiContentAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
                return "[ERROR] Missing API key.";

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_settings.GeminiApiKey}";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },
                tools = new[]
                {
            new { googleSearch = new { } }
        }
            };

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(apiUrl, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"[ERROR] API call failed: {response.StatusCode}\n{error}";
            }

            var json = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var content = json?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            return content?.Trim() ?? "[ERROR] No response content.";
        }

        public async Task<AiContentResult> GenerateContentAsync(string topic, string tone = "Professional", int wordCount = 500)
        {
            if (string.IsNullOrWhiteSpace(_settings.GeminiApiKey))
            {
                return new AiContentResult
                {
                    IsError = true,
                    ErrorMessage = "Invalid API key. Please set your Gemini API key in settings."
                };
            }
            // 1. Generate Blog Title
            var titlePrompt = $@"
You are a professional blog title generator.

Task:
Generate a compelling and click-worthy blog post title on the topic: ""{topic}""
Tone: {tone}
Requirements:
- Use between 6 to 12 words
- Do NOT include quotation marks or any extra text
- Just output the title alone, no prefixes

Output:
Title:
";

            var title = await GenerateSingleRetryAsync(titlePrompt);


            // 2. Generate Short Description (Meta Description / Excerpt)
            var shortPrompt = $@"
You are an expert blog summarizer.

Task:
Write a concise 1–2 sentence summary for a blog post on the topic: ""{topic}""
Tone: {tone}
Goal:
- Provide a summary that could serve as a meta description or preview
- Do not exceed 2 sentences

Output:
";

            var shortDescription = await GenerateSingleRetryAsync(shortPrompt);


            // 3. Generate Full HTML Blog Content
            var contentPrompt = $@"
You are an expert content writer generating a structured HTML blog post.

Task:
Write an engaging HTML-formatted blog post about: ""{topic}""
Tone: {tone}
Length: Approx. {wordCount} words

Structure:
- <h2>Introduction</h2> (2–3 sentences)
- <h2>Main Body</h2> with:
  - <h3>Point 1</h3>
  - <h3>Point 2</h3>
  - <h3>Point 3</h3>
  (Each point should contain rich examples or real-world context in <p> tags)
- <h2>Conclusion</h2> with a summary or call to action

Formatting:
- Use semantic HTML tags only
- Do not wrap the whole content in a single <div> or <article> tag
- Use proper <p>, <ul>, and <strong> where necessary
- Avoid inline styles or script tags

Output:
";

            var fullContentHtml = await GenerateSingleRetryAsync(contentPrompt);


            return new AiContentResult
            {
                Title = title,
                ShortDescription = shortDescription,
                FullContentHtml = fullContentHtml
            };
        }

        private async Task<string> GenerateSingleRetryAsync(string prompt)
        {
            int retry = 0;
            while (retry < 3)
            {
                var result = await GenerateGeminiContentAsync(prompt);

                if (!result.StartsWith("[ERROR]"))
                    return result;

                if (result.Contains("model is overloaded"))
                {
                    await Task.Delay(2000); // wait 2s before retry
                    retry++;
                }
                else
                {
                    return result; // return non-retryable error
                }
            }

            return "[ERROR] Max retries exceeded.";
        }
    }

    public class AiContentResult
    {
        public bool IsError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string FullContentHtml { get; set; } = string.Empty;
    }

    public class GeminiResponse
    {
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }
}
