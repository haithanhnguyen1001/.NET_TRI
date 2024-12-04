using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080050.BusinessLayers;
using SV21T1080050.DomainModels;
using SV21T1080050.Web.Models;

namespace SV21T1080050.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 5;
        //public IActionResult Index(int page = 1, string searchValue = "")
        //{
        //    int rowCount;
        //    var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");


        //    int pageCount = 1;
        //    pageCount = rowCount / PAGE_SIZE;
        //    if (rowCount % PAGE_SIZE > 0)
        //        pageCount += 1;

        //    ViewBag.Page = page;
        //    ViewBag.RowCount = rowCount;
        //    ViewBag.PageCount = pageCount;
        //    ViewBag.SearchValue = searchValue;

        //    return View(data);
        //}

        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);

        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };

            //lưu lại session
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }



        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Vui lòng nhập tên người giao hàng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");

            if(!ModelState.IsValid)
            {
                return View("Edit", data);
            }    

            //TODO: kiểm tra dữ liệu đầu vào đúng hay không?
            if(data.ShipperID == 0)
            {
                int id = CommonDataService.AddShipper(data);
                if(id <= 0)
                {
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng bị trùng");
                    return View("Eidt",data);
                }    
            }    
            else
            {
                bool result = CommonDataService.UpdateShipper(data);
                if(result == false)
                {
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng bị trùng");
                    return View("Edit", data);
                }    
            }    
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");

            }    
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            
            return View(data);

        }
    }
}
