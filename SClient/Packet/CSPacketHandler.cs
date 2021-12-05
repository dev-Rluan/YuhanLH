using SClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

class PacketHandler
{

    public static void SS_ResultHandler(PacketSession session, IPacket packet)
    {
        SS_Result result = packet as SS_Result;
        ServerSession serverSession = session as ServerSession;
        if (result.result == true)
        {
            if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
            {
                SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    SClientForm.sclientform.lbLogin.Text = "로그인 : success";
                });
            }
            else
            {
                SClientForm.sclientform.lbLogin.Text = "로그인 : success";
            }
        }
        else
        {
            if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
            {
                SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    SClientForm.sclientform.lbLogin.Text = "로그인 : Faild";
                });
            }
            else
            {
                SClientForm.sclientform.lbLogin.Text = "로그인 : Faild";
            }
        }
    }
    public static void SS_LoginFailedHandler(PacketSession session, IPacket packet)
    {
        SS_LoginFailed pkt = packet as SS_LoginFailed;
        if (pkt.result == 1)
        {
            if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
            {
                SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    SClientForm.sclientform.lbLogin.Text = "로그인 : 실패";
                });
            }
            else
            {
                SClientForm.sclientform.lbLogin.Text = "로그인 : 실패";
            }
        }
        else
        {
            if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
            {
                SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    SClientForm.sclientform.lbLogin.Text = "로그인 : 실패";
                });
            }
            else
            {
                SClientForm.sclientform.lbLogin.Text = "로그인 : 실패";
            }
        }
    }     
    public static void SS_LoginResultHandler(PacketSession session, IPacket packet)
    {
        SS_LoginResult loginResult_packet = packet as SS_LoginResult;
        List<SS_LoginResult.Lecture> LectureResult = loginResult_packet.lectures;
        string nowtime = "1205";
        foreach(SS_LoginResult.Lecture result in LectureResult)
        {
            if (Convert.ToInt32(result.strat_time) <= Convert.ToInt32(nowtime) && Convert.ToInt32(result.end_time) >= Convert.ToInt32(nowtime))
            {

                if (result.weekday == "수")
                {

                   //현재 수업 시간

                }
            }
        }
        
        if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
            {
                SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
                {
                    SClientForm.sclientform.lbLogin.Text = "로그인 : success";
                });
            }
            else
            {
                SClientForm.sclientform.lbLogin.Text = "로그인 : success";
            }
        
        
    }
    public static void SS_EnterRoomHandler(PacketSession session, IPacket packet)
    {
        Thread.Sleep(1000);
        SClientForm.sessionManager.LoginSend();
    }
    public static void SS_QResultHandler(PacketSession session, IPacket packet)
        {
            
        }     
        public static void SS_AtdRequestHandler(PacketSession session, IPacket packet)
        {

        }     
    public static void SS_QuizOXHandler(PacketSession session, IPacket packet)
    {
        SS_QuizOX pkt = packet as SS_QuizOX;
        if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
        {
            SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
            {
                SClientForm.sclientform.textBox1.Text += pkt.quiz;
            });
        }
        else
        {
            SClientForm.sclientform.textBox1.Text += pkt.quiz;
        }
    }     
        public static void SS_QuizHandler(PacketSession session, IPacket packet)
        {
        SS_Quiz pkt = packet as SS_Quiz;
        if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
        {
            SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
            {
                SClientForm.sclientform.textBox1.Text += pkt.quiz;
            });
        }
        else
        {
            SClientForm.sclientform.textBox1.Text += pkt.quiz;
        }
    }
        public static void SS_ImgSendFaildHandler(PacketSession session, IPacket packet)
        {

        }
    public static void SS_ScreenRequestHandler(PacketSession session, IPacket packet)
    {
         
        if (SClientForm.sclientform.lbLogin.InvokeRequired == true)
        {
            SClientForm.sclientform.lbLogin.Invoke((MethodInvoker)delegate
            {
                SClientForm.sclientform.textBox1.Text += "스크린샷 요청";
            });
        }
        else
        {
            SClientForm.sclientform.textBox1.Text += "스크린샷 요청";
        }
        SClientForm.sessionManager.ImgSend();


    }
    public static void SS_LogoutHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SS_EndOfClassHandler(PacketSession session, IPacket packet)
    {

    }
    public static void SS_QustionFaildHandler(PacketSession session, IPacket packet)
    {
        
    }

}