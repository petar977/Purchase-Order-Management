using Po.Common.Models;
using Po.Common.Models.Dto;

namespace Po.Common.Interfaces
{
    public interface IPurchaseOrderItemsManager
    {
        List<PurchaseOrderItemsDto> GetItems(int id);
        JsonResponse AddItem(PurchaseOrderItemsDto item,string user,bool isAdmin);
        JsonResponse Delete(int id,string user, bool isAdmin);
        JsonResponse Clone(int cloneId, int currId, string user, bool isAdmin);
        JsonResponse DeleteAll(int id, string user, bool isAdmin);
        PurchaseOrderItemsDto GetItem(int itemId);
        JsonResponse EditItem(string name, int qty, double price, int itemId, string link);
        Task<JsonResponse> GetCloneComp(string userName);
        
    }
}
