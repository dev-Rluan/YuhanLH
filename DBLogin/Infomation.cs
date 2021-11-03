using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLogin
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
}
