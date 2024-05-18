using DataLayer.Models;
using Po.Common.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Interfaces
{
    public interface ICompanyManager
    {
        List<CompanyDto> GetCompanies();
        JsonResponse AddCompany(CompanyDto name);
        JsonResponse EditCompany(CompanyDto modelDto);
        CompanyDto GetCompany(int id);
        JsonResponse ChangeStatus(int id);
    }
}
