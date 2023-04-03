# DatabaseWinForm
Thư viện giúp thao tác với cơ sở dữ liệu trong winform dễ dàng hơn!

## Hướng dẫn sử dụng:

### [+] Kết nối database
- Đầu tiên vào class DB, tìm class DatabaseConnect, vào constructor và config 2 dòng
````csharp
//điền vào server sql của bạn
this.dataSource = "DESKTOP-FJ0Q38O\\SQLEXPRESS";
//điền vào tên database
this.database = "QLBanGiay";
````
&nbsp;

### [+] Cách lấy dữ liệu từ table: `all()`, `get()`
+ Lưu ý: Kiểu trả về sẽ là `DataTable`

- Lấy tất cả dữ liệu trong bảng, ở đây sẽ lấy tất cả dữ liệu từ bảng `danh_muc`
````csharp
/** Lấy tất cả dữ liệu từ bảng **/
DataTable duLieuTuBang = DB.table("danh_muc").all(); // Phương thức all là lấy tất cả

//Sau đó gửi vào dataGrid (table trên winform), ví dụ tên là dsDanhMuc
this.dsDanhMuc.DataSource = duLieuTuBang;
````
&nbsp;

### [+] Cách thêm điều kiện: `where()`, `orWhere()`
- Muốn thêm điều kiện thì chỉ cần thêm `.where()` là được.
````csharp
/** Lấy dữ liệu đạt yêu cầu từ điều kiện **/
// duLieuTuBang sẽ nhận được các dữ liệu trong bảng ngoại trừ dữ liệu có id là 1
DataTable duLieuTuBang = DB.table("danh_muc").where("id", ">", 2).get();

//Sau đó gửi vào dataGrid (table trên winform), ví dụ tên là dsDanhMuc
this.dsDanhMuc.DataSource = duLieuTuBang;
````

- Hoặc thêm nhiều điều kiện
````csharp
/** Lấy dữ liệu đạt yêu cầu từ điều kiện **/
// duLieuTuBang sẽ nhận được các dữ liệu trong bảng ngoại trừ dữ liệu có id là 1
DataTable duLieuTuBang = DB.table("danh_muc")
.where("id", ">", 2)
.where("name", "=", "ABC")
.where("trang_thai", "=", true)
.get();

//Sau đó gửi vào dataGrid (table trên winform), ví dụ tên là dsDanhMuc
this.dsDanhMuc.DataSource = duLieuTuBang;
````
+ Bằng cách thêm nhiều where cũng giống như việc thêm and trong sql
+ Ví dụ đoạn code trên là `where("id", ">", 2).where("name", "=", "ABC")` thì trong sql sẽ là `SELECT * FROM danh_muc WHERE id > 2 AND name = "ABC"`. Quá dễ hiểu luôn!
Bằng cách như vậy bạn có thể ghi điều kiện `OR` thông qua phương thức `.orWhere()`. Ví dụ: `.orWhere("id", "=", 2)`.
- Tip: Có thể ghi kiểu này `where("id", 2)` thay vì `where("id", "=", 2)` (nhưng chỉ bỏ qua được dấu `=` thôi, các dấu khác thì không ghi kiểu đó được nhé).

&nbsp;

### [+] Thêm dữ liệu `create()`
- Thêm dữ liệu có 2 cách tùy thuộc vào bạn sử dụng thôi!
- Lần này cần sử dụng phương thức `create()` của DB. Ví dụ: `DB.table("danh_muc").create()`.
- Phương thức `create()` sẽ trả về kiểu `DatabaseError`, bạn có thể dùng phương thức `isError()` của `DatabaseError` để kiểm tra lỗi.

+ Cách 1: Bạn cần khai báo `DatabaseInsert`. Sau đó, dùng phương thức `add()` để thêm vào.
- Lưu ý: Phương thức `add()` gồm 2 tham số là `key` và `value`. `Key` đại diện cho tên cột, `value` đại diện cho dữ liệu bạn muốn nhấn vào!
````csharp
DatabaseInsert dbInsert = new DatabaseInsert(); //khai báo đối tượng
dbInsert.add("id", 5);
dbInsert.add("name", "AAA");
dbInsert.add("trang_thai", true);
// Đoạn trên thể hiện bạn muốn thêm vào bảng gồm id là 5, name là "AAA" và trang_thai là true

// Sau khi khai báo xong bạn cần dùng phương thức create() của DB
// Ví dụ dưới đây sẽ thêm vào table danh_muc
DatabaseError result = DB.table("danh_muc").create(dbInsert);
if(result.isError() == true) {
  MessageBox.Show(result.message); //Show lỗi ra
  return;
}
MessageBox.Show("Thêm dữ liệu thành công");

````

+ Cách 2: Bạn cần khởi tạo ra 2 mảng. `Mảng thứ 1` sẽ điền tên cột và có kiểu `string`, `Mảng thứ 2` sẽ điền giá trị tương ứng và có kiểu `object`.
````csharp
//Khởi tạo 2 List
string[] key = { "id", "name", "trang_thai" };
object[] value = { 55, "OOO", true };

// Bằng cách này bạn đang muốn thêm id là 55, name là "OOO" và trang_thai là true.
// sau đó cần đưa vào create()
DatabaseError result = DB.table("danh_muc")
.create(new List<string>(key), new List<object>(value));

if(result.isError() == true) {
  MessageBox.Show(result.message); //Show lỗi ra
  return;
}
MessageBox.Show("Thêm dữ liệu thành công");
````
