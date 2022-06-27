using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DectalkNET;

namespace ExampleTest
{
    class Program
    {

        static void Main(string[] args)
        {
            Dectalk.Startup(0);
            //Dectalk.Startup(false);
            //Dectalk.WaveOut(Console.ReadLine(), "./output.wav");

            ConsoleKeyInfo cki = new ConsoleKeyInfo();
            string outp = "";

            while (cki.Key != ConsoleKey.Escape)
            {
                cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.Enter:
                        Console.Write("\r\n");
                        process(outp);
                        outp = string.Empty;
                        break;
                    case ConsoleKey.Backspace:
                        if (outp.Length > 0)
                        {
                            outp = outp.Remove(outp.Length - 1);
                            Console.Write("\b \b");
                        }
                        break;
                    default:
                        outp += cki.KeyChar;
                        Console.Write(cki.KeyChar);
                        break;
                }
            }

            Dectalk.Shutdown();
        }

        static void process(string output)
        {
            if (output.StartsWith("vol:")) Dectalk.SetVolume(int.Parse(output.Substring(4)));
            else if (output.StartsWith("log:"))
            {
                Dectalk.Say($"[:log phoneme on]{output.Substring(4)}[:log phoneme off]");
                Dectalk.WaitForSpeech();
                string temp = File.ReadAllText("log.txt");
                Console.WriteLine(temp);
                File.Delete("log.txt");
            }
            else
            {
                Dectalk.Say(output);
                //Dectalk.WaveOut(output, "./output.wav");
                Dectalk.WaitForSpeech();
            }
        }
    }
}
