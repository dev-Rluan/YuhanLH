using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Student : IInformation
    {
        public string Id { get;}
        public string Pw { get;}
        public string StudentId { get;}
        public string Name { get;}

        public Student()
        {
        }

        public Student(string Id, string Pw, string StudentId, string Name)
        {
            this.Id = Id;
            this.Pw = Pw;
            this.StudentId = StudentId;
            this.Name = Name;
        }

        public void Print()
        {
            Console.WriteLine($"ID = {Id}, PW = {Pw}, StudentId = {StudentId}, Name = {Name}");
        }
    }

    class Professor : IInformation
    {
        public string Id { get;}
        public string Pw { get;}
        public string ProfessorId { get;}
        public string Name { get;}

        public Professor()
        {
        }

        public Professor(string Id, string Pw, string ProfessorId, string Name)
        {
            this.Id = Id;
            this.Pw = Pw;
            this.ProfessorId = ProfessorId;
            this.Name = Name;
        }
        public void Print()
        {
            Console.WriteLine($"ID = {Id}, PW = {Pw}, ProfessorId = {ProfessorId}, Name = {Name}");
        }
    }

    class Schedule : IInformation
    {
        public string LectureCode { get; set; }
        public string LectureName { get; set; }

        public Schedule()
        {
        }

        public Schedule(string LectureCode, string LectureName)
        {
            this.LectureCode = LectureCode;
            this.LectureName = LectureName;
        }
        public void Print()
        {
            Console.WriteLine($"LctureCode = {LectureCode}, LectureName = {LectureName}");
        }
    }

    class Lecture : IInformation
    {
        public string lecture_code { get;}
        public string professor_id { get;}
        public string lecture_name { get; }
        public string week_day { get; }
        public string start_time { get; }
        public string end_time { get; }
        public int credit { get; }

        public Lecture()
        {
        }

        public Lecture(string lecture_code, string professor_id, string lecture_name, int credit,
                        string week_day, string start_time, string end_time)
        {
            this.lecture_code = lecture_code;
            this.lecture_name = lecture_name;
            this.professor_id = professor_id;
            this.credit = credit;
            this.week_day = week_day;
            this.start_time = start_time;
            this.end_time = end_time;
        }
        public void Print()
        {
            Console.WriteLine($"Lecture_code = {lecture_code}, Lecture_name = {lecture_name}, Professor_Id = {professor_id}"
                            + $"Credit = {credit}, Week_day = {week_day}, Start_time = {start_time}, End_Time = {end_time}");
        }
    }

    class Attendance : IInformation
    {
        private int Attendance_code { get; set; }
        public string student_Id { get; set; }
        public string lecture_code { get; set; }
        public int Week_Code { get; set; }
        public int first_class { get; set; }
        public int second_class { get; set; }
        public int third_class { get; set; }

        public Attendance(int Attendance_code, string student_Id, string lecture_code,
                    int Week_Code, int first_class,
                    int second_class, int third_class)
        {
            this.Attendance_code = Attendance_code;
            this.student_Id = student_Id;
            this.lecture_code = lecture_code;
            this.Week_Code = Week_Code;
            this.first_class = first_class;
            this.second_class = second_class;
            this.third_class = third_class;
        }

        public void Print()
        {
           Console.WriteLine("Attendance_code = " + Attendance_code);
           Console.WriteLine("student_Id = " + student_Id);
           Console.WriteLine("Week_Code = " + Week_Code);
           Console.WriteLine("first_class = " + first_class);
           Console.WriteLine("second_class = " + second_class);
           Console.WriteLine("third_class = " + third_class);
        }
    }

}
