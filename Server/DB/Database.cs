using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Server
{
    class Database : IDatabase
    {
        /*  
            readme.txt 를 꼭 읽어주세요!
        */
        private const int STUDENT = 0;
        private const int PROFESSOR = 1;
        private const int SCHEDULE = 2;


        private static string dbIp = "10.102.0.14";
        private static string dbName = "deskDB";
        private static string dbId = "C##capstone_admin";
        private static string dbPw = "yuhanunivcapstone1212";
        private static bool attFlag = false;
        private OracleConnection conn;
        private OracleCommand command;
        private OracleDataAdapter adapter;
        private DataSet data = null;


        public Database()
        {
            string strConn = string.Format($"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={dbIp})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={dbName})));User ID={dbId};Password={dbPw};Connection Timeout=30;");
            conn = new OracleConnection(strConn);
            try
            {
                conn.Open();
                Console.WriteLine("db연결됨");
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
                        return ds;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR : 데이터 베이스 연결이 실패했습니다.");
                }
            }
            return null;
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
            string query = @$"select {col}
                               from {table}
                               where {where}  ";

            //using (data = Select("s.id, s.pw, s.name, s.student_id", "student s, student_lecture sl", @$"s.student_id = sl.student_id
            //                                                                                                 sl.lecture_code = {Lecture_Code}"))

            using (ds = new DataSet())
            {
                if (IsOpen())
                {
                    try
                    {
                        using (adapter = new OracleDataAdapter(query, conn))
                        {
                            Console.WriteLine("SELECT......");
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                    catch (Exception e )
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                }
                else
                {
                    Console.WriteLine("ERROR : 데이터 베이스 연결이 실패했습니다.");
                }
            }
            return null;
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

            using (data = Select("*", "student", $"id = '{id}' order by student_id"))
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
        /// 특정 학생의 아이디를 가져오는 함수입니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetStudentID(string student_id)
        {

            string Id = null;

            using (data = Select("id", "student", $"student_id = '{student_id}'"))
            {
                DataRow[] row = data.Tables[0].Select();
                Id = row[0].ItemArray[0].ToString();
            }

            return Id;
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
        /// 특정 교수의 아이디를 가져오는 함수입니다.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetProfessorID(string professor_id)
        {
            
            string Id = null;

            using (data = Select("id", "professor", $"professor_id = '{professor_id}'"))
            {
                DataRow[] row = data.Tables[0].Select();
                Id = row[0].ItemArray[0].ToString();               
            }

            return Id;
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
                            values(att_seq.nextval, '{Student_Id}', '{Lecture_code}', {Week_Code}, 0, 0, 0)
                    ";

                Console.WriteLine(query);
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

            switch (Class)
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
                        where student_Id = '{Student_Id}'
                        and Lecture_code = '{Lecture_code}'
                        and week_code = {Week_Code}
                        ";

            Execute(query);
        }

        /// <summary>
        /// 셀렉트 조인
        /// </summary>
        /// <param name="where"></param>
        /// <param name="time"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private DataSet SelectInnerJoin(string where, string time, string day)
        {
            DataSet ds = new DataSet();
            string query = @$"select stu_lec.lecture_code, stu_lec.lecture_name
                                from student_lecture stu_lec
                                INNER JOIN lecture lec on (stu_lec.lecture_code = lec.lecture_code AND 
                                                            lec.start_time <= '{time}' AND 
                                                            lec.end_time >= '{time}' AND 
                                                            lec.week_day = '{day}')
                                where {where}";

            using (ds = new DataSet())
            {
                if (IsOpen())
                {
                    using (adapter = new OracleDataAdapter(query, conn))
                    {
                        adapter.Fill(ds);
                        return ds;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR : 데이터 베이스 연결이 실패했습니다.");
                }
            }
            return null;
        }

        /// <summary>
        /// 학생이 수강하고 있는 강의들 중 시작시간에 해당하는 강의를 가져오는 함수입니다. <br/>
        /// 시간은 "HHmm" 형식입니다.
        /// </summary>
        /// <param name="time"></param>
        public Schedule GetScheduleExistTime(string time, string studentID)
        {
            string lecture_code, lecture_name;
            Schedule schedule;

            //using (data = SelectInnerJoin($"stu_lec.student_id = {studentID}", time, getDay(DateTime.Now))
            using (data = SelectInnerJoin($"stu_lec.student_id = {studentID}", time, "수"))
            {
                if (data.Tables[0].Rows.Count == 0)
                {
                    Console.WriteLine("없음");
                    return null;
                }

                DataRow[] r = data.Tables[0].Select();
                lecture_code = r[0].ItemArray[0].ToString();
                lecture_name = r[0].ItemArray[1].ToString();
                schedule = new Schedule(lecture_code, lecture_name);
            }

            return schedule;
        }

        /// <summary>
        /// 특정 강의를 수강하고 있는 학생들의 리스트를 반환받는 함수입니다.
        /// </summary>
        /// <param name="Lecture_Code"></param>
        public List<Student> GetStudentsExistLecture(string Lecture_Code)
        {
            List<Student> Students = new List<Student>();
            Student student;
            string id, pw, name, studentId;

            using (data = Select("s.id, s.pw, s.name, s.student_id",
                                 "student s, student_lecture sl",
                                 @$"s.student_id = sl.student_id
                                 AND sl.lecture_code = '{Lecture_Code}'"))
            {
                foreach (DataRow r in data.Tables[0].Rows)
                {
                    id = r["id"].ToString();
                    pw = r["pw"].ToString();
                    name = r["name"].ToString();
                    studentId = r["student_id"].ToString();
                    student = new Student(id, pw, studentId, name);
                    Students.Add(student);
                }
            }

            return Students;
        }

        /// <summary>
        /// 교수가 강의하고 있는 강의목록을 반환받는 함수입니다.
        /// </summary>
        /// <param name="Professor_Id"></param>
        public List<Lecture> GetLectureExistProfessor(string Professor_Id)
        {
            List<Lecture> lectures = new List<Lecture>();
            Lecture lecture;

            using (data = Select("*", "Lecture", $"Professor_Id = '{Professor_Id}'"))
            {
                foreach (DataRow r in data.Tables[0].Rows)
                {
                    lecture = new Lecture(r["lecture_code"].ToString(),
                                            r["professor_id"].ToString(),
                                            r["lecture_name"].ToString(),
                                            int.Parse(r["credit"].ToString()),
                                            r["week_day"].ToString(),
                                            r["start_time"].ToString(),
                                            r["end_time"].ToString());
                    lectures.Add(lecture);
                }
            }

            return lectures;
        }
        

        /// <summary>
        /// 교수가 강의하고 있는 강의들 중 특정 시간의 강의를 반환받는 함수입니다.
        /// </summary>
        /// <param name="Professor_Id"></param>
        /// <param name="Start_Time"></param>
        public Lecture GetLectureExistProfessorTime(string Professor_Id, string Time )
        {
            Lecture lecture;

            using (data = Select("*", "Lecture", @$"Professor_Id = '{Professor_Id}' AND
                                                    Start_Time <= '{Time}' AND end_time >= '{Time}' AND
                                                    Week_Day = '수'"))
                //Week_Day = '{getDay(DateTime.Now)}'"))
            {
                DataRow[] row = data.Tables[0].Select();
                lecture = new Lecture(row[0].ItemArray[0].ToString(),
                                        row[0].ItemArray[1].ToString(),
                                        row[0].ItemArray[2].ToString(),
                                        int.Parse(row[0].ItemArray[3].ToString()),
                                        row[0].ItemArray[4].ToString(),
                                        row[0].ItemArray[5].ToString(),
                                        row[0].ItemArray[6].ToString());
                if (data.Tables[0].Rows.Count == 0)
                {
                    Console.WriteLine("없음");
                    return null;
                }
            }

            return lecture;
        }

        private string getDay(DateTime now)
        {
            string day;

            switch (now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    day = "월";
                    break;
                case DayOfWeek.Tuesday:
                    day = "화";
                    break;
                case DayOfWeek.Wednesday:
                    day = "수";
                    break;
                case DayOfWeek.Thursday:
                    day = "목";
                    break;
                case DayOfWeek.Friday:
                    day = "금";
                    break;
                case DayOfWeek.Saturday:
                    day = "토";
                    break;
                case DayOfWeek.Sunday:
                    day = "일";
                    break;
                default:
                    day = "일";
                    break;
            }

            return day;
        }

        /// <summary>
        /// id, pwd로 로그인 체크 _return = 0 : 성공, return = 1  : 비밀번호 불일치, return = 2 : 아이디가 존재하지 않음
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public int LoginReturn(string id, string pwd, int flag)
        {
            int _return = 0;
            string Pw;
            if (flag == 0)
            {
                using (data = Select("pw", "professor", $"id = '{id}'"))
                {
                    if (data.Tables[0].Rows.Count != 0)
                    {
                        DataRow[] row = data.Tables[0].Select();
                        Pw = row[0].ItemArray[0].ToString();
                        if (Pw == pwd)
                        {
                            _return = 0;
                        }
                        else
                        {
                            _return = 1;
                        }
                    }
                    else
                    {
                        _return = 2;
                    }

                }
            }
            else
            {
                using (data = Select("pw", "student", $"id = '{id}'"))
                {
                    if (data.Tables[0].Rows.Count != 0)
                    {
                        DataRow[] row = data.Tables[0].Select();
                        Pw = row[0].ItemArray[0].ToString();
                        if (Pw == pwd)
                        {
                            _return = 0;
                        }
                        else
                        {
                            _return = 1;
                        }
                    }
                    else
                    {
                        _return = 2;
                    }
                }
            }
            return _return;
        }

        /// <summary>
        /// 주차에 해당하는 강의의 출석부를 가져옵니다.
        /// </summary>
        /// <param name="Lecture_code"></param>
        /// <param name="Week_Code"></param>
        /// <returns></returns>
        public List<Attendance> GetAttendanceList(string Lecture_code, int Week_Code)
        {
            List<Attendance> attendances = new List<Attendance>();
            Attendance attendance;
            string student_Id;
            int Attendance_code, first_class, second_class, third_class;

            using (data = Select("*",
                                "Attendance_mark",
                                $@"lecture_code = '{Lecture_code}'
                                   AND Week_code = {Week_Code}"))
            {
                foreach (DataRow r in data.Tables[0].Rows)
                {
                    Attendance_code = int.Parse(r["attendance_code"].ToString());
                    student_Id = r["student_id"].ToString();
                    first_class = int.Parse(r["first_class"].ToString());
                    second_class = int.Parse(r["second_class"].ToString());
                    third_class = int.Parse(r["third_class"].ToString());
                    attendance = new Attendance(Attendance_code, student_Id, Lecture_code, Week_Code, first_class, second_class, third_class);
                    attendances.Add(attendance);
                }
            }

            return attendances;
        }
        /// <summary>
        /// 주차에 해당하는 강의의 출석부를 가져옵니다.
        /// </summary>
        /// <param name="Lecture_code"></param>
        /// <param name="Week_Code"></param>
        /// <returns></returns>
        public List<Attendance> GetAttendanceListAll(string Lecture_code)
        {
            List<Attendance> attendances = new List<Attendance>();
            Attendance attendance;
            string student_Id;
            int Attendance_code, first_class, second_class, third_class, week_code;

            using (data = Select("*",
                                "Attendance_mark",
                                $@"lecture_code = '{Lecture_code}'"))
            {
                foreach (DataRow r in data.Tables[0].Rows)
                {
                    Attendance_code = int.Parse(r["attendance_code"].ToString());
                    student_Id = r["student_id"].ToString();
                    first_class = int.Parse(r["first_class"].ToString());
                    second_class = int.Parse(r["second_class"].ToString());
                    third_class = int.Parse(r["third_class"].ToString());
                    week_code = int.Parse(r["week_code"].ToString());
                    attendance = new Attendance(Attendance_code, student_Id, Lecture_code, week_code, first_class, second_class, third_class);
                    attendances.Add(attendance);
                }
            }

            return attendances;
        }

        /// <summary>
        /// 출석부에 생성되어 있는 특정 강의의 출석부 중 제일 최근 주차가 몇주차인지 알려줍니다.<br>
        /// 해당 강의가 한번도 출석부를 생성하지 않았다면 0 을 반환합니다.
        /// </summary>
        /// <param name="Lecture_Code"></param>
        /// <returns></returns>
        public int GetAttrRecentWeekCode(string Lecture_Code)
        {
            int RecentWeekCode = 0;

            using (data = Select("max(Week_code)", "Attendance_mark", $"lecture_code = {Lecture_Code}'"))
            {
                if (data.Tables[0].Rows.Count != 0)
                {
                    DataRow[] row = data.Tables[0].Select();
                    RecentWeekCode = int.Parse(row[0].ItemArray[0].ToString());
                }
            }

            return RecentWeekCode;
        }
    }
}
