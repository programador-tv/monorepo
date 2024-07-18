using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.RequestModels;

namespace Domain.Utils
{
    public sealed class TimeSelectionHelper
    {
        public static bool ValidaSeDataNaoSobrepoem(
            List<TimeSelection> timeSelectionList,
            TimeSelection timeSelection
        )
        {
            foreach (var item in timeSelectionList)
            {
                // "termina:02:00" "inicia:02:00" -precisa ser valido
                if (
                    item.StartTime <= timeSelection.StartTime
                    && item.EndTime > timeSelection.StartTime
                )
                {
                    return false;
                }
                if (item.StartTime < timeSelection.EndTime && item.EndTime >= timeSelection.EndTime)
                {
                    return false;
                }
                if (
                    item.StartTime >= timeSelection.StartTime
                    && item.EndTime <= timeSelection.EndTime
                )
                {
                    return false;
                }
            }
            return true;
        }
    }
}
