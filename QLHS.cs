using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;


namespace hainhph35304_SD18305_Lab3
{
    public partial class QLHS : Form
    {
        static string connectionString = @"Data Source=Nghh;Initial Catalog=QLHOCSINH;Persist Security Info=True;User ID=sa;Password=nghh206983";

        public QLHS()
        {
            InitializeComponent();
        }
        public DataSet FillDataSet(string sqlCommand)
        {
            DataSet ds = new DataSet();
            if (string.IsNullOrEmpty(sqlCommand))
            {
                return ds;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand, conn);
                    adapter.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public DataTable GetDSLop()
        {
            string query = "SELECT MaLop FROM LOP";
            DataSet ds = FillDataSet(query);
            return ds.Tables[0];
        }
        private void LoadDSLop()
        {
            DataTable dt = GetDSLop();
            cbLop.DataSource = dt;
            cbLop.DisplayMember = "MaLop";
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrEmpty(txtMaHS.Text) ||
                string.IsNullOrEmpty(txtTenHS.Text) ||
                string.IsNullOrEmpty(dtpNgaySinh.Text) ||
                string.IsNullOrEmpty(txtDiaChi.Text) ||
                string.IsNullOrEmpty(cbLop.Text) ||
                string.IsNullOrEmpty(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin học sinh!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tạo kết nối tới database
            SqlConnection conn = new SqlConnection("Data Source=Nghh;Initial Catalog=QLHOCSINH;Persist Security Info=True;User ID=sa;Password=nghh206983");
            conn.Open();

            // Tạo câu truy vấn SELECT để kiểm tra MaHS
            string checkQuery = "SELECT COUNT(*) FROM HocSinh WHERE MaHS=@MaHS";
            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@MaHS", txtMaHS.Text);

            // Thực thi câu truy vấn SELECT
            int count = (int)checkCmd.ExecuteScalar();

            // Kiểm tra nếu MaHS đã tồn tại trong cơ sở dữ liệu
            if (count > 0)
            {
                MessageBox.Show("Mã học sinh đã tồn tại trong cơ sở dữ liệu. Vui lòng nhập lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaHS.Focus();
                return;
            }
            // Tạo câu truy vấn INSERT
            string query = "INSERT INTO HocSinh (MaHS ,TenHS, NgaySinh, DiaChi, MaLop, DTB) VALUES (@MaHS ,@TenHS, @NgaySinh, @DiaChi, @MaLop, @DTB)";
            SqlCommand cmd = new SqlCommand(query, conn);                   //tạo một đối tượng SqlCommand để thực hiện câu truy vấn SQL
            cmd.Parameters.AddWithValue("@MaHS", txtMaHS.Text);             //thiết lập tham số @MaHS trong câu truy vấn SQL bằng txtMaHS.Text
            cmd.Parameters.AddWithValue("@TenHS", txtTenHS.Text);
            cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
            cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@MaLop", cbLop.Text);
            cmd.Parameters.AddWithValue("@DTB", txtDiemTB.Text);
            // Thực thi câu truy vấn INSERT
            cmd.ExecuteNonQuery();

            // Đóng kết nối
            conn.Close();

            // Hiển thị thông báo thành công
            MessageBox.Show("Thêm học sinh thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtMaHS_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu phím được nhấn không phải là chữ, số
            if (!char.IsLetterOrDigit(e.KeyChar))
            {
                // Không cho phép nhập ký tự đó vào ô văn bản
                e.Handled = true;
            }
        }

        private void txtTenHS_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu phím được nhấn không phải là chữ hoặc khoảng trắng
            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                // Không cho phép nhập ký tự đó vào ô văn bản
                e.Handled = true;
            }
        }

        private void txtDiaChi_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu phím được nhấn không phải là số, chữ, khoảng trắng hoặc dấu -
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && e.KeyChar != '-')
            {
                // Không cho phép nhập ký tự đó vào ô văn bản
                e.Handled = true;
            }
        }

        private void txtDiemTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu phím được nhấn không phải là số hoặc kí tự .
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                // Không cho phép nhập ký tự đó vào ô văn bản
                e.Handled = true;
            }
            else if (e.KeyChar == '.' && txtDiemTB.Text.IndexOf('.') > -1)
            {
                // Không cho phép nhập kí tự . nếu đã có một kí tự . được nhập vào trước đó
                e.Handled = true;
            }
        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem giá trị trong ô DateTimePicker có thay đổi hay không
            if (dtpNgaySinh.Value == DateTime.Today)
            {
                // Hiển thị thông báo yêu cầu người dùng thay đổi giá trị
                MessageBox.Show("Bạn phải chọn một ngày sinh hợp lệ.");
            }
        }

        private void cbLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem giá trị trong ô ComboBox có được chọn hay không
            if (cbLop.SelectedIndex == -1)
            {
                // Hiển thị thông báo yêu cầu người dùng phải chọn lớp
                MessageBox.Show("Bạn phải chọn một lớp.");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Lấy giá trị trong ô văn bản txtMaHS
            string maHS = txtMaHS.Text;

            // Kiểm tra xem người dùng đã nhập mã sinh viên hay chưa
            if (maHS != "")
            {
                // Tạo kết nối tới database
                SqlConnection conn = new SqlConnection("Data Source=Nghh;Initial Catalog=QLHOCSINH;Persist Security Info=True;User ID=sa;Password=nghh206983");

                // Tạo câu lệnh SQL để xóa thông tin sinh viên khỏi cơ sở dữ liệu
                string sql = "DELETE FROM HOCSINH WHERE MaHS = @MaHS";

                // Tạo đối tượng SqlCommand để thực thi câu lệnh SQL
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Thêm tham số vào câu lệnh SQL
                cmd.Parameters.AddWithValue("@MaHS", maHS);

                // Mở kết nối đến cơ sở dữ liệu
                conn.Open();

                // Thực thi câu lệnh SQL
                int rowsAffected = cmd.ExecuteNonQuery();

                // Đóng kết nối đến cơ sở dữ liệu
                conn.Close();

                // Kiểm tra xem có thông tin sinh viên nào bị xóa khỏi cơ sở dữ liệu hay không
                if (rowsAffected > 0)
                {
                    // Hiển thị thông báo xóa thành công
                    MessageBox.Show("Đã xóa thông tin sinh viên có mã " + maHS);
                }
                else
                {
                    // Hiển thị thông báo không tìm thấy sinh viên
                    MessageBox.Show("Không tìm thấy sinh viên có mã " + maHS);
                }
            }
            else
            {
                // Hiển thị thông báo yêu cầu nhập mã sinh viên
                MessageBox.Show("Bạn phải nhập mã sinh viên để xóa thông tin.");
            }
        }

        private void btnNhapLai_Click(object sender, EventArgs e)
        {
            // Reset về ban đầu 
            txtMaHS.Text = "";
            txtTenHS.Text = "";
            txtDiaChi.Text = "";
            dtpNgaySinh.Value = DateTime.Now;
            txtDiaChi.Text = "";
            cbLop.SelectedIndex = 0;
            txtDiemTB.Text = "";
            txtMaHS.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Nếu người dùng xác nhận muốn thoát
            if (result == DialogResult.Yes)
            {
                // Đóng form hiện tại
                this.Close();
            }
        }
    }
}
