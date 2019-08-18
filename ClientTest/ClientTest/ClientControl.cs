using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientTest
{
    public class ClientControl
    {
        // 声明变量
        private Socket clientSocket;

        // 自定义有参构造方法（（IP地址，流程传输方式，TCP协议））
        public ClientControl()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }

        // 创建通过IP与端口号连接的方法
        public void Connect(string ip,int port)
        {
            clientSocket.Connect(ip, port);
            Console.WriteLine("连接服务器成功");

            // 客户端接收服务器消息的线程
            Thread threadReceive = new Thread(Receive);
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }

        // 用于测试服务器向客户端返回一条消息
        private void Receive()
        {
            while(true)
            {
                try
                {
                    // 用于接收服务器的回复信息
                    byte[] msg = new byte[1024];
                    int msgLen = clientSocket.Receive(msg);
                    Console.WriteLine("服务器："+Encoding.UTF8.GetString(msg,0,msgLen));
                }
                // 异常处理方法
                catch
                {
                    Console.WriteLine("服务器积极拒绝！！");
                    // 退出while循环
                    break;
                }
            }
        }

        // Send方法测试：即发送消息，以字节为单位
        public void Send()
        {
            Thread threadSend = new Thread(ReadAndSend);
            // 将该线程设为非后台线程。
            // threadSend.IsBackground = true;
            threadSend.Start();
        }

        private void ReadAndSend()
        {
            // 提示操作方法
            Console.WriteLine("请输入发送至服务器的内容或者输入quit退出");
            // 输入内容
            string msg = Console.ReadLine();
            // 非退出情况下操作方式，使用while可以持续不断的接收用户输入
            while (msg != "quit")
            {
                clientSocket.Send(Encoding.UTF8.GetBytes(msg));
                msg = Console.ReadLine();
            }
        }
    }
}
