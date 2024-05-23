using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLHotel.SQLHotel;

namespace QLHotel.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            List<tbl_room> allRoom = db.tbl_room.ToList();

            
            return View(allRoom);
        }

        public ActionResult bookingRoom(int? idRoom)
        {
            //ViewBag.roomID = idRoom;
            ////QLKhachSanEntities1 db = new QLKhachSanEntities1();
            ////List<tbl_room> allRoom = db.tbl_room.ToList();
            //return View();

            //=====================================================
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (user == null)
            {
                TempData["error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("login", "Admin");
            }


            // Tạo đối tượng DbContext để truy vấn dữ liệu
            QLKhachSanEntities1 db = new QLKhachSanEntities1();

            // Truy vấn dữ liệu từ bảng 'tbl_room' bằng ID phòng
            tbl_room room = db.tbl_room.FirstOrDefault(r => r.roomID == idRoom);

            // Kiểm tra nếu room không tồn tại
            if (room == null)
            {
                // Chuyển hướng về trang Index
                return RedirectToAction("Index");
            }

            // Chuyển dữ liệu sang View bằng ViewBag
            ViewBag.Room = room;

            return View();
        }
    }
}