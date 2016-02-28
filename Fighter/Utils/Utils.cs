using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Fighter
{
    internal class Utils
    {
        public static void InvokeOn(Control ctl, Action action)
        {
            if (ctl != null)
            {
                if (ctl.InvokeRequired)
                {
                    ctl.Invoke(new Action(() => InvokeOn(ctl, action)));
                }
                else action();
            }
        }

        public static string GenerateMD5Hash(string value)
        {
            MD5 md5 = MD5.Create();
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] hash = md5.ComputeHash(input);

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }



        public static void Delay(int ms, CancellationToken token)
        {
            Task.Delay(ms, token).Wait();
        }

        public static void Delay(int min, int max, CancellationToken token)
        {
            Task.Delay(RandomNum(min, max), token).Wait();
        }

        public static void Delay(int[] setA, int[] setB, CancellationToken token)
        {
            switch (RandomNum(0, 2))
            {
                case 0:
                    Delay(setA[0], setA[1], token);
                    break;

                case 1:
                    Delay(setB[0], setB[1], token);
                    break;
            }
        }

        public static void Delay(int[] setA, int[] setB, int[] setC, CancellationToken token)
        {
            switch(RandomNum(0, 3))
            {
                case 0:
                    Delay(setA[0], setA[1], token);
                    break;

                case 1:
                    Delay(setB[0], setB[1], token);
                    break;

                case 2:
                    Delay(setC[0], setC[1], token);
                    break;
            }
        }



        public static void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        public static void Sleep(int minms, int maxms)
        {
            Random rand = new Random();
            int ms = rand.Next(minms, maxms);

            Thread.Sleep(ms);
        }



        public static int RandomNum(int min, int max)
        {
            Random rand = new Random();
            return rand.Next(min, max);
        }

        public static int RandomNum(int[] setA, int[] setB)
        {
            switch (RandomNum(0, 2))
            {
                case 0:
                    return RandomNum(setA[0], setA[1]);

                case 1:
                    return RandomNum(setB[0], setB[1]);

                default:
                    return 0;
            }
        }

        public static int RandomNum(int[] setA, int[] setB, int[] setC)
        {
            switch (RandomNum(0, 3))
            {
                case 0:
                    return RandomNum(setA[0], setA[1]);

                case 1:
                    return RandomNum(setB[0], setB[1]);

                case 2:
                    return RandomNum(setC[0], setC[1]);

                default:
                    return 0;
            }
        }

        public static double RandomDouble(double min, double max)
        {
            Random rand = new Random();
            return rand.NextDouble() * (min - max) + max;
        }
        


        public static T[] RandomPermutation<T>(T[] array)
        {
            T[] retArray = new T[array.Length];
            array.CopyTo(retArray, 0);

            Random random = new Random();
            for (int i = 0; i < array.Length; i += 1)
            {
                int swapIndex = random.Next(i, array.Length);
                if (swapIndex != i)
                {
                    T temp = retArray[i];
                    retArray[i] = retArray[swapIndex];
                    retArray[swapIndex] = temp;
                }
            }

            return retArray;
        }



        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;

                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }

        internal static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ResolveAssembly(args.Name);
        }

        private static List<string> externalLibs = new List<string>() { }; // Library names
        private static List<byte[]> resourceLibs = new List<byte[]>() { }; // Library as resource

        private static Assembly ResolveAssembly(string name)
        {
            string resolve = name.Split(',')[0];

            if (externalLibs.Contains(resolve))
            {
                string lib = AssemblyDirectory + Path.DirectorySeparatorChar + resolve + ".dll";

                if (!File.Exists(lib))
                {
                    string dir = Path.GetDirectoryName(lib);

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    using (MemoryStream resource = new MemoryStream(resourceLibs[externalLibs.IndexOf(resolve)]))
                    {
                        using (var gz = new GZipStream(resource, CompressionMode.Decompress))
                        {
                            using (MemoryStream extract = new MemoryStream())
                            {
                                byte[] tmp = new byte[1024 * 256];

                                int r = 0;
                                while ((r = gz.Read(tmp, 0, tmp.Length)) > 0)
                                {
                                    extract.Write(tmp, 0, r);
                                }

                                File.WriteAllBytes(lib, extract.ToArray());
                            }
                        }
                    }
                }

                return Assembly.LoadFile(lib);
            }
            return null;
        }
    }
}
