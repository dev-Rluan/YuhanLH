using PClient;
using System;
using System.Drawing;
using System.Windows.Forms;

class PacketHandler
{

    public static void SP_ResultHandler(PacketSession session, IPacket packet)
    {
        SP_Result result = packet as SP_Result;
        ServerSession serverSession = session as ServerSession;
        if (result.result == true)
        {
            if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
            {
                PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    PClientForm.pclientform.lbLogin.Text = "로그인 : success";
                });
            }
            else
            {
                PClientForm.pclientform.lbLogin.Text = "로그인 : success";
            }
        }
        else
        {
            if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
            {
                PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    PClientForm.pclientform.lbLogin.Text = "로그인 : Faild";
                });
            }
            else
            {
                PClientForm.pclientform.lbLogin.Text = "로그인 : Faild";
            }
        }
    }
    
    public static void SP_LoginFailedHandler(PacketSession session, IPacket packet)
    {
        SP_LoginFailed pkt = packet as SP_LoginFailed;
        if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
        {
            PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
            {
                PClientForm.pclientform.lbLogin.Text = "로그인 : Fail";
            });
        }
        else
        {
            PClientForm.pclientform.lbLogin.Text = "로그인 : Fail";
        }

    }     
    public static void SP_LoginResultHandler(PacketSession session, IPacket packet)
    {
        SP_LoginResult pkt = packet as SP_LoginResult;
        ServerSession serverSession = session as ServerSession; 


        if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
        {
            PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
            {
                PClientForm.pclientform.lbLogin.Text = "로그인 : success";
            });
        }
        else
        {
            PClientForm.pclientform.lbLogin.Text = "로그인 : success";
        }

        foreach (SP_LoginResult.Student s in pkt.students)
        {
            if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
            {
                PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    PClientForm.pclientform.txtBox.Text += s.studentId + ", " + s.studentName + " ";
                });
            }
            else
            {
                PClientForm.pclientform.txtBox.Text += s.studentId + ", " + s.studentName + " ";
            }
        }
        CP_StudentList student_pkt = new CP_StudentList();
        serverSession.Send(student_pkt.Write());

    }
    public static void SP_StudentInfoHandler(PacketSession session, IPacket packet)
    {
        SP_StudentInfo pkt = packet as SP_StudentInfo;
        foreach (SP_StudentInfo.Student s in pkt.students)
        {
            if (PClientForm.pclientform.lbLogin.InvokeRequired == true)
            {
                PClientForm.pclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    PClientForm.pclientform.txtBox.Text += s.studentId + ", " + s.studentId + " 0 ";
                });
            }
            else
            {
                PClientForm.pclientform.txtBox.Text += s.studentId + ", " + s.studentId + " 0 ";
            }
        }
    }

    public static void SP_ScreenResultHandler(PacketSession session, IPacket packet)
    {
        SP_ScreenResult sp_screenPacket = packet as SP_ScreenResult;
        ServerSession serverSession = session as ServerSession;
        Bitmap bmp;
        bmp = ScreenCopy.GetBitmap(sp_screenPacket.img);


        PClientForm.pclientform.lbImg.Invoke((MethodInvoker)delegate
        {
            PClientForm.pclientform.lbImg.Text = "\n 성공";
        });
        if (PClientForm.pclientform.txtBox.InvokeRequired == true)
        {
            PClientForm.pclientform.txtBox.Invoke((MethodInvoker)delegate
            {
                PClientForm.pclientform.txtBox.Text += sp_screenPacket.studentId;
                PClientForm.pclientform.txtBox.Text += "\n 성공";
            });
        }
        else
        {
            PClientForm.pclientform.txtBox.Text = sp_screenPacket.studentId;
        }

        if (PClientForm.pclientform.ptBox.InvokeRequired == true)
        {
            PClientForm.pclientform.ptBox.Invoke((MethodInvoker)delegate
            {
                PClientForm.pclientform.ptBox.Image = bmp;
            });
        }
        else
        {
            PClientForm.pclientform.ptBox.Image = bmp;
        }
    }
    public static void SP_QustionTextHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QustionImgHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QustionHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QuizResultHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_EndClassHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_QuizOXResultHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_AddStudentHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_LeaveStudentHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SP_AddAtdHandler(PacketSession session, IPacket packet)
        {

        }
    

}