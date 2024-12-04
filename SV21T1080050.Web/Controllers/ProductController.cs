using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080050.BusinessLayers;
using SV21T1080050.DomainModels;
using SV21T1080050.Web.Models;

namespace SV21T1080050.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class ProductController : Controller
    {
        public const int PAGE_SIZE = 30;

        //public IActionResult Index(int page = 1, string searchValue = "")
        //{
        //    int rowCount;
        //    var data = ProductDataService.ListProducts(out rowCount, page, PAGE_SIZE, searchValue ?? "");

        //    int pageCount = 1;
        //    pageCount = rowCount / PAGE_SIZE;
        //    if (rowCount % PAGE_SIZE > 0)
        //        pageCount += 1;

        //    ViewBag.Page = page;
        //    ViewBag.RowCount = rowCount;
        //    ViewBag.PageCount = pageCount;
        //    ViewBag.SearchValue = searchValue;

        //    return View(data); //List<Customer>
        //}
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);

        }
        public IActionResult Search(ProductSearchInput condition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu dữ liệu không hợp lệ
            }
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                SupplierID = condition.SupplierID,
                CategoryID = condition.CategoryID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                RowCount = rowCount,
                Data = data,

            };

            //lưu lại session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png",
                IsSelling = true

            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin về mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
            {
                ModelState.AddModelError(nameof(data.ProductName), "Vui lòng nhập tên mặt hàng");
            }


            if (string.IsNullOrWhiteSpace(data.ProductDescription))
            {
                data.ProductDescription = "";
            }

            if (data.ProductDescription.Length > 1999)
            {
                ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả không được vượt quá 1999 ký tự.");
                return View("Edit", data);

            }


            if (data.CategoryID == 0)
            {
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            }
            if (data.SupplierID == 0)
            {
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
            }
            if (string.IsNullOrWhiteSpace(data.Unit))
            {
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
            }
            if (data.Price <= 0)
            {
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá lớn hơn 0.");
            }


            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }
            //Bổ sung ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file đã lưu
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);//đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                if (id <= 0)
                {
                    ModelState.AddModelError("InvalidProductName", "Tên mặt hàng bị trùng");
                    return View("Edit", data);
                }
                return RedirectToAction("Edit", new {id = id});
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);
                if (!result)
                {
                    ModelState.AddModelError("InvalidProductName", "Tên mặt hàng bị trùng");
                    return View("Edit", data);

                }
            }

            return RedirectToAction("Index");

        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)//nếu dữ liệu k tồn tại thì ta quay về index
                return RedirectToAction("Index");

            return View(data);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var data = new ProductPhoto()
                    {
                        ProductID = id,
                        PhotoID = photoId,
                        IsHidden = false,
                        Photo = "nophoto.png"
                    };
                    return View("Photo", data);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    var photo = ProductDataService.GetPhoto(photoId);
                    if (photo == null)
                    {
                        return RedirectToAction("Index");

                    }
                    return View("Photo", photo);
                case "delete":
                    //TODO: xóa ảnh(xóa trực tiếp,không cần confirm)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh của mặt hàng";
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file đã lưu
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products\photos", fileName);//đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (string.IsNullOrWhiteSpace(data.Description))
            {
                data.Description = "";
            }
            if (data.Description.Length > 100)
            {
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được vượt quá 100 ký tự...");
                return View("Photo", data);
            }
            if (data.DisplayOrder <= 0)
            {

                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng nhập thứ tự hiển thị ");
            }
            

            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }

            if (data.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(data);
                if (id <= 0)
                {
                    ModelState.AddModelError("Invalid", "File hoặc thứ tự hiển thị của ảnh bị trùng");
                    return View("Photo", data);
                }

            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                if (result == false)
                {
                    ModelState.AddModelError("Invalid", "File hoặc thứ tự hiển thị của ảnh bị trùng");
                    return View("Photo", data);


                }
            }

            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var data = new ProductAttribute()
                    {
                        AttributeID = attributeId,
                        ProductID = id,
                    };
                    return View("Attribute", data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    var attribute = ProductDataService.GetAttribute(attributeId);
                    if (attribute == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View("Attribute", attribute);
                case "delete":
                    //TODO: xóa thuộc tính(xóa trực tiếp,không cần confirm)
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Thay đổi thuộc tính của mặt hàng";
            if (string.IsNullOrWhiteSpace(data.AttributeName))
            {
                ModelState.AddModelError(nameof(data.AttributeName), "Vui lòng nhập tên thuộc tính");
            }
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
            {
                ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị thuộc tính");
            }


            if (data.DisplayOrder == 0)
            {
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng nhập thứ tự hiển thị");
            }

            if (!ModelState.IsValid)
            {
                return View("Attribute", data);
            }

            if (data.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(data);
                if(id <= 0)
                {
                    ModelState.AddModelError("Invalid", "Tên thuộc tính hoặc thứ tự hiển thị bị trùng");
                    return View("Attribute", data);
                }  
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                if(result == false)
                {
                    ModelState.AddModelError("Invalid", "Tên thuộc tính hoặc thứ tự hiển thị bị trùng");
                    return View("Attribute", data);
                }    
            }
            return RedirectToAction("Edit", new {id = data.ProductID});
        }

    }
}
