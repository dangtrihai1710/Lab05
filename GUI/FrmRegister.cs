using BUS;
using DAL.Models;
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

namespace GUI
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();
            cmbFaculty.SelectedIndexChanged += cmbFaculty_SelectedIndexChanged;
            CreateStudentGridColumns();
        }

        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        private void FrmRegister_Load(object sender, EventArgs e)
        {
            try
            {
                var listFaculties = facultyService.GetAll();
                FillFacultyCombobox(listFaculties);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            this.cmbFaculty.DataSource = listFaculties;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void FillMajorCombobox(List<Major> listMajors)
        {
            this.cmbMajor.DataSource = listMajors;
            this.cmbMajor.DisplayMember = "DisplayName";  
            this.cmbMajor.ValueMember = "MajorID";
            this.cmbMajor.SelectedIndex = -1;
        }


        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = false; 
                dgvStudent.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[2].Value = item.FullName;
                dgvStudent.Rows[index].Cells[3].Value = item.Faculty?.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = item.AverageScore;
            }
        }

        private void CreateStudentGridColumns()
        {
            dgvStudent.Columns.Clear();
            dgvStudent.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "Chọn", Name = "SelectColumn" });
            dgvStudent.Columns.Add("StudentID", "Mã sinh viên");
            dgvStudent.Columns.Add("FullName", "Họ tên");
            dgvStudent.Columns.Add("FacultyName", "Khoa");
            dgvStudent.Columns.Add("AverageScore", "Điểm TB");
            dgvStudent.Columns["FullName"].Width = 150;
            dgvStudent.Columns["FacultyName"].Width = 150;
        }

        private void btnRegister_Click(object sender, EventArgs e) {

                try
                {
                    Major selectedMajor = cmbMajor.SelectedItem as Major;
                    if (selectedMajor == null)
                    {
                        MessageBox.Show("Vui lòng chọn chuyên ngành.");
                        return;
                    }
                foreach (DataGridViewRow row in dgvStudent.Rows)
                {
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells["SelectColumn"];
                    if (chk.Value != null)
                    {
                        Student student = studentService.FindById(row.Cells["StudentID"].Value.ToString());
                        student.MajorID = (int)cmbMajor.SelectedValue;
                        studentService.InsertUpdate(student);
                    }
                }



                MessageBox.Show("Đăng ký thành công cho sinh viên.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
                }
        }
    }
}