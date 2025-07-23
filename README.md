# ccfx - C# Utility Library

A C# utility library containing functions to speed general development and prototyping. This is the C# port of the Python ccfx module.

## Features

- **File Operations**: List files, copy files, delete files, get extensions, read/write text files, file size utilities
- **Directory Operations**: List folders, create paths, copy/move directories, recursive file operations
- **System Utilities**: Get platform information, display progress bars, file counts, run shell commands
- **String Utilities**: Format text blocks with line wrapping, byte formatting
- **Compression**: Create and extract zip files
- **Network Operations**: HTTP notifications, file downloads with progress tracking
- **Monitoring**: Watch file count changes, file age utilities, cleanup operations
- **Cross-Platform**: Works on Windows, Linux, and macOS

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package ccfx
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package ccfx
```

## Usage

```csharp
using ccfx;

// List all .txt files in a directory
var txtFiles = Fx.ListFiles("./documents", "txt");

// Create a directory if it doesn't exist
Fx.CreatePath("./output/results");

// Copy files and directories
Fx.CopyFile("source.txt", "destination.txt", verbose: true);
Fx.CopyDirectory("./source", "./backup", verbose: true);

// Delete a file with verbose output
bool deleted = Fx.DeleteFile("./temp.txt", verbose: true);

// Format a long string into a text block
string formatted = Fx.FormatStringBlock("This is a very long string that needs to be wrapped", 50);

// Show detailed progress bar
for (int i = 0; i <= 100; i++)
{
    Fx.ShowProgress(i, 100, "Processing files...", 50);
    Thread.Sleep(50);
}

// File size utilities
long size = Fx.GetFileSize("./large-file.zip");
Console.WriteLine($"File size: {Fx.FormatBytes(size)}");

// Network operations (async)
bool notified = await Fx.AlertAsync("Process completed successfully!");
bool downloaded = await Fx.DownloadFileAsync("https://example.com/file.zip", "./downloads/file.zip", verbose: true);

// Run system commands
var (exitCode, output, error) = Fx.RunCommand("git", "status");
Console.WriteLine($"Git status: {output}");

// File monitoring and cleanup
Fx.WatchFileCount("./downloads", "zip", intervalMs: 1000, durationMinutes: 5);
int cleanedFiles = Fx.CleanupOldFiles("./temp", days: 7, verbose: true);

// Utility functions
bool inRange = Fx.IsBetween(5.5, 1.0, 10.0); // true
```

// Get system platform
string platform = Fx.SystemPlatform();

// Count files in directory
int count = Fx.FileCount("./data", "*.json");

// Read and write files
string[] lines = Fx.ReadFile("input.txt");
Fx.WriteFile("output.txt", lines);

// Extract zip file
Fx.Uncompress("archive.zip", "./extracted");
```

## API Reference

### File Operations

- `ListFiles(string path, string? ext = null)` - List files in directory with optional extension filter
- `GetExtension(string filePath)` - Get file extension without the dot
- `DeleteFile(string filePath, bool verbose = false)` - Delete a file
- `ReadFile(string filename, Encoding? encoding = null, bool verbose = false)` - Read text file
- `WriteFile(string filename, string[] lines, Encoding? encoding = null, bool verbose = false)` - Write text file
- `GetFileBaseName(string filePath, bool includeExtension = false)` - Get base filename

### Directory Operations

- `ListFolders(string path)` - List all folders in directory
- `CreatePath(string pathName, bool verbose = false)` - Create directory if it doesn't exist
- `DeletePath(string path, bool verbose = false)` - Delete directory
- `MoveDirectory(string srcDir, string destDir, bool verbose = false)` - Move directory
- `MoveDirectoryFiles(string srcDir, string destDir, bool verbose = false)` - Move all files between directories

### System Utilities

- `SystemPlatform()` - Get current operating system platform
- `ProgressBar(int count, int total, string message = "")` - Display console progress bar
- `FileCount(string path = "./", string extension = ".*", bool verbose = true)` - Count files in directory

### String Utilities

- `FormatStringBlock(string inputStr, int maxChars = 70)` - Format text with line wrapping

### Compression

- `Uncompress(string inputFile, string outputDir, bool verbose = false)` - Extract zip files

## Building from Source

```bash
# Clone the repository
git clone https://github.com/celray/ccfx-cs.git
cd ccfx-cs

# Build the project
dotnet build

# Create NuGet package
dotnet pack
```

## Requirements

- .NET 8.0 or later

## License

MIT License - see LICENSE file for details.

## Author

**Celray James CHAWANDA**  
Email: celray@chawanda.com  
Repository: https://github.com/celray/ccfx-cs

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## Notes

This C# library is a port of the Python ccfx module. Some Python-specific functions that don't translate well to C# (such as YouTube downloading, MP3 metadata handling, etc.) have been omitted from this version. The focus is on core utility functions that are commonly needed in C# development.

