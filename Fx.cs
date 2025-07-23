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

    /// <summary>
    /// Serialize an object to a file using binary serialization
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <param name="filePath">File path to save to</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool SaveVariable<T>(T obj, string filePath, bool verbose = false)
    {
        try
        {
            CreatePath(Path.GetDirectoryName(filePath) ?? "");
            
            using var stream = new FileStream(filePath, FileMode.Create);
            using var writer = new BinaryWriter(stream);
            
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            writer.Write(json);
            
            if (verbose)
                Console.WriteLine($"> saved variable to {GetFileBaseName(filePath, true)}");
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error saving variable to {filePath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deserialize an object from a file using binary serialization
    /// </summary>
    /// <param name="filePath">File path to load from</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Deserialized object or default value if failed</returns>
    public static T? LoadVariable<T>(string filePath, bool verbose = false)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (verbose)
                    Console.WriteLine($"! file {filePath} does not exist");
                return default;
            }
            
            using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);
            
            var json = reader.ReadString();
            var obj = System.Text.Json.JsonSerializer.Deserialize<T>(json);
            
            if (verbose)
                Console.WriteLine($"> loaded variable from {GetFileBaseName(filePath, true)}");
                
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error loading variable from {filePath}: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Calculate basic statistics for two datasets (observed vs simulated)
    /// </summary>
    /// <param name="observed">Observed values</param>
    /// <param name="simulated">Simulated values</param>
    /// <returns>Dictionary containing statistical metrics</returns>
    public static Dictionary<string, double> CalculateStats(double[] observed, double[] simulated)
    {
        if (observed.Length != simulated.Length)
            throw new ArgumentException("observed and simulated arrays must have the same length");

        var stats = new Dictionary<string, double>();
        
        try
        {
            int n = observed.Length;
            
            // Filter out NaN values
            var validPairs = new List<(double obs, double sim)>();
            for (int i = 0; i < n; i++)
            {
                if (!double.IsNaN(observed[i]) && !double.IsNaN(simulated[i]))
                    validPairs.Add((observed[i], simulated[i]));
            }
            
            if (validPairs.Count == 0)
            {
                foreach (var key in new[] { "r2", "rmse", "mae", "mse", "mape", "nse", "pbias" })
                    stats[key] = double.NaN;
                return stats;
            }
            
            var obs = validPairs.Select(p => p.obs).ToArray();
            var sim = validPairs.Select(p => p.sim).ToArray();
            
            double obsMean = obs.Average();
            double simMean = sim.Average();
            
            // R-squared (coefficient of determination)
            double ssRes = obs.Zip(sim, (o, s) => Math.Pow(o - s, 2)).Sum();
            double ssTot = obs.Select(o => Math.Pow(o - obsMean, 2)).Sum();
            stats["r2"] = ssTot != 0 ? 1 - (ssRes / ssTot) : double.NaN;
            
            // Root Mean Square Error (RMSE)
            stats["rmse"] = Math.Sqrt(obs.Zip(sim, (o, s) => Math.Pow(o - s, 2)).Average());
            
            // Mean Absolute Error (MAE)
            stats["mae"] = obs.Zip(sim, (o, s) => Math.Abs(o - s)).Average();
            
            // Mean Square Error (MSE)
            stats["mse"] = obs.Zip(sim, (o, s) => Math.Pow(o - s, 2)).Average();
            
            // Mean Absolute Percentage Error (MAPE)
            var mapeValues = obs.Zip(sim, (o, s) => o != 0 ? Math.Abs((o - s) / o) * 100 : 0).ToArray();
            stats["mape"] = mapeValues.Average();
            
            // Nash-Sutcliffe Efficiency (NSE)
            double denominator = obs.Select(o => Math.Pow(o - obsMean, 2)).Sum();
            stats["nse"] = denominator != 0 ? 1 - ssRes / denominator : double.NaN;
            
            // Percent Bias (PBIAS)
            double obsSum = obs.Sum();
            stats["pbias"] = obsSum != 0 ? 100 * (sim.Sum() - obsSum) / obsSum : double.NaN;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error calculating statistics: {ex.Message}");
            foreach (var key in new[] { "r2", "rmse", "mae", "mse", "mape", "nse", "pbias" })
                stats[key] = double.NaN;
        }
        
        return stats;
    }

    /// <summary>
    /// Calculate correlation coefficient between two datasets
    /// </summary>
    /// <param name="x">First dataset</param>
    /// <param name="y">Second dataset</param>
    /// <returns>Pearson correlation coefficient</returns>
    public static double CalculateCorrelation(double[] x, double[] y)
    {
        if (x.Length != y.Length)
            throw new ArgumentException("arrays must have the same length");

        try
        {
            double meanX = x.Average();
            double meanY = y.Average();
            
            double numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
            double denominatorX = x.Select(xi => Math.Pow(xi - meanX, 2)).Sum();
            double denominatorY = y.Select(yi => Math.Pow(yi - meanY, 2)).Sum();
            
            double denominator = Math.Sqrt(denominatorX * denominatorY);
            
            return denominator != 0 ? numerator / denominator : double.NaN;
        }
        catch
        {
            return double.NaN;
        }
    }

    /// <summary>
    /// Convert bytes to human-readable format with different units
    /// </summary>
    /// <param name="bytes">Number of bytes</param>
    /// <param name="useBinaryUnits">Use binary units (1024) instead of decimal (1000)</param>
    /// <param name="decimalPlaces">Number of decimal places (default is 2)</param>
    /// <returns>Formatted string</returns>
    public static string FormatBytesAdvanced(long bytes, bool useBinaryUnits = true, int decimalPlaces = 2)
    {
        if (bytes == 0) return "0 B";

        string[] decimalSizes = { "B", "KB", "MB", "GB", "TB", "PB" };
        string[] binarySizes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB" };
        
        double divisor = useBinaryUnits ? 1024.0 : 1000.0;
        string[] sizes = useBinaryUnits ? binarySizes : decimalSizes;
        
        int order = 0;
        double size = bytes;

        while (size >= divisor && order < sizes.Length - 1)
        {
            order++;
            size /= divisor;
        }

        return $"{Math.Round(size, decimalPlaces)} {sizes[order]}";
    }

    /// <summary>
    /// Generate a random string with specified length and character set
    /// </summary>
    /// <param name="length">Length of the string</param>
    /// <param name="includeNumbers">Include numbers (default is true)</param>
    /// <param name="includeUppercase">Include uppercase letters (default is true)</param>
    /// <param name="includeLowercase">Include lowercase letters (default is true)</param>
    /// <param name="includeSpecialChars">Include special characters (default is false)</param>
    /// <returns>Random string</returns>
    public static string GenerateRandomString(int length, bool includeNumbers = true, 
        bool includeUppercase = true, bool includeLowercase = true, bool includeSpecialChars = false)
    {
        var chars = "";
        
        if (includeLowercase) chars += "abcdefghijklmnopqrstuvwxyz";
        if (includeUppercase) chars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (includeNumbers) chars += "0123456789";
        if (includeSpecialChars) chars += "!@#$%^&*()_+-=[]{}|;:,.<>?";
        
        if (string.IsNullOrEmpty(chars))
            chars = "abcdefghijklmnopqrstuvwxyz";
        
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Create a timestamped backup of a file
    /// </summary>
    /// <param name="filePath">Original file path</param>
    /// <param name="backupDir">Backup directory (optional, defaults to same directory)</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>Path to the backup file, or null if failed</returns>
    public static string? CreateBackup(string filePath, string? backupDir = null, bool verbose = true)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (verbose)
                    Console.WriteLine($"! source file {filePath} does not exist");
                return null;
            }
            
            backupDir ??= Path.GetDirectoryName(filePath) ?? "./";
            CreatePath(backupDir);
            
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            
            var backupFileName = $"{fileName}_backup_{timestamp}{extension}";
            var backupPath = Path.Combine(backupDir, backupFileName);
            
            File.Copy(filePath, backupPath);
            
            if (verbose)
                Console.WriteLine($"> created backup: {backupPath}");
                
            return backupPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error creating backup: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Find duplicate files in a directory based on file content hash
    /// </summary>
    /// <param name="directoryPath">Directory to search</param>
    /// <param name="recursive">Search subdirectories (default is true)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Dictionary where key is hash and value is list of file paths with that hash</returns>
    public static Dictionary<string, List<string>> FindDuplicateFiles(string directoryPath, bool recursive = true, bool verbose = false)
    {
        var duplicates = new Dictionary<string, List<string>>();
        
        try
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(directoryPath, "*", searchOption);
            
            if (verbose)
                Console.WriteLine($"> scanning {files.Length} files for duplicates...");
            
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            
            foreach (var file in files)
            {
                try
                {
                    using var stream = File.OpenRead(file);
                    var hash = Convert.ToBase64String(sha256.ComputeHash(stream));
                    
                    if (!duplicates.ContainsKey(hash))
                        duplicates[hash] = new List<string>();
                    
                    duplicates[hash].Add(file);
                }
                catch (Exception ex)
                {
                    if (verbose)
                        Console.WriteLine($"! error processing {file}: {ex.Message}");
                }
            }
            
            // Remove entries with only one file (not duplicates)
            var actualDuplicates = duplicates.Where(kvp => kvp.Value.Count > 1)
                                             .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            if (verbose)
                Console.WriteLine($"> found {actualDuplicates.Count} sets of duplicate files");
                
            return actualDuplicates;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error finding duplicates: {ex.Message}");
            return new Dictionary<string, List<string>>();
        }
    }

    /// <summary>
    /// Monitor a directory for file changes
    /// </summary>
    /// <param name="directoryPath">Directory to monitor</param>
    /// <param name="filter">File filter pattern (default is "*.*")</param>
    /// <param name="includeSubdirectories">Monitor subdirectories (default is false)</param>
    /// <param name="onChanged">Action to execute when file changes</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>FileSystemWatcher instance</returns>
    public static FileSystemWatcher? MonitorDirectory(string directoryPath, string filter = "*.*", 
        bool includeSubdirectories = false, Action<string>? onChanged = null, bool verbose = true)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                if (verbose)
                    Console.WriteLine($"! directory {directoryPath} does not exist");
                return null;
            }
            
            var watcher = new FileSystemWatcher(directoryPath, filter)
            {
                IncludeSubdirectories = includeSubdirectories,
                EnableRaisingEvents = true
            };
            
            watcher.Changed += (sender, e) =>
            {
                if (verbose)
                    Console.WriteLine($"> file changed: {e.FullPath}");
                onChanged?.Invoke(e.FullPath);
            };
            
            watcher.Created += (sender, e) =>
            {
                if (verbose)
                    Console.WriteLine($"> file created: {e.FullPath}");
                onChanged?.Invoke(e.FullPath);
            };
            
            watcher.Deleted += (sender, e) =>
            {
                if (verbose)
                    Console.WriteLine($"> file deleted: {e.FullPath}");
                onChanged?.Invoke(e.FullPath);
            };
            
            if (verbose)
                Console.WriteLine($"> monitoring {directoryPath} for changes...");
                
            return watcher;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error setting up directory monitor: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Execute a shell command asynchronously with timeout support
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <param name="arguments">Command arguments (optional)</param>
    /// <param name="workingDirectory">Working directory (optional)</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default is 30000ms)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Tuple containing (exitCode, output, error)</returns>
    public static async Task<(int exitCode, string output, string error)> RunCommandAsync(string command, 
        string? arguments = null, string? workingDirectory = null, int timeoutMs = 30000, bool verbose = false)
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
                Console.WriteLine($"> running async: {command} {arguments}");

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            var completed = await Task.WhenAny(
                Task.WhenAll(outputTask, errorTask),
                Task.Delay(timeoutMs)
            );

            if (completed == Task.WhenAll(outputTask, errorTask))
            {
                await process.WaitForExitAsync();
                string output = await outputTask;
                string error = await errorTask;

                if (verbose && !string.IsNullOrEmpty(output))
                    Console.WriteLine($"output: {output}");
                if (verbose && !string.IsNullOrEmpty(error))
                    Console.WriteLine($"error: {error}");

                return (process.ExitCode, output, error);
            }
            else
            {
                process.Kill();
                return (-1, "", "command timed out");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error running async command: {ex.Message}");
            return (-1, "", ex.Message);
        }
    }

    /// <summary>
    /// Safely read a text file with automatic encoding detection
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>File contents as string, or null if failed</returns>
    public static string? ReadTextFile(string filePath, bool verbose = false)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                if (verbose)
                    Console.WriteLine($"! file {filePath} does not exist");
                return null;
            }

            // Try UTF-8 first, then fallback to system default
            try
            {
                var content = File.ReadAllText(filePath, Encoding.UTF8);
                if (verbose)
                    Console.WriteLine($"> read {GetFileBaseName(filePath, true)} as UTF-8");
                return content;
            }
            catch
            {
                var content = File.ReadAllText(filePath, Encoding.Default);
                if (verbose)
                    Console.WriteLine($"> read {GetFileBaseName(filePath, true)} as {Encoding.Default.EncodingName}");
                return content;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error reading {filePath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Write text to file with atomic operation (write to temp file first, then move)
    /// </summary>
    /// <param name="filePath">Target file path</param>
    /// <param name="content">Content to write</param>
    /// <param name="encoding">Text encoding (optional, defaults to UTF-8)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool WriteTextFileAtomic(string filePath, string content, Encoding? encoding = null, bool verbose = false)
    {
        try
        {
            encoding ??= Encoding.UTF8;
            
            CreatePath(Path.GetDirectoryName(filePath) ?? "");
            
            var tempPath = filePath + ".tmp";
            
            File.WriteAllText(tempPath, content, encoding);
            
            // Atomic move
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.Move(tempPath, filePath);
            
            if (verbose)
                Console.WriteLine($"> atomically wrote {GetFileBaseName(filePath, true)}");
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error writing {filePath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get disk space information for a directory
    /// </summary>
    /// <param name="directoryPath">Directory path</param>
    /// <returns>Tuple containing (totalBytes, freeBytes, usedBytes)</returns>
    public static (long totalBytes, long freeBytes, long usedBytes) GetDiskSpace(string directoryPath)
    {
        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(directoryPath)) ?? "C:");
            var totalBytes = drive.TotalSize;
            var freeBytes = drive.AvailableFreeSpace;
            var usedBytes = totalBytes - freeBytes;
            
            return (totalBytes, freeBytes, usedBytes);
        }
        catch
        {
            return (-1, -1, -1);
        }
    }

    /// <summary>
    /// Convert a CSV file to a simple dictionary structure
    /// </summary>
    /// <param name="csvFilePath">Path to CSV file</param>
    /// <param name="hasHeader">Whether the first row contains headers (default is true)</param>
    /// <param name="delimiter">CSV delimiter (default is comma)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>List of dictionaries representing rows, or null if failed</returns>
    public static List<Dictionary<string, string>>? ReadCsv(string csvFilePath, bool hasHeader = true, 
        char delimiter = ',', bool verbose = false)
    {
        try
        {
            var lines = File.ReadAllLines(csvFilePath);
            if (lines.Length == 0)
                return new List<Dictionary<string, string>>();

            var result = new List<Dictionary<string, string>>();
            string[] headers;
            int startIndex;

            if (hasHeader)
            {
                headers = lines[0].Split(delimiter);
                startIndex = 1;
            }
            else
            {
                var firstLine = lines[0].Split(delimiter);
                headers = Enumerable.Range(0, firstLine.Length).Select(i => $"column{i}").ToArray();
                startIndex = 0;
            }

            for (int i = startIndex; i < lines.Length; i++)
            {
                var values = lines[i].Split(delimiter);
                var row = new Dictionary<string, string>();
                
                for (int j = 0; j < Math.Min(headers.Length, values.Length); j++)
                {
                    row[headers[j]] = values[j].Trim();
                }
                
                result.Add(row);
            }

            if (verbose)
                Console.WriteLine($"> read {result.Count} rows from {GetFileBaseName(csvFilePath, true)}");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error reading CSV {csvFilePath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Write data to a CSV file
    /// </summary>
    /// <param name="csvFilePath">Path to output CSV file</param>
    /// <param name="data">List of dictionaries to write</param>
    /// <param name="delimiter">CSV delimiter (default is comma)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool WriteCsv(string csvFilePath, List<Dictionary<string, string>> data, 
        char delimiter = ',', bool verbose = false)
    {
        try
        {
            if (data.Count == 0)
                return true;

            CreatePath(Path.GetDirectoryName(csvFilePath) ?? "");

            var headers = data[0].Keys.ToArray();
            var lines = new List<string> { string.Join(delimiter, headers) };

            foreach (var row in data)
            {
                var values = headers.Select(h => row.ContainsKey(h) ? row[h] : "").ToArray();
                lines.Add(string.Join(delimiter, values));
            }

            File.WriteAllLines(csvFilePath, lines);

            if (verbose)
                Console.WriteLine($"> wrote {data.Count} rows to {GetFileBaseName(csvFilePath, true)}");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error writing CSV {csvFilePath}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Get the current timestamp in various formats
    /// </summary>
    /// <param name="format">Format string ("iso", "filename", "unix", or custom format)</param>
    /// <returns>Formatted timestamp string</returns>
    public static string GetTimestamp(string format = "iso")
    {
        var now = DateTime.Now;
        
        return format.ToLower() switch
        {
            "iso" => now.ToString("yyyy-MM-dd HH:mm:ss"),
            "filename" => now.ToString("yyyyMMdd_HHmmss"),
            "unix" => ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(),
            "utc" => now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss UTC"),
            _ => now.ToString(format)
        };
    }

    /// <summary>
    /// Retry an operation with exponential backoff
    /// </summary>
    /// <param name="operation">Operation to retry</param>
    /// <param name="maxRetries">Maximum number of retries (default is 3)</param>
    /// <param name="baseDelayMs">Base delay in milliseconds (default is 1000)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if operation succeeded, false otherwise</returns>
    public static async Task<bool> RetryOperation(Func<Task<bool>> operation, int maxRetries = 3, 
        int baseDelayMs = 1000, bool verbose = false)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (verbose)
                    Console.WriteLine($"> attempt {attempt}/{maxRetries}");
                    
                if (await operation())
                    return true;
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.WriteLine($"! attempt {attempt} failed: {ex.Message}");
            }

            if (attempt < maxRetries)
            {
                var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                if (verbose)
                    Console.WriteLine($"> waiting {delay}ms before retry...");
                await Task.Delay(delay);
            }
        }

        return false;
    }

    /// <summary>
    /// Retry a synchronous operation with exponential backoff
    /// </summary>
    /// <param name="operation">Operation to retry</param>
    /// <param name="maxRetries">Maximum number of retries (default is 3)</param>
    /// <param name="baseDelayMs">Base delay in milliseconds (default is 1000)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if operation succeeded, false otherwise</returns>
    public static bool RetryOperationSync(Func<bool> operation, int maxRetries = 3, 
        int baseDelayMs = 1000, bool verbose = false)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (verbose)
                    Console.WriteLine($"> attempt {attempt}/{maxRetries}");
                    
                if (operation())
                    return true;
            }
            catch (Exception ex)
            {
                if (verbose)
                    Console.WriteLine($"! attempt {attempt} failed: {ex.Message}");
            }

            if (attempt < maxRetries)
            {
                var delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                if (verbose)
                    Console.WriteLine($"> waiting {delay}ms before retry...");
                Thread.Sleep(delay);
            }
        }

        return false;
    }

    /// <summary>
    /// Calculate simple hash of a string using SHA256
    /// </summary>
    /// <param name="input">Input string</param>
    /// <param name="useShortHash">Return short hash (first 8 characters) instead of full hash</param>
    /// <returns>Hash string</returns>
    public static string CalculateHash(string input, bool useShortHash = false)
    {
        try
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            var hash = Convert.ToHexString(hashBytes).ToLower();
            
            return useShortHash ? hash[..8] : hash;
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Check if a string is a valid email address
    /// </summary>
    /// <param name="email">Email string to validate</param>
    /// <returns>True if valid email, false otherwise</returns>
    public static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if a string is a valid URL
    /// </summary>
    /// <param name="url">URL string to validate</param>
    /// <returns>True if valid URL, false otherwise</returns>
    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Convert a string to title case (first letter of each word capitalized)
    /// </summary>
    /// <param name="input">Input string</param>
    /// <returns>Title case string</returns>
    public static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + 
                          (words[i].Length > 1 ? words[i][1..].ToLower() : "");
            }
        }
        return string.Join(" ", words);
    }

    /// <summary>
    /// Slugify a string (make it URL-friendly)
    /// </summary>
    /// <param name="input">Input string</param>
    /// <param name="maxLength">Maximum length of the slug (default is 50)</param>
    /// <returns>Slugified string</returns>
    public static string Slugify(string input, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        var slug = input.ToLower()
                        .Replace(" ", "-")
                        .Replace("_", "-");

        // Remove invalid characters
        var validChars = slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray();
        slug = new string(validChars);

        // Remove multiple consecutive dashes
        while (slug.Contains("--"))
            slug = slug.Replace("--", "-");

        // Trim dashes from start and end
        slug = slug.Trim('-');

        // Limit length
        if (slug.Length > maxLength)
            slug = slug[..maxLength].TrimEnd('-');

        return slug;
    }

    /// <summary>
    /// Generate a simple UUID-like string
    /// </summary>
    /// <param name="includeHyphens">Include hyphens in the UUID format (default is true)</param>
    /// <returns>UUID string</returns>
    public static string GenerateUuid(bool includeHyphens = true)
    {
        var guid = Guid.NewGuid().ToString();
        return includeHyphens ? guid : guid.Replace("-", "");
    }

    /// <summary>
    /// Convert seconds to human-readable duration
    /// </summary>
    /// <param name="seconds">Duration in seconds</param>
    /// <param name="includeMilliseconds">Include milliseconds in output (default is false)</param>
    /// <returns>Human-readable duration string</returns>
    public static string FormatDuration(double seconds, bool includeMilliseconds = false)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        else if (timeSpan.TotalHours >= 1)
            return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        else if (timeSpan.TotalMinutes >= 1)
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        else if (includeMilliseconds && timeSpan.TotalSeconds < 1)
            return $"{timeSpan.TotalMilliseconds:F0}ms";
        else
            return $"{timeSpan.TotalSeconds:F1}s";
    }

    /// <summary>
    /// Find files by pattern with advanced options
    /// </summary>
    /// <param name="directory">Directory to search</param>
    /// <param name="pattern">Search pattern (supports wildcards)</param>
    /// <param name="recursive">Search subdirectories (default is true)</param>
    /// <param name="includeHidden">Include hidden files (default is false)</param>
    /// <param name="maxDepth">Maximum depth for recursive search (default is -1 for unlimited)</param>
    /// <returns>List of matching file paths</returns>
    public static List<string> FindFiles(string directory, string pattern = "*", bool recursive = true, 
        bool includeHidden = false, int maxDepth = -1)
    {
        var results = new List<string>();
        
        try
        {
            FindFilesRecursive(directory, pattern, recursive, includeHidden, maxDepth, 0, results);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error finding files: {ex.Message}");
        }
        
        return results;
    }

    private static void FindFilesRecursive(string directory, string pattern, bool recursive, 
        bool includeHidden, int maxDepth, int currentDepth, List<string> results)
    {
        if (maxDepth >= 0 && currentDepth > maxDepth)
            return;

        try
        {
            // Get files in current directory
            var files = Directory.GetFiles(directory, pattern);
            foreach (var file in files)
            {
                if (!includeHidden && IsHiddenFile(file))
                    continue;
                    
                results.Add(file);
            }

            // Recursively search subdirectories
            if (recursive)
            {
                var subdirectories = Directory.GetDirectories(directory);
                foreach (var subdir in subdirectories)
                {
                    if (!includeHidden && IsHiddenFile(subdir))
                        continue;
                        
                    FindFilesRecursive(subdir, pattern, recursive, includeHidden, 
                                     maxDepth, currentDepth + 1, results);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have permission to access
        }
    }

    private static bool IsHiddenFile(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            return attributes.HasFlag(FileAttributes.Hidden);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Create a simple log entry with timestamp
    /// </summary>
    /// <param name="message">Log message</param>
    /// <param name="logFile">Log file path (optional)</param>
    /// <param name="level">Log level (default is "INFO")</param>
    /// <param name="verbose">Also print to console (default is true)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool Log(string message, string? logFile = null, string level = "INFO", bool verbose = true)
    {
        var timestamp = GetTimestamp("iso");
        var logEntry = $"[{timestamp}] [{level}] {message}";
        
        if (verbose)
            Console.WriteLine(logEntry);
            
        if (!string.IsNullOrEmpty(logFile))
        {
            try
            {
                CreatePath(Path.GetDirectoryName(logFile) ?? "");
                File.AppendAllText(logFile, logEntry + Environment.NewLine);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"! error writing to log file {logFile}: {ex.Message}");
                return false;
            }
        }
        
        return true;
    }

    /// <summary>
    /// Merge multiple text files into one file
    /// </summary>
    /// <param name="inputFiles">List of input file paths</param>
    /// <param name="outputFile">Output file path</param>
    /// <param name="separator">Separator between files (default is empty line)</param>
    /// <param name="includeFilenames">Include filename headers (default is false)</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>True if successful, false otherwise</returns>
    public static bool MergeTextFiles(List<string> inputFiles, string outputFile, 
        string separator = "\n", bool includeFilenames = false, bool verbose = true)
    {
        try
        {
            CreatePath(Path.GetDirectoryName(outputFile) ?? "");
            
            using var writer = new StreamWriter(outputFile);
            
            for (int i = 0; i < inputFiles.Count; i++)
            {
                var inputFile = inputFiles[i];
                
                if (!File.Exists(inputFile))
                {
                    if (verbose)
                        Console.WriteLine($"! skipping missing file: {inputFile}");
                    continue;
                }
                
                if (includeFilenames)
                {
                    writer.WriteLine($"=== {GetFileBaseName(inputFile, true)} ===");
                }
                
                var content = File.ReadAllText(inputFile);
                writer.Write(content);
                
                if (i < inputFiles.Count - 1)
                    writer.Write(separator);
                    
                if (verbose)
                    Console.WriteLine($"> merged {GetFileBaseName(inputFile, true)}");
            }
            
            if (verbose)
                Console.WriteLine($"> created merged file: {GetFileBaseName(outputFile, true)}");
                
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! error merging files: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Simple HTTP GET request with timeout and headers support
    /// </summary>
    /// <param name="url">URL to request</param>
    /// <param name="timeoutSeconds">Timeout in seconds (default is 30)</param>
    /// <param name="headers">Optional headers dictionary</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Response content as string, or null if failed</returns>
    public static async Task<string?> HttpGet(string url, int timeoutSeconds = 30, 
        Dictionary<string, string>? headers = null, bool verbose = false)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            
            if (headers != null)
            {
                foreach (var header in headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            
            if (verbose)
                Console.WriteLine($"> GET {url}");
                
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            if (verbose)
                Console.WriteLine($"> received {content.Length} characters");
                
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! HTTP GET error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Simple HTTP POST request with JSON payload
    /// </summary>
    /// <param name="url">URL to post to</param>
    /// <param name="jsonPayload">JSON payload as string</param>
    /// <param name="timeoutSeconds">Timeout in seconds (default is 30)</param>
    /// <param name="headers">Optional headers dictionary</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>Response content as string, or null if failed</returns>
    public static async Task<string?> HttpPost(string url, string jsonPayload, int timeoutSeconds = 30, 
        Dictionary<string, string>? headers = null, bool verbose = false)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            
            if (headers != null)
            {
                foreach (var header in headers)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            if (verbose)
                Console.WriteLine($"> POST {url} with {jsonPayload.Length} characters");
                
            var response = await client.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (verbose)
                Console.WriteLine($"> received {responseContent.Length} characters");
                
            return responseContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"! HTTP POST error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if a URL is reachable (ping-like test)
    /// </summary>
    /// <param name="url">URL to test</param>
    /// <param name="timeoutSeconds">Timeout in seconds (default is 10)</param>
    /// <param name="verbose">Verbose output (default is false)</param>
    /// <returns>True if reachable, false otherwise</returns>
    public static async Task<bool> IsUrlReachable(string url, int timeoutSeconds = 10, bool verbose = false)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            
            if (verbose)
                Console.WriteLine($"> testing connectivity to {url}");
                
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var isReachable = response.IsSuccessStatusCode;
            
            if (verbose)
                Console.WriteLine($"> {url} is {(isReachable ? "reachable" : "not reachable")}");
                
            return isReachable;
        }
        catch (Exception ex)
        {
            if (verbose)
                Console.WriteLine($"> {url} is not reachable: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Split a large list into smaller chunks
    /// </summary>
    /// <param name="source">Source list</param>
    /// <param name="chunkSize">Size of each chunk</param>
    /// <returns>Enumerable of chunks</returns>
    public static IEnumerable<List<T>> ChunkList<T>(IEnumerable<T> source, int chunkSize)
    {
        var list = source.ToList();
        for (int i = 0; i < list.Count; i += chunkSize)
        {
            yield return list.Skip(i).Take(chunkSize).ToList();
        }
    }

    /// <summary>
    /// Measure execution time of an operation
    /// </summary>
    /// <param name="operation">Operation to measure</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>Execution time in milliseconds</returns>
    public static long MeasureExecutionTime(Action operation, string operationName = "operation", bool verbose = true)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            if (verbose)
                Console.WriteLine($"> starting {operationName}...");
                
            operation();
            
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;
            
            if (verbose)
                Console.WriteLine($"> {operationName} completed in {elapsed}ms");
                
            return elapsed;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"! {operationName} failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Measure execution time of an async operation
    /// </summary>
    /// <param name="operation">Async operation to measure</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="verbose">Verbose output (default is true)</param>
    /// <returns>Execution time in milliseconds</returns>
    public static async Task<long> MeasureExecutionTimeAsync(Func<Task> operation, string operationName = "operation", bool verbose = true)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            if (verbose)
                Console.WriteLine($"> starting {operationName}...");
                
            await operation();
            
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;
            
            if (verbose)
                Console.WriteLine($"> {operationName} completed in {elapsed}ms");
                
            return elapsed;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"! {operationName} failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Create a simple in-memory cache with expiration
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expirationMinutes">Expiration time in minutes (default is 60)</param>
    public static void CacheSet<T>(string key, T value, int expirationMinutes = 60)
    {
        var expiration = DateTime.Now.AddMinutes(expirationMinutes);
        _cache[key] = (value, expiration);
    }

    /// <summary>
    /// Get a value from the simple in-memory cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or default if not found/expired</returns>
    public static T? CacheGet<T>(string key)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            if (DateTime.Now <= cached.Expiration)
            {
                return cached.Value is T value ? value : default;
            }
            else
            {
                _cache.Remove(key); // Remove expired entry
            }
        }
        return default;
    }

    /// <summary>
    /// Clear all cached values
    /// </summary>
    public static void CacheClear()
    {
        _cache.Clear();
    }

    private static readonly Dictionary<string, (object? Value, DateTime Expiration)> _cache = new();

    /// <summary>
    /// Convert a file path to use the correct directory separators for the current OS
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Normalized path</returns>
    public static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;
            
        return Path.GetFullPath(path.Replace('\\', Path.DirectorySeparatorChar)
                                   .Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>
    /// Safe string format with error handling
    /// </summary>
    /// <param name="format">Format string</param>
    /// <param name="args">Format arguments</param>
    /// <returns>Formatted string or original format string if formatting fails</returns>
    public static string SafeFormat(string format, params object[] args)
    {
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return format;
        }
    }
}
