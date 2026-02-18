# PhonoArk Developer Documentation

## Architecture Overview

PhonoArk follows the MVVM (Model-View-ViewModel) pattern using Avalonia UI framework and CommunityToolkit.Mvvm for code generation.

### Project Structure

```
PhonoArk/
├── Models/              - Domain models and entities
├── ViewModels/          - MVVM view models with business logic
├── Views/               - Avalonia XAML user interface
├── Services/            - Business logic and data access
├── Data/                - Entity Framework Core DbContext
├── Converters/          - XAML value converters
└── Assets/              - Images, icons, and resources
```

### Key Components

#### Models
- **Phoneme**: Represents an IPA phoneme with example words
- **ExampleWord**: Word with IPA transcription and audio paths
- **FavoritePhoneme**: User's bookmarked phonemes
- **ExamResult**: Exam history record
- **AppSettings**: Application configuration

#### Services
- **PhonemeDataService**: Provides IPA data (44 phonemes with examples)
- **AudioService**: Handles audio playback (placeholder implementation)
- **FavoriteService**: Manages phoneme bookmarks
- **ExamService**: Generates and manages exam questions
- **ExamHistoryService**: Tracks exam results
- **SettingsService**: Manages app settings

#### ViewModels
- **MainViewModel**: Root view model with navigation
- **IpaChartViewModel**: IPA chart display and interaction
- **ExamViewModel**: Exam flow and question management
- **ExamHistoryViewModel**: History display
- **FavoritesViewModel**: Favorites management
- **SettingsViewModel**: Settings configuration

## Database Schema

Using SQLite with Entity Framework Core:

```sql
-- FavoritePhonemes
Id (INTEGER PRIMARY KEY)
PhonemeSymbol (TEXT)
GroupName (TEXT DEFAULT 'Default')
CreatedAt (TEXT)

-- ExamResults
Id (INTEGER PRIMARY KEY)
ExamDate (TEXT)
TotalQuestions (INTEGER)
CorrectAnswers (INTEGER)
ExamScope (TEXT DEFAULT 'All')
Duration (TEXT)

-- Settings
Id (INTEGER PRIMARY KEY)
DefaultAccent (INTEGER)
Volume (REAL)
ExamQuestionCount (INTEGER)
DarkMode (INTEGER)
RemindersEnabled (INTEGER)
```

## Adding New Features

### Adding a New Phoneme

Edit `Services/PhonemeDataService.cs`:
```csharp
_phonemes.Add(new Phoneme
{
    Symbol = "ə",
    Type = PhonemeType.Vowel,
    Description = "Schwa vowel",
    ExampleWords = new List<ExampleWord>
    {
        new() { Word = "about", IpaTranscription = "/əˈbaʊt/" }
    }
});
```

### Adding a New View

1. Create XAML file in `Views/` folder
2. Create corresponding ViewModel in `ViewModels/`
3. Register in `MainViewModel.cs` navigation
4. Add DataTemplate in `MainView.axaml`

### Implementing Audio Playback

Currently, the AudioService is a placeholder. To add real audio:

1. Add audio files to `Assets/Audio/` folder
2. Update `Phoneme` and `ExampleWord` models with correct paths
3. Implement platform-specific audio in `AudioService.cs`
4. Use Avalonia's AssetLoader for cross-platform file access

Example:
```csharp
var uri = new Uri($"avares://PhonoArk/Assets/Audio/{audioPath}");
var stream = AssetLoader.Open(uri);
// Play stream with platform audio API
```

## Testing

### Manual Testing Checklist

1. **IPA Chart**
   - [ ] All phonemes display correctly
   - [ ] Click phoneme shows detail panel
   - [ ] Accent switching works
   - [ ] Favorites toggle works

2. **Exam**
   - [ ] Can start exam with different settings
   - [ ] Questions display correctly
   - [ ] Answer selection works
   - [ ] Feedback shows correct/incorrect
   - [ ] Results save to history

3. **Settings**
   - [ ] All settings save/load correctly
   - [ ] Volume slider works
   - [ ] Theme toggle works (when implemented)

## Common Issues

### Database Not Creating
- Check write permissions in LocalApplicationData folder
- Verify connection string in App.axaml.cs
- Run `dotnet ef database update` if migrations exist

### UI Not Updating
- Ensure properties use `[ObservableProperty]` attribute
- Check DataContext is set correctly
- Verify bindings in XAML use correct property names

### Navigation Not Working
- Check MainViewModel has all ViewModels registered
- Verify DataTemplates in MainView.axaml
- Ensure event handlers in MainView.axaml.cs call correct methods

## Build & Deployment

### Desktop (Windows/Linux/macOS)
```bash
# Debug build
dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj

# Release build
dotnet publish PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Release -o ./publish

# Self-contained (includes .NET runtime)
dotnet publish PhonoArk.Desktop/PhonoArk.Desktop.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o ./publish
```

### Android
```bash
# Install workload
dotnet workload install android

# Build APK
dotnet build PhonoArk.Android/PhonoArk.Android.csproj -c Release

# APK location: PhonoArk.Android/bin/Release/net10.0-android/
```

### iOS (macOS only)
```bash
# Install workload
dotnet workload install ios

# Build
dotnet build PhonoArk.iOS/PhonoArk.iOS.csproj -c Release
```

## Performance Optimization

1. **Lazy Load Data**: Load phonemes on-demand
2. **Virtualization**: Use VirtualizingStackPanel for long lists
3. **Image Caching**: Cache loaded images/assets
4. **Database Indexing**: Add indexes to frequently queried columns
5. **Async Operations**: Always use async/await for I/O operations

## Future Enhancements

1. **Audio Integration**
   - Add real phoneme and word audio files
   - Implement platform-specific audio playback
   - Add recording functionality

2. **Advanced Exam Modes**
   - Timed exams
   - Multiple choice for phoneme recognition
   - Speaking practice with pronunciation scoring

3. **Cloud Sync**
   - User authentication
   - Cloud database (Firebase/Azure)
   - Cross-device progress sync

4. **Analytics**
   - Track learning progress
   - Identify weak areas
   - Personalized recommendations

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write tests (when test infrastructure is added)
5. Submit a pull request

## License

MIT License - See LICENSE file for details
