using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using DataLayer.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Po.Common.Models.Dto;

namespace PurchaseOrderWeb.Controllers
{
    [Authorize]
    public class PDFController : Controller
    {
        private readonly PoDbContext _db;
        private readonly IMapper _mapper;
        public PDFController (PoDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GeneratePdf(int id)
        {
            var purchaseOrder = _db.PurchaseOrders.Where(o => o.Id == id);
            var orderDto = _mapper.ProjectTo<PurchaseOrderDto>(purchaseOrder).FirstOrDefault();
            var company = _db.Companies.FirstOrDefault(x => x.Name == orderDto.CompanyName);
            var counter = _db.OrderCount.FirstOrDefault(x => x.Id == orderDto.CounterId);
            var list = _db.PurchaseOrderItem.Where(x => x.PurchaseOrderId == id);

            var data = new PdfDocument();
            string htmlContent = "<div style = 'margin: auto; padding: 20px; background-color: #FFFFFF; font-family: Arial, sans-serif;' >";
            htmlContent += "<div style = 'text-align: center; margin-bottom: 30px;'>";
            htmlContent += "<h1 style='margin: 0'> Purchase Order " + counter.FullNameLetter + "<span style='margin-left: 40px;'>" + orderDto.PoNumber + "</span></h1>";
            htmlContent += "</div>";

            htmlContent += "<div>" +
                "<table style='border-collapse: collapse; width:100%;'> " +
                "<tr>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>" + orderDto.CompanyName + ", Inc</td>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'></td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>Date</td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'></td>" +
                "</tr>" +
                "<tr>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>" + company.Address + "</td>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'></td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>Vendor</td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>" + orderDto.VendorName + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>" + company.City + " " + company.ZipCode + "</td>" +
                "<td style='border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'></td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>Payment type</td>" +
                "<td style='border: 1px solid black; border-collapse: collapse; text-align: left; padding: 5px 8px 0px 2px;'>" + orderDto.PaymentType + "</td>" +
                "</tr>" +
                "</table>" +
                "</div>";
            htmlContent += "<table style ='width:100%; border-collapse: collapse; margin-top: 30px;'>";
            htmlContent += "<thead>";
            htmlContent += "<tr>";
            htmlContent += "<th style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>Requested items</th>";
            htmlContent += "<th style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>Qty</th>";
            htmlContent += "<th style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>Unit price</th>";
            htmlContent += "<th style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>Total</th>";
            htmlContent += "</tr>";
            htmlContent += "</thead>";
            htmlContent += "<tbody>";
            double price = 0;
            foreach (var item in list)
            {
                htmlContent += "<tr>";
                htmlContent += "<td style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>" + item.ItemsName + "</td>";
                htmlContent += "<td style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>" + item.Qty + "</td>";
                htmlContent += "<td style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>" + item.UnitPrice + "$</td>";
                htmlContent += "<td style='border: 1px solid black; border-collapse: collapse; text-align: center; padding: 8px 8px 0px 2px;'>" + Math.Round(item.Total, 3) + "$</td>";
                htmlContent += "</tr>";
                price += item.Total;
            }
            htmlContent += "</tbody>";

            htmlContent += "<tfoot>" +
            "<tr><td colspan='4' style='border: 1px solid black; border-collapse: collapse; text-align: right; padding: 8px 8px 0px 2px;'>Total: " + Math.Round(price, 3) + "$</td></tr>" +
            "</tfoot>";
            htmlContent += "</table>";
            htmlContent += "<table style ='width:100%; border-collapse: collapse; margin-top: 15px;'>" +
                "<tr>" +
                "<th colspan='2' style='border: 1px solid black; border-collapse: collapse;border: 1px solid black;border-collapse: collapse;text-align: left;padding: 8px 8px 0px 2px;'>Approved by " + orderDto.ApprovedBy + "</th>" +
                "</tr>" +
                "<tr>" +
                "<td style='border: 1px solid black;border-collapse: collapse;text-align: left;padding: 8px 8px 0px 2px;'>Order date &nbsp;" + orderDto.Date + "</td>" +
                "<td style='border: 1px solid black;border-collapse: collapse;text-align: left;padding: 8px 8px 0px 2px;'>Ordered By  " + orderDto.OrderedBy + "</td>" +
                "</tr>" +
                "<tr>";
            htmlContent += "<td colspan='2' style='border: none ;text-align: left; padding-top: 8px'>Prior purchase information:<div style='border: 1px solid black; padding: 8px 8px 0px 2px;border-collapse: collapse;text-align: left'>" + orderDto.Info + "</div></td>" +
                "</tr>" +
                "</table></div>";

            PdfGenerator.AddPdfPages(data, htmlContent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                data.Save(ms);
                response = ms.ToArray();
            }
            return File(response, "application/pdf");
        }
    }
}
