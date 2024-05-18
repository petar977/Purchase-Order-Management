using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.Data;
using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Models.Dto;

namespace BussinesLayer.Managers
{
    public class ItemsManager : IItemsManager
    {
        private readonly PoDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        public ItemsManager(PoDbContext dbContext,IMapper mapper,UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        public ItemsDto Add(ItemsDto item)
        {
            var item1 = _mapper.Map<Items>(item);
            _dbContext.Item.Add(item1);
            _dbContext.SaveChanges();
            var item2 = _mapper.Map<ItemsDto>(item1);
            return item2;
        }
        public List<ItemsDto> AutoComplete(string q)
        {
            IQueryable<Items> list = _dbContext.Item;
            list = list.Where(x => x.Name.Contains(q));

            var results = list.Select(obj => _mapper.Map<ItemsDto>(obj)).ToList();

            return results;
        }
        public DataTableItems GetItems(FilterItems filter)
        {
            IQueryable<Items> query = _dbContext.Item;
            if(filter.selectedByCompany != 0)
            {
                query=query.Where(x=>x.CompanyId == filter.selectedByCompany);
            }
            var companies = _dbContext.Companies.Where(x => x.Status == Status.Active);
            query = query.Where(x=>x.Companies.Id == x.CompanyId && x.Companies.Status == Status.Active);
            if(filter.search.value != null)
            {
                query = query.Where(x => x.Name.Contains(filter.search.value));
            }
            switch (filter.sortCol)
            {
                case "name":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    break;
                case "unitPrice":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.UnitPrice) : query.OrderByDescending(x => x.UnitPrice);
                    break;
                case "companyName":
                    query = filter.orderDir == "asc" ? query.OrderBy(x => x.Companies.Name) : query.OrderByDescending(x => x.Companies.Name);
                    break;
            }

            var recordsFiltered = query.Count();
            query = query.Skip(filter.start).Take(filter.length);         
            var results =query.ProjectTo<ItemsDto>(_mapper.ConfigurationProvider).ToList();

            return new DataTableItems() { Data=results, RecordsFiltered=recordsFiltered};
        }
        public ItemsDto GetItem(int id)
        {
            var item = _dbContext.Item.FirstOrDefault(x=>x.Id == id);
            if (item == null)
            {
                return null ;
            }
            var itemDto = _mapper.Map<ItemsDto>(item);
            return itemDto;
        }
        public JsonResponse EditItem(ItemsDto modelDto)
        {
            var item = _dbContext.Item.FirstOrDefault(x => x.Id == modelDto.Id);
            if (item == null)
            {
                return new JsonResponse() { Message = "Item not found!" };
            }
            item.UnitPrice = modelDto.UnitPrice;
            item.Name = modelDto.Name;
            _dbContext.Item.Update(item);
            _dbContext.SaveChanges();
            return new JsonResponse() { Message="Item successfully edited!", Success=true };
        }
        public JsonResponse Delete(int id)
        {
            var item = _dbContext.Item.FirstOrDefault(x=>x.Id == id);
            if(item == null)
            {
                return new JsonResponse() { Message = "Item not found!" };
            }
            _dbContext.Item.Remove(item);
            _dbContext.SaveChanges();
            return new JsonResponse() { Message = "Item deleted!", Success = true };
        }
        public async Task<ItemsDto> Index (string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var companies = await _userManager.GetClaimsAsync(user);
            var list = new List<ManageUserCompaniesViewModel>();
            var company = _dbContext.Companies.ToList();           
            foreach (var i in companies)
            {
                var company1 = company.FirstOrDefault(x => x.Id == int.Parse(i.Value));
                var manageUserCompaniesViewModel = new ManageUserCompaniesViewModel()
                {
                    CompanyId = int.Parse(i.Value),
                    CompanyName = company1.Name,
                    Status = company1.Status
            };
                list.Add(manageUserCompaniesViewModel);
            }
            var dto = new ItemsDto()
            {
                Companys = list,
     
            };
            return dto;
        }
    }
}
