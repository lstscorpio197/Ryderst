using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAdmin.Areas.Admin.Controllers;
using ShopAdmin.Authorize;
using ShopAdmin.Common;
using ShopAdmin.Dto;
using ShopAdmin.Models;
using ShopAdmin.WHttpMessage;

namespace ShopAdmin.Areas.Admin.Controllers.Systems
{
    [Area("Admin")]
    public class SPermissionController : BaseController
    {
        // GET: SPermission
        [AuthorizeAccessRole(typeHandle: "view")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTable(SearchDto itemSearch)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var query = db.SPermissions.AsNoTracking().Where(x => true);
                if (!string.IsNullOrEmpty(itemSearch.Ma))
                {
                    query = query.Where(x => x.Controller.Contains(itemSearch.Ma) || x.ControllerName.Contains(itemSearch.Ma));
                }
                if (!string.IsNullOrEmpty(itemSearch.Ten))
                {
                    query = query.Where(x => x.Action.Contains(itemSearch.Ten) || x.ActionName.Contains(itemSearch.Ten));
                }
                if (itemSearch.Enable != -1)
                {
                    query = query.Where(x => x.Enable == itemSearch.Enable);
                }
                var result = query.OrderBy(x => x.Id).Skip(itemSearch.Skip).Take(itemSearch.PageSize).ToList();
                httpMessage.Body.Data = result;
                httpMessage.Body.Pagination = new HttpMessagePagination
                {
                    NumberRowsOnPage = itemSearch.PageSize,
                    PageNumber = itemSearch.PageNum,
                    TotalRowsOnPage = result.Count(),
                    TotalRows = query.Count()
                };
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }


        public JsonResult Generate()
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var lstAppMenu = AppMenu.ListMenu.Select(x => x.Controller);
                var lstMenuExist = db.SPermissions.AsNoTracking().Where(x => lstAppMenu.Contains(x.Controller)).Select(x => x.Controller).ToList();
                var lstMenu = new List<SPermission>();
                foreach (var item in AppMenu.ListMenu.Where(x => x.Type == 1))
                {
                    if (lstMenuExist.Contains(item.Controller))
                    {
                        continue;
                    }
                    foreach (var action in AppMenu.ListActionDefault)
                    {
                        lstMenu.Add(new SPermission
                        {
                            Controller = item.Controller,
                            ControllerName = item.ControllerName,
                            Action = action.TypeHandle,
                            ActionName = action.Name,
                            Enable = 1
                        });
                    }
                }
                db.SPermissions.AddRange(lstMenu);
                db.SaveChanges();
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }

        [HttpPost]
        [AuthorizeAccessRole(typeHandle: "delete")]
        public JsonResult Delete(int id)
        {
            HttpMessage httpMessage = new HttpMessage(true);
            try
            {
                var item = db.SPermissions.Find(id);
                if (item == null)
                {
                    httpMessage.IsOk = false;
                    httpMessage.Body.MsgNoti = new HttpMessageNoti("400", null, "Không tìm thấy thông tin");
                    return Json(httpMessage);
                }
                db.SPermissions.Remove(item);
                db.SaveChanges();
                return Json(httpMessage);
            }
            catch (Exception ex)
            {
                httpMessage.IsOk = false;
                httpMessage.Body.MsgNoti = new HttpMessageNoti("500", null, ex.Message);
                return Json(httpMessage);
            }
        }
    }
}