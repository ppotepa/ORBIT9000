# ORBIT9000

**ORBIT9000** is an experimental, modular **.NET 8** application built with a dynamic plugin-based architecture.  
It provides a flexible framework to develop, discover, and execute loosely coupled plugins, supporting runtime composition and dependency injection.

> ⚠️ **Work in progress** – This project is under active development and not yet production-ready.

---

## 🧩 Features

- ⚙️ **Plugin Architecture** – Load, register, and execute plugins at runtime.
- 🔌 **Multiple Loader Strategies** – Directory-based, debug, and string-array plugin loaders.
- 🧠 **Runtime Engine** – Plugin lifecycle management, execution strategy, and state handling.
- 💬 **Messaging Interface** – `IMessageChannel` abstraction for internal communication.
- 🧰 **Attribute-based DI** – `[Service]`, `[Singleton]`, `[DefaultProject]` and other custom attributes.
- 🧪 **PoC Ready** – Terminal-based GUI and sample plugins for testing/demos.
- 📊 **Data Processing** – Channel-based data flow with reactive UI updates.

---

## 🗂️ Structure Overview

```
src/
├── App/
│   ├── Core/                  # Interfaces, abstractions, attributes
│   ├── Engine/                # Plugin engine, loaders, runtime configuration
│   └── GUI/EngineTerminal/    # Terminal GUI frontend using Terminal.Gui
│
├── Plugins/
│   ├── Example/               # Weather-based plugin with data provider
│   └── Example2/              # Plugin returning random numbers
│
└── PoC/PoCDemo/               # Minimal demo app for engine execution
```

---

## 🚀 Getting Started

1. **Clone and Build**
   ```sh
   dotnet build ORBIT9000.sln
   ```

2. **Run the Engine Terminal**
   ```sh
   dotnet run --project src/App/GUI/EngineTerminal
   ```

3. **Run the PoC Demo (engine only)**
   ```sh
   dotnet run --project src/App/PoC/PoCDemo
   ```

4. **Add Plugins**
   - Drop compiled plugin DLLs into the designated plugin directory.
   - Use attributes and interfaces to define behaviour.

---

## 🧱 Example Plugin

```csharp
[Service]
public class WeatherQuery : IOrbitPlugin
{
    // Plugin logic here
}
```

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
- **ORBIT9000.Engine** – Core engine (loading strategies, DI container)  
- **EngineTerminal** – Console interface for plugin execution  
- **PoCDemo** – Example consumer app  
- **Plugins** – Example/test plugins  

[![Build](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml)  
[![Publish NuGet Package](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml)

---

## 📜 License & Status

This is an **experimental project under active development**.  
Use at your own discretion.
