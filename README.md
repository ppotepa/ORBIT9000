# ORBIT9000

**ORBIT9000** is an experimental, modular .NET 8 application built with a dynamic plugin-based architecture. It provides a flexible framework to develop, discover, and execute loosely coupled plugins, supporting runtime composition and dependency injection.

> ⚠️ **Work in progress** – This project is under active development and not yet production-ready.

---

<<<<<<< HEAD
<<<<<<< HEAD
## 🧩 Features

- ⚙️ **Plugin Architecture** – Load, register, and execute plugins at runtime.
- 🔌 **Multiple Loader Strategies** – Support for directory-based, debug, and string-array plugin loaders.
=======
## 🧩 Features and Goals

- ⚙️ **Plugin Architecture** – Load, register, and execute plugins at runtime.
>>>>>>> 3b2119a (Update README.md)
=======
## 🧩 Features

- ⚙️ **Plugin Architecture** – Load, register, and execute plugins at runtime.
- 🔌 **Multiple Loader Strategies** – Support for directory-based, debug, and string-array plugin loaders.
>>>>>>> 29269cd (Update README.md)
- 🧠 **Runtime Engine** – Plugin lifecycle management, execution strategy, and state handling.
- 💬 **Messaging Interface** – `IMessageChannel` abstraction for internal communication.
- 🧰 **Attribute-based DI** – Use `[Service]`, `[Singleton]`, `[DefaultProject]` and other custom attributes.
- 🧪 **PoC Ready** – Includes a terminal-based GUI and sample plugins for testing and demo.
<<<<<<< HEAD
- 📊 **Data Processing** – Channel-based data flow with reactive UI updates.
=======
>>>>>>> 3b2119a (Update README.md)

---

## 🗂️ Structure Overview
<<<<<<< HEAD
=======

```
>>>>>>> 3b2119a (Update README.md)
src/
├── App/
│   ├── Core/             # Interfaces, abstractions, and attributes
│   ├── Engine/           # Plugin engine, loaders, runtime configuration
│   └── GUI/EngineTerminal/  # Terminal GUI frontend using Terminal.Gui
│
├── Plugins/
│   ├── Example/          # Weather-based plugin with data provider
│   └── Example2/         # Plugin with a service returning random numbers
│
└── PoC/PoCDemo/          # Minimal demo app for engine execution
<<<<<<< HEAD
=======
```

>>>>>>> 3b2119a (Update README.md)
---

## 🚀 Getting Started

<<<<<<< HEAD
1. **Clone and Build**dotnet build ORBIT9000.sln
2. **Run the Engine Terminal**dotnet run --project src/App/GUI/EngineTerminal
3. **Run the PoC Demo (starts the engine directly)**dotnet run --project src/App/PoC/PoCDemo
4. **Add Plugins**
=======
1. **Clone and Build**
   ```bash
   dotnet build ORBIT9000.sln
   ```

2. **Run the Engine Terminal**
   ```bash
   dotnet run --project src/App/GUI/EngineTerminal
   ```

<<<<<<< HEAD
3. **Add Plugins**
>>>>>>> 3b2119a (Update README.md)
=======
3. **Run the PoC Demo (starts the engine directly)**
   ```bash
   dotnet run --project src/App/PoC/PoCDemo
   ```

4. **Add Plugins**
>>>>>>> 29269cd (Update README.md)
   - Drop compiled plugin DLLs into a specified plugin directory.
   - Use attributes and interfaces to define behaviour.

---

## 🧱 Example Plugin
<<<<<<< HEAD
=======

```csharp
>>>>>>> 3b2119a (Update README.md)
[Service]
public class WeatherQuery : IOrbitPlugin
{
    // Logic here
}
<<<<<<< HEAD
=======
```

>>>>>>> 3b2119a (Update README.md)
---

## 🔧 Tech Stack

- **.NET 8**
- **Terminal.Gui** – TUI frontend
- **Autofac** – Dependency Injection
- **MessagePack / Newtonsoft.Json** – Serialization
- **Plugin LoadContext** – Isolated plugin loading

---

## 📦 Projects

- **ORBIT9000.Core** – Contracts and definitions
- **ORBIT9000.Engine** – Core engine with loading strategies and DI container
- **EngineTerminal** – Console interface for plugin execution and exclusion (also a small PoC using System.IO.Pipes
- **PoCDemo** – Example consumer app
- **Plugins** – Example and test plugins

<<<<<<< HEAD
[![Build](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml)
[![Publish NuGet Package](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml)

=======
>>>>>>> 3b2119a (Update README.md)
---

## 📜 License & Status

This is an **experimental project under active development**. Use at your own discretion.
<<<<<<< HEAD
        
=======
>>>>>>> 3b2119a (Update README.md)
