# PhonoArk

音律方舟,将"音标法则"比作"声音的韵律"，而"方舟"是承载希望的工具.

A cross-platform English learning application for mastering International Phonetic Alphabet (IPA) and pronunciation.

## Features

### 🎯 IPA Learning
- **Interactive IPA Chart**: Complete phonetic alphabet organized by vowels, diphthongs, and consonants
- **Dual Accent Support**: Switch between American (GenAm) and British (RP) pronunciation
- **Example Words**: 4-6 example words for each phoneme with IPA transcriptions
- **Audio Playback**: Listen to pure phoneme sounds and word pronunciations
- **Favorites System**: Bookmark phonemes and organize them into custom groups

### 📝 Practice & Exams
- **Random Exams**: Test your knowledge with customizable question counts
- **Scope Selection**: Practice all phonemes or focus on favorites
- **Instant Feedback**: See correct answers immediately after selection
- **Progress Tracking**: View exam history with scores, dates, and duration
- **Statistical Analysis**: Track your average performance over time

### ⚙️ Settings
- **Default Accent**: Choose between American or British pronunciation
- **Volume Control**: Adjust audio playback volume
- **Exam Configuration**: Set default number of questions
- **Theme Toggle**: Switch between light and dark modes
- **Study Reminders**: Optional learning reminders (coming soon)

### 📖 Additional Features
- **Word Learning Module**: Placeholder for future vocabulary study feature
- **Local Persistence**: All data stored locally with SQLite
- **Cross-Platform**: Runs on Windows, Android, iOS (with appropriate workloads)

## Technical Stack

- **Framework**: .NET 10.0 with Avalonia UI 11.3
- **UI Rendering**: SkiaSharp for graphics
- **Database**: SQLite with Entity Framework Core
- **Architecture**: MVVM pattern with CommunityToolkit.Mvvm
- **Platform Support**: Desktop (Windows/Linux/macOS), Mobile (Android/iOS), Web (Browser)

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later
- Visual Studio 2022 or JetBrains Rider (recommended)

### Building the Application

1. Clone the repository:
\`\`\`bash
git clone https://github.com/kukisama/PhonoArk.git
cd PhonoArk
\`\`\`

2. Restore dependencies:
\`\`\`bash
cd PhonoArk
dotnet restore
\`\`\`

3. Build the desktop application:
\`\`\`bash
dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj
\`\`\`

4. Run the application:
\`\`\`bash
dotnet run --project PhonoArk.Desktop/PhonoArk.Desktop.csproj
\`\`\`

### Building for Mobile

For Android:
\`\`\`bash
# Install Android workload first
dotnet workload install android
dotnet build PhonoArk.Android/PhonoArk.Android.csproj
\`\`\`

For iOS:
\`\`\`bash
# Install iOS workload first (macOS only)
dotnet workload install ios
dotnet build PhonoArk.iOS/PhonoArk.iOS.csproj
\`\`\`

## Project Structure

\`\`\`
PhonoArk/
├── PhonoArk/              # Core shared library
│   ├── Models/            # Data models
│   ├── ViewModels/        # MVVM view models
│   ├── Views/             # Avalonia XAML views
│   ├── Services/          # Business logic services
│   ├── Data/              # Database context
│   └── Converters/        # Value converters
├── PhonoArk.Desktop/      # Desktop platform project
├── PhonoArk.Android/      # Android platform project
├── PhonoArk.iOS/          # iOS platform project
└── PhonoArk.Browser/      # WebAssembly project
\`\`\`

## Data Models

- **Phoneme**: IPA symbol with type, description, and example words
- **ExampleWord**: Word with IPA transcription and audio paths
- **FavoritePhoneme**: User's bookmarked phonemes with groups
- **ExamResult**: Exam history with scores and statistics
- **AppSettings**: User preferences and configuration

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is open source and available under the MIT License.

## Roadmap

- [ ] Add actual audio files for phonemes and words
- [ ] Implement word learning module with flashcards
- [ ] Add spaced repetition algorithm
- [ ] Include pronunciation recording and comparison
- [ ] Add more comprehensive exam modes
- [ ] Implement cloud sync for cross-device progress
- [ ] Support additional languages and accents
- [ ] Add gamification elements (achievements, streaks)

## Credits

Developed with ❤️ using Avalonia UI and .NET
