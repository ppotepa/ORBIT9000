using ORBIT9000.Core.Abstractions.Parsing;
using ORBIT9000.Core.Attributes.Engine;
using System.Text.RegularExpressions;

namespace ORBIT9000.Core.Parsing
{
    public class TextScheduleParser : IParser<Schedule>
    {
        private static readonly Regex _rx = new Regex(
            @"run every\s+(?<i>\d*)\s*(?<iu>second|minute|hour|day)s?" +
            @"(?:\s+for\s+(?<d>\d+)\s*(?<du>second|minute|hour|day)s?)?" +
            @"(?:\s+on\s+(?<days>(?:Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)(?:\s*,\s*(?:Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday))*))?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Schedule Parse(string text)
        {
            var match = _rx.Match(text);
            if (!match.Success) throw new FormatException($"Cannot parse “{text}”");

            var intervalCount = string.IsNullOrEmpty(match.Groups["i"].Value) ? 1 : int.Parse(match.Groups["i"].Value);
            var intervalUnit = match.Groups["iu"].Value;

            TimeSpan MakeSpan(int count, string unit) => unit.ToLower() switch
            {
                "second" => TimeSpan.FromSeconds(count),
                "minute" => TimeSpan.FromMinutes(count),
                "hour" => TimeSpan.FromHours(count),
                "day" => TimeSpan.FromDays(count),
                _ => throw new ArgumentException($"Unknown unit “{unit}”")
            };

            var interval = MakeSpan(intervalCount, intervalUnit);

            DateTime? end = null;
            if (match.Groups["d"].Success)
            {
                var durationCount = int.Parse(match.Groups["d"].Value);
                var durationUnit = match.Groups["du"].Value;
                end = DateTime.UtcNow + MakeSpan(durationCount, durationUnit);
            }

            IReadOnlyCollection<DayOfWeek>? days = null;
            if (match.Groups["days"].Success)
            {
                days = match.Groups["days"].Value
                           .Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(dayString => Enum.Parse<DayOfWeek>(dayString.Trim(), ignoreCase: true))
                           .ToArray();
            }

            return new SimpleSchedule
            {
                Start = DateTime.UtcNow,
                Interval = interval,
                End = end,
                DaysOfWeek = days
            };
        }
    }
}
