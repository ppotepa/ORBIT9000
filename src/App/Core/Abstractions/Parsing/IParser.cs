namespace ORBIT9000.Core.Abstractions.Parsing
{
    public interface IParser
    {
        object Parse(string input);
    }

    public interface IParser<out TTarget> : IParser
    {
        new TTarget Parse(string input);

        object IParser.Parse(string input) => this.Parse(input)!;
    }
}