using BUS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Models;

namespace GUI
{
    public partial class frmStudent : Form
    {
        StudentModel context = new StudentModel();
        BindingSource bsStudent = new BindingSource();

        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        public frmStudent()
        {
            InitializeComponent();
            LoadData();
            ClearInputFields();
        }



        private void LoadFacultyData()
        {
            var faculties = facultyService.GetAll();
            FillFacultyCombobox(faculties);
        }

        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            cmbFaculty.DataSource = listFaculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }



        void LoadData()
        {
            try
            {
                LoadFacultyData();

                var students = (from s in studentService.GetAll()
                                select new
                                {
                                    StudentID = s.StudentID,
                                    FullName = s.FullName,
                                    AverageScore = s.AverageScore,
                                    FacultyID = s.FacultyID,
                                    FacultyName = s.Faculty != null ? s.Faculty.FacultyName : "N/A",
                                    MajorName = s.Major != null ? s.Major.Name : "N/A",
                                    Avatar = s.Avatar != null ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", s.Avatar) : null
                                }).OrderBy(p => p.StudentID).ToList();


                bsStudent.DataSource = students;
                dgvStudent.DataSource = bsStudent;

                dgvStudent.Columns["StudentID"].HeaderText = "Mã SV";
                dgvStudent.Columns["FullName"].HeaderText = "Họ và Tên";
                dgvStudent.Columns["FacultyName"].HeaderText = "Khoa";
                dgvStudent.Columns["AverageScore"].HeaderText = "ĐTB";
                dgvStudent.Columns["AverageScore"].DefaultCellStyle.Format = "N1";
                dgvStudent.Columns["MajorName"].HeaderText = "Chuyên Ngành";

                if (dgvStudent.Columns.Contains("FacultyID"))
                {
                    dgvStudent.Columns["FacultyID"].Visible = false;
                }
                if (dgvStudent.Columns.Contains("Avatar"))
                {
                    dgvStudent.Columns["Avatar"].Visible = false;
                }

                dgvStudent.Columns["FullName"].Width = 150;
                dgvStudent.Columns["FacultyName"].Width = 150;
                dgvStudent.Columns["MajorName"].Width = 150;

                dgvStudent.Columns["StudentID"].DisplayIndex = 0;
                dgvStudent.Columns["FullName"].DisplayIndex = 1;
                dgvStudent.Columns["FacultyName"].DisplayIndex = 2;
                dgvStudent.Columns["AverageScore"].DisplayIndex = 3;
                dgvStudent.Columns["MajorName"].DisplayIndex = 4;

                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi khi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddBinding()
        {
            txtMSSV.DataBindings.Clear();
            txtName.DataBindings.Clear();
            txtAverageScore.DataBindings.Clear();
            cmbFaculty.DataBindings.Clear();
            picAvatar.DataBindings.Clear();

            txtMSSV.DataBindings.Add(new Binding("Text", bsStudent, "StudentID", true, DataSourceUpdateMode.Never));
            txtName.DataBindings.Add(new Binding("Text", bsStudent, "FullName", true, DataSourceUpdateMode.Never));
            txtAverageScore.DataBindings.Add(new Binding("Text", bsStudent, "AverageScore", true, DataSourceUpdateMode.Never));
            cmbFaculty.DataBindings.Add(new Binding("SelectedValue", bsStudent, "FacultyID", true, DataSourceUpdateMode.Never));
            picAvatar.DataBindings.Add(new Binding("ImageLocation", bsStudent, "Avatar", true, DataSourceUpdateMode.Never));
        }

        private void ClearInputFields()
        {
            txtMSSV.Clear();
            txtName.Clear();
            txtAverageScore.Clear();
            cmbFaculty.SelectedIndex = -1;
            picAvatar.Image = null;
        }

        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", imageName);

                if (File.Exists(imagePath))
                {
                    picAvatar.Image = Image.FromFile(imagePath);
                    picAvatar.Refresh();
                }
                else
                {
                    MessageBox.Show($"Image not found: {imagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    picAvatar.Image = null;
                }
            }
        }

        public void SetGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp) | *.jpg; *.jpeg; *.png; *.bmp";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = txtMSSV.Text + Path.GetExtension(filePath);
                    string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                    try
                    {
                        if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images")))
                        {
                            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"));
                        }

                        using (var image = Image.FromFile(filePath))
                        {
                            if (Path.GetExtension(filePath).ToLower() == ".png")
                            {
                                image.Save(targetPath, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                image.Save(targetPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                        picAvatar.ImageLocation = targetPath; // Cập nhật đường dẫn ảnh trong PictureBox

                        var student = studentService.FindById(txtMSSV.Text);
                        if (student != null)
                        {
                            student.Avatar = fileName;
                            studentService.InsertUpdate(student);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void chkMajority_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                List<Student> listStudents;
                if (chkMajority.Checked)
                    listStudents = studentService.GetAllHasNoMajor();
                else
                    listStudents = studentService.GetAll();

                var studentData = listStudents.Select(s => new
                {
                    StudentID = s.StudentID,
                    FullName = s.FullName,
                    AverageScore = s.AverageScore,
                    FacultyName = s.Faculty != null ? s.Faculty.FacultyName : "N/A",
                    FacultyID = s.FacultyID,
                    MajorName = s.Major != null ? s.Major.Name : "N/A",
                    Avatar = s.Avatar != null ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", s.Avatar) : null
                }).ToList();

                bsStudent.DataSource = studentData;
                dgvStudent.DataSource = bsStudent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi khi tải dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var student = studentService.FindById(txtMSSV.Text);
                if (student != null)
                {
                    student.FullName = txtName.Text;
                    student.AverageScore = float.Parse(txtAverageScore.Text);
                    student.FacultyID = (int)cmbFaculty.SelectedValue;

                    if (!string.IsNullOrEmpty(picAvatar.ImageLocation))
                    {
                        string fileName = txtMSSV.Text + Path.GetExtension(picAvatar.ImageLocation).ToLower();
                        student.Avatar = fileName;
                    }

                    studentService.InsertUpdate(student);
                    MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    student = new Student
                    {
                        StudentID = txtMSSV.Text,
                        FullName = txtName.Text,
                        AverageScore = float.Parse(txtAverageScore.Text),
                        FacultyID = (int)cmbFaculty.SelectedValue,
                        Avatar = !string.IsNullOrEmpty(picAvatar.ImageLocation) ? txtMSSV.Text + Path.GetExtension(picAvatar.ImageLocation).ToLower() : null
                    };

                    studentService.InsertUpdate(student);
                    MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi khi thêm hoặc cập nhật sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteNote_Click(object sender, EventArgs e)
        {
            txtMSSV.Text = string.Empty;
            txtName.Text = string.Empty;
            cmbFaculty.SelectedIndex = -1;
            txtAverageScore.Text = string.Empty;
            picAvatar.Image = null;
        }

        private void frmRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRegister frmRegister = new FrmRegister();
            frmRegister.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string studentId = txtMSSV.Text;

                if (string.IsNullOrEmpty(studentId))
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var student = studentService.FindById(studentId);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    studentService.Delete(studentId);
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi khi xóa sinh viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }

        private void dgvStudent_Click(object sender, EventArgs e)
        {
            AddBinding();
        }
    }


}