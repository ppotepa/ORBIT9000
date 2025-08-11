<<<<<<< HEAD
﻿#nullable disable

using MessagePack;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class PluginInfo
    {
        #region Fields

        [IgnoreMember]
        private bool? _containsPlugins;

        [Key(0)]
        private bool? _isSingleton;

        #endregion Fields

        #region Properties

        [Key(3)]
        public bool Activated { get; internal set; }

        [IgnoreMember]
        public Assembly Assembly { get; internal set; }
        [Key(2)]
        public bool ContainsPlugins
        {
            get
            {
                return _containsPlugins ??= PluginType is not null &&
                                       PluginType is not VoidType &&
                                       typeof(IOrbitPlugin).IsAssignableFrom(PluginType);
            }
        }

        [IgnoreMember]
        public FileInfo FileInfo { get; internal set; }

        [Key(1)]
        public bool IsSingleton
        {
            get
            {
                _isSingleton ??= PluginType?.IsDefined(typeof(SingletonAttribute), true) ?? false;
                return _isSingleton.Value;
            }
        }

        [IgnoreMember]
        public Type PluginType { get; internal set; }

        #endregion Properties
    }
}
=======
﻿using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    public class PluginInfo
    {
        public Assembly Assembly { get; internal set; }
        public Type PluginType { get; internal set; }
        public FileInfo FileInfo { get; internal set; }
        public bool ContainsPlugins => PluginType is not null;
    }
}
>>>>>>> 254394d (Remove OverLogging)
