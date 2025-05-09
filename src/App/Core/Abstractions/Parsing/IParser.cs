namespace ORBIT9000.Core.Abstractions.Parsing
{
    internal interface IParser
    {
        object Parse(string input);
    }

    internal interface IParser<out TTarget> : IParser
    {
        new TTarget Parse(string input);
        object IParser.Parse(string input) => Parse(input)!;
    }
}
