using System;
using System.IO;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Sandbox;
using AudioSwitcher.Scripting.JavaScript;
using AudioSwitcher.Scripting.JavaScript.Internal;

namespace AudioSwitcher.CLI
{
    internal static class Program
    {
        private static bool _isDebug;

        private static int Main(string[] args)
        {
            if (args.Length > 2 || args.Length < 1)
                return PrintUsage();

            //Process CLI Arguments
            for (var i = 0; i < args.Length - 1; i++)
            {
                switch (args[i])
                {
                    case "--debug":
                        _isDebug = true;
                        break;
                }
            }

            //Process file name
            var fName = args[args.Length - 1];
            if (!fName.EndsWith(".js"))
            {
                Console.WriteLine("Invalid input file");
                Console.WriteLine();
                return PrintUsage();
            }

            IAudioController controller;

            if (_isDebug)
                controller = new SandboxAudioController(new CoreAudioController());
            else
                controller = new CoreAudioController();

            var eng = new JsEngine();
            using (var engine = eng.CreateExecutionContext())
            {

                engine.AddCoreLibrary();
                engine.AddAudioSwitcherLibrary(controller);

                //Enable to log to CLI
                //engine.SetGlobalValue("console", new ConsoleOutput(engine));
                //engine.InternalEngine.SetGlobalValue("console", new FirebugConsole(engine.InternalEngine));
                engine.SetOutput(new ConsoleScriptOutput());

                try
                {
                    using (var file = File.OpenText(fName))
                    {
                        Console.WriteLine("Executing {0}...", fName);
                        var result = engine.Execute(file.ReadToEnd());
                        if (!result.Success)
                            throw result.ExecutionException;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

#if DEBUG
            Console.ReadKey();
#endif

            return 0;
        }

        private static int PrintUsage()
        {
            Console.WriteLine("-----  USAGE -----");
            Console.WriteLine();
            Console.WriteLine("ascli.exe [options] inputFile");
            Console.WriteLine();
            Console.WriteLine("InputFile:");
            Console.WriteLine("Must be a valid JavaScript file, and end with .js");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("--debug (optional) - Forces Debug Audio Context. Scripts will not affect actual System Devices.");
            Console.WriteLine();
            Console.WriteLine("-----  USAGE END -----");

            return -1;
        }
    }
}