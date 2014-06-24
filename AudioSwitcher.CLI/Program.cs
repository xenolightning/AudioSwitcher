using System;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Isolated;
using AudioSwitcher.Scripting;
using Jurassic;
using Jurassic.Library;

namespace AudioSwitcher.CLI
{
    internal class Program
    {

        public static bool IsDebug = false;

        private static int Main(string[] args)
        {
            if (args.Length > 2 || args.Length < 1)
                return PrintUsage();

            //Process CLI Arguments
            for (var i = 0; i < args.Length - 1; i++)
            {
                switch(args[i])
                {
                    case "--debug":
                        IsDebug = true;
                        break;
                }
            }

            //Process file name
            string fName = args[args.Length - 1];
            if (!fName.EndsWith(".js"))
            {
                Console.WriteLine("Invalid input file");
                Console.WriteLine();
                return PrintUsage();
            }

            Controller controller;

            if(IsDebug)
                controller = new IsolatedController();
            else
                controller = new CoreAudioController();

            var engine = new ScriptEngine();
            engine.AddCoreLibrary();
            engine.AddAudioSwitcherLibrary(controller);

            //Enable to log to CLI
            //engine.SetGlobalValue("console", new ConsoleOutput(engine));
            engine.SetGlobalValue("console", new FirebugConsole(engine));

            try
            {
                engine.Evaluate(new FileScriptSource(fName));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }

        private static int PrintUsage()
        {
            Console.WriteLine("-----  USAGE -----");
            Console.WriteLine();
            Console.WriteLine("asc.exe [options] inputFile");
            Console.WriteLine();
            Console.WriteLine("InputFile:");
            Console.WriteLine("Must be a valid javascript file, and end with .js");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("--debug (optional) - Forces Debug Audio Context. Scripts will not affect actual System Devices.");
            Console.WriteLine();
            Console.WriteLine("-----  USAGE END -----");

            return -1;
        }
    }
}