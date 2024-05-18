using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Po.Common.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Interfaces
{
    public interface IPurchaseOrderManager
    {
        Task<DataTableOrder> GetAll(PurchaseOrderFilter filter,string user);
        Task<PurchaseOrderDto> GetComp(string userName);
        JsonResponse Add(PurchaseOrderDto order, string user,HostString baseurl);
        PurchaseOrderDto Get(int id,string user,bool isAdmin);
        JsonResponse Update(PurchaseOrderDto order,string user,bool isAdmin);
        Task<JsonResponse> ChangeStatus(int id, State status,string user,bool isAdmin, HostString baseurl);
        string GetInfo(int id);

    }
}
