using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Philosophical_Artificial_Intelligence
{
    public enum ResultType
    {
        NONE, INFORMATION, ERROR
    }

    public struct CommandResult
    {
        public string Result;
        public ResultType ResultTypes;
    }

    public class Command
    {
        private Dictionary<string, MethodInfo> commandList = new Dictionary<string, MethodInfo>();

        public CommandResult Submit(string command, string args)
        {
            if (command.StartsWith("?"))
            {
                if (!commandList.ContainsKey(command.Substring(1)))
                    return new CommandResult() { ResultTypes = ResultType.NONE };
                try
                {
                    MethodInfo m = commandList[command.Substring(1)];
                    Attribute[] attrs = m.GetCustomAttributes(Type.GetType("Philosophical_Artificial_Intelligence.CMD"), false) as Attribute[];
                    if (attrs == null || attrs.Length == 0) return new CommandResult() { ResultTypes = ResultType.NONE };
                    CMD info = new CMD();

                    for (int i = 0; i < attrs.Length; i++ )
                    {
                        if(attrs[i] is CMD)
                        {
                            info = attrs[i] as CMD;
                        }
                    }

                    return new CommandResult() { ResultTypes = ResultType.INFORMATION, Result = info.Help + "\r\n" };
                }
                catch (MissingMethodException)
                {
                    return new CommandResult() { ResultTypes = ResultType.ERROR, Result = "ERROR : Wrong Method\r\n" };
                }
                catch (TargetParameterCountException)
                {
                    return new CommandResult() { ResultTypes = ResultType.ERROR, Result = "ERROR : Wrong Parameters\r\n" };
                }
            }
            else
            {
                if (!commandList.ContainsKey(command))
                    return new CommandResult() { ResultTypes = ResultType.NONE };
                try
                {
                    MethodInfo m = commandList[command];
                    ParameterInfo[] infos = m.GetParameters();
                    
                    object parameters = Activator.CreateInstance(Type.GetType("Philosophical_Artificial_Intelligence.Command"));

                    return string.IsNullOrWhiteSpace(args) ? (CommandResult)(m.Invoke(parameters, new object[] {})) : (CommandResult)(m.Invoke(parameters, new object[] { args }));
                }
                catch (MissingMethodException)
                {
                    return new CommandResult() { ResultTypes = ResultType.ERROR, Result = "ERROR : Wrong Method\r\n" };
                }
                catch (TargetParameterCountException)
                {
                    return new CommandResult() { ResultTypes = ResultType.ERROR, Result = "ERROR : Wrong Parameters\r\n" };
                }
            }
        }

        [CMD("Philosophical_Artificial_Intelligence.Command", "doThis", ">test\r\nTESTING! TESTING!", "test")]
        public CommandResult doThis()
        {
            return new CommandResult() { Result = "TESTING!\r\n", ResultTypes = ResultType.NONE };
        }

        [CMD("Philosophical_Artificial_Intelligence.Command", "Exit", ">exit\r\nClose application.", "exit")]
        public CommandResult Exit()
        {
            System.Windows.Forms.Application.Exit();
            return new CommandResult() { Result = "Application Exited.\r\n", ResultTypes = ResultType.NONE };
        }
        [CMD("Philosophical_Artificial_Intelligence.Command", "CMD", ">cmd 'command' 'arguments' 'arguments' ...\r\nRun command prompt commands.", "cmd")]
        public CommandResult CMD(string commands)
        {

            ProcessStartInfo cmd = new ProcessStartInfo();
            Process process = new Process();
            cmd.FileName = "cmd";
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.UseShellExecute = false;
            cmd.RedirectStandardError = true;
            cmd.RedirectStandardInput = true;
            cmd.RedirectStandardOutput = true;
            cmd.WorkingDirectory = @"C:\";

            process.EnableRaisingEvents = false;
            process.StartInfo = cmd;
            process.Start();
            process.StandardInput.Write(commands + Environment.NewLine);
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd();
            CommandResult cr = new CommandResult() { Result = result + "\r\n", ResultTypes = ResultType.NONE };
            process.WaitForExit();
            process.Close();

            return cr;
        }

        [CMD("Philosophical_Artificial_Intelligence.Command", "Start", ">start 'path' 'argument'\r\nStart file, or folder with new process.", "start")]
        public CommandResult Start(string pathArgs)
        {
            string[] lolthisshit = pathArgs.Split(' ');
            string path = "", argc = "";

            path = lolthisshit[0];
            argc = lolthisshit.Length < 2 ? "" : lolthisshit[1];
            try
            {
                if (string.IsNullOrEmpty(argc))
                {
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    System.Diagnostics.Process.Start(path, argc);
                }
                return new CommandResult() { Result = "Successfully runned " + path + "\r\n", ResultTypes = ResultType.NONE };
            }
            catch(System.ComponentModel.Win32Exception)
            {
                return new CommandResult() { Result = "ERROR : File or folder doesn't exist\r\n", ResultTypes = ResultType.ERROR };
            }
        }

        public Command()
        {
            MethodInfo[] TheMethods = Type.GetType("Philosophical_Artificial_Intelligence.Command").GetMethods();
            for (int i = 0; i < TheMethods.Length; i++)
            {
                Attribute[] attrs = TheMethods[i].GetCustomAttributes(Type.GetType("Philosophical_Artificial_Intelligence.CMD"), false) as Attribute[];
                if (attrs == null || attrs.Length == 0) continue;
                foreach (Attribute attr in attrs)
                {
                    if(attr is CMD)
                    {
                        CMD c = attr as CMD;
                        this.commandList.Add(c.Command, c.Method);
                    }
                }
            }
        }

    }
}