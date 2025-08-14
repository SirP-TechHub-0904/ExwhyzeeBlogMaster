# ExwhyzeeBlogMaster 🚀  
**A modern AI-powered multi-portal blogging platform** built with ASP.NET Core for powering diverse blog ecosystems — including news, policies, people updates, entertainment, and editorial content.


![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Tech Stack](https://img.shields.io/badge/tech-.NET%208%20%7C%20Razor%20Pages%20%7C%20Vite-blue)

---

## ✨ Overview

ExwhyzzeeBlogMaster is an intelligent, scalable, and extensible blog platform designed to accommodate multiple content domains. It includes admin dashboards, public portals, AI integration, and dynamic content tools — empowering creators, journalists, and admins alike.

## ✨ Key Features

### 🔹 Modular Blog Portals

- **PoliciesBlog**: Publish and manage civic and governance-related articles.
- **PeopleNewsBlog**: Highlight achievements, celebrations, and community updates.
- **Enewspaper**: Structured like an online newspaper with daily sections.
- **TalkDTalk**: Opinion and editorial zone for open dialogue.
- **AfriRanking**: Highlight rankings across various sectors (institutions, personalities, innovation).

### 🔹 AdminUI Dashboard

- Role-based access for admins and contributors.
- Content management with WYSIWYG editors.
- Media uploads and storage.
- Dynamic statistics and post performance.

### 🔹 AI-Powered Content Generation 🧠

- Generate blog **titles**, **summaries**, and **full HTML content** using Gemini API.
- Custom prompts per tone and topic.
- Spinner loading effect + error alerts when API quota is exceeded.

> ⚠️ _Note: API integration includes quota error handling for Google Gemini. Upgrade or manage your usage limits [here](https://ai.google.dev/gemini-api/docs/rate-limits)._

### 🔹 Intelligent Features

- Auto SEO with meta injection per post.
- Social sharing bar.
- Bookmarkable and shareable slugs.
- Real-time date search and tag filtering.
- Dynamic newsletter section.

---

## 🏗️ Project Structure

## 📸 Screenshots

> *Upload screenshots to `docs/images/` and link here.*

| Dashboard | AI Content Prompt | Post Editor |
|----------|------------------|-------------|
| ![Dashboard](docs/images/dashboard.png) | ![Prompt](docs/images/prompt.png) | ![Editor](docs/images/editor.png) |

---

## 🛠️ Built With

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/)
- [Bootstrap 5](https://getbootstrap.com/)
- [Vite](https://vitejs.dev/)
- [Google Gemini API](https://ai.google.dev/gemini-api/docs)
- [Summernote WYSIWYG Editor](https://summernote.org/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## 📦 Folder Structure

ExwhyzzeeBlogMaster/
├── AdminUI/ # Admin dashboard project
│ ├── Pages/
│ ├── Controllers/
│ ├── Areas/
│ ├── wwwroot/ # Static assets
│ ├── Program.cs
│ └── ServiceCollectionExtensions.cs
├── BlogWorkerService/ # Background worker for publishing/sync tasks
├── AfriRanking/ # Sub-project for ranking stories
├── PeopleNewsBlog/
├── PoliciesBlog/
├── TalkDTalk/
├── Enewspaper/
├── ModelData/ # Shared EF models & database logic


---

## 💻 Technologies Used

- **ASP.NET Core Razor Pages**
- **Entity Framework Core**
- **C# 10**
- **JavaScript (Vanilla + jQuery)**
- **HTML/CSS/Bootstrap**
- **Google Gemini AI API**
- **SQL Server**

---

## 🧠 AI Content Integration Example

```csharp
// Generate title
var titlePrompt = $"Generate a compelling blog post title for the topic: \"{topic}\" in a {tone} tone. The title should be between 6 to 12 words.";
var title = await GenerateSingleRetryAsync(titlePrompt);

// Generate summary
var shortPrompt = $"Write a concise 1-2 sentence summary of a blog post on \"{topic}\" in a {tone} tone.";
var summary = await GenerateSingleRetryAsync(shortPrompt);

// Generate full HTML content
var contentPrompt = $@"
Write an **HTML-formatted** blog post on \"{topic}\" in a {tone} tone, around {wordCount} words.
Structure:
- Introduction (2–3 sentences)
- Three distinct body paragraphs
- Conclusion with key insights
";
var fullContentHtml = await GenerateSingleRetryAsync(contentPrompt);

git clone https://github.com/SirP-TechHub-0904/ExwhyzeeBlogMaster.git
cd ExwhyzeeBlogMaster



dotnet restore
dotnet build
dotnet run

{
  "GeminiApiKey": "YOUR_GOOGLE_GEMINI_KEY",
  "UploadSettings": {
    "MaxImageSizeKB": 500,
    "MaxVideoSizeKB": 10240
  }
}



⚠️ Notes
You need a valid Google Gemini API Key to use the AI generation features.

Make sure billing is enabled to avoid 429 TooManyRequests errors.

To avoid quota exhaustion, consider caching or rate-limiting your calls.

🧪 Future Improvements
🔄 Real-time autosave while writing

🌐 Multilingual content generation

📊 Post analytics dashboard

🧩 Plugin support system

✍️ Scheduled publishing

🙌 Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you’d like to change.

📄 License
MIT License

🤝 Acknowledgments
Google Gemini API

Bootstrap Icons

All open-source contributors


---

Would you like me to save this as a `.md` file and upload it for you as well?

