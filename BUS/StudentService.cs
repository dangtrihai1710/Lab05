using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;


namespace BUS
{
    public class StudentService 
    {
        public List<Student> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Students.ToList();
        }


        // Phương thức lấy tất cả sinh viên không có chuyên ngành
        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Students.Where(p => p.MajorID == null).ToList();
        }

        // Phương thức lấy tất cả sinh viên không có chuyên ngành theo khoa
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModel context = new StudentModel();
            return context.Students.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        // Phương thức tìm sinh viên theo mã sinh viên
        public Student FindById(string studentId)
        {
            StudentModel context = new StudentModel();
            return context.Students.FirstOrDefault(p => p.StudentID == studentId);
        }

        // Phương thức thêm hoặc cập nhật sinh viên
        public void InsertUpdate(Student s)
        {
            StudentModel context = new StudentModel();
            context.Students.AddOrUpdate(s);
            context.SaveChanges();
        }

        // Phương thức xóa sinh viên theo mã sinh viên
        public void Delete(string studentId)
        {
            using (StudentModel context = new StudentModel())
            {
                var student = context.Students.FirstOrDefault(p => p.StudentID == studentId);

                if (student != null)
                {
                    context.Students.Remove(student);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Không tìm thấy sinh viên với mã này!");
                }
            }
        }



    }

}
