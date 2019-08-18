using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTest
{
    public class ServerControl
    {
        // 声明变量（使用Socket需using System.Net.Sockets;）
        private Socket serverSocket;
        // 声明一个集合
        private List<Socket> clientList;

        // 自定义有参构造函数，包含两个对象（IP地址，流程传输方式，TCP协议）
        public ServerControl()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new List<Socket>();
        }

        // 创建启动方法（IPEndPoint用于指定地址及端口初始化，需using System.Net;）
        public void Start()
        {
            // 服务器启动
            // 绑定IP地址（为任意IP）与端口（设置为12345）
            serverSocket.Bind(new IPEndPoint(IPAddress.Any,12345));
            serverSocket.Listen(10);
            Console.WriteLine("服务器启动成功");

            // 开启线程：目的实现服务器和客户端一对多连接
            Thread threadAccept = new Thread(Accept);
            threadAccept.IsBackground = true;
            threadAccept.Start();
        }
         // Accept方法测试：接收客户端连接
        private void Accept()
        {
            // 接收客户端方法，会挂起当前线程（.RemoteEndPoint表示远程地址）
            Socket client = serverSocket.Accept();
            IPEndPoint point = client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine(point.Address + "[" + point.Port + "] 连接成功！");
            clientList.Add(client);

            // 开启一个新线程线程，实现消息多次接收
            Thread threadReceive = new Thread(Receive);
            threadReceive.IsBackground = true;
            threadReceive.Start(client);

            // 尾递归
            Accept();
        }

        // Receive方法的使用测试
        // 接收客户端发送过来的消息，以字节为单位进行操作
        // 该方法会阻塞当前线程，所以适合开启新的线程使用该方法
        // Accept()中将Receive作为线程传递对象，所以要注意一点，使用线程传递对象只能是object类型的！！
        private void Receive(object obj)
        {
            // 将object类型强行转换成socket
            Socket client = obj as Socket;

            IPEndPoint point = client.RemoteEndPoint as IPEndPoint;

            // 此处的异常抛出主要针对客户端异常的问题
            // 比如，客户端关闭或者连接中断
            // 程序会停留在int msgLen = client.Receive(msg);这段代码，而导致无法继续往下走
            try
            {
                byte[] msg = new byte[1024];
                // 实际接收到字节数组长度，该方法会阻塞当前线程，即（client.Receive(msg)开始挂起）
                // 同时，这里还是尾递归挂起处
                int msgLen = client.Receive(msg);
                // 将msg装换成字符串
                string msgStr = point.Address + "[" + point.Port + "]:" + Encoding.UTF8.GetString(msg, 0, msgLen);
                Console.WriteLine(msgStr);

                // 调用广播函数
                Broadcast(client,msgStr);
                // 尾递归实现多条消息的接收；和while同理。
                Receive(client);
            }
            catch
            {
                Console.WriteLine(point.Address + "[" + point.Port + "]积极断开");
                // 若客户端中断，则将他在集合中删除
                clientList.Remove(client);
            }
        }

        private void Broadcast(Socket clientOther,string msg)
        {
            foreach(var client in clientList)
            {
                if(client == clientOther)
                {
                    // 不做任何响应
                }
                else
                {
                    client.Send(Encoding.UTF8.GetBytes(msg));
                }
            }
        }
    }
}
