# Steam Community Market Manager (SCMM)
SCMM is a fan-created project dedicated to collecting and analyzing data from the Steam Community Market and third-party marketplaces. Our goal is to document the history of the Steam market (and game item stores), detect market trends and patterns, and provide useful quality of life features not found in the official Steam Community Market app/website. Currently only a small subset of Steam apps are supported by SCMM, primarily [Rust](https://store.steampowered.com/app/252490/Rust/).

| | Website | Status |
|----|----|----|
|**Rust**|https://rust.scmm.app/store|✅ Fully supported.|
|**Unturned**|https://unturned.scmm.app/items|⚠ Work in progress. Basic historical data is available, but all data update jobs are disabled.|
|**CSGO**|https://csgo.scmm.app/items|⚠ Work in progress. Basic historical data is available, but all data update jobs are disabled.|

# Project architecture
SCMM started as a personal project to gain practical and hands-on experience using [Azure](https://azure.microsoft.com/en-us) and [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor). Because of this, a lot of decisions within the project may seem strange, hacky, or overengineered. Sometimes things were done they way they are just as an excuse to try out a specific technology or feature and not because it was the most sensible or pragmatic option.

- TODO: List key technologies and frameworks
- TODO: High-level component diagram
- TODO: Infrastructure architecture diagram

# Contributor Guide for Steam Community Market Manager (SCMM)

Welcome, and thank you for your interest in contributing to the **Steam Community Market Manager (SCMM)** project! This guide outlines how to set up your development environment, compile and debug the project, and submit contributions. Please follow these guidelines to ensure a smooth contribution process.

## Table of Contents

- [How to Contribute](#how-to-contribute)
- [Preparing Your Development Environment](#preparing-your-development-environment)
  - [Prerequisites](#prerequisites)
  - [Cloning the Repository](#cloning-the-repository)
  - [Setting Up Your Environment](#setting-up-your-environment)
- [Compiling the Project](#compiling-the-project)
- [Debugging the Project](#debugging-the-project)
  - [Debugging Tips](#debugging-tips)
- [Submitting a Pull Request](#submitting-a-pull-request)
- [Publishing New Versions](#publishing-new-versions)

---

## How to Contribute

We welcome contributions of all kinds—whether code, documentation, testing, bug reports, or feature requests! For code contributions, please follow the detailed steps below to get started.

## Preparing Your Development Environment

This section helps you set up a development environment with the necessary software, libraries, and configuration files.

You will need:

- [Visual Studio](https://visualstudio.microsoft.com/vs/community/) (2022/v17.7+) with the following workloads and components installed:
  - **ASP.NET and web development**
    - .NET SDK
    - .NET 7.0 Runtime
    - .NET 7.0 WebAssembly Build Tools
  - **Azure development**
    - Azure Compute Emulator 
    - [Azure Storage Emulator](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator#get-the-storage-emulator)
    - [Azure Data Studio](https://learn.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver16&tabs=redhat-install%2Credhat-uninstall#download-azure-data-studio)
  - **Data storage and processing**
    - SQL Server Express LocalDB

### Prerequisites

Ensure you have the following software installed on your machine. SCMM is built using .NET, so Visual Studio and relevant components are essential.

1. **Visual Studio 2022** (version 17.7 or higher)
   - Install the following workloads via Visual Studio Installer:
     - **ASP.NET and Web Development**
     - **Azure Development**
     - **Data Storage and Processing**
   - Ensure the following individual components are installed:
     - **.NET SDK**
     - **.NET 7.0 Runtime**
     - **.NET 7.0 WebAssembly Build Tools**
     - **Azure Compute Emulator**
     - **Azure Storage Emulator**
     - **SQL Server Express LocalDB**

2. **Azure Data Studio**
   - Recommended for database management, especially if your work involves data processing or debugging SQL queries.

### Cloning the Repository

Clone the SCMM repository to your local machine and navigate into the project directory:

- Open a terminal or command prompt and run the following commands:

  ```bash
  git clone https://github.com/Steam-Community-Market-Manager/SCMM.git
  ```
  
  ```bash
  cd SCMM
  ```

### Setting Up Your Environment

1. **Environment Configuration**:
   - Locate the configuration file in the `config` or `env` directory (e.g., `config.example.json`). Make a copy of this file and rename it to `config.json`.
   - Open `config.json` and populate any required environment-specific values, such as API keys, database connection strings, or other service configurations specific to your setup.

2. **Restoring Dependencies**:
   - To ensure all required .NET dependencies are installed, use the following command within the project directory:

     `dotnet restore`

   - This will download all necessary packages specified in the project’s `.csproj` file.

3. **Azure Storage and Compute Emulators**:
   - SCMM may depend on Azure services, so make sure the **Azure Storage Emulator** and **Azure Compute Emulator** are set up and running if you’re working with features that require Azure interaction.
   - You can launch these emulators manually from the start menu or configure them to start automatically when debugging.

4. **SQL Server Express LocalDB**:
   - SCMM uses SQL Server for database functionality. Confirm that SQL Server Express LocalDB is running by opening **SQL Server Configuration Manager** and verifying that the `LocalDB` instance is active.
   - Connect to `LocalDB` through **Azure Data Studio** or **SQL Server Management Studio (SSMS)** if you need to manage or inspect database tables.

### Final Checks

- **Build the Solution in Visual Studio**: Open the project in Visual Studio and build the solution (right-click the solution in **Solution Explorer** and select **Build Solution**). This will verify that your setup is complete and that there are no configuration issues.
- **Run Initial Tests**: Consider running any available tests to ensure your environment is correctly configured and the project builds successfully.

---

With these steps, you should have a fully set up environment ready for SCMM development. If you encounter issues or need additional help, please refer to the project’s README or reach out to the SCMM maintainers.

---

## Compiling the Project

The SCMM project is designed for .NET, making Visual Studio the recommended integrated development environment (IDE) for building and managing it. Follow these steps to compile the project successfully.

### Steps to Compile

1. **Build in Visual Studio**:
   - Open Visual Studio and load the SCMM project by selecting the solution file (`.sln`).
   - In **Solution Explorer**, right-click the solution name and select **Build Solution**. This action will compile all the projects within the solution.
   - If any compilation errors occur, ensure that all dependencies have been restored (see the **Restoring Dependencies** section) and check for any configuration issues in your project settings.

2. **Build from the Command Line**:
   - If you prefer using the command line, open a terminal or command prompt and navigate to the project directory where the `.csproj` file is located.
   - Use the following command to compile the project:

     ```bash
     dotnet build
     ```

   - This command will compile the project, and any compilation errors will be displayed in the console. Review the output to identify and resolve any issues.

### Troubleshooting Common Issues

- **Missing Dependencies**: If you encounter errors related to missing packages, make sure to run `dotnet restore` before building.
- **Configuration Errors**: Check your `config.json` and other configuration files for correctness, as misconfigurations can lead to build failures.
- **Framework Compatibility**: Ensure that you are targeting the correct .NET framework version as specified in the project settings.

By following these instructions, you should be able to compile the SCMM project without issues. If you encounter persistent problems, refer to the project's README or seek assistance from the SCMM community.

---

## Debugging the Project

Effective debugging is essential for identifying and fixing issues in SCMM. The following instructions guide you through the debugging process in Visual Studio, as well as debugging specific areas such as Azure emulators, database interactions, and frontend components.

### Steps to Debug in Visual Studio

1. **Start Debugging**:
   - Open the project in Visual Studio.
   - In the **Debug** menu, select **Start Debugging** (or press `F5`).
   - The application will launch in debugging mode, allowing you to view runtime information and interact with the application as it runs. Visual Studio will pause execution at breakpoints or throw errors as needed, enabling you to inspect code behavior.

2. **Azure Emulators**:
   - SCMM relies on Azure services for certain functionalities. Before starting your debug session, ensure both the **Azure Compute Emulator** and **Azure Storage Emulator** are running if your work involves Azure-based features.
   - You can manually start these emulators from the start menu or configure them to launch automatically with your project to avoid interruptions during debugging.

3. **Database Debugging**:
   - For debugging features that involve database queries or interactions, connect to the **SQL Server Express LocalDB** instance using **Azure Data Studio** or **SQL Server Management Studio (SSMS)**.
   - This connection allows you to view, modify, and troubleshoot database tables and records in real-time. You can also inspect any database logs or query results directly, which is especially helpful for isolating database-specific issues.

4. **Frontend Debugging**:
   - SCMM may include frontend components using WebAssembly or JavaScript. To debug these, open the browser’s developer tools (press `F12` in most browsers).
   - Use the **Console** and **Network** tabs to identify JavaScript errors or issues in network requests. To debug specific lines of JavaScript, use the **Sources** tab to set breakpoints and inspect variable states as the code executes.

### Debugging Tips

- **Setting Breakpoints**: Place breakpoints in Visual Studio to pause the code execution at specific lines. This lets you step through code line-by-line to identify where an issue occurs.
- **Inspect Variables**: Use the **Watch**, **Autos**, and **Locals** windows in Visual Studio to check the values of variables and properties during runtime. This can reveal unexpected changes in data that lead to bugs.
- **Log Statements**: Consider adding logging statements to output information to the console or a log file. SCMM should include a logging configuration you can utilize for consistent logging across the application.
  - For example, adding `Console.WriteLine("Debug message")` in C# or `console.log("Debug message")` in JavaScript helps track data flow or variable states without stopping the program.

With these tools and techniques, you should be able to identify and resolve issues within SCMM effectively. If further assistance is needed, consult the project's README or reach out to the SCMM maintainers.

---

## Submitting a Pull Request

Contributing to SCMM is straightforward. Follow these steps to submit a pull request (PR) with your changes.

### Step-by-Step Guide

1. **Fork the Repository**:
   - Navigate to the [SCMM GitHub repository](https://github.com/Steam-Community-Market-Manager/SCMM) and click **Fork** in the top-right corner. This will create a copy of the repository under your GitHub account.

2. **Clone Your Fork**:
   - Clone your forked repository to your local machine.

     ```bash
     git clone https://github.com/your-username/SCMM.git
     cd SCMM
     ```

3. **Create a Branch**:
   - Create a new branch for your changes. It’s best practice to name it according to the feature or fix you are working on.

     ```bash
     git checkout -b feature/my-new-feature
     ```

4. **Make Changes**:
   - Implement your changes, testing thoroughly to ensure that your code works as expected. If necessary, update any relevant documentation to reflect your modifications.

5. **Commit Your Changes**:
   - Stage and commit your changes with clear, descriptive messages. Use a concise and informative message to indicate what the commit accomplishes.

     ```bash
     git add .
     ```
     ```bash
     git commit -m "Add feature to handle X functionality"
     ```

6. **Push to Your Fork**:
   - Push your branch to your forked repository on GitHub.

     ```bash
     git push origin feature/my-new-feature
     ```

7. **Open a Pull Request (PR)**:
   - Go to the original SCMM repository on GitHub.
   - Click **New Pull Request**. Select your branch from your forked repository, and provide a clear title and description for your PR, including the purpose of the changes and any relevant context (such as issue numbers or background information).

8. **Review and Feedback**:
   - Your pull request will undergo review by the SCMM maintainers. Be prepared to make any changes they request. Once approved, your contribution will be merged into the main codebase.

### Tips for a Successful Pull Request

- **Follow the Code Style**: Make sure your code adheres to SCMM’s coding standards.
- **Keep Commits Atomic**: Avoid large, monolithic commits. Break down changes into logical chunks.
- **Provide Context**: In the PR description, explain the purpose of your changes and how they fit into SCMM. This helps reviewers understand the intent of your work.

By following these steps, you’ll help maintainers review your code efficiently and contribute to SCMM’s continuous improvement.

---

## Publishing New Versions

> _Note: Only maintainers or authorized contributors should publish new versions._

1. **Update Version Number**:
   - Increment the version number according to the [semantic versioning](https://semver.org/) guidelines (e.g., `1.2.0` to `1.3.0` for new features).

2. **Build and Test**:
   - Perform a full build and ensure all tests pass. This includes running any automated tests, and manually testing critical functionality if necessary.

3. **Update Release Notes**:
   - Update the `CHANGELOG.md` file or the release notes with a summary of new features, bug fixes, and breaking changes.

4. **Tag and Release**:
   - On GitHub, go to **Releases** > **Draft a New Release**. Tag the release with the new version number, fill in the release notes, and click **Publish Release**.

5. **Publish to Users**:
   - Once published, the new version will be available to users, and notifications may be sent to interested parties based on GitHub’s release settings.

---

This contributor guide provides a comprehensive walkthrough of the SCMM setup, development, and contribution processes. We appreciate your contributions and thanks for making this possible!

