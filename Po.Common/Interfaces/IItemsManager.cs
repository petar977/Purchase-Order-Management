using Po.Common.Models;
using Po.Common.Models.Dto;

namespace Po.Common.Interfaces
{
    public interface IItemsManager
    {
        ItemsDto Add(ItemsDto item); 
        List<ItemsDto> AutoComplete(string q);
        DataTableItems GetItems(FilterItems filter);
        ItemsDto GetItem(int id);
        JsonResponse EditItem(ItemsDto modelDto);
        JsonResponse Delete(int id);
        Task<ItemsDto> Index(string userName);
    }
}
