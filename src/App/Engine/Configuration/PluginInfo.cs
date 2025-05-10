#nullable disable

using MessagePack;
using ORBIT9000.Core.Attributes.Engine;
using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    [MessagePackObject(AllowPrivate = true)]
    public partial class PluginInfo
    {
        [Key(0)]
        private bool? _isSingleton;

        [Key(3)]
        public bool Activated { get; internal set; }

        [IgnoreMember]
        public Assembly Assembly { get; internal set; }

        [Key(2)]
        public bool ContainsPlugins => this.PluginType is not null;

        [IgnoreMember]
        public FileInfo FileInfo { get; internal set; }

        [Key(1)]
        public bool IsSingleton
        {
            get
            {
                this._isSingleton ??= this.PluginType?.IsDefined(typeof(SingletonAttribute), true) ?? false;
                return this._isSingleton.Value;
            }
        }

        [IgnoreMember]
        public Type PluginType { get; internal set; }
    }
}