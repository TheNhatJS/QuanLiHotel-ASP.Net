using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLHotel.SQLHotel;

using System.Security.Cryptography;
using System.Text;

namespace QLHotel.Controllers
{
    public class GuestController : Controller
    {
        // GET: Guest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult gu_info()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        // --------------- UPDATE USER -------------------
        //[HttpPost]
        //public ActionResult gu_postUpdateInfo(FormCollection form)
        //{
        //    var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
        //    if (user == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    string name = form["name"];
        //    string email = form["email"];
        //    string phone = form["phone"];
        //    string OldPassword = form["oldPassword"];
        //    string NewPassword = form["newPassword"];
        //    string ReNewPassword = form["reNewPassword"];

        //    int idUser = user.userID;


        //    //Mã hóa pass nhập vào
        //    string HashedPassword;
        //    using (MD5 md5 = MD5.Create())
        //    {
        //        byte[] inputBytes = Encoding.ASCII.GetBytes(OldPassword);
        //        byte[] hashBytes = md5.ComputeHash(inputBytes);
        //        HashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //    }

        //    if (HashedPassword != user.password)
        //    {
        //        TempData["error"] = "Mật khẩu cũ không chính xác!";
        //        return RedirectToAction("gu_info");
        //    }

        //    if (NewPassword != ReNewPassword)
        //    {
        //        TempData["error"] = "Nhập mật khẩu không khớp!";
        //        return RedirectToAction("gu_info");
        //    }

        //    string HashedNewPassword;
        //    using (MD5 md5 = MD5.Create())
        //    {
        //        byte[] inputBytes = Encoding.ASCII.GetBytes(NewPassword);
        //        byte[] hashBytes = md5.ComputeHash(inputBytes);
        //        HashedNewPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //    }

        //    QLKhachSanEntities1 db = new QLKhachSanEntities1();

        //    tbl_user upUser = db.tbl_user.Find(idUser);
        //    if (upUser != null)
        //    {
        //        upUser.name = name;
        //        upUser.phone = phone;
        //        upUser.email = email;
        //        upUser.password = HashedNewPassword;

        //        upUser.created_at = DateTime.Now;
        //        upUser.updated_at = DateTime.Now;

        //        db.SaveChanges();
        //        TempData["error"] = "Cập nhật tài khoản thành công!";
        //        return RedirectToAction("gu_info");
        //    }
        //    else
        //    {
        //        TempData["error"] = "Cập nhật tài khoản không thành công!";
        //        return RedirectToAction("gu_info");
        //    }
        //}

        [HttpPost]
        public ActionResult gu_postUpdateInfo(FormCollection form)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string name = form["name"];
            string email = form["email"];
            string phone = form["phone"];
            string OldPassword = form["oldPassword"];
            string NewPassword = form["newPassword"];
            string ReNewPassword = form["reNewPassword"];

            int idUser = user.userID;

            // Kiểm tra xem người dùng đã nhập mật khẩu mới hay chưa
            bool isPasswordChanged = !string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(ReNewPassword);

            // Mã hóa mật khẩu cũ nếu người dùng đã nhập
            string HashedPassword = string.Empty;
            if (!string.IsNullOrEmpty(OldPassword))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(OldPassword);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    HashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                // Kiểm tra tính đúng đắn của mật khẩu cũ
                if (HashedPassword != user.password)
                {
                    TempData["error"] = "Mật khẩu cũ không chính xác!";
                    return RedirectToAction("gu_info");
                }
            }

            // Kiểm tra nếu người dùng đã nhập mật khẩu mới và nhập lại mật khẩu mới
            if (isPasswordChanged && NewPassword != ReNewPassword)
            {
                TempData["error"] = "Nhập mật khẩu không khớp!";
                return RedirectToAction("gu_info");
            }

            // Mã hóa mật khẩu mới nếu người dùng đã nhập
            string HashedNewPassword = string.Empty;
            if (isPasswordChanged)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(NewPassword);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    HashedNewPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
            else
            {
                // Nếu người dùng không nhập mật khẩu mới, giữ nguyên mật khẩu cũ
                HashedNewPassword = user.password;
            }

            using (QLKhachSanEntities1 db = new QLKhachSanEntities1())
            {
                tbl_user upUser = db.tbl_user.Find(idUser);
                if (upUser != null)
                {
                    upUser.name = name;
                    upUser.phone = phone;
                    upUser.email = email;
                    upUser.password = HashedNewPassword;

                    upUser.created_at = DateTime.Now;
                    upUser.updated_at = DateTime.Now;

                    db.SaveChanges();
                    TempData["error"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("gu_info");
                }
                else
                {
                    TempData["error"] = "Cập nhật tài khoản không thành công!";
                    return RedirectToAction("gu_info");
                }
            }
        }


        public ActionResult gu_list_booking()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int idUser = user.userID;

            using (QLKhachSanEntities1 db = new QLKhachSanEntities1())
            {
                var bookings = db.tbl_booking.Where(b => b.userID == idUser).ToList();
                return View(bookings);
            }
        }

        public ActionResult removeBooking(int idBooking)
        {
            using (QLKhachSanEntities1 db = new QLKhachSanEntities1())
            {
                // Xóa tất cả các hóa đơn có khóa ngoại trỏ đến bản ghi idBooking
                var bills = db.tbl_bill.Where(b => b.bookingID == idBooking).ToList();
                foreach (var bill in bills)
                {
                    db.tbl_bill.Remove(bill);
                }

                // Xóa bản ghi từ bảng tbl_booking
                tbl_booking booking = db.tbl_booking.Find(idBooking);
                if (booking != null)
                {
                    db.tbl_booking.Remove(booking);
                    db.SaveChanges();
                    TempData["error"] = "Hủy đặt phòng thành công!";
                }
                else
                {
                    TempData["error"] = "Không tìm thấy đặt phòng!";
                }
            }

            return RedirectToAction("gu_list_booking");
        }


        public ActionResult gu_updateBooking(int idBooking)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            using (QLKhachSanEntities1 db = new QLKhachSanEntities1())
            {
                tbl_booking upBooking = db.tbl_booking.FirstOrDefault(b => b.bookingID == idBooking);
                ViewBag.Booking = upBooking;
            }

            return View();
        }

        [HttpPost]
        public ActionResult gu_postUpdateBooking(FormCollection form)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            int UserID = user.userID;
            int roomID = int.Parse(form["roomID"]);
            int bookingID = int.Parse(form["bookingID"]);
            string name = form["name"];
            string email = form["email"];
            string phone = form["phone"];
            int countPeople = int.Parse(form["countPeople"]);
            string Note = form["note"];

            DateTime checkIn = DateTime.Parse(form["checkIn"]);
            DateTime checkOut = DateTime.Parse(form["checkOut"]);

            if (checkIn > checkOut)
            {
                TempData["error"] = "Có lỗi trong ngày đặt!";
                // Chuyển hướng người dùng đến trang đăng nhập sau khi đăng ký
                return RedirectToAction("gu_updateBooking", "Guest", new { idBooking = bookingID });
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();

            tbl_booking upBooking = db.tbl_booking.Find(bookingID);
            if(upBooking != null)
            {
                upBooking.userID = UserID;
                upBooking.roomID = roomID;
                upBooking.checkInDate = checkIn;
                upBooking.checkOutDate = checkOut;
                upBooking.odCountPeople = countPeople;
                upBooking.note = Note;

                upBooking.created_at = DateTime.Now;
                upBooking.updated_at = DateTime.Now;

                db.SaveChanges();
                TempData["error"] = "Cập nhật đặt phòng thành công!";
                return RedirectToAction("gu_list_booking");
            }
            else
            {
                TempData["error"] = "Không tìm thấy phòng!";
                return RedirectToAction("gu_list_booking");
            }

        }


    }
}