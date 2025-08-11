<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Parsing
{
    public interface IParser
=======
﻿namespace ORBIT9000.Core.Abstractions.Parsing
{
<<<<<<< HEAD
    internal interface IParser
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
=======
    public interface IParser
>>>>>>> bfa6c2d (Try fix pipeline)
    {
        object Parse(string input);
    }

<<<<<<< HEAD
<<<<<<< HEAD
    public interface IParser<out TTarget> : IParser
    {
        new TTarget Parse(string input);

        object IParser.Parse(string input) => Parse(input)!;
    }
}
=======
    internal interface IParser<out TTarget> : IParser
=======
    public interface IParser<out TTarget> : IParser
>>>>>>> bfa6c2d (Try fix pipeline)
    {
        new TTarget Parse(string input);
        object IParser.Parse(string input) => this.Parse(input)!;
    }
}
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
