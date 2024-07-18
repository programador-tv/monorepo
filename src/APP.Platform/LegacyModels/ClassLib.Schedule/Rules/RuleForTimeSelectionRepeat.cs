using Domain.Entities;
using Domain.Enums;

namespace ClassLib.Schedule.Rules
{
    public static class RuleForTimeSelectionRepeat
    {
        public static bool RuleForShouldContinueVerify(
            DateTime start,
            DateTime end,
            EnumWeekPattern weekPattern,
            EnumRepeatWeekParttern? repeatWeekParttern
        )
        {
            if (repeatWeekParttern == EnumRepeatWeekParttern.ano)
            {
                return RuleForYear(start, end);
            }

            return RuleForMonth(start.Month, end.Month);
        }

        public static bool RuleForVerifyDayOfWeek(
            DayOfWeek weekDay,
            EnumWeekPattern rule,
            DayOfWeek tsWeekDay
        )
        {
            switch (rule)
            {
                case EnumWeekPattern.todasSemanasDoMes:
                    return RuleForWeekOfMonth(weekDay, tsWeekDay);
                case EnumWeekPattern.todosDias:
                    return true;
                case EnumWeekPattern.todosDiasExcetoFinalDeSemana:
                    return RuleForAllDaysLessWeekend(weekDay);
                default:
                    return false;
            }
        }

        private static bool RuleForYear(DateTime start, DateTime end)
        {
            return start != end;
        }

        private static bool RuleForMonth(int start, int end)
        {
            return start != end;
        }

        private static bool RuleForWeekOfMonth(DayOfWeek weekDay, DayOfWeek tsWeekDay)
        {
            return weekDay == tsWeekDay;
        }

        private static bool RuleForAllDaysLessWeekend(DayOfWeek weekDay)
        {
            return weekDay != DayOfWeek.Saturday && weekDay != DayOfWeek.Sunday;
        }
    }
}
