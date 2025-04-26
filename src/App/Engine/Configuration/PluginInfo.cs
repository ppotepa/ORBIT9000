#nullable disable

using MessagePack;
using ORBIT9000.Core.Attributes.Engine;
using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class PluginInfo
    {
        [IgnoreMember]
        public Assembly Assembly { get; internal set; }

        [IgnoreMember]
        public Type PluginType { get; internal set; }

        [IgnoreMember]
        public FileInfo FileInfo { get; internal set; }

        [Key(0)]
        private bool? _isSingleton;

        [Key(1)]
        public bool IsSingleton
        {
            get
            {
                _isSingleton ??= PluginType?.IsDefined(typeof(SingletonAttribute), true) ?? false;
                return _isSingleton.Value;
            }
        }

        [Key(2)]
        public bool ContainsPlugins => PluginType is not null;

        [Key(3)]
        public bool Activated { get; internal set; }
    }
}