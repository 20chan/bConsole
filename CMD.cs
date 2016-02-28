using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philosophical_Artificial_Intelligence
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CMD : Attribute
    {
        public CMD() { }
        public CMD(string className, string methodName, string helpMessage, string command)
        {
            this.Method = Type.GetType(className).GetMethod(methodName);
            this.Help = helpMessage;
            this.Command = command;
        }

        public string Command { get; set; }
        public string Help { get; set; }
        public System.Reflection.MethodInfo Method { get; set; }
    }
}
