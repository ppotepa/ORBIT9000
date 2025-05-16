# ORBIT9000

**ORBIT9000** is an experimental, modular .NET 8 application built with a dynamic plugin-based architecture. It provides a flexible framework to develop, discover, and execute loosely coupled plugins, supporting runtime composition and dependency injection.

> ⚠️ **Work in progress** – This project is under active development and not yet production-ready.

---

## 🧩 Features

- ⚙️ **Plugin Architecture** – Load, register, and execute plugins at runtime.
- 🔌 **Multiple Loader Strategies** – Support for directory-based, debug, and string-array plugin loaders.
- 🧠 **Runtime Engine** – Plugin lifecycle management, execution strategy, and state handling.
- 💬 **Messaging Interface** – `IMessageChannel` abstraction for internal communication.
- 🧰 **Attribute-based DI** – Use `[Service]`, `[Singleton]`, `[DefaultProject]` and other custom attributes.
- 🧪 **PoC Ready** – Includes a terminal-based GUI and sample plugins for testing and demo.
- 📊 **Data Processing** – Channel-based data flow with reactive UI updates.

---

## 🗂️ Structure Overview
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
---

## 🚀 Getting Started

1. **Clone and Build**dotnet build ORBIT9000.sln
2. **Run the Engine Terminal**dotnet run --project src/App/GUI/EngineTerminal
3. **Run the PoC Demo (starts the engine directly)**dotnet run --project src/App/PoC/PoCDemo
4. **Add Plugins**
   - Drop compiled plugin DLLs into a specified plugin directory.
   - Use attributes and interfaces to define behaviour.

---

## 🧱 Example Plugin
[Service]
public class WeatherQuery : IOrbitPlugin
{
    // Logic here
}
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
- **EngineTerminal** – Console interface for plugin execution
- **PoCDemo** – Example consumer app
- **Plugins** – Example and test plugins

[![Build](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/build.yml)
[![Publish NuGet Package](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/ppotepa/ORBIT9000/actions/workflows/nuget-publish.yml)

---

## 📜 License & Status

This is an **experimental project under active development**. Use at your own discretion.
        