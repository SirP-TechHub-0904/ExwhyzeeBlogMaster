# ExwhyzeeBlogMaster 🚀  
A modern, flexible, and AI-powered blogging platform.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Tech Stack](https://img.shields.io/badge/tech-.NET%208%20%7C%20Razor%20Pages%20%7C%20Vite-blue)

---

## ✨ Overview

**ExwhyzeeBlogMaster** is a next-gen blogging CMS built with simplicity, performance, and AI integration at its core. Designed for both content creators and developers, it blends elegant UI with advanced content generation and customization features. It is perfect for individual bloggers, news platforms, and publishing companies looking to automate and enhance their publishing workflow.

---

## 💡 Features

- ✅ **AI Content Generator (Gemini API)**
  - Generate blog titles, summaries, and full articles using Google Gemini.
  - Customizable tone and word count per request.

- ✅ **Modern Blog Management**
  - Post editor with rich text support using Summernote.
  - Create, update, delete posts.
  - Manage featured images and metadata.

- ✅ **SEO Ready**
  - Meta description, tags, social sharing setup.
  - SEO preview built-in.

- ✅ **Live Preview and Publishing**
  - See generated content before publishing.
  - Insert AI-generated content into your post with one click.

- ✅ **Custom Themes**
  - Light & Dark mode support.
  - Easily switch themes from settings.

- ✅ **Media Upload Support**
  - Supports video and image uploads with size limits.
  - Progress bars and resumable uploads.

- ✅ **Modular Codebase**
  - Clean Razor Page structure.
  - DI ready, highly maintainable.

- ✅ **User Roles & Access**
  - Admin dashboard with post analytics.
  - User access control and permission logic.

---

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

```bash
ExwhyzeeBlogMaster/
├── src/
│   ├── Pages/
│   ├── Services/
│   ├── ViewModels/
│   └── wwwroot/
├── .gitignore
├── ExwhyzeeBlogMaster.sln
└── README.md



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

