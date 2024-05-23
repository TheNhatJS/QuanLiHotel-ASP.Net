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
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult ad_add_room()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ad_add_room(FormCollection form)
        {
            // Lấy thông tin từ form
            string roomName = form["roomName"];
            int countPeo = int.Parse(form["countPeo"]);
            long roomPrice = long.Parse(form["roomPrice"]);
            byte roomStatus = byte.Parse(form["roomStatus"]);
            string roomType = form["roomType"];
            string roomInfo = form["roomInfo"];
            string roomImage = form["roomImage"];

            using (var db = new QLKhachSanEntities1())
            {
                if(db.tbl_room.Any(r => r.name == roomName))
                {
                    TempData["error"] = "Phòng đã tồn tại!";
                    return RedirectToAction("ad_add_room");
                }
            }

            // Tạo một đối tượng tbl_room mới và gán thông tin từ form vào
            tbl_room newRoom = new tbl_room
            {
                name = roomName,
                countPeople = countPeo,
                price = roomPrice,
                status = roomStatus,
                type = roomType,
                info = roomInfo,
                image = roomImage,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            // Thêm đối tượng mới vào cơ sở dữ liệu
            using (var db = new QLKhachSanEntities1())
            {
                db.tbl_room.Add(newRoom);
                db.SaveChanges();
            }
            TempData["error"] = "Thêm phòng thành công!";
            // Chuyển hướng người dùng đến trang hiển thị danh sách phòng sau khi thêm
            return RedirectToAction("ad_list_room", "Admin");
        }
        // ----------- UPDATE ROOM ------------
        public ActionResult updateRoom(int idRoom)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            var db = new QLKhachSanEntities1();
            tbl_room room = db.tbl_room.FirstOrDefault(r => r.roomID == idRoom);
            ViewBag.Room = room;
            return View();
        }

        [HttpPost]
        public ActionResult postUpdateRoom(FormCollection form)
        {
            var db = new QLKhachSanEntities1();
            tbl_room upRoom = db.tbl_room.Find(int.Parse(form["roomID"]));
            string nameNow = upRoom.name;
            if (upRoom != null)
            {
                upRoom.name = form["roomName"];
                upRoom.countPeople = int.Parse(form["countPeo"]);
                upRoom.price = long.Parse(form["roomPrice"]);
                upRoom.status = byte.Parse(form["roomStatus"]);
                upRoom.type = form["roomType"];
                upRoom.info = form["roomInfo"];
                upRoom.image = form["roomImage"];
                upRoom.created_at = DateTime.Now;
                upRoom.updated_at = DateTime.Now;

                var existingRoom = db.tbl_room.Any(r => r.name == upRoom.name);
                if (existingRoom && upRoom.name != nameNow)
                {
                    TempData["error"] = "Tên phòng đã tồn tại!";
                    return RedirectToAction("updateRoom", new { idRoom = int.Parse(form["roomID"]) });
                }

                db.SaveChanges();
                TempData["error"] = "Cập nhật phòng thành công!";
                return RedirectToAction("ad_list_room");
            }
            else
            {
                TempData["error"] = "Không tìm thấy phòng!";
                return RedirectToAction("ad_list_room");
            }
        }
        public ActionResult removeRoom(int idRoom)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            tbl_room room = db.tbl_room.Find(idRoom);
            db.tbl_room.Remove(room);
            db.SaveChanges();

            TempData["error"] = "Xóa phòng thành công!";

            return RedirectToAction("ad_list_room", "Admin");
        }


        public ActionResult ad_list_bill()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            List<tbl_bill> allBill = db.tbl_bill.ToList();
            return View(allBill);
        }

        // -----------BEGIN: Xuất hóa đơn, Cập nhật lại trạng thái của phòng -------------
        
        public ActionResult updateStatus1Room(int idbooking)
        {
            var db = new QLKhachSanEntities1();
            tbl_booking booking = db.tbl_booking.FirstOrDefault(b => b.bookingID == idbooking);
            tbl_room room = db.tbl_room.FirstOrDefault(r => r.roomID == booking.roomID);

            int priceRoom = (int)room.price;

            var existingBill = db.tbl_bill.Any(b => b.bookingID == booking.bookingID);
            if (existingBill)
            {
                TempData["error"] = "Hóa đơn đã tồn tại!";
                return RedirectToAction("ad_list_booking_room");
            }

            tbl_bill newBill = new tbl_bill
            {
                bookingID = idbooking,
                userID = booking.userID,
                pay = priceRoom,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            
            db.tbl_bill.Add(newBill);
            db.SaveChanges();

            var roomUpdate = db.tbl_room.Find(room.roomID);
            roomUpdate.status = 1;
            db.SaveChanges();

            TempData["error"] = "Đã Xuất hóa đơn, xác nhận phòng đã trống!";
            return RedirectToAction("ad_list_booking_room");

        }
        // -----------END: Xuất hóa đơn, Cập nhật lại trạng thái của phòng -------------


        // -----------BEGIN: Xóa hóa đơn -------------
        //public ActionResult removeBill(int idBooking)
        //{
        //    QLKhachSanEntities1 db = new QLKhachSanEntities1();
        //    tbl_bill bill = db.tbl_bill.Find(idBooking);
        //    db.tbl_bill.Remove(bill);
        //    db.SaveChanges();

        //    tbl_booking booking = db.tbl_booking.Find(idBooking);
        //    db.tbl_booking.Remove(booking);
        //    db.SaveChanges();

        //    TempData["error"] = "Xóa hóa đơn thành công!";

        //    return RedirectToAction("ad_list_bill", "Admin");
        //}

        public ActionResult removeBill(int idBooking)
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
                    TempData["error"] = "Xóa hóa đơn thành công!";
                }
                else
                {
                    TempData["error"] = "Không tìm thấy hóa đơn!";
                }
            }

            return RedirectToAction("ad_list_bill", "Admin");
        }


        // -----------BEGIN: Xóa hóa đơn -------------


        public ActionResult ad_list_room()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            List<tbl_room> allRoom = db.tbl_room.ToList();


            return View(allRoom);
        }

        public ActionResult ad_list_booking_room()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            List<tbl_booking> allBooking = db.tbl_booking.ToList();


            return View(allBooking);
        }
        //---------------Đặt phòng----------------
        public ActionResult postBookingRoom(FormCollection form)
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            int UserID = user.userID;
            int roomID = int.Parse(form["roomID"]);
            string name = form["name"];
            string email = form["email"];
            string phone = form["phone"];
            int countPeople = int.Parse(form["countPeople"]);
            string Note = form["note"];

            DateTime checkIn = DateTime.Parse(form["checkIn"]);
            DateTime checkOut = DateTime.Parse(form["checkOut"]);

            using (var db = new QLKhachSanEntities1())
            {
                tbl_room checkCountPeople = db.tbl_room.FirstOrDefault(r => r.roomID == roomID);
                if (countPeople > checkCountPeople.countPeople)
                {
                    TempData["error"] = "Số lượng người vượt quá quy định phòng!";
                    // Chuyển hướng người dùng đến trang đăng nhập sau khi đăng ký
                    return RedirectToAction("bookingRoom", "Home", new { idRoom = roomID });
                }

            }
            // Lấy ngày tháng năm hiện tại
            DateTime currentDate = DateTime.Now.Date;


            // Kiểm tra nếu ngày nhận phòng nhỏ hơn ngày hiện tại
            if (checkIn < currentDate)
            {
                TempData["error"] = "Ngày nhận phòng phải lớn hơn hoặc bằng ngày hiện tại!";
                // Chuyển hướng người dùng đến trang đặt phòng sau khi đặt phòng
                return RedirectToAction("bookingRoom", "Home", new { idRoom = roomID });
            }

            if (checkIn > checkOut)
            {
                TempData["error"] = "Có lỗi trong ngày đặt!";
                // Chuyển hướng người dùng đến trang đăng nhập sau khi đăng ký
                return RedirectToAction("bookingRoom", "Home", new { idRoom = roomID });
            }

            tbl_booking newBooking = new tbl_booking
            {
                userID = UserID,
                roomID = roomID,
                checkInDate = checkIn,
                checkOutDate = checkOut,
                odCountPeople = countPeople,
                note = Note,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            // Thêm đối tượng mới vào cơ sở dữ liệu
            using (var db = new QLKhachSanEntities1())
            {
                db.tbl_booking.Add(newBooking);

                db.SaveChanges();

            }

            TempData["error"] = "Đặt phòng thành công!";
            // Chuyển hướng người dùng đến trang đăng nhập sau khi đăng ký
            return RedirectToAction("bookingRoom", "Home", new { idRoom = roomID });
        }
        // ------------Cập nhật phòng đã được đặt --------------
        public ActionResult updateStatus0Room(int idRoom)
        {
            using (var db = new QLKhachSanEntities1())
            {
                var room = db.tbl_room.Find(idRoom);
                if(room != null)
                {
                    room.status = 0;
                    db.SaveChanges();
                    TempData["error"] = "Xác nhận phòng đã được đặt!";
                    return RedirectToAction("ad_list_booking_room");
                }
                else
                {
                    TempData["error"] = "Không tìm thấy phòng!";
                    return RedirectToAction("ad_list_booking_room");
                }
            }   
        }



        public ActionResult ad_list_user()
        {
            var user = Session["userr"] as QLHotel.SQLHotel.tbl_user;
            if (user == null || user.role == "GUEST")
            {
                return RedirectToAction("Index", "Home");
            }

            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            List<tbl_user> allUser = db.tbl_user.Where(u => u.role == "GUEST").ToList();


            return View(allUser);
        }



        //LOGIN - REGISTER - LOGOUT
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult postLogin(String email, String password)
        {
            // Mã hóa mật khẩu thành MD5
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password); // Chuyển đổi chuỗi mật khẩu thành mảng byte sử dụng mã hóa ASCII.
                byte[] hashBytes = md5.ComputeHash(inputBytes); //Tính toán giá trị băm MD5 cho mảng byte của mật khẩu sử dụng phương pháp ComputeHash của đối tượng MD5.

                // Chuyển mảng byte thành chuỗi hex
                StringBuilder sb = new StringBuilder(); //Tạo một đối tượng StringBuilder để xây dựng chuỗi hex từ các byte của giá trị băm.
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); //Duyệt qua mỗi byte trong giá trị băm và chuyển đổi nó thành một chuỗi hex (hai ký tự),
                                                            //sau đó nối vào chuỗi kết quả.
                }
                string hashedPassword = sb.ToString();

                //Check db
                QLKhachSanEntities1 db = new QLKhachSanEntities1();
                var user = db.tbl_user.SingleOrDefault(m => m.email == email && m.password == hashedPassword);
                if (user != null)
                {
                    Session["userr"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Email hoặc mật khẩu không chính xác!";
                    //TempData["error"] = password;
                    return RedirectToAction("login");
                }
            }
        }

        public ActionResult logout()
        {
            // Xóa các thông tin phiên liên quan đến người dùng
            Session.Clear(); // Xóa toàn bộ dữ liệu trong phiên
            Session.Abandon(); // Kết thúc phiên hiện tại

            // Điều hướng người dùng đến trang đăng nhập hoặc trang chính của ứng dụng
            return RedirectToAction("Index", "Home");
        }

        public ActionResult register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult register(FormCollection form)
        {
            string Name = form["name"];
            string Email = form["email"];
            string Phone = form["phone"];
            string Password = form["password"];
            string REpassword = form["REpassword"];

            if (Password != REpassword)
            {
                TempData["error"] = "Mật khẩu không khớp!";
                return RedirectToAction("register");
            }

            using (var db = new QLKhachSanEntities1())
            {
                if (db.tbl_user.Any(u => u.email == Email))
                {
                    TempData["error"] = "Email đã tồn tại!";
                    return RedirectToAction("register");
                }
            }

            // Mã hóa mật khẩu bằng MD5
            string HashedPassword;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(Password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                HashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            tbl_user newUser = new tbl_user
            {
                name = Name,
                email = Email,
                phone = Phone,
                password = HashedPassword, // Lưu mật khẩu đã được mã hóa
                role = "GUEST",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            // Thêm đối tượng mới vào cơ sở dữ liệu
            using (var db = new QLKhachSanEntities1())
            {
                db.tbl_user.Add(newUser);

                db.SaveChanges();
               
            }

            TempData["error"] = "Tạo tài khoản thành công!";
            // Chuyển hướng người dùng đến trang đăng nhập sau khi đăng ký
            return RedirectToAction("login");
        }


        public ActionResult removeUser(int idUser)
        {
            QLKhachSanEntities1 db = new QLKhachSanEntities1();
            tbl_user user = db.tbl_user.Find(idUser);
            db.tbl_user.Remove(user);
            db.SaveChanges();

            TempData["error"] = "Xóa tài khoản thành công!";

            return RedirectToAction("ad_list_user", "Admin");
        }



    }
}