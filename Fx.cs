using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace ccfx;

/// <summary>
/// ccfx - A C# utility library containing functions to speed general development and prototyping.
/// 
/// Author: Celray James CHAWANDA
/// Email: celray@chawanda.com
/// License: MIT
/// Repository: https://github.com/celray/ccfx-cs
/// </summary>
public static class Fx
{
    /// <summary>
    /// List all files in a directory with a specific extension
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="ext">File extension (optional). Variations allowed like 'txt', '.txt', '*txt', '*.txt'</param>
    /// <returns>List of file paths</returns>
    public static List<string> ListFiles(string path, string? ext = null)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"! Warning: {path} is not a directory");
            return new List<string>();
        }

        string searchPattern = "*";
        if (!string.IsNullOrEmpty(ext))
        {
            ext = ext.TrimStart('*');
            if (!ext.StartsWith('.'))
                ext = '.' + ext;
            searchPattern = $"*{ext}";
        }

        try
        {
            return Directory.GetFiles(path, searchPattern).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error listing files: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Get the extension of a file
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <returns>File extension without the dot</returns>
    public static string GetExtension(string filePath)
    {
        return Path.GetExtension(filePath).TrimStart('.');
    }

    /// <summary>
    /// Delete a file
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if the file is deleted, false otherwise</returns>
    public static bool DeleteFile(string filePath, bool verbose = false)
    {
        if (!File.Exists(filePath))
        {
            if (verbose)
                Console.WriteLine($"! {filePath} does not exist");
            return false;
        }

        try
        {
            File.Delete(filePath);
            if (verbose)
                Console.WriteLine($"> {filePath} deleted");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Could not delete {filePath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Delete a directory
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if the directory is deleted, false otherwise</returns>
    public static bool DeletePath(string path, bool verbose = false)
    {
        if (!Directory.Exists(path))
        {
            if (verbose)
                Console.WriteLine($"! {path} does not exist");
            return false;
        }

        try
        {
            Directory.Delete(path, true);
            if (verbose)
                Console.WriteLine($"> {path} deleted");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Could not delete {path}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Format a string into a block of text with a maximum number of characters per line
    /// </summary>
    /// <param name="inputStr">The string to format</param>
    /// <param name="maxChars">The maximum number of characters per line (default is 70)</param>
    /// <returns>Formatted string with line breaks</returns>
    public static string FormatStringBlock(string inputStr, int maxChars = 70)
    {
        var words = inputStr.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            // If adding the next word to the current line would exceed the max_chars limit
            if (currentLine.Length + word.Length > maxChars)
            {
                // Append current line to lines and start a new one
                lines.Add(currentLine.Trim());
                currentLine = word;
            }
            else
            {
                // Add the word to the current line
                currentLine += " " + word;
            }
        }

        // Append any remaining words
        lines.Add(currentLine.Trim());

        return string.Join('\n', lines);
    }

    /// <summary>
    /// Get the system platform
    /// </summary>
    /// <returns>System platform as string</returns>
    public static string SystemPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "Darwin";
        else
            return "Unknown";
    }

    /// <summary>
    /// Display a progress bar in the console
    /// </summary>
    /// <param name="count">Current count</param>
    /// <param name="total">Total count</param>
    /// <param name="message">Optional message to display</param>
    public static void ProgressBar(int count, int total, string message = "")
    {
        int percent = (int)(count / (double)total * 100);
        int filled = percent / 2;
        string bar = new string('█', filled) + new string('░', 50 - filled);
        Console.Write($"\r{message} |{bar}| {percent}% [{count}/{total}]");
        
        if (count == total)
            Console.WriteLine();
    }

    /// <summary>
    /// Get the number of files in a directory with a specific extension
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <param name="extension">File extension</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>Number of files</returns>
    public static int FileCount(string path = "./", string extension = ".*", bool verbose = true)
    {
        int count = ListFiles(path, extension).Count;
        if (verbose)
        {
            string extDisplay = extension == ".*" ? "" : extension;
            Console.WriteLine($"> there are {count} {extDisplay} files in {path}");
        }
        return count;
    }

    /// <summary>
    /// List all folders in a directory
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <returns>List of folder names</returns>
    public static List<string> ListFolders(string path)
    {
        if (!path.EndsWith('/'))
            path += '/';

        if (Directory.Exists(path))
        {
            return Directory.GetDirectories(path)
                           .Select(d => Path.GetFileName(d))
                           .ToList();
        }
        else
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Create a directory if it does not exist
    /// </summary>
    /// <param name="pathName">The path to create</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>The created path</returns>
    public static string CreatePath(string pathName, bool verbose = false)
    {
        if (string.IsNullOrEmpty(pathName))
            return "./";

        pathName = pathName.TrimEnd('\\');
        if (!pathName.EndsWith('/'))
            pathName += '/';

        if (!Directory.Exists(pathName))
        {
            Directory.CreateDirectory(pathName);
            if (verbose)
                Console.WriteLine($"> created {pathName}");
        }

        return pathName.TrimEnd('/');
    }

    /// <summary>
    /// Get the base name of a file from a file path
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <param name="includeExtension">Include file extension (default is false)</param>
    /// <returns>Base name of the file</returns>
    public static string GetFileBaseName(string filePath, bool includeExtension = false)
    {
        if (includeExtension)
            return Path.GetFileName(filePath);
        else
            return Path.GetFileNameWithoutExtension(filePath);
    }

    /// <summary>
    /// Move a directory from source to destination
    /// </summary>
    /// <param name="srcDir">Source directory</param>
    /// <param name="destDir">Destination directory</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool MoveDirectory(string srcDir, string destDir, bool verbose = false)
    {
        if (!Directory.Exists(srcDir))
        {
            if (verbose)
                Console.WriteLine($"! Source directory {srcDir} does not exist");
            return false;
        }

        try
        {
            Directory.Move(srcDir, destDir);
            if (verbose)
                Console.WriteLine($"> Moved {srcDir} to {destDir}");
            return true;
        }
        catch (Exception ex)
        {
            if (verbose)
                Console.WriteLine($"! Error moving directory: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Move all files from source directory to destination directory
    /// </summary>
    /// <param name="srcDir">Source directory</param>
    /// <param name="destDir">Destination directory</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool MoveDirectoryFiles(string srcDir, string destDir, bool verbose = false)
    {
        if (!Directory.Exists(srcDir))
        {
            if (verbose)
                Console.WriteLine($"! Source directory {srcDir} does not exist");
            return false;
        }

        try
        {
            CreatePath(destDir, verbose);
            
            foreach (string file in Directory.GetFiles(srcDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Move(file, destFile);
                
                if (verbose)
                    Console.WriteLine($"> Moved {file} to {destFile}");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            if (verbose)
                Console.WriteLine($"! Error moving files: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Read text from a file
    /// </summary>
    /// <param name="filename">File path</param>
    /// <param name="encoding">Text encoding (optional)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Array of lines from the file</returns>
    public static string[]? ReadFile(string filename, Encoding? encoding = null, bool verbose = false)
    {
        try
        {
            encoding ??= Encoding.UTF8;
            var lines = File.ReadAllLines(filename, encoding);
            
            if (verbose)
                Console.WriteLine($"> read {GetFileBaseName(filename, true)}");
                
            return lines;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error reading {filename}, make sure the file exists: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Write text to a file
    /// </summary>
    /// <param name="filename">File path</param>
    /// <param name="lines">Lines to write</param>
    /// <param name="encoding">Text encoding (optional)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool WriteFile(string filename, string[] lines, Encoding? encoding = null, bool verbose = false)
    {
        try
        {
            encoding ??= Encoding.UTF8;
            
            CreatePath(Path.GetDirectoryName(filename) ?? "", verbose);
            File.WriteAllLines(filename, lines, encoding);
            
            if (verbose)
                Console.WriteLine($"> wrote {GetFileBaseName(filename, true)}");
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error writing to {filename}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Write text to a file
    /// </summary>
    /// <param name="filename">File path</param>
    /// <param name="content">Content to write</param>
    /// <param name="encoding">Text encoding (optional)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool WriteFile(string filename, string content, Encoding? encoding = null, bool verbose = false)
    {
        return WriteFile(filename, content.Split('\n'), encoding, verbose);
    }

    /// <summary>
    /// Extract/decompress a zip file to a directory
    /// </summary>
    /// <param name="inputFile">Input zip file</param>
    /// <param name="outputDir">Output directory</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Uncompress(string inputFile, string outputDir, bool verbose = false)
    {
        try
        {
            CreatePath(outputDir, verbose);
            ZipFile.ExtractToDirectory(inputFile, outputDir);
            
            if (verbose)
                Console.WriteLine($"> Extracted {inputFile} to {outputDir}");
                
            return true;
        }
        catch (Exception ex)
        {
            if (verbose)
                Console.WriteLine($"! Error extracting {inputFile}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Copy a file from source to destination
    /// </summary>
    /// <param name="source">Source file path</param>
    /// <param name="destination">Destination file path</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool CopyFile(string source, string destination, bool verbose = true)
    {
        try
        {
            CreatePath(Path.GetDirectoryName(destination) ?? "");
            File.Copy(source, destination, true);
            
            if (verbose)
                Console.WriteLine($"> {source} copied to {destination}");
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error copying file: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Copy a directory from source to destination recursively
    /// </summary>
    /// <param name="source">Source directory</param>
    /// <param name="destination">Destination directory</param>
    /// <param name="recursive">Copy subdirectories (default is true)</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <param name="excludeExtensions">List of file extensions to exclude</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool CopyDirectory(string source, string destination, bool recursive = true, bool verbose = true, List<string>? excludeExtensions = null)
    {
        try
        {
            excludeExtensions ??= new List<string>();
            CreatePath(destination);

            var sourceInfo = new DirectoryInfo(source);
            var destInfo = new DirectoryInfo(destination);

            if (!sourceInfo.Exists)
            {
                if (verbose)
                    Console.WriteLine($"! Source directory {source} does not exist");
                return false;
            }

            // Copy files
            foreach (var file in sourceInfo.GetFiles())
            {
                string ext = GetExtension(file.Name);
                if (!excludeExtensions.Contains(ext))
                {
                    string destPath = Path.Combine(destInfo.FullName, file.Name);
                    file.CopyTo(destPath, true);
                    
                    if (verbose)
                        Console.WriteLine($"> copying {file.Name}");
                }
            }

            // Copy subdirectories if recursive
            if (recursive)
            {
                foreach (var subDir in sourceInfo.GetDirectories())
                {
                    string destSubDir = Path.Combine(destInfo.FullName, subDir.Name);
                    CopyDirectory(subDir.FullName, destSubDir, recursive, verbose, excludeExtensions);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error copying directory: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Check if a number is between two values (inclusive)
    /// </summary>
    /// <param name="number">Number to check</param>
    /// <param name="a">First boundary value</param>
    /// <param name="b">Second boundary value</param>
    /// <returns>True if number is between a and b, false otherwise</returns>
    public static bool IsBetween(double number, double a, double b)
    {
        if (a > b)
            (a, b) = (b, a); // Swap if necessary
        return a <= number && number <= b;
    }

    /// <summary>
    /// Show a more detailed progress bar with percentage and count
    /// </summary>
    /// <param name="count">Current count</param>
    /// <param name="end">End count</param>
    /// <param name="message">Message to display</param>
    /// <param name="barLength">Length of the progress bar (default is 100)</param>
    public static void ShowProgress(int count, int end, string message, int barLength = 100)
    {
        double percent = (double)count / end * 100;
        string percentStr = $"{percent:F1}";
        int filled = (int)(barLength * count / end);
        string bar = new string('█', filled) + new string('░', barLength - filled);
        
        Console.Write($"\r{bar}| {percentStr}% [{count}/{end}] | {message}       ");
        
        if (count == end)
        {
            Console.WriteLine($"\r{bar}| {percentStr}% [{count}/{end}]                          ");
        }
    }

    /// <summary>
    /// List all files recursively in a directory with optional extension filter
    /// </summary>
    /// <param name="folder">Directory to search</param>
    /// <param name="extension">File extension filter (default is "*" for all files)</param>
    /// <returns>List of all file paths</returns>
    public static List<string> ListAllFiles(string folder, string extension = "*")
    {
        var files = new List<string>();
        
        try
        {
            string searchPattern = "*";
            if (extension != "*")
            {
                if (!extension.StartsWith('.'))
                    extension = '.' + extension;
                searchPattern = $"*{extension}";
            }

            files.AddRange(Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error listing files: {ex.Message}");
        }

        return files;
    }

    /// <summary>
    /// Send an HTTP notification alert to a server (like ntfy.sh)
    /// </summary>
    /// <param name="message">The message to send</param>
    /// <param name="server">The server to send the message to (default is http://ntfy.sh)</param>
    /// <param name="topic">The topic to send the message to (default is csharpAlerts)</param>
    /// <param name="messageTitle">The title of the message (optional)</param>
    /// <param name="priority">The priority of the message (optional)</param>
    /// <param name="tags">A list of tags to add to the message (optional)</param>
    /// <param name="printIt">Whether to print the message to the console (default is true)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if the alert was sent successfully, false otherwise</returns>
    public static async Task<bool> Alert(string message, string server = "http://ntfy.sh", string topic = "csharpAlerts", 
        string? messageTitle = null, int? priority = null, List<string>? tags = null, bool printIt = true, bool verbose = false)
    {
        if (printIt)
            Console.WriteLine(message);

        try
        {
            using var client = new HttpClient();
            var headers = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(messageTitle))
                headers["Title"] = messageTitle;
            if (priority.HasValue)
                headers["Priority"] = priority.Value.ToString();
            if (tags != null && tags.Count > 0)
                headers["Tags"] = string.Join(",", tags);

            var request = new HttpRequestMessage(HttpMethod.Post, $"{server}/{topic}")
            {
                Content = new StringContent(message, Encoding.UTF8, "text/plain")
            };

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            if (verbose)
                Console.WriteLine($"sending alert to {server}/{topic}");

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            if (verbose)
                Console.WriteLine($"! Error sending alert: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Download a file from a URL with progress tracking
    /// </summary>
    /// <param name="url">URL to download from</param>
    /// <param name="savePath">Path to save the file</param>
    /// <param name="verbose">Show download progress (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static async Task<bool> DownloadFile(string url, string savePath, bool verbose = false)
    {
        try
        {
            CreatePath(Path.GetDirectoryName(savePath) ?? "");

            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var readBytes = 0L;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true);

            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                readBytes += bytesRead;

                if (verbose && totalBytes > 0)
                {
                    int percent = (int)((double)readBytes / totalBytes * 100);
                    Console.Write($"\rDownloading... {percent}% [{readBytes}/{totalBytes} bytes]");
                }
            }

            if (verbose)
                Console.WriteLine($"\n> Downloaded {url} to {savePath}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error downloading file: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Run a shell command and return the output
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <param name="arguments">Command arguments (optional)</param>
    /// <param name="workingDirectory">Working directory (optional)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Tuple containing (exitCode, output, error)</returns>
    public static (int exitCode, string output, string error) RunCommand(string command, string? arguments = null, 
        string? workingDirectory = null, bool verbose = false)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments ?? "",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!string.IsNullOrEmpty(workingDirectory))
                startInfo.WorkingDirectory = workingDirectory;

            if (verbose)
                Console.WriteLine($"> Running: {command} {arguments}");

            using var process = Process.Start(startInfo);
            if (process == null)
                return (-1, "", "Failed to start process");

            process.WaitForExit();
            
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (verbose && !string.IsNullOrEmpty(output))
                Console.WriteLine($"Output: {output}");
            if (verbose && !string.IsNullOrEmpty(error))
                Console.WriteLine($"Error: {error}");

            return (process.ExitCode, output, error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error running command: {ex.Message}");
            return (-1, "", ex.Message);
        }
    }

    /// <summary>
    /// Watch the number of files in a directory with a specific extension for a duration
    /// </summary>
    /// <param name="path">Directory path (default is current directory)</param>
    /// <param name="extension">File extension to filter (default is all files)</param>
    /// <param name="intervalMs">Time interval in milliseconds (default is 200ms)</param>
    /// <param name="durationMinutes">Duration in minutes (default is 3 minutes)</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    public static void WatchFileCount(string path = "./", string extension = ".*", int intervalMs = 200, 
        double durationMinutes = 3, bool verbose = true)
    {
        var endTime = DateTime.Now.AddMinutes(durationMinutes);
        
        while (DateTime.Now < endTime)
        {
            int count = FileCount(path, extension, false);
            string extDisplay = extension == ".*" ? "" : extension;
            
            if (verbose)
            {
                Console.Write($"\r> {count} {extDisplay} files in {path}   ");
            }
            
            Thread.Sleep(intervalMs);
        }
        
        if (verbose)
            Console.WriteLine();
    }

    /// <summary>
    /// Create a compressed zip archive from a directory
    /// </summary>
    /// <param name="sourceDir">Source directory to compress</param>
    /// <param name="outputFile">Output zip file path</param>
    /// <param name="compressionLevel">Compression level (Optimal, Fastest, NoCompression, SmallestSize)</param>
    /// <param name="excludeExtensions">List of file extensions to exclude</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool CompressDirectory(string sourceDir, string outputFile, CompressionLevel compressionLevel = CompressionLevel.Optimal, 
        List<string>? excludeExtensions = null, bool verbose = false)
    {
        try
        {
            excludeExtensions ??= new List<string>();
            CreatePath(Path.GetDirectoryName(outputFile) ?? "");

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using var archive = ZipFile.Open(outputFile, ZipArchiveMode.Create);
            var sourceInfo = new DirectoryInfo(sourceDir);

            foreach (var file in sourceInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                string ext = GetExtension(file.Name);
                if (excludeExtensions.Contains(ext))
                    continue;

                string relativePath = Path.GetRelativePath(sourceDir, file.FullName);
                archive.CreateEntryFromFile(file.FullName, relativePath, compressionLevel);
            }

            if (verbose)
                Console.WriteLine($"> compressed {sourceDir} to {outputFile}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error compressing directory: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get file size in bytes
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <returns>File size in bytes, or -1 if file doesn't exist</returns>
    public static long GetFileSize(string filePath)
    {
        try
        {
            return new FileInfo(filePath).Length;
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Get directory size in bytes (including all subdirectories)
    /// </summary>
    /// <param name="directoryPath">Path to the directory</param>
    /// <returns>Directory size in bytes, or -1 if directory doesn't exist</returns>
    public static long GetDirectorySize(string directoryPath)
    {
        try
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>
    /// Format bytes into human readable format (B, KB, MB, GB, TB)
    /// </summary>
    /// <param name="bytes">Number of bytes</param>
    /// <param name="decimalPlaces">Number of decimal places (default is 2)</param>
    /// <returns>Formatted string</returns>
    public static string FormatBytes(long bytes, int decimalPlaces = 2)
    {
        if (bytes == 0) return "0 B";

        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{Math.Round(size, decimalPlaces)} {sizes[order]}";
    }

    /// <summary>
    /// Check if a file is older than the specified number of days
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="days">Number of days</param>
    /// <returns>True if file is older than specified days, false otherwise</returns>
    public static bool IsFileOlderThan(string filePath, int days)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.LastWriteTime < DateTime.Now.AddDays(-days);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get all files in directory that are older than specified number of days
    /// </summary>
    /// <param name="directoryPath">Directory to search</param>
    /// <param name="days">Number of days</param>
    /// <param name="extension">File extension filter (optional)</param>
    /// <returns>List of file paths that are older than specified days</returns>
    public static List<string> GetOldFiles(string directoryPath, int days, string? extension = null)
    {
        var oldFiles = new List<string>();
        
        try
        {
            var files = ListFiles(directoryPath, extension);
            foreach (var file in files)
            {
                if (IsFileOlderThan(file, days))
                    oldFiles.Add(file);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error getting old files: {ex.Message}");
        }

        return oldFiles;
    }

    /// <summary>
    /// Clean up old files in a directory (delete files older than specified days)
    /// </summary>
    /// <param name="directoryPath">Directory to clean</param>
    /// <param name="days">Number of days (files older than this will be deleted)</param>
    /// <param name="extension">File extension filter (optional)</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>Number of files deleted</returns>
    public static int CleanupOldFiles(string directoryPath, int days, string? extension = null, bool verbose = true)
    {
        int deletedCount = 0;
        
        try
        {
            var oldFiles = GetOldFiles(directoryPath, days, extension);
            
            foreach (var file in oldFiles)
            {
                if (DeleteFile(file, verbose))
                    deletedCount++;
            }

            if (verbose)
                Console.WriteLine($"> Cleaned up {deletedCount} old files from {directoryPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! Error during cleanup: {ex.Message}");
        }

        return deletedCount;
    }
}
