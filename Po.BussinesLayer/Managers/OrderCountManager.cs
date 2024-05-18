using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.Data;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using Po.Common.Interfaces;
using Po.Common.Models;
using Po.Common.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BussinesLayer.Managers
{
    public class OrderCountManager : IOrderCountManager
    {
        private readonly PoDbContext _db;
        private readonly IMapper _mapper;
        public OrderCountManager(PoDbContext db, IMapper mapper) 
        { 
            _db = db; 
            _mapper = mapper;
        }
        public DataTableCounter GetAllCounters()
        {
            IQueryable<OrderCount> list = _db.OrderCount;
            var listDto = list.ProjectTo<OrderCountDto>(_mapper.ConfigurationProvider).ToList();
            return new DataTableCounter() {Data= listDto};
        }
        public CounterViewModel GetCounterVM()
        {
            var model = new CounterViewModel() 
            { 
                CompanyList = _db.Companies,

            };
            return model;
        }
        public JsonResponse Create(CounterViewModel model) 
        {
            var counterModel = new OrderCount()
            {
                CompanyId = model.OrderCount.CompanyId,
                Count = 0,
                FirstLetter = model.OrderCount.FirstLetter,
                FullNameLetter = model.OrderCount.FullNameLetter,
                Year = DateTime.Now.Year
            };
            _db.OrderCount.Add(counterModel);
            _db.SaveChanges();
            return new JsonResponse() { Message=$"Counter created",Success=true};
        }
        public OrderCountDto GetEditData(int id)
        {
            var model = _db.OrderCount.FirstOrDefault(x=>x.Id == id);
            if (model == null)
            {
                return null;
            }
            var modelDto = _mapper.Map<OrderCountDto>(model);
            return modelDto;
        }
        public JsonResponse Edit(int id, string firstLetter, int counter, string type)
        {
            var model = _db.OrderCount.FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                return new JsonResponse() { Message = "Counter not found!" };
            }
            model.FirstLetter = firstLetter;
            model.FullNameLetter = type;
            model.Count = counter;
            _db.OrderCount.Update(model);
            _db.SaveChanges();
            return new JsonResponse() { Message = "Counter edited!", Success = true };
        }
    }
}
