using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class StudentInfo
    {
        public bool status { get; set; }
        Student student = null;
        StudentInfo(Student s)
        {
            this.student = s;
        }
        public Student getStudent()
        {
            return this.student;
        }
       
        ClientSession clientSession;
    }
}
