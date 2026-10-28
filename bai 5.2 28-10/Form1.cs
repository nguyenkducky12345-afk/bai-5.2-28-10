using System;
using System.Collections.Generic;
using System.Data.SqlClient; 
using System.Text;
using System.Windows.Forms;

namespace StudentRegistrationApp
{
    public partial class Form1 : Form
    {
        // Chuỗi kết nối tới Database StudentDB.mdf trong cùng thư mục chạy
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=QuanLyDangKyDB;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCourseComboBox();
        }

        // 1. Binding dữ liệu vào ComboBox bằng DisplayMember & ValueMember
        private void LoadCourseComboBox()
        {
            List<Course> courses = new List<Course>()
            {
                new Course("CS01", "Lập trình C# Windows Forms"),
                new Course("WEB02", "Lập trình Web ASP.NET Core"),
                new Course("AI03", "Trí tuệ nhân tạo & Machine Learning"),
                new Course("DB04", "Thiết kế & Tối ưu SQL Server")
            };

            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "CourseName"; // Tên hiển thị ra ngoài
            cboCourse.ValueMember = "CourseId";     // Giá trị ẩn bên dưới
        }

        // 2. Xử lý khi nhấn nút "Đăng ký"
        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Kiểm tra rỗng Họ tên
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ và tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            // Kiểm tra SĐT đã điền đủ số theo Mask chưa (MaskCompleted)
            if (!mtxtPhone.MaskCompleted)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Số điện thoại đúng định dạng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtxtPhone.Focus();
                return;
            }

            // Thu thập dữ liệu
            string fullName = txtFullName.Text.Trim();
            string phone = mtxtPhone.Text;
            DateTime birthDate = dtpBirthDate.Value;
            string gender = radMale.Checked ? "Nam" : "Nữ";

            string courseId = cboCourse.SelectedValue?.ToString() ?? "";
            string courseName = cboCourse.Text;

            // Xử lý CheckBox (Dịch vụ bổ sung)
            List<string> extrasList = new List<string>();
            if (chkLaptop.Checked) extrasList.Add(chkLaptop.Text);
            if (chkCert.Checked) extrasList.Add(chkCert.Text);
            string extras = extrasList.Count > 0 ? string.Join(", ", extrasList) : "Không có";

            // Hiển thị thông tin lên MessageBox
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== THÔNG TIN ĐĂNG KÝ ===");
            sb.AppendLine($"Họ và tên: {fullName}");
            sb.AppendLine($"Số điện thoại: {phone}");
            sb.AppendLine($"Ngày sinh: {birthDate:dd/MM/yyyy}");
            sb.AppendLine($"Giới tính: {gender}");
            sb.AppendLine($"Khóa học: {courseName} (Mã: {courseId})");
            sb.AppendLine($"Tùy chọn: {extras}");

            MessageBox.Show(sb.ToString(), "Xác nhận Đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Lưu dữ liệu vào SQL Database
            SaveToDatabase(fullName, phone, birthDate, gender, courseId, courseName, extras);
        }

        // 3. Hàm lưu dữ liệu vào Database bằng ADO.NET
        private void SaveToDatabase(string fullName, string phone, DateTime birthDate, string gender, string courseId, string courseName, string extras)
        {
            string query = @"INSERT INTO Registrations (FullName, Phone, BirthDate, Gender, CourseId, CourseName, Extras) 
                             VALUES (@FullName, @Phone, @BirthDate, @Gender, @CourseId, @CourseName, @Extras)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Dùng Parameters để chống lỗi SQL Injection và sai format ngày tháng
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate.Date);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@CourseName", courseName);
                        cmd.Parameters.AddWithValue("@Extras", extras);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Đã lưu thông tin đăng ký vào Database thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ResetForm();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào Database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reset lại form sau khi đăng ký
        private void ResetForm()
        {
            txtFullName.Clear();
            mtxtPhone.Clear();
            dtpBirthDate.Value = DateTime.Now;
            radMale.Checked = true;
            chkLaptop.Checked = false;
            chkCert.Checked = false;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;
            txtFullName.Focus();
        }
    }
}