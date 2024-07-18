using Domain.Entities;
using Domain.Enums;
using Domain.Models.ViewModels;
using Domain.RequestModels;
using tags;

namespace APP.Platform.Services
{
    public static class ProcessTimeSelection
    {
        public static void ApplyBrazilianTimezone(TimeSelection timeSelection)
        {
            DateTime utcDateStart = timeSelection.StartTime;
            DateTime utcDateEnd = timeSelection.EndTime;

            TimeZoneInfo saoPauloTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                "America/Sao_Paulo"
            );

            timeSelection.StartTime = TimeZoneInfo.ConvertTimeFromUtc(
                utcDateStart,
                saoPauloTimeZone
            );
            timeSelection.EndTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateEnd, saoPauloTimeZone);
        }

        public static DateTime CheckIfNeedSkipActualMonth(DateTime date, bool needAddActualMonth)
        {
            if (!needAddActualMonth)
            {
                int dateOfMonth = DateTime.DaysInMonth(date.Year, date.Month);
                int dateRemaining = dateOfMonth - date.Day;
                date = date.AddDays(dateRemaining + 1);
            }
            return date;
        }

        public static DateTime GetLimitForRepeat(
            DateTime limit,
            EnumRepeatWeekParttern? repeatPattern,
            bool needAddThisMonth
        )
        {
            if (repeatPattern == EnumRepeatWeekParttern.proximoMes)
            {
                limit = limit.AddMonths(1);
            }
            if (needAddThisMonth)
            {
                limit = limit.AddMonths(1);
            }

            return limit;
        }

        public static List<TimeSelection> CreateRepeatedTimeSelection(
            List<TimeSelectionViewModel> timeSelectionViewModels
        )
        {
            List<TimeSelection> timeSelectionsRepeated = new List<TimeSelection>();

            foreach (var ts in timeSelectionViewModels)
            {
                timeSelectionsRepeated.Add(
                    new TimeSelection()
                    {
                        Id = ts.Id,
                        PerfilId = ts.PerfilId,
                        Status = ts.Status,
                        TituloTemporario = ts.Title,
                        Tipo = ts.Tipo,
                        StartTime = ts.Start,
                        EndTime = ts.End,
                    }
                );
            }

            return timeSelectionsRepeated;
        }

        public static TimeSelectionViewModel ConverterToTimeSelectionViewModel(
            DateTime start,
            Guid id,
            TimeSelection ts,
            string? descricao,
            List<string> TagsSelected
        )
        {
            DateTime endTime = ts.EndTime;

            var tsViewModel = new TimeSelectionViewModel()
            {
                Id = ts.Id,
                PerfilId = id,
                Status = ts.Status,
                Title = ts.TituloTemporario,
                Tipo = ts.Tipo,
                Tags = TagsSelected,
                Start = start,
                End = endTime,
                Descricao = descricao,
            };

            if (ts.Tipo == EnumTipoTimeSelection.FreeTime)
            {
                ts.TituloTemporario = "Mentoria:" + GetCategoriaTagsCom3Pontinhos(TagsSelected);
            }

            return tsViewModel;
        }

        public static string GetCategoriaTagsCom3Pontinhos(List<string> tagsSelected)
        {
            var categorias = new List<string>();
            var all = DataTags.GetTags();

            foreach (var tagSelected in tagsSelected)
            {
                var actualValue = tagSelected;
                if (tagSelected.Contains(':')) // Verifica se a tag não contém ":" (dois pontos)
                {
                    var pai = tagSelected.Split(" : ");
                    actualValue = pai[0];
                }
                Console.WriteLine(actualValue);

                var categoria = all.FirstOrDefault(item => item.Value.Contains(actualValue));
                if (categoria.Key != null)
                {
                    categorias.Add(categoria.Key);
                }
            }

            var full = string.Join(",", categorias.Distinct());

            return full;
        }

        public static List<Tag> GetTagsListForRepeatTime(
            List<TimeSelection> tsList,
            List<string> tagsSelected
        )
        {
            var tags = new List<Tag>();
            foreach (var ts in tsList)
            {
                var tsId = ts.Id.ToString();
                tags.AddRange(ProcessingTags(tsId, tagsSelected));
            }
            return tags;
        }

        public static List<Tag> ProcessingTags(string id, List<string> tagsSelected)
        {
            List<Tag> tags = new List<Tag>();

            if (tagsSelected != null)
            {
                foreach (var t in tagsSelected)
                {
                    var tag = new Tag { Titulo = t, FreeTimeRelacao = id, };
                    tags.Add(tag);
                }
            }
            return tags;
        }
    }
}
