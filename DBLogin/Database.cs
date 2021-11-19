using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Collections;

namespace DBLogin
{
    class Database : IDatabase
    {
        /*
            readme.txt 를 꼭 읽어주세요!
        */
        private const int STUDENT = 0;
        private const int PROFESSOR = 1;
        private const int SCHEDULE = 2;
        

        private static string dbIp = "192.168.55.85";
        private static string dbName = "deskDB";
        private static string dbId = "C##capstone_admin";
        private static string dbPw = "yuhanunivcapstone1212";
        private static bool attFlag = false;
        private OracleConnection conn;
        private OracleCommand command;
        private OracleDataAdapter adapter;
        private DataSet data;


        public Database()
        {
            Console.WriteLine("db연결중");
            string strConn = string.Format($"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={dbIp})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={dbName})));User ID={dbId};Password={dbPw};Connection Timeout=30;");
            conn = new OracleConnection(strConn);
            try
            {
                conn.Open();
                Console.WriteLine("연결 성공");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        ~Database()
        {
            conn.Close();
        }

        public void Execute(string query)
        {
            int row;

            if (IsOpen())
            {
                using (command = new OracleCommand(query, conn))
                {
                    row = command.ExecuteNonQuery();
                }
                Console.WriteLine($"{row}");
            }
            else
            {
                Console.WriteLine("ERROR : 데이터 베이스 연결에 실패했습니다.");
            }
        }

        public List<IInformation> ExecuteList(int flag, string PM = "")
        {
            List<IInformation> result = new List<IInformation>();
            string id, pw, name, studentId, professorId, lecture_code, lecture_name;

            switch (flag)
            {
                case STUDENT:
                    Student student;

                    using (data = Select("*", "student"))
                    {
                        foreach (DataRow r in data.Tables[0].Rows)
                        {
                            id = r["id"].ToString();
                            pw = r["pw"].ToString();
                            name = r["name"].ToString();
                            studentId = r["student_id"].ToString();
                            student = new Student(id, pw, studentId, name);
                            result.Add(student);
                        }
                    }
                    break;
                case PROFESSOR:
                    Professor professor;

                    using (data = Select("*", "Professor"))
                    {
                        foreach (DataRow r in data.Tables[0].Rows)
                        {
                            id = r["id"].ToString();
                            pw = r["pw"].ToString();
                            name = r["name"].ToString();
                            professorId = r["Professor_id"].ToString();
                            professor = new Professor(id, pw, professorId, name);
                            result.Add(professor);
                        }
                    }
                    break;
                case SCHEDULE:
                    Schedule schedule;

                    using (data = Select("Lecture_Code, Lecture_Name", "student_lecture", $"student_id = {PM}"))
                    {
                        foreach (DataRow r in data.Tables[0].Rows)
                        {
                            lecture_code = r["lecture_code"].ToString();
                            lecture_name = r["lecture_name"].ToString();
                            schedule = new Schedule(lecture_code, lecture_name);
                            result.Add(schedule);
                        }
                    }
                    break;
            }
            return result;
        }

        public bool IsOpen()
        {
            return (!Equals(conn.State, ConnectionState.Closed));
            
        }

        /// <summary>
        /// 데이터를 조회하는 함수입니다. 
        /// 컬럼명, 테이블명
        /// </summary>
        /// <param name="col"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public DataSet Select(string col, string table)
        {
            string query = $"select {col} from {table}";
            DataSet ds;

            using (ds = new DataSet())
            {
                if (IsOpen())
                {
                    using (adapter = new OracleDataAdapter(query, conn))
                    {
                        adapter.Fill(ds);
                    }
                }
                else
                {
                    Console.WriteLine("ERROR : 데이터 베이스 연결이 실패했습니다.");
                }
                return ds;
            }
        }

        /// <summary>
        /// 데이터를 조회하는 함수입니다. 
        /// 컬럼명, 테이블명, 조건
        /// </summary>
        /// <param name="col"></param>
        /// <param name="table"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataSet Select(string col, string table, string where)
        {
            DataSet ds = new DataSet();
            string query = $"select {col} from {table} where {where}";

            if (IsOpen())
            {
                using (adapter = new OracleDataAdapter(query, conn))
                {
                    adapter.Fill(ds);
                }
            }
            else
            {
                Console.WriteLine("ERROR : 데이터 베이스 연결에 실패했습니다.");
            }
            return ds;
        }

        /// <summary>
        /// 데이터를 삽입하는 함수입니다. 테이블명, 값 <br/>
        /// 값은 괄호로 묶이며 Comma(,)로 구분합니다. 문자열의 경우 Quotation(' ')로 감싸입니다.<br/>
        /// 예 ) table = "student", values = "('Yuhan', 1234)"
        /// </summary>
        /// <param name="table"></param>
        /// <param name="values"></param>
        public void Insert(string table, string values)
        {
            string query = @$"
                insert into {table} 
                values({values})
            ";

            Execute(query);
        }

        /// <summary>
        /// 데이터를 수정하는 함수입니다. 테이블명, 값, 조건 <br/>
        /// 조건은 " id = 'Yuhan' " 형식 입니다.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        public void Update(string table, string value, string where)
        {
            string query = @$"
                update {table} 
                set {value}
                where {where}
            ";

            Execute(query);
        }

        /// <summary>
        /// 데이터를 삭제하는 함수입니다. 테이블명, 조건
        /// 조건은 " id = 'Yuhan' " 형식 입니다.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="where"></param>
        public void Delete(string table, string where)
        {
            string query = @$"
                delete from {table}
                where {where}
            ";

            Execute(query);
        }

        /// <summary>
        /// 전체 학생의 모든 정보를 가져오는 함수입니다.
        /// </summary>
        /// <returns></returns>
        public List<IInformation> GetStudentList()
        {
            return ExecuteList(STUDENT);
        }

        /// <summary>
        /// 교수의 모든 정보를 가져오는 함수입니다.
        /// </summary>
        /// <returns></returns>
        public List<IInformation> GetProfessorList()
        {
            return ExecuteList(PROFESSOR);
        }

        /// <summary>
        /// 학생의 시간표의 정보를 가져오는 함수입니다.
        /// </summary>
        /// <param name="student_Id"></param>
        /// <returns></returns>
        public List<IInformation> GetScheduleList(string student_Id)
        {
            return ExecuteList(SCHEDULE, student_Id);
        }

        /// <summary>
        /// 수업 정보를 가져오는 함수입니다.
        /// </summary>
        /// <param name="Lecture_code"></param>
        /// <returns></returns>
        public Lecture GetLecture(string Lecture_code)
        {
            Lecture lecture = null;
            string code, pro_id, name, week_day, start_time, end_time;
            int credit;

            using (data = Select("*", "lecture", $"lecture_code = '{Lecture_code}'"))
            {
                DataRow[] row = data.Tables[0].Select();

                code = row[0].ItemArray[0].ToString();
                pro_id = row[0].ItemArray[1].ToString();
                name = row[0].ItemArray[2].ToString();
                credit = int.Parse(row[0].ItemArray[3].ToString());
                week_day = row[0].ItemArray[4].ToString();
                start_time = row[0].ItemArray[5].ToString();
                end_time = row[0].ItemArray[6].ToString();


                lecture = new Lecture(code, pro_id, name, credit, week_day, start_time, end_time);
            }
            return lecture;
        }

        /// <summary>
        /// 특정 학생의 정보를 가져오는 함수입니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Student GetStudent(string id)
        {
            Student student;
            string Id, Pw, student_id, name;

            using (data = Select("*", "student", $"id = '{id}'"))
            {
                DataRow[] row = data.Tables[0].Select();

                Id = row[0].ItemArray[0].ToString();
                Pw = row[0].ItemArray[1].ToString();
                student_id = row[0].ItemArray[2].ToString();
                name = row[0].ItemArray[3].ToString();

                student = new Student(Id, Pw, student_id, name);
            }

            return student;
        }

        /// <summary>
        /// 특정 교수의 정보를 가져오는 함수입니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Professor GetProfessor(string id)
        {
            Professor professor;
            string Id, Pw, professor_id, name;

            using (data = Select("*", "professor", $"id = '{id}'"))
            {
                DataRow[] row = data.Tables[0].Select();

                Id = row[0].ItemArray[0].ToString();
                Pw = row[0].ItemArray[1].ToString();
                professor_id = row[0].ItemArray[2].ToString();
                name = row[0].ItemArray[3].ToString();

                professor = new Professor(Id, Pw, professor_id, name);
            }

            return professor;
        }

        /// <summary>
        /// 특정 강의의 출석표를 생성합니다.
        /// </summary>
        /// <param name="Student_Id"></param>
        /// <param name="Lecture_code"></param>
        /// <param name="Week_Code"></param>
        public void PR_Attendance(string Student_Id, string Lecture_code, int Week_Code)
        {
            if (!attFlag)
            {
                string query = $@"
                            insert into attendance_mark
                            values(att_seq.nextval, '{Student_Id}', {Week_Code}, 0, 0, 0);
                    ";

                Execute(query);
                attFlag = true;
            }
        }

        /// <summary>
        /// 학생이 특정 강의를 출석하는 함수입니다. <br/>
        /// </summary>
        /// <param name="Student_Id"></param>
        /// <param name="Lecture_code"></param>
        /// <param name="Week_Code"></param>
        /// <param name="Class"></param>
        /// <param name="Att"></param>
        public void ST_Attendance(string Student_Id, string Lecture_code, int Week_Code, int Class, int Att)
        {
            string query;
            string strClass = "";
            
            switch(Class)
            {
                case 1:
                    strClass = "first_class";
                    break;
                case 2:
                    strClass = "second_class";
                    break;
                case 3:
                    strClass = "third_class";
                    break;
                default:
                    Console.WriteLine("값이 올바르지 않습니다.");
                    return;
            }
            query = $@"
                        update attendance_mark
                        set {strClass} = {Att}
                        where student_Id = {Student_Id}
                        and Lecture_code = {Lecture_code}
                        and week_code = {Week_Code}
                        ";

            Execute(query);
        }
    }
}
