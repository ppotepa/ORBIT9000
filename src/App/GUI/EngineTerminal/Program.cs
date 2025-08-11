<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5ae5b98 (Add Inversion of Control)
﻿using EngineTerminal.Contracts;
using EngineTerminal.Managers;
using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine;
using System.Threading.Channels;
<<<<<<< HEAD

namespace EngineTerminal
{
    public static class Program
    {
        #region Methods

        public static async Task Main(string[] args)
        {
            if (args is not null)
            {
                Channel<ExampleData> dataChannel = Channel.CreateUnbounded<ExampleData>();
                Channel<string> statusChannel = Channel.CreateUnbounded<string>();

                ServiceCollection services = new();

                services.AddSingleton(dataChannel);
                services.AddSingleton(statusChannel);

                services.AddSingleton<IDataManager, DataManager>();
                services.AddSingleton<IUIManager, UIManager>();

                services.AddSingleton<IPipeManager>(provider =>
                {
                    Channel<ExampleData> dataChannel = provider.GetRequiredService<Channel<ExampleData>>();
                    Channel<string> propertyChannel = provider.GetRequiredService<Channel<string>>();

                    return new NamedPipeManager(dataChannel, propertyChannel, ".", nameof(OrbitEngine));
                });

                services.AddSingleton<ApplicationController>();

                ServiceProvider provider = services.BuildServiceProvider();
                ApplicationController app = provider.GetRequiredService<ApplicationController>();

                await app.RunAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(args));
            }
        }

        #endregion Methods
    }
}
=======
﻿using System.IO.Pipes;
using System.Text;
=======
﻿using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
>>>>>>> 1aafd5a (Add Basic Messaging)
=======
﻿using EngineTerminal.Views;
using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
using Terminal.Gui;
>>>>>>> 4502f33 (Add GUI BoilerPlate)
=======
﻿using Terminal.Gui;
>>>>>>> f9f63ea (Add Simple Html View Parsing to Terminal PoC)
=======
﻿using System.ComponentModel;
=======
﻿using EngineTerminal.Bindings;
using EngineTerminal.Processors;
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe;
using System.Buffers;
using System.ComponentModel;
<<<<<<< HEAD
>>>>>>> 9942610 (Update Project Structure)
=======
using System.IO.Pipes;
<<<<<<< HEAD
>>>>>>> 122b62b (Fix Engine Main Thread)
=======
using System.Reflection;
>>>>>>> 147c461 (Refactor Program.CS)
using Terminal.Gui;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
=======
﻿using EngineTerminal.Managers;
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
=======
>>>>>>> 5ae5b98 (Add Inversion of Control)

namespace Orbit9000.EngineTerminal
{
    public static class Program
    {
        #region Methods

        public static async Task Main(string[] args)
        {
            var dataChannel = Channel.CreateUnbounded<ExampleData>();
            var statusChannel = Channel.CreateUnbounded<string>();

            var services = new ServiceCollection();

            services.AddSingleton(dataChannel);
            services.AddSingleton(statusChannel);

            services.AddSingleton<IDataManager, DataManager>();
            services.AddSingleton<IUIManager, UIManager>();

            services.AddSingleton<IPipeManager>(provider =>
            {
                var dataChannel = provider.GetRequiredService<Channel<ExampleData>>();
                var propertyChannel = provider.GetRequiredService<Channel<string>>();

                return new NamedPipeManager(dataChannel, propertyChannel, ".", nameof(OrbitEngine));
            });

            services.AddSingleton<ApplicationController>();

            var provider = services.BuildServiceProvider();
            var app = provider.GetRequiredService<ApplicationController>();

            await app.RunAsync();
        }

        #endregion Methods
    }
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
}
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
=======
}
>>>>>>> f9f63ea (Add Simple Html View Parsing to Terminal PoC)
=======
}
>>>>>>> 147c461 (Refactor Program.CS)
=======
}
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
