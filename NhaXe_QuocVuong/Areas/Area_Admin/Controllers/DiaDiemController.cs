using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class DiaDiemController : Controller
    {
        
        NhaXeDataContext db = new NhaXeDataContext();
        //Danh sách địa điểm
        public ActionResult DanhSachDD(string search)
        {
            var dd = from d in db.DiaDiems
                     select d;

            if (!string.IsNullOrEmpty(search))
            {
                dd = dd.Where(e => e.TEN_TINH_THANH.Contains(search));
            }

            return View(dd.ToList());
        }
        // GET: Thêm Địa Điểm
        public ActionResult ThemDD()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDD(DiaDiem id)
        {
            if (ModelState.IsValid)
            {
                db.DiaDiems.InsertOnSubmit(id);
                db.SubmitChanges();
                return RedirectToAction("DanhSachDD");
            }
            return View(id);
        }
        // Xóa Địa Điểm
        [HttpPost]
        public JsonResult XoaDD(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new { success = false, message = "Không có ID nào được cung cấp." });
            }

            // Tìm các địa điểm với ID đã chọn
            var diaDiemsToDelete = db.DiaDiems.Where(d => ids.Contains(d.ID_DIADIEM)).ToList();

            if (!diaDiemsToDelete.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy địa điểm nào với ID đã cho." });
            }

            // Thực hiện xóa
            db.DiaDiems.DeleteAllOnSubmit(diaDiemsToDelete);
            db.SubmitChanges();

            return Json(new { success = true, message = "Các địa điểm đã được xóa thành công." });
        }
        //Cập nhật địa điểm
        public ActionResult CapNhatDD(int id)
        {
            var diaDiem = db.DiaDiems.SingleOrDefault(d => d.ID_DIADIEM == id);
            if (diaDiem == null)
            {
                return HttpNotFound();
            }
            return View(diaDiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatDD(DiaDiem diaDiem)
        {
            if (ModelState.IsValid)
            {
                db.DiaDiems.Attach(diaDiem); // Gắn đối tượng với db
                db.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, diaDiem); // Cập nhật thông tin
                db.SubmitChanges();
                return RedirectToAction("DanhSachDD");
            }
            return View(diaDiem);
        }
    }
}
