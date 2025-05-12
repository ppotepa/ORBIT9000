using ORBIT9000.Core.Abstractions.Parsing;
using ORBIT9000.Core.Models;

namespace ORBIT9000.Core.TempTools
{
    public interface ITextScheduleParser : IParser<IScheduleJob>
    {
        IScheduleJob Parse(string input, string jobName = "[Unnamed]");
    }
}