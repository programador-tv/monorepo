using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Utils
{
    public sealed class TimeHelper
    {
        public static string FormatTimeToDuration(double time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);

            if (timeSpan.TotalSeconds < 60)
            {
                return $"0:{timeSpan.Seconds:D2}";
            }
            else if (timeSpan.TotalMinutes < 60)
            {
                return $"{timeSpan.Minutes:D1}:{timeSpan.Seconds:D2}";
            }
            else if (timeSpan.TotalHours < 24)
            {
                return $"{timeSpan.Hours:D1}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            else
            {
                int days = (int)timeSpan.TotalDays;
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;
                return $"{days:D2}:{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
        }

        public static string GetTempoQuePassouFormatado(DateTime data)
        {
            var tempoQuePassou = string.Empty;
            var seconds = Math.Ceiling((DateTime.Now - data).TotalSeconds);

            if (seconds < 2)
            {
                tempoQuePassou = "Há " + seconds + " segundo";
                return tempoQuePassou;
            }
            if (seconds > 1 && seconds < 60)
            {
                tempoQuePassou = "Há " + seconds + " segundos";
                return tempoQuePassou;
            }

            var minutes = Math.Ceiling(seconds / 60);
            if (minutes < 2)
            {
                tempoQuePassou = "Há " + minutes + " minuto";
                return tempoQuePassou;
            }

            if (minutes > 1 && minutes < 60)
            {
                tempoQuePassou = "Há " + minutes + " minutos";
                return tempoQuePassou;
            }

            var hours = Math.Ceiling(minutes / 60);
            if (hours < 2)
            {
                tempoQuePassou = "Há " + 1 + " hora";
                return tempoQuePassou;
            }
            hours = Math.Ceiling(minutes / 60);
            if (hours >= 2 && hours < 24)
            {
                tempoQuePassou = "Há " + hours + " horas";
                return tempoQuePassou;
            }
            //  hours = Math.Ceiling(minutes / 60);
            var days = Math.Ceiling(hours / 24);
            if (days < 2)
            {
                tempoQuePassou = "Há " + days + " dia";
                return tempoQuePassou;
            }

            if (days > 1 && days < 30)
            {
                tempoQuePassou = "Há " + days + " dias";
                return tempoQuePassou;
            }

            var month = Math.Floor(days / 30);
            if (month < 2)
            {
                tempoQuePassou = "Há " + month + " mês";
                return tempoQuePassou;
            }

            if (month >= 2 && month < 12)
            {
                tempoQuePassou = "Há " + month + " meses";
                Console.WriteLine(month);
                return tempoQuePassou;
            }

            var year = Math.Floor(month / 12);
            if (year < 2)
            {
                month = days / 30;
                tempoQuePassou = "Há " + year + " ano";
                return tempoQuePassou;
            }

            if (year >= 2)
            {
                tempoQuePassou = "Há " + year + " anos";
                return tempoQuePassou;
            }
            return tempoQuePassou;
        }

        public static string ReturnRemainingTimeString(TimeSpan timeDelta)
        {
            string tempoRestante = "Neste momento";
            if (timeDelta.TotalMinutes > 0)
            {
                if (timeDelta.TotalMinutes == 1)
                {
                    tempoRestante = Math.Floor(timeDelta.TotalMinutes) + " minuto";
                }
                else if (timeDelta.TotalMinutes < 60)
                {
                    tempoRestante = Math.Floor(timeDelta.TotalMinutes) + " minutos";
                }
                else if (timeDelta.TotalHours == 1)
                {
                    tempoRestante = Math.Floor(timeDelta.TotalHours) + " hora";
                }
                else if (timeDelta.TotalHours < 24)
                {
                    tempoRestante = Math.Floor(timeDelta.TotalHours) + " horas";
                }
                else if (timeDelta.TotalDays == 1)
                {
                    tempoRestante = Math.Floor(timeDelta.TotalDays) + " dia";
                }
                else
                {
                    tempoRestante = Math.Floor(timeDelta.TotalDays) + " dias";
                }
            }
            return tempoRestante;
        }
    }
}
