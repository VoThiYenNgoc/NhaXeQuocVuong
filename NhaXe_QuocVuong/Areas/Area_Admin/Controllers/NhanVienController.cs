using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NhaXe_QuocVuong.Models;
namespace NhaXe_QuocVuong.Areas.Area_Admin.Controllers
{
    public class NhanVienController : Controller
    {
      
        NhaXeDataContext db = new NhaXeDataContext();

        public ActionResult DanhSachNV(string search)
        {
            var nv = from n in db.NHANVIENs
                     select n;

            if (!string.IsNullOrEmpty(search))
            {
                nv = nv.Where(e => e.TEN_NHANVIEN.Contains(search));
            }

            return View(nv.ToList());
        }
        //Xóa nhân viên
        public ActionResult XoaNV(string username)
        {
            // Tìm nhân viên theo username
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        [HttpPost, ActionName("XoaNV")]
        public ActionResult XoaNVConfirmed(string username)
        {
            // Tìm nhân viên và xóa
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien != null)
            {
                // Xóa nhân viên
                db.NHANVIENs.DeleteOnSubmit(nhanVien);

                // Xóa tài khoản trong bảng UserAccount nếu có
                var userAccount = db.userAccounts.SingleOrDefault(u => u.username == username);
                if (userAccount != null)
                {
                    db.userAccounts.DeleteOnSubmit(userAccount);
                }

                db.SubmitChanges();
                return RedirectToAction("DanhSachNV");
            }
            return HttpNotFound();
        }
        //Thêm nhân viên
        public ActionResult ThemNV()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNV(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu username đã tồn tại trong bảng UserAccount
                var userAccount = db.userAccounts.SingleOrDefault(u => u.username == model.USERNAME);
                if (userAccount != null)
                {
                    // Tạo đối tượng nhân viên mới
                    var nhanVien = new NHANVIEN
                    {
                        USERNAME = model.USERNAME,
                        TEN_NHANVIEN = model.TEN_NHANVIEN,
                        EMAIL = model.EMAIL,
                        SDT = model.SDT,
                        LOAI_NV = model.LOAI_NV
                    };

                    // Thêm nhân viên vào database
                    db.NHANVIENs.InsertOnSubmit(nhanVien);
                    db.SubmitChanges();
                    return RedirectToAction("DanhSachNV");
                }
                else
                {
                    ModelState.AddModelError("", "Username không tồn tại trong bảng UserAccount.");
                }
            }
            return View(model);
        }
        //Sửa thông tin nhân viên
        public ActionResult SuaNV(string username)
        {
            // Tìm nhân viên theo username
            var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == username);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaNV(NHANVIEN model)
        {
            if (ModelState.IsValid)
            {
                // Tìm nhân viên theo username
                var nhanVien = db.NHANVIENs.SingleOrDefault(n => n.USERNAME == model.USERNAME);
                if (nhanVien != null)
                {
                    // Cập nhật thông tin nhân viên
                    nhanVien.TEN_NHANVIEN = model.TEN_NHANVIEN;
                    nhanVien.EMAIL = model.EMAIL;
                    nhanVien.SDT = model.SDT;
                    nhanVien.LOAI_NV = model.LOAI_NV;

                    db.SubmitChanges();
                    return RedirectToAction("DanhSachNV");
                }
                return HttpNotFound();
            }
            return View(model);
        }
    }
}

