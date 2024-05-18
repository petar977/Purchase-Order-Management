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
    public class CompanyManager : ICompanyManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PoDbContext _dbContext;
        private readonly IMapper _mapper;
        public CompanyManager(PoDbContext dbContext, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public List<CompanyDto> GetCompanies()
        {
            IQueryable<Company> list = _dbContext.Companies;            
            var results = _mapper.Map<List<CompanyDto>>(list).ToList();
            return results;
        }
        public JsonResponse AddCompany(CompanyDto name)
        {
            name.Status = Status.Active;
            var obj = _mapper.Map<Company>(name);           
            _dbContext.Companies.Add(obj);
            _dbContext.SaveChanges();

            return new JsonResponse() { Message = "Company has been added!", Success = true };
        }
        public JsonResponse EditCompany(CompanyDto modelDto)
        {
            var company = _dbContext.Companies.Any(x => x.Id == modelDto.Id);
            if (company == false)
            {
                return new JsonResponse() { Message = "Company Not Found", Success = false };
            }

            var model = _mapper.Map<Company>(modelDto);
            _dbContext.Companies.Update(model);
            _dbContext.SaveChanges();
            return new JsonResponse() { Message = "Company has been changed!" ,Success = true };
        }
        public CompanyDto GetCompany(int id)
        {
            var company = _dbContext.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
            {
                return null;
            }
            var dto = _mapper.Map<CompanyDto>(company);

            return dto;
        }
        public JsonResponse ChangeStatus(int id)
        {
            var company = _dbContext.Companies.FirstOrDefault(x => x.Id == id);
            if( company == null )
            {
                return new JsonResponse() { Message = "Company not found!" };
            }
            if( company.Status == Status.Active)
            {
                company.Status = Status.Inactive;
            }
            else
            {
                company.Status = Status.Active;
            }
            _dbContext.Companies.Update(company);
            _dbContext.SaveChanges();
            return new JsonResponse() { Message = $"Company status changed to: {company.Status}", Success = true };
        }
    }
}
