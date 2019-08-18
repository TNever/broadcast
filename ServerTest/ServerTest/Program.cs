using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // 调用构造函数，使用Start方法
            ServerControl server = new ServerControl();
            server.Start();

            Console.ReadKey();
        }
    }
}
