using ORBIT9000.Core.Abstractions.Parsing;
using ORBIT9000.Core.Attributes.Engine;
using System.Text.RegularExpressions;

namespace ORBIT9000.Core.TempTools
{
    public class TextScheduleParser : IParser<IScheduleJob>
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

        private static readonly Regex _rx = new Regex(FullPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IScheduleJob Parse(string input)
        {
            var match = _rx.Match(input);
            if (!match.Success) throw new FormatException($"Cannot parse “{input}”");

            var intervalCount = string.IsNullOrEmpty(match.Groups[GroupIntervalCount].Value) ? 1 : int.Parse(match.Groups[GroupIntervalCount].Value);
            var intervalUnit = match.Groups[GroupIntervalUnit].Value;

            TimeSpan MakeSpan(int count, string unit) => unit.ToLower() switch
            {
                UnitSecond => TimeSpan.FromSeconds(count),
                UnitMinute => TimeSpan.FromMinutes(count),
                UnitHour => TimeSpan.FromHours(count),
                UnitDay => TimeSpan.FromDays(count),
                _ => throw new ArgumentException($"Unknown unit “{unit}”")
            };

            var interval = MakeSpan(intervalCount, intervalUnit);

            DateTime? end = null;
            if (match.Groups[GroupDurationCount].Success)
            {
                var durationCount = int.Parse(match.Groups[GroupDurationCount].Value);
                var durationUnit = match.Groups[GroupDurationUnit].Value;
                end = DateTime.UtcNow + MakeSpan(durationCount, durationUnit);
            }

            IReadOnlyCollection<DayOfWeek>? days = null;
            if (match.Groups[GroupDays].Success)
            {
                days = match.Groups[GroupDays].Value
                           .Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(dayString => Enum.Parse<DayOfWeek>(dayString.Trim(), ignoreCase: true))
                           .ToArray();
            }

            return new Schedule(async (x) => { Console.Title = DateTime.Now.ToString(); await Task.CompletedTask; })
            {
                Start = DateTime.UtcNow,
                Interval = interval,
                End = end,
                DaysOfWeek = days
            };
        }
    }
}
