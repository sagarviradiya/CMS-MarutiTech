using System;
using System.Collections.Generic;
using CMS.Core.Services;
using CMS.Data.Common;
using CMS.Data.Models;

namespace CMS.Data.DataServices
{
    public class StudentDBService : IStudentService
    {
        private readonly string _connectionString;
        private readonly SqlHelper _dbHelper;


        public StudentDBService()
        {
            _connectionString = ConfigurationService.Get("ConnectionString");
            _dbHelper = new SqlHelper(_connectionString);
        }


        public IList<Student> GetAllStudents()
        {
            throw new NotImplementedException();
        }

        public Student GetStudentById(int? Id)
        {
            throw new NotImplementedException();
        }

        public void InsertNew(Student student)
        {
            var p = new Dictionary<string, object>();
            p.Add("@p_Password", student.Password);
            p.Add("@p_FirstName", student.FirstName);
            p.Add("@p_LastName", student.LastName);
            p.Add("@p_Email", student.Email);
            p.Add("@p_status", 0);
            p.Add("@p_Address", student.Address);
            p.Add("@p_ProfilePic", student.ProfilePic);

            _dbHelper.ExecNonQueryProc("Proc_SaveStudent", p);
        }
    }
}