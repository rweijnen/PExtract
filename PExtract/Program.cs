using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using HarmonyLib; // NuGet Lib.Harmony

namespace PExtract
{
    class Patch
    {
        static void PreExecuteHook(string command, string[] args)
        {
            File.WriteAllText("Pre.ps1", command);
            Console.WriteLine(command);
        }
        static void PostExecuteHook(string command, string[] args)
        {
            File.WriteAllText("Post.ps1", command);
            Console.WriteLine(command);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"c:\temp\Harmony\Test13.exe";
            Assembly asm = Assembly.LoadFrom(filePath);
            if (null == asm)
            {
                return;
            }

            Harmony harmony = new Harmony("patch");
            Type VBSPoSH = asm.GetType("PoshExeHostCmd.VBSPoSH");
            if (null == VBSPoSH)
            {
                return;
            }
            var original = VBSPoSH.GetMethod("Execute", BindingFlags.Public | BindingFlags.Instance);
            Type me = typeof(PExtract.Patch);
            var prefix = me.GetMethod("PreExecuteHook", BindingFlags.NonPublic | BindingFlags.Static);
            var postfix = me.GetMethod("PostExecuteHook", BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            MethodInfo target = asm.EntryPoint;
            var commandArgs = new string[0];
            target.Invoke(null, new object[] { commandArgs });
        }
    }
}
