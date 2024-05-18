using AutoMapper;
using DataLayer.Data;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Models.Dto;

namespace BussinesLayer.Managers
{
    public class PurchaseOrderItemsManager : IPurchaseOrderItemsManager
    {
        public readonly PoDbContext _dbContext;
        public readonly IMapper _mapper;
        public readonly UserManager<IdentityUser> _userManager;
        public PurchaseOrderItemsManager(PoDbContext dbContext, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
        }
        public List<PurchaseOrderItemsDto> GetItems(int id)
        {
            var list = from items in _dbContext.PurchaseOrderItem
                       join purchase in _dbContext.PurchaseOrders
                       on items.PurchaseOrderId equals purchase.Id
                       where items.PurchaseOrderId == id
                       select items;
            var list1 = _mapper.Map<List<PurchaseOrderItemsDto>>(list).ToList();
            
            return list1;
        }
        public JsonResponse AddItem(PurchaseOrderItemsDto item, string user, bool isAdmin)
        {
            var order = _dbContext.PurchaseOrders.FirstOrDefault(x => x.Id == item.PurchaseOrderId);
            if (order == null)
            {
                return new JsonResponse() { Message = "Order not found!" };
            }

            item.Total = item.UnitPrice * item.Qty;

            if (order.Status == State.Approved || order.Status == State.Denied || order.Status == State.Canceled || order.Status == State.Pending)
            {
                return new JsonResponse() { Message = "Cannot add items when status is approved, denied, canceled or in Pending" };
            }          
            else if(order.OrderedBy == user || isAdmin)
            {
                var item1 = _mapper.Map<PurchaseOrderItems>(item);
                if (item1.Link.Contains("https://"))
                {
                    _dbContext.Add(item1);
                    _dbContext.SaveChanges();
                    return new JsonResponse() { Message = "Item added to list", Success = true };
                }
                item1.Link = $"https://{item1.Link}";
                _dbContext.Add(item1);
                _dbContext.SaveChanges();
                return new JsonResponse() { Message = "Item added to list", Success = true };
            }           
            return new JsonResponse() { Message="You dont have permission to add item to this order" };
        }
        public JsonResponse Delete(int id, string user, bool isAdmin)
        {
            var order = GetOrder(id);
            if (order == null)
            {
                return new JsonResponse() { Message = "Order not found!" };
            }
            if (order.Status == State.Approved || order.Status == State.Denied || order.Status == State.Canceled || order.Status == State.Pending)
            {               
                return new JsonResponse() { Message = "Cannot delete when status is approved, denied, canceled or in Pending" };

            }else if(order.OrderedBy == user || isAdmin)
            {
                var item = _dbContext.PurchaseOrderItem.FirstOrDefault(x => x.Id == id);

                if (item != null)
                {
                    _dbContext.Remove(item);
                    _dbContext.SaveChanges();
                    return new JsonResponse() { Message = "Item deleted",Success=true};
                }
                return new JsonResponse() { Message="Invalid id"};
            }           
            return new JsonResponse() { Message="You dont have permission to delete in this order"};
            
        }
        private PurchaseOrder? GetOrder(int id)
        {
            var order = _dbContext.PurchaseOrderItem.Include(x => x.PurchaseOrder).FirstOrDefault(x => x.Id == id)?.PurchaseOrder;
            return order;
        }
        
        public JsonResponse Clone(int cloneId, int currId, string user, bool isAdmin)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var order = _dbContext.PurchaseOrders.FirstOrDefault(x => x.Id == currId);
                if (order == null)
                {
                    return new JsonResponse() { Message = "Order not found!" };
                }
                if (order.Status == State.Approved || order.Status == State.Denied || order.Status == State.Canceled || order.Status == State.Pending)
                {
                    return new JsonResponse() { Message = "Cannot delete when status is approved, denied, canceled or in Pending" };
                }
                else if (order.OrderedBy == user || isAdmin)
                {
                    IQueryable<PurchaseOrderItems> query = _dbContext.PurchaseOrderItem;
                    query = query.Where(x => x.PurchaseOrderId == cloneId);

                    foreach (var x in query.ToList())
                    {
                        var obj = new PurchaseOrderItems() { PurchaseOrderId = currId, ItemsName = x.ItemsName, Qty = x.Qty, Total = x.Total, UnitPrice = x.UnitPrice };

                        _dbContext.Add(obj);
                        _dbContext.SaveChanges();
                    }
                    transaction.Commit();

                    return new JsonResponse() { Message = "Order items successfully added to list", Success = true };
                }
                return new JsonResponse() { Message = "You dont have permission to add items to this order" };
            }
            catch(Exception)
            {
                transaction.Rollback();
                return new JsonResponse() { Message = "error" };
            }           
        }
        public JsonResponse DeleteAll(int id,string user,bool isAdmin)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                IQueryable<PurchaseOrderItems> query = _dbContext.PurchaseOrderItem;
                query = query.Where(x => x.PurchaseOrderId == id);
                var order = _dbContext.PurchaseOrders.FirstOrDefault(x => x.Id == id);
                if (order == null)
                {
                    return new JsonResponse() { Message = "Order not found!" };
                }
                if (order.Status == State.Approved || order.Status == State.Denied || order.Status == State.Canceled || order.Status == State.Pending)
                {
                    return new JsonResponse() { Message = "Cannot add items when status is approved, denied, canceled or in Pending" };
                }
                else if (order.OrderedBy == user || isAdmin)
                {
                    foreach (var x in query.ToList())
                    {
                        _dbContext.Remove(x);
                        _dbContext.SaveChanges();
                    }
                    transaction.Commit();
                    return new JsonResponse() { Message = "All order items has been deleted", Success = true };
                }
                else
                {
                    return new JsonResponse() { Message = "You dont have permission to delete in this order" };
                }           
            }
            catch (Exception)
            {
                transaction.Rollback();
                return new JsonResponse() { Message = "Transaction failed" };
            }           
        }
        public PurchaseOrderItemsDto GetItem(int itemId)
        {
            var item = _dbContext.PurchaseOrderItem.FirstOrDefault(x=>x.Id == itemId);
            var dto = _mapper.Map<PurchaseOrderItemsDto>(item);
            
            return dto;
        }
        public JsonResponse EditItem(string name, int qty, double price, int itemId, string link)
        {
            var item = _dbContext.PurchaseOrderItem.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
            {
                return new JsonResponse() { Message = "Item not found!" };
            }
            var order = _dbContext.PurchaseOrders.FirstOrDefault(x => x.Id == item.PurchaseOrderId);
            if (order.Status == State.Approved || order.Status == State.Denied || order.Status == State.Canceled || order.Status == State.Pending)
            {
                return new JsonResponse() { Message = "Cannot add items when status is approved, denied, canceled or in Pending" };
            }                      
            item.ItemsName = name;
            item.UnitPrice = price;
            item.Qty = qty;
            item.Total = price * qty;
            item.Link = link;
            _dbContext.PurchaseOrderItem.Update(item);
            _dbContext.SaveChanges();
            return new JsonResponse() { Message="Item edited successfully!",Success=true};
        }
        public async Task<JsonResponse> GetCloneComp(string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return new JsonResponse() { Message = "User not found!" };
            }
            var userCompanies = await _userManager.GetClaimsAsync(user);
            List<object> Companies = [];
            foreach (var i in _dbContext.Companies)
            {
                foreach (var claims in userCompanies)
                {
                    if(i.Status == Status.Active)
                    {
                        if (i.Id.ToString() == claims.Value)
                        {
                            Companies.Add(new { i.Name, i.Id });
                        }
                    }                
                }
            }
            return new JsonResponse() { Message = "Done" ,Companies=Companies,Success=true};
        }
    }
}
