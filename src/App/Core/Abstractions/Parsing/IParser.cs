<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Parsing
{
    public interface IParser
=======
﻿namespace ORBIT9000.Core.Abstractions.Parsing
{
    internal interface IParser
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
    {
        object Parse(string input);
    }

<<<<<<< HEAD
    public interface IParser<out TTarget> : IParser
    {
        new TTarget Parse(string input);

        object IParser.Parse(string input) => Parse(input)!;
    }
}
=======
    internal interface IParser<out TTarget> : IParser
    {        
        TTarget Parse(string input);
        object IParser.Parse(string input) => Parse(input);
    }
}
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
