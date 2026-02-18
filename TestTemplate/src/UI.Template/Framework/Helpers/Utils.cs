using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.XPath;
using OpenQA.Selenium;
using UI.Template.Framework.Exceptions;

namespace UI.Template.Framework.Helpers;

/// <summary>
/// Commonly used utility methods. For example working with files, parsing, etc.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Constant that holds regex for parsing the hostname.
    /// </summary>
    public const string HostnameRegex = @"^(?<protocol>http{1}[s]?)(://){1}(?<hostname>[^/]+?)(?<port>:\d+)?(?<path>/?|/{1}.*)$";

    /// <summary>
    /// Constant that holds regex for parsing the email.
    /// </summary>
    public const string EmailRegex = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

    /// <summary>
    /// Represents a string containing all the characters of the Czech alphabet, including both uppercase and lowercase letters.
    /// </summary>
    public const string CzechAlphabetCharacters = "AÁBCČDĎEÉĚFGHIÍJKLMNŇOÓPQRŘSŠTŤUÚŮVWXYÝZŽaábcčdďeéěfghiíjklmnňoópqrřsštťuúůvwxyýzž";

    /// <summary>
    /// Represents a string containing special characters.
    /// </summary>
    public const string SpecialCharacters = " !@#$%^&*()_+-=[]{}|;:',.<>?/";

    /// <summary>
    /// Represents a string containing number characters.
    /// </summary>
    public const string NumberCharacters = "0123456789";

    /// <summary>
    /// Random instance for generating random values.
    /// </summary>
    private static readonly Random _random = new();

    /// <summary>
    /// <see cref="JsonSerializerOptions"/> for serialization and deserialization of the objects.
    /// By default it's set to write indented json with fields included.
    /// </summary>
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new() { WriteIndented = true, IncludeFields = true };

    /// <summary>
    /// Method returns <see cref="DirectoryInfo"/> of the project directory based on <paramref name="currentPath"/>
    /// </summary>
    /// <param name="currentPath">Path where the searching is started</param>
    /// <returns><see cref="DirectoryInfo"/> of the directory with a project file</returns>
    public static DirectoryInfo GetProjectDirectoryInfo(string? currentPath = null) => GetDirectoryInfo("*.csproj", currentPath);

    /// <summary>
    /// Method returns <see cref="DirectoryInfo"/> of the solution directory based on <paramref name="currentPath"/>
    /// </summary>
    /// <param name="currentPath">Path where the searching is started</param>
    /// <returns><see cref="DirectoryInfo"/> of the directory with a solution file</returns>
    public static DirectoryInfo GetSolutionDirectoryInfo(string? currentPath = null) => GetDirectoryInfo("*.sln", currentPath);

    /// <summary>
    /// Method returns <see cref="DirectoryInfo"/> of the directory based on <paramref name="searchPattern"/> and <paramref name="currentPath"/>
    /// </summary>
    /// <param name="searchPattern">Pattern to be searched for in the directory</param>
    /// <param name="currentPath">Path where the searching is started</param>
    /// <returns><see cref="DirectoryInfo"/> of the directory with a <paramref name="searchPattern"/></returns>
    public static DirectoryInfo GetDirectoryInfo(string searchPattern, string? currentPath = null)
    {
        DirectoryInfo? directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
        while (directory != null && !(directory?.GetFiles(searchPattern).Length > 0))
        {
            directory = directory?.Parent;
        }

        return directory ?? throw new DirectoryNotFoundException($"No directory containing '{searchPattern}' was found starting from '{currentPath ?? Directory.GetCurrentDirectory()}'.");
    }

    /// <summary>
    /// Method returns content of the file
    /// </summary>
    /// <param name="filePath">Path to file</param>
    /// <returns>Content of the file as string</returns>
    public static string ReadFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Cannot find specified file '{filePath}'");

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Method saves content into file
    /// </summary>
    /// <param name="content">String that will be saved</param>
    /// <param name="path">Path of the logged file</param>
    /// <returns>File path if saving is successful, empty string if not</returns>
    public static string SaveAsFile(string content, string path)
    {
        if (content?.Length > 0)
        {
            try
            {
                File.AppendAllText(path, content);
                return path;
            }
            catch (Exception e)
            {
                Globals.Logger.LogError(e, $"Some problems appeared when trying to store a file '{path}'");
            }
        }
        else
        {
            Globals.Logger.LogWarning("There is no content available and it won't be stored");
        }
        return string.Empty;
    }

    /// <summary>
    /// Methods return type based on name or full name of the type.
    /// If type is not found, method returns null.
    /// </summary>
    /// <param name="typeName">Name or full name of the type</param>
    /// <returns><see cref="Type"/></returns>
    public static Type? GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }

    /// <summary>
    /// Waits for a specified time (Suspends the thread).
    /// </summary>
    /// <param name="delayInMilliSeconds">Amount of time in milliseconds to wait</param>
    public static void PauseThread(int delayInMilliSeconds)
    {
        Thread.Sleep(delayInMilliSeconds);
    }

    /// <summary>
    /// Method parse hostname from the url.
    /// E.g. https://www.test.cz/foo/bar or https://www.test.cz returns www.test.cz.
    /// </summary>
    /// <param name="url">Url to be parsed</param>
    /// <param name="hostname">Parsed hostname</param>
    /// <returns>True if the parsed hostname is found, otherwise false</returns>
    public static bool TryParseHostname(string url, out string hostname)
    {
        hostname = string.Empty;
        Regex r = new(HostnameRegex, RegexOptions.None, TimeSpan.FromMilliseconds(150));
        Match m = r.Match(url);
        if (m.Success)
        {
            hostname = m.Result("${hostname}");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds to Uri query new parameter with given value.
    /// </summary>
    /// <param name="paramName">parameter name</param>
    /// <param name="paramValue">parameter value</param>
    /// <returns>New Uri containing new parameter with value</returns>
    public static Uri SetUrlParam(string paramName, string paramValue)
    {
        var urlBuilder = new UriBuilder(Globals.WebDriver.Url);
        var query = HttpUtility.ParseQueryString(urlBuilder.Query);

        query[paramName] = paramValue;
        urlBuilder.Query = query.ToString();

        return new Uri(urlBuilder.ToString());
    }

    /// <summary>
    /// Sets param in synthetic query string inside fragment. Used by react components to maintain app state.
    /// </summary>
    /// <param name="url">Original URL</param>
    /// <param name="paramName">Parameter name</param>
    /// <param name="paramValue">Parameter value</param>
    /// <returns>Changed <see cref="Uri"/></returns>
    public static Uri SetFragmentParam(string url, string paramName, string paramValue)
    {
        var originalUrl = new Uri(url);
        var leftPart = originalUrl.GetLeftPart(UriPartial.Authority);
        var query = originalUrl.PathAndQuery;
        var fragment = originalUrl.Fragment;

        // no fragment at all, we simply create a new one
        if (!fragment.Contains('#'))
        {
            return new Uri(leftPart + query + $"#f&{paramName}={paramValue}");
        }

        // remove leading # if present
        fragment = fragment.Replace("#", "");

        // let's pretend we have normal query string delimited with &
        var fragmentParamsCollection = HttpUtility.ParseQueryString(fragment);
        fragmentParamsCollection.Set(paramName, paramValue);

        // manually construct the fragment string because of automatic encoding of the fragment parts
        // The method .ToString() encode sub-delims characters to percent-encoded characters. For example ',' in original url to %2c in output.  
        // See https://stackoverflow.com/questions/26088849/url-fragment-allowed-characters
        var fragmentParams = new StringBuilder();
        foreach (string key in fragmentParamsCollection)
        {
            if (key == null)
            {
                fragmentParams.Append(fragmentParamsCollection[key]);
                continue;
            }
            if (fragmentParams.Length > 0)
            {
                fragmentParams.Append('&');
            }
            fragmentParams.Append(CultureInfo.InvariantCulture, $"{key}={fragmentParamsCollection[key]}");
        }

        return new Uri($"{leftPart}{query}#{fragmentParams}");
    }

    /// <summary>
    /// Method returns value of the attribute name found by xpath
    /// </summary>
    /// <param name="html">Html page or part of the html to be searched through</param>
    /// <param name="xpathSelector">Valid xpath strategy</param>
    /// <param name="attributeName">Name of the attribute</param>
    /// <returns>Value of the attribute</returns>
    public static string GetAttributeViaXPathSelector(string html, string xpathSelector, string attributeName)
    {
        var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithXPath());
        var document = context.OpenAsync(res => res.Content(html)).Result;
        var element = document.Body.SelectSingleNode(xpathSelector) as IElement ?? throw new NoSuchElementException($"Cannot find element via XPath selector '{xpathSelector}' in html:\n{html}");
        return element.GetAttribute(null, attributeName) ?? throw new NoSuchValueException($"Cannot find specified attribute name '{attributeName}' via XPath selector '{xpathSelector}' in html:\n{html}");
    }

    /// <summary>
    /// Method returns value of the attribute found by CSS selector
    /// </summary>
    /// <param name="html">Html page or part of the html to be searched through</param>
    /// <param name="cssSelector">Valid CSS selector strategy</param>
    /// <param name="attributeName">Name of the attribute</param>
    /// <returns>Value of the attribute</returns>
    public static string GetAttributeViaCssSelector(string html, string cssSelector, string attributeName)
    {
        HtmlParser parser = new();
        var document = parser.ParseDocument(html);
        var node = document.QuerySelector(cssSelector) ?? throw new NoSuchElementException($"Cannot find element via CSS selector '{cssSelector}' in html:\n{html}");
        return node.GetAttribute(null, attributeName) ?? throw new NoSuchValueException($"Cannot find specified attribute name '{attributeName}' via CSS selector '{cssSelector}' in html:\n{html}");
    }

    /// <summary>
    /// Method returns value of the inner text found by CSS selector
    /// </summary>
    /// <param name="html">Html page or part of the html to be searched through</param>
    /// <param name="cssSelector">Valid CSS selector strategy</param>
    /// <returns>Inner text</returns>
    public static string GetTextContent(string html, string cssSelector)
    {
        HtmlParser parser = new();
        var document = parser.ParseDocument(html);
        var node = document.QuerySelector(cssSelector) ?? throw new NoSuchElementException($"Cannot find element via CSS selector '{cssSelector}' in html:\n{html}");
        return node.TextContent ?? throw new NoSuchValueException($"Cannot find inner text via xpath selector '{cssSelector}' in html:\n{html}");
    }

    /// <summary>
    /// Method parses input string size of the browser window.
    /// </summary>
    /// <param name="windowSize">String representation of the window size in format '1920,1080' or '375x850'</param>
    /// <param name="delimiter">String used as delimiter of the width and height</param>
    /// <returns><see cref="Size"/> of the browser window</returns>
    public static Size ParseWindowSize(string windowSize, string delimiter = ",")
    {
        string formatErrorMessage = $"Accepted format is 'width{delimiter}height' (e.g. 1920{delimiter}1080 or 375{delimiter}850) and width and height must be greater than 0.";
        if (string.IsNullOrEmpty(windowSize)) return new Size();

        int delimiterPosition = windowSize.IndexOf(delimiter, StringComparison.Ordinal);
        if (delimiterPosition <= 0)
            throw new ArgumentException($"Window size '{windowSize}' doesn't contain width and height separated by '{delimiter}'.\n{formatErrorMessage}.");

        string widthToParsing = windowSize[..delimiterPosition].Trim();
        string heightToParsing = windowSize[(delimiterPosition + 1)..].Trim();
        if (int.TryParse(widthToParsing, out int width) && int.TryParse(heightToParsing, out int height) && width > 0 && height > 0)
            return new Size(width, height);
        else
            throw new ArgumentException($"Window size '{windowSize}' cannot be parsed.\n{formatErrorMessage}.");
    }

    /// <summary>
    /// Method formats json string to be more readable.
    /// </summary>
    /// <param name="json">string to be formatted</param>
    /// <returns>Formatted string</returns>
    /// <exception cref="JsonException">If the string cannot be formatted</exception>
    public static string FormatJson(string json)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IncludeFields = true };
            dynamic parsedJson = JsonNode.Parse(json) ?? throw new JsonException("Parsed JSON is null.");
            return JsonSerializer.Serialize(parsedJson, jsonOptions);
        }
        catch (JsonException)
        {
            Globals.Logger.LogFatal($"Cannot parse '{json}'. Please, check if json is parsable.");
            throw;
        }
    }

    /// <summary>
    /// Method returns current date time in specific <paramref name="format"/>.
    /// </summary>
    /// <param name="format">Format of the current date time</param>
    /// <returns>Formatted current date time</returns>
    /// <exception cref="Exception"></exception>
    public static string GetCurrentDateTime(string format = "yyyy-MM-dd HH:mm:ss.fff")
    {
        try
        {
            return DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            Globals.Logger.LogFatal($"Cannot format current date time with '{format}'.");
            throw;
        }
    }

    /// <summary>
    /// Method generates random string with the specified length and characters.
    /// By default, it uses Czech alphabet characters, special characters and numbers to generate string of the length 10.
    /// </summary>
    /// <param name="characters">Set of characters to be used for the random string</param>
    /// <param name="length">Length of the random string</param>
    /// <returns>Random string</returns>
    public static string GenerateRandomString(char[]? characters = null, int length = 10)
    {
        StringBuilder result = new(length);
        characters ??= [.. CzechAlphabetCharacters, .. SpecialCharacters, .. NumberCharacters];

        for (int i = 0; i < length; i++)
        {
            int index = _random.Next(characters.Length);
            result.Append(characters[index]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Method checks if the status code is in the range of success status codes.
    /// </summary>
    /// <param name="statusCode"><see cref="HttpStatusCode"/></param>
    /// <returns>True if the status code is in the range of success status codes, otherwise false.</returns>
    public static bool IsHttpStatusCodeSuccess(HttpStatusCode statusCode)
    {
        var numericStatusCode = (int)statusCode;
        return numericStatusCode >= 200 && numericStatusCode <= 299;
    }

    /// <summary>
    /// Method to create a directory.
    /// </summary>
    /// <param name="directory">Directory path</param>
    public static void CreateDirectory(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
        }
        catch (Exception e)
        {
            throw new FileSystemException($"Folder {directory} could not be created for some reasons. Please check the error.", e);
        }
    }

    /// <summary>
    /// Method to delete a directory.
    /// </summary>
    /// <param name="directory">Directory path</param>
    public static void DeleteDirectory(string directory)
    {
        try
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
        catch (Exception e)
        {
            throw new FileSystemException($"Folder {directory} could not be deleted for some reasons. Please check the error.", e);
        }
    }

    /// <summary>
    /// Method parses url and returns path and query.
    /// If url starts with https://, it is parsed as Uri, otherwise it is returned as is.
    /// </summary>
    /// <param name="url">String in the format starting either "https://" or anything else assuming it's "path and query"</param>
    /// <returns>Path and query</returns>
    public static string ParsePathAndQueryFromUrl(string url)
    {
        return url.StartsWith("https://", StringComparison.Ordinal) ? new Uri(url).PathAndQuery : url;
    }

    /// <summary>
    /// Returns full filename (filepath + filename).
    /// </summary>
    /// <param name="folderPath">Name of the subfolder for different levels of logs. For example, MobileWeb for project log.</param>
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileExtension">Type/extension of the file with logging data (e.g. text file, screenshot)</param>
    /// <param name="optionalFileNameSuffix">Suffix used in filename, 'fileName_optionalFileNameSuffix'</param>
    /// <returns>Full filename with path</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetFilePath(string folderPath, string fileName, string fileExtension, string optionalFileNameSuffix = "")
    {
        if (string.IsNullOrEmpty(fileName) || fileName.Length == 0 ||
            string.IsNullOrEmpty(fileExtension) || fileExtension.Length == 0)
        {
            throw new ArgumentNullException($"There is no file name '{fileName}' or file extension '{fileExtension}' passed as argument.");
        }

        if (optionalFileNameSuffix.Length > 0)
        {
            optionalFileNameSuffix = "_" + optionalFileNameSuffix;
        }

        string fullFileName = fileName + optionalFileNameSuffix + "." + fileExtension;
        return Path.Combine(folderPath, fullFileName);
    }

    /// <summary>
    /// Convert base64String to MP4 file
    /// </summary>
    //// <param name="base64String">MP4 file in base64 encoded string</param>
    public static string ConvertBase64ToMP4(string base64String)
    {
        string filePath = GetFilePath(Globals.Logger.LogPath, "video", "mp4");
        FileInfo fileVideo = new(filePath);

        byte[] byteStream = Convert.FromBase64String(base64String);
        using FileStream streamFile = fileVideo.OpenWrite();
        streamFile.Write(byteStream, 0, byteStream.Length);
        streamFile.Close();

        Globals.Logger.LogInformation($"Saving video to: '{filePath}'");
        return filePath;
    }

    /// <summary>
    /// Method convert object to json string according to the <see cref="_defaultJsonSerializerOptions"/>.
    /// </summary>
    /// <param name="obj">Instance of the any object to be serialized into json format.</param>
    /// <returns>Serialized object according to the default <see cref="JsonSerializerOptions"/> containing WriteIndented = true, IncludeFields = true.</returns>
    public static string ConvertToJson(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return JsonSerializer.Serialize(obj, _defaultJsonSerializerOptions);
    }

    /// <summary>
    /// Method convert object to json string according to the <paramref name="options"/>.
    /// </summary>
    /// <param name="obj">Instance of the any object to be serialized into json format.</param>
    /// <param name="options"><see cref="JsonSerializerOptions"/> for serialization of the objects.</param>
    /// <returns>Serialized object according to the <paramref name="options"/>.</returns>
    public static string ConvertToJson(object obj, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Function returns inner text of the node html
    /// </summary>
    /// <param name="nodeHTML">Part of the html</param>
    /// <returns>Inner text of the node html</returns>
    public static string GetTextValueFromHtml(string nodeHTML)
    {
        HtmlParser html = new();

        var node = html.ParseDocument(nodeHTML).Body
                       .SelectSingleNode("//*[text()]");
        string? text = node.ChildNodes.OfType<IText>().Select(m => m.Text).FirstOrDefault();

        return GeneratedRegex.WhiteCharactersRegex().Replace(text ?? string.Empty, " ");
    }
}
