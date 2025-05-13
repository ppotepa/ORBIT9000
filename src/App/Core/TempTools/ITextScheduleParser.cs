using ORBIT9000.Abstractions.Parsing;
using ORBIT9000.Abstractions.Scheduling;

namespace ORBIT9000.Core.TempTools
{
    public interface ITextScheduleParser : IParser<IScheduleJob>
    {
        IScheduleJob Parse(string input, string jobName = "[Unnamed]");
    }
}