using ORBIT9000.Core.Abstractions.Parsing;
using ORBIT9000.Core.Models;
using System.Text.RegularExpressions;

namespace ORBIT9000.Core.TempTools
{
    public class TextScheduleParser : ITextScheduleParser
    {
        private const string IntervalPattern = @"run every\s+(?<i>\d*)\s*(?<iu>second|minute|hour|day)s?";
        private const string DurationPattern = @"(?:\s+for\s+(?<d>\d+)\s*(?<du>second|minute|hour|day)s?)?";
        private const string DaysPattern = @"(?:\s+on\s+(?<days>(?:Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)(?:\s*,\s*(?:Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday))*))?";
        private const string FullPattern = IntervalPattern + DurationPattern + DaysPattern;

        private const string GroupIntervalCount = "i";
        private const string GroupIntervalUnit = "iu";
        private const string GroupDurationCount = "d";
        private const string GroupDurationUnit = "du";
        private const string GroupDays = "days";

        private const string UnitSecond = "second";
        private const string UnitMinute = "minute";
        private const string UnitHour = "hour";
        private const string UnitDay = "day";

        private static readonly Regex _rx = new(FullPattern, RegexOptions.IgnoreCase
            | RegexOptions.Compiled, matchTimeout: TimeSpan.FromSeconds(1));

        public IScheduleJob Parse(string input, string jobName = "[Unnamed]")
        {
            Match match = _rx.Match(input);
            if (!match.Success) throw new FormatException($"Cannot parse “{input}”");

            int intervalCount = string.IsNullOrEmpty(match.Groups[GroupIntervalCount].Value) ? 1 : int.Parse(match.Groups[GroupIntervalCount].Value);
            string intervalUnit = match.Groups[GroupIntervalUnit].Value;

            static TimeSpan MakeSpan(int count, string unit) => unit.ToLower() switch
            {
                UnitSecond => TimeSpan.FromSeconds(count),
                UnitMinute => TimeSpan.FromMinutes(count),
                UnitHour => TimeSpan.FromHours(count),
                UnitDay => TimeSpan.FromDays(count),
                _ => throw new ArgumentException($"Unknown unit “{unit}”")
            };

            TimeSpan interval = MakeSpan(intervalCount, intervalUnit);

            DateTime? end = null;
            if (match.Groups[GroupDurationCount].Success)
            {
                int durationCount = int.Parse(match.Groups[GroupDurationCount].Value);
                string durationUnit = match.Groups[GroupDurationUnit].Value;
                end = DateTime.UtcNow + MakeSpan(durationCount, durationUnit);
            }

            IReadOnlyCollection<DayOfWeek>? days = null;
            if (match.Groups[GroupDays].Success)
            {
                days = [.. match.Groups[GroupDays].Value
                           .Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(dayString => Enum.Parse<DayOfWeek>(dayString.Trim(), ignoreCase: true))];
            }

            return new Schedule()
            {
                Start = DateTime.UtcNow,
                Interval = interval,
                End = end,
                DaysOfWeek = days,
                OriginalExpression = input,
                Name = jobName
            };
        }

        IScheduleJob ITextScheduleParser.Parse(string input, string jobName)
        {
            return this.Parse(input, jobName);
        }

        IScheduleJob IParser<IScheduleJob>.Parse(string input)
        {
            return this.Parse(input, string.Empty);
        }
    }
}
