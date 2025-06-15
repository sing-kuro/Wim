# Wim

Wim is a highly extensible Windows application framework designed to enable Vim-like keybindings and interactions with the Windows GUI. The application itself is a minimal foundation that requires plugins to provide actual functionality.

This application is still in early development, and we are actively working on expanding its capabilities.

## Overview

Wim provides a robust plugin system that allows developers to extend its functionality in various ways:

- Add Vim-like keybindings for Windows applications
- Implement Lua scripting support
- Create custom GUI overlays and interactions
- Handle system-wide keyboard events
- And much more through the plugin system

## Architecture

The application is built with a modular architecture centered around a plugin system:

### Core Components

- **Plugin System**: A dynamic plugin loading mechanism that allows for runtime loading and unloading of functionality
- **Message System**: A pub/sub message system for inter-plugin communication
- **Runtime Path Management**: Flexible path management for configuration, data, and plugin storage
- **Main Window**: A WPF-based window system that supports overlay text

### Plugin Interface

Plugins implement the `IPlugin` interface, which is available through the `Wim.Abstractions` NuGet package. This package contains all the necessary interfaces and types for plugin development.

```csharp
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    void Initialize(IApp app);
    void Unload();
}
```

## Getting Started

### Prerequisites

- Windows 7 or later
- .NET 8.0 or later
- Visual Studio 2022 (for development)

### Installation

Currently, Wim does not have an installer. To run the application, follow these steps:

1. Download the latest release from the releases page
2. Extract the files to your desired location (do not modify the directory structure)
3. Run `Wim.exe`

### Creating a Plugin

1. Create a new .NET class library project
2. Install the `Wim.Abstractions` NuGet package:
   ```powershell
   dotnet add package Wim.Abstractions
   ```
3. Implement the `IPlugin` interface
4. Build your plugin and place it in the plugins directory

Example plugin structure:

```csharp
using Wim.Abstractions;

public class MyPlugin : IPlugin
{
    public string Name => "MyPlugin";
    public string Version => "1.0.0";
    public string Description => "A sample plugin";

    public void Initialize(IApp app)
    {
        // Initialize your plugin
    }

    public void Unload()
    {
        // Clean up resources
    }
}
```

#### Available Services

Plugins have access to several services through the `IApp` interface from the `Wim.Abstractions` package:

- **MessageManager**: For inter-plugin communication
- **RuntimePathManager**: For managing file paths and resources
- **MainWindow**: For displaying and overlays
- **Constants**: For accessing application-wide constants
- **InstallationChecker**: For checking whether the application is installed or running from a specific path

### Installing Plugins

Plugins are loaded from the following locations:

- The `Wim` installation directory
- The `config` subdirectory of the `Wim` installation directory
- The `data` subdirectory of the `Wim` installation directory

We are working on a plugin manager plugin that will allow users to manage plugins by toml configuration files.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the terms of the license included in the repository.

## Acknowledgments

- Built with WPF for modern Windows UI capabilities
