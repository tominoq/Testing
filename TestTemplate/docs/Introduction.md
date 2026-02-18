# Introduction

### Content:
- Introduction
- [Prerequisites](Prerequisites.md)
- [Guidelines](Guidelines.md)
- [Running Tests](Running-Tests.md)
---

## Overview
This repository is a template for building UI test automation projects. It provides a robust foundation for scalable and maintainable UI tests, leveraging advanced Selenium-based utilities and patterns. 
The project architecture is based on the Page Object Model (POM) pattern, which promotes maintainability and readability.

## Key Features
- **Component-Based Architecture:** Encapsulates UI elements as reusable components (e.g., buttons, checkboxes, containers, product cards).
- **Page Object Model:** Organizes web pages as classes for maintainable and readable test code. Learn more about POM here: [Selenium Page Object Models](https://www.selenium.dev/documentation/test_practices/encouraged/page_object_models/)
- **Extensive Framework Utilities:** Includes helpers, extensions, and factories for WebDriver, logging, waiting, and configuration.
- **Custom Logging:** Integrates with Serilog and provides enriched logging for test classes and methods.
- **Flexible Configuration:** Supports multiple configuration files (e.g., `appsettings.json`, `appsettings.local.json`) and environment-specific settings.
- **Exception Handling:** Custom exceptions for robust error reporting and debugging.
- **Test Structure:** Built-in base test classes and example tests using NUnit.
- **Extensible:** Easily add new components, pages, and utilities to fit evolving project needs.
- **Logs and Reporting:** Organizes logs by date and test run for easy traceability.

## Project Structure
- `Framework/`: Core framework utilities, configuration, logging, and helpers
- `Components/`: UI components and containers
- `Pages/`: Page object classes
- `Tests/`: Test classes and setup
- `.editorconfig`, `.gitattributes`, `.gitignore`: Configuration files for code formatting and version control
- `README.md`: Project documentation, including instructions for running tests

### Components (UI components and containers)
- **BaseComponent.cs:** The base class for all UI elements. It provides universal methods like click, get text, etc. Before adding new functionality, check this class to see if it already exists.
- **Basic folder:** Contains simple UI elements such as `Button`, `Checkbox`, and `Simple.cs` as a generic basic element.
- **Containers folder:** A container is an object in code that groups and serves multiple UI elements that logically belong together (e.g., product card, basket, form).
- **Root of Components folder:** UI elements directly in the root (e.g., `ProductCard`, `Basket`) are complex elements composed of multiple parts (image, text, etc.).

### Pages (Page object classes)
Contains page object classes that represent individual pages or views in the application.
- **BasePage.cs:** The base class for all page models. It provides shared logic, navigation, and common methods used by all pages, promoting code reuse and consistency across the project. It encapsulates locators and methods for interacting with specific pages.
- **Root of Pages folder:** Page models represent groups of UI elements and provide methods to perform actions and checks on the current page. Each page class describes a specific page in the application, encapsulating its structure and behavior for maintainable and reusable test code.

### Tests (Test classes)
Contains tests.
- **BaseTest.cs:** Provides setup and teardown methods for initializing and cleaning up test environments. Includes shared functionality like logging, browser initialization, and test configuration.
- **SetUpProject.cs:** Handles global test setup and configuration for the test suite.
- **TestInfo.cs:** Stores and manages metadata or context information for test execution.
- **Root of Tests folder:** Contains individual automated tests (e.g., `ExampleTest.cs`) that implement test cases for different features or pages, following best practices for structure and maintainability.

## Key Files
- `Globals.cs`: Contains global variables and constants used across the project.

## Test Configuration
- **`appsettings.json`**: Default configuration for tests.
- **`appsettings.local.json`**: Local configuration that overrides default values (ignored in Git).
- **Configuration fields**:
  - WebConfiguration
    - `BaseUrl`: URL of the environment being tested.
    - `UserName`: Username for basic authorization.
    - `UserPassword`: User password for basic authorization.
  - TestConfiguration
    - `LogsPath`: Path for log files. Uses build output directory as base.
    - `StoreLogsAlways`: Creates log files for passing tests.
    - `PageElementTimeout`: Timeout for waiting for a UI element.
    - `PageLoadTimeout`: Timeout for waiting for a page to fully load/render.
    - `SeleniumHubUrl`: URL for remote Selenium hub. Valid only if `IsRemote` is true.
    - `IsHeadless`: Runs web driver in headless mode—browser runs in background without GUI.
    - `WindowSize`: Browser window size. Will be set after start.


