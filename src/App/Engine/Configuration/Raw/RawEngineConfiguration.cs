<<<<<<< HEAD
<<<<<<< HEAD
﻿namespace ORBIT9000.Engine.Configuration.Raw
{
    public class PluginsConfiguration
    {
        public required bool AbortOnError { get; set; }
        public required string[] ActivePlugins { get; set; } = [];
        public required bool LoadAsBinary { get; set; }
    }

    public class RawEngineConfiguration
    {
        public bool EnableTerminal { get; set; } = false;
        public required PluginsConfiguration Plugins { get; set; }
        public bool SharePluginScopes { get; set; }
        public bool UseDefaultFolder => Plugins.ActivePlugins.Length == 0;
        public required object Database { get; set; }
    }

    public class Database
    {
        public ConnectionStringInfo? Debug { get; set; }
        public ConnectionStringInfo? Release { get; set; }
    }

    public class ConnectionStringInfo
    {
        public string? ConnectionString { get; set; }
        public string? Provider { get; set; }
    }
=======
﻿using Microsoft.Extensions.Options;

namespace ORBIT9000.Engine.Configuration.Raw
=======
﻿namespace ORBIT9000.Engine.Configuration.Raw
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
{
    public class PluginsConfiguration
    {
        public required bool AbortOnError { get; set; }
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();
        public required bool LoadAsBinary { get; set; }
    }
<<<<<<< HEAD
>>>>>>> 0f347bd (Add Dirty Plugin Scope Resolution)
=======

    public class RawEngineConfiguration
    {
        public bool EnableTerminal { get; set; } = false;
        public required PluginsConfiguration Plugins { get; set; }
        public bool SharePluginScopes { get; set; }
        public bool UseDefaultFolder => this.Plugins.ActivePlugins.Length == 0;
    }
>>>>>>> 53879fa (Add AutoInitialization to PluginProvider)
}