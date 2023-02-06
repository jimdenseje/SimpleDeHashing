using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptSharp;
using CryptSharp.Utility;
using UniqueKey;

namespace SimpleDeHashing
{
    static class Program
    {
        static void Main()
        {

            string ipOrUrl = EasyCommandLine.PromtString("IP or URL of redis server");
            if (ipOrUrl.Trim() == "")
            {
                ipOrUrl = "localhost";
            }
            RedisConnectorHelper.Connect(ipOrUrl);

            while (true)
            {

                int threads = EasyCommandLine.PromtInt("Number of Threads");
                int chars = EasyCommandLine.PromtInt("Number of Chars");

                Console.WriteLine("\nPress 1 for Client");
                Console.WriteLine("Press 2 for Server\n");

                int press = EasyCommandLine.PromtInt("Input", new List<int> {1,2});
                if (press == 1)
                {
                    Console.WriteLine();
                    startClient(threads);
                } else
                {
                    Console.WriteLine();

                    string passwordHash = EasyCommandLine.PromtString("passwordHash");
                    if (passwordHash.Trim() == "")
                    {
                        passwordHash = "$2y$10$Ot4wqtcJQ0VKQjWmJ/8.7.2rjkqIooRa1clgcra5ExBn4DjWbGryu";
                    }
                    //a check whatever it's a valid hash could be a good idea

                    Console.WriteLine();
                    startServer(passwordHash, threads, chars);
                }

                Thread.Sleep(2000);

                Console.WriteLine();
                Console.WriteLine("".PadLeft(Console.WindowWidth, '#'));
                Console.WriteLine();
                Console.WriteLine("RUN AGAIN?\n");
                EasyCommandLine.scrollDown(10);

            }
            //EncodeDecodeExcample();
        }

        static void startServer(string passwordHash, int threads, int chars)
        {
            var cache = RedisConnectorHelper.Connection().GetDatabase();
            cache.StringSet("chars", chars);

            //save passwordHash
            cache.StringSet("passwordHash", passwordHash);

            // cache.SetLength("mylist") how to get total number of items in set?

            /*
            for (long x = 0; x < 99999; x++)
            {
                Console.WriteLine("Old key: " + cache.SetPop("mylist"));
            }
            */

            //remove old CACHE
            cache.KeyDelete("mylist");
            cache.KeyDelete("foundpassword");
            cache.KeyDelete("trys");

            Hack(threads);

        }

        static void startClient(int threads)
        {

            Hack(threads);

        }

        static void Hack(int threads) {

            for (int x = 0; x < threads; x++)
            {
                Thread thread = new Thread(() => {

                    var cache = RedisConnectorHelper.Connection().GetDatabase();
                    string? crypt = cache.StringGet("passwordHash");
                    int chars = Convert.ToInt32(cache.StringGet("chars"));

                    int trys = 0;
                    while (true)
                    {
                        var rnd = new Random();
                        trys++;
                        string testPassword = KeyGenerator.GetUniqueKey(rnd.Next(1, chars));

                        while (cache.SetContains("mylist", testPassword))
                        {
                            testPassword = KeyGenerator.GetUniqueKey(rnd.Next(1, chars));
                        }

                        cache.SetAdd("mylist", testPassword);
                        if (Crypter.CheckPassword(testPassword, crypt))
                        {
                            cache.StringSet("foundpassword", testPassword);
                            break;
                        }

                        if (trys == 1)
                        {
                            trys = 0;
                            if (String.IsNullOrEmpty(cache.StringGet("foundpassword")) == false)
                            {
                                break;
                            }
                        }

                        Console.WriteLine("Try " + ("" + cache.StringIncrement("trys")).PadLeft(20) + " test password: " + testPassword);

                    }

                });
                thread.Start();
            }

            var cache = RedisConnectorHelper.Connection().GetDatabase();

            while (String.IsNullOrEmpty(cache.StringGet("foundpassword")))
            {
                Thread.Sleep(2000);
            }

            Console.WriteLine();
            Console.WriteLine("Found Password: " + cache.StringGet("foundpassword"));
            Console.WriteLine("Number of trys: " + cache.StringGet("trys"));

        }

        static void EncodeDecodeExcample()
        {

            CrypterOptions options = new();
            options.Add(CrypterOption.Rounds, 10);
            options.Add(CrypterOption.Variant, BlowfishCrypterVariant.Corrected);
            string crypt = Crypter.Blowfish.Crypt("hej", options);

            Console.WriteLine("Is Password the same as hash?");

            string password = EasyCommandLine.PromtString("Passowrd");

            bool matches = Crypter.CheckPassword(password, crypt);

            if (matches)
            {
                Console.WriteLine("Password matches");
            }
            else
            {
                Console.WriteLine("Wrong Password");
            }

        }
    }
}