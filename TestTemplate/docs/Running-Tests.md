# Running Tests

### Content:
- [Introduction](Introduction.md)
- [Prerequisites](Prerequisites.md)
- [Guidelines](Guidelines.md)
- Running Tests
---


## Steps

#### 1. Prepare the Environment:
You need to create a new file called appsettings.local.json next to appsettings.json (just copy the content). Then you can override values in there or remove rows that are completely same and you are not going to change them. This appsettings.local.json file is present in .gitconfig so that it is ignored by git.

**Example for local run**:

```json
{
  "WebConfiguration": {
    "BaseUrl": "https://eshop-demo-766125429055.europe-central2.run.app",
    "UserName": "Login",
    "UserPassword": "Pass"
  },
  "TestConfiguration": {
    "LogsPath": "../../../Logs"
  },
  "Serilog": {
    "WriteTo:NUnit": {
      "Name": "NUnitOutput",
      "Args": {
        "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{TestMethod}] {Message:lj}{NewLine}{Exception}",
        "restrictedToMinimumLevel": "Verbose"
      }
    }
  }
}
```

#### 2. Adjust values
You need to update `UserName`, `UserPassword`.

#### 3. Run tests
- For full run call `dotnet test`
- For partial run call `dotnet test --filter "TetsName"`

#### 4. Fail test debug
- Open the folder specified in the LogsPath parameter in [appsettings.json](../src/UI.Template/appsettings.json) or appsettings.local.json.
- Search test logs by run timestamp
- Analyze log files, web page source code, web page js console log, screenshot, etc.
