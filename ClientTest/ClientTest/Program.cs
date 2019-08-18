using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // 调用构造函数
            ClientControl client = new ClientControl();
            // 输入本机IP与端口号
            client.Connect("127.0.0.1", 12345);
            // 启动send方法
            client.Send();

            Console.ReadKey();
        }
    }
}
