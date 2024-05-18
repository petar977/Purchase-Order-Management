using Po.Common.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Interfaces
{
    public interface IOrderCountManager
    {
        DataTableCounter GetAllCounters();
        CounterViewModel GetCounterVM();
        JsonResponse Create(CounterViewModel model);
        OrderCountDto GetEditData(int id);
        JsonResponse Edit(int id, string firstLetter, int counter, string type);
    }
}
