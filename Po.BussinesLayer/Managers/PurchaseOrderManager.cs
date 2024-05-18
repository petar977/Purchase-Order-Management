using AutoMapper;
using DataLayer.Models;
using Po.Common.Interfaces;
using Po.Common.Models.Dto;
using DataLayer.Data;
using Po.Common.Models;
using Po.Common.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;

namespace BussinesLayer.Managers
{
    public class PurchaseOrderManager : IPurchaseOrderManager
    { 
        private readonly PoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ISenderEmail _senderEmail;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PurchaseOrderManager> _logger;
        
        public PurchaseOrderManager(
            PoDbContext dbContext, 
            IMapper mapper,
            ISenderEmail senderEmail,
            UserManager<IdentityUser> userManager,
            ILogger<PurchaseOrderManager> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _senderEmail = senderEmail;
            _userManager = userManager;
            _logger = logger;
        }
        DateTime timestamp = DateTime.UtcNow;

        public async Task<DataTableOrder> GetAll(PurchaseOrderFilter filter, string user)
        {             
            IQueryable<PurchaseOrder> query = _dbContext.PurchaseOrders;
            
            if (filter.selectedByCompany != 0)
            {
                query = query.Where(x => x.CompanyId == filter.selectedByCompany);
            }
            else
            {
                var userName = _userManager.Users.FirstOrDefault(x => x.UserName == user);
                var claims = await _userManager.GetClaimsAsync(userName);
                var companies = claims.Where(x => x.Type == "Company").Select(x => int.Parse(x.Value)); 

                query = query.Where(x => companies.Contains(x.CompanyId) && x.Companys.Status == Status.Active);
            }

            query = query.Where(x=> x.Status == State.InProgress ? x.OrderedBy == user : true);
            
            if(filter.selectedByStatus != 0)
            {
                query = query.Where(x => x.Status.Equals(filter.selectedByStatus));
            }
            if(filter.selectedByDays != 0)
            {
                var date = timestamp.AddDays(-filter.selectedByDays);
                query = query.Where(x => x.Date >= date);
            }
            if (filter.search.value != null)
            {
                query = query.Where(x =>
                x.VendorName.Contains(filter.search.value) || x.PaymentType.Contains(filter.search.value) || x.OrderedBy.Contains(filter.search.value) || x.PoNumber.Contains(filter.search.value));
            }

            switch (filter.sortCol)
            {
                case "date":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);
                    break;
                case "vendorName":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.VendorName) : query.OrderByDescending(x => x.VendorName);
                    break;
                case "paymentType":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.PaymentType) : query.OrderByDescending(x => x.PaymentType);
                    break;
                case "status":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status);
                    break;
                case "approvedDate":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.ApprovedDate) : query.OrderByDescending(x => x.ApprovedDate);
                    break;
                case "approvedBy":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.ApprovedBy) : query.OrderByDescending(x => x.ApprovedBy);
                    break;
                case "orderedBy":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.OrderedBy) : query.OrderByDescending(x => x.OrderedBy);
                    break;
                case "poNumber":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.PoNumber) : query.OrderByDescending(x => x.PoNumber);
                    break;
            }
            var recordsFilter = query.Count();
            query = query.Skip(filter.start).Take(filter.length);

            var results = query.ProjectTo<PurchaseOrderDto>(_mapper.ConfigurationProvider).ToList();
            return new DataTableOrder() { recordsFiltered = recordsFilter, data = results};
        }
        public async Task<PurchaseOrderDto> GetComp(string userName)
        {
            var user = _userManager.Users.FirstOrDefault(x=> x.UserName == userName);
            var claims = await _userManager.GetClaimsAsync(user);
            var companies = claims.Where(x => x.Type == "Company");
            var company = _dbContext.Companies.ToList();
            var model = new PurchaseOrderDto();
            var comp = new List<ManageUserCompaniesViewModel>();
            foreach (var i in companies)
            {
                var company1 = company.FirstOrDefault(x => x.Id == int.Parse(i.Value));
                var some = new ManageUserCompaniesViewModel
                {                  
                    CompanyName = company1.Name,
                    CompanyId = int.Parse(i.Value),
                    Status = company1.Status,
                   
                };
                comp.Add(some);
            }
            model.Companies = comp;
            var counter = _dbContext.OrderCount.ToList();
            
            var listCounter = _mapper.Map<List<OrderCountDto>>(counter);

            model.Counter = listCounter;
            return model;
        }
        public JsonResponse Add(PurchaseOrderDto order, string user, HostString baseurl)
        {
            var order1 = _mapper.Map<PurchaseOrder>(order);
            order1.Date = timestamp;
            order1.OrderedBy = user;
            order1.Status = State.InProgress;
            _dbContext.PurchaseOrders.Add(order1);
            _dbContext.SaveChanges();
            var order2 = _mapper.Map<PurchaseOrderDto>(order1);

            return new JsonResponse() { Message = "New order created.", Success = true, Id = order2.Id };
        }
        public JsonResponse Update(PurchaseOrderDto order, string user, bool isAdmin)
        {
            try
            {
                if (order.Status == "Approved" || order.Status == "Denied" || order.Status == "Canceled" || order.Status == "Pending" || order.Status == null)
                {
                    return new JsonResponse() { Message = "Cannot edit order when status is approved, denied, canceled or pending" };
                }
                else if (order.OrderedBy == user || isAdmin)
                {

                    var order1 = _mapper.Map<PurchaseOrder>(order);
                    _dbContext.Update(order1);
                    _dbContext.SaveChanges();
                    var order2 = _mapper.Map<PurchaseOrderDto>(order1);
                    return new JsonResponse() { Message = "Order edited", Success = true };
                }
                return new JsonResponse() { Message = "You dont have permission to edit this order" };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new JsonResponse() { Message = "Bad request!" };
            }
            
        }
        public PurchaseOrderDto Get(int id, string user, bool isAdmin)
        {
            try
            {
                var obj = _dbContext.PurchaseOrders.Where(x => x.Id == id).FirstOrDefault();
                if (obj == null)
                {
                    return null;
                }
                var company = _dbContext.Companies.FirstOrDefault(x => x.Id == obj.CompanyId);
 
                var obj1 = _mapper.Map<PurchaseOrderDto>(obj);

                if (obj1.OrderedBy == user || isAdmin)
                {
                    obj1.IsReadOnly = false;
                }
                else
                {
                    obj1.IsReadOnly = true;
                }
                return obj1;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.ToString());
                return null;
            }          
        }  
        public async Task<JsonResponse> ChangeStatus(int id, State status, string user, bool isAdmin, HostString baseurl)
        {             
            var obj = _dbContext.PurchaseOrders.Where(x => x.Id == id).FirstOrDefault();
            if(obj.OrderedBy == user || isAdmin)
            {
                if (obj.Status == State.InProgress && obj.PoNumber == null)
                {
                    using var transaction = _dbContext.Database.BeginTransaction();
                    try
                    {
                        var counter = _dbContext.OrderCount.FirstOrDefault(x => x.Id == obj.CounterId);
                        counter.Count = counter.Count + 1;
                        var zerosAdd = counter.Count.ToString().PadLeft(4, '0');
                        obj.PoNumber = counter.FirstLetter + zerosAdd;
                        _dbContext.Update(obj);
                        _dbContext.Update(counter);
                        transaction.Commit();
                    }
                    catch(Exception ex) 
                    {
                        _logger.LogError(ex.ToString());
                        transaction.Rollback();
                    }
                    
                }
                obj.Status = status;
                if (obj.Status == State.Approved)
                {
                    obj.ApprovedDate = DateTime.UtcNow;
                    obj.ApprovedBy = user;                    
                }
                else
                {
                    obj.ApprovedDate = null;
                    obj.ApprovedBy = null;
                }
                
                _dbContext.SaveChanges();
                _mapper.Map<PurchaseOrderDto>(obj);

                // auto send message
                if (status == State.InProgress && isAdmin)
                {
                    var mes = new Message(new string[] { obj.OrderedBy }, $"Order {obj.PoNumber} status has been changed", string.Format("Dear {0},\nYour order status has been back to Progress. " +
                "\nYou can check it on this url: https://{2}/PurchaseOrder/Edit/{3}", obj.OrderedBy, obj.Status, baseurl, obj.Id));
                    await Task.FromResult(_senderEmail.SendEmailAsync(mes));

                    return new JsonResponse() { Message = string.Format("Status changed to {0}", obj.Status), Success = true };
                }
                if (status == State.Canceled || status == State.InProgress) 
                {
                    return new JsonResponse() { Message = string.Format("Status changed to {0}", obj.Status), Success = true };
                }            
                if (status == State.Pending)
                {
                    var admins = GetAllAdmins();
                    foreach (var x in admins)
                    {
                        var msg = new Message(new string[] { x.Email }, $"New order created {obj.PoNumber}", string.Format("Dear {0},\nNew order has been added by {1} " +
                            "\nYou can check it on this url: https://{2}/PurchaseOrder/Edit/{3}", x.UserName, obj.OrderedBy, baseurl, obj.Id));
                        await Task.FromResult(_senderEmail.SendEmailAsync(msg));                        
                    }
                    return new JsonResponse() { Message = string.Format("Status changed to {0}", obj.Status), Success = true };
                }
                if (status == State.Approved)
                {                   
                    var email = _dbContext.Companies.FirstOrDefault(x=>x.Id == obj.CompanyId);
                    if (email.Email != null)
                    {
                        var msg1 = new Message(new string[] { email.Email }, $"Order {obj.PoNumber} status has been changed", string.Format("Dear {0},\nOrder status has been changed to {1} " +
                        "\nYou can check it on this url: https://{2}/PurchaseOrder/Edit/{3}", email.Email, obj.Status, baseurl, obj.Id));
                        await Task.FromResult(_senderEmail.SendEmailAsync(msg1));
                    }
                   
                    var msg2 = new Message(new string[] { obj.OrderedBy }, $"Order {obj.PoNumber} status has been changed", string.Format("Dear {0},\nYour order has been {1} " +
                    "\nYou can check it on this url: https://{2}/PurchaseOrder/Edit/{3}", obj.OrderedBy, obj.Status, baseurl, obj.Id));           
                    await Task.FromResult(_senderEmail.SendEmailAsync(msg2));
                        return new JsonResponse() { Message = string.Format("Status changed to {0}", obj.Status), Success = true };
                }
                var message = new Message(new string[] { obj.OrderedBy }, $"Order {obj.PoNumber} status has been changed", string.Format("Dear {0},\nYour order has been {1} " +
                "\nYou can check it on this url: https://{2}/PurchaseOrder/Edit/{3}", obj.OrderedBy, obj.Status, baseurl ,obj.Id));
                await Task.FromResult(_senderEmail.SendEmailAsync(message));

                return new JsonResponse() { Message = string.Format("Status changed to {0}",obj.Status), Success=true }; 
            }
            return new JsonResponse() { Message = "You dont have permission to change state" };
            
        }
        private IdentityUser[] GetAllAdmins()
        {
            var admins = from user in _dbContext.Users
                       join userRole in _dbContext.UserRoles
                       on user.Id equals userRole.UserId
                       join role in _dbContext.Roles
                       on userRole.RoleId equals role.Id
                       where role.Name == "Admin"
                       select user;
            
            return admins.ToArray();
        }
        public string GetInfo(int id)
        {
            var order = _dbContext.PurchaseOrders.FirstOrDefault(x => x.Id == id).Info;
            return order;
        }
    }
}
