using System.Collections.Generic;
using CMS.Data.Models;

namespace CMS.Core.Services
{
    public interface IStudentService
    {
        IList<Student> GetAllStudents();
        Student GetStudentById(int? Id);
        void InsertNew(Student student);
    }
}