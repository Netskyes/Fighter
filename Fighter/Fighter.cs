using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;
using System.Windows.Forms;

namespace Fighter
{
    internal class Host : Core
    {
        internal Instance _instance;
        private bool RequestShutdown = false;


        private void ResourceCheck()
        {
            if(!Directory.Exists(Paths.Root))
            {
                Directory.CreateDirectory(Paths.Root);
            }


            var structure = new string[] { "Settings", @"Templates", "Logs", "Navigation" };

            foreach(var folder in structure)
            {
                if(!Directory.Exists(Paths.RootFolder + @folder))
                {
                    Directory.CreateDirectory(Paths.RootFolder + @folder);
                }
            }
        }

        private void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Utils.CurrentDomainAssemblyResolve);


            ResourceCheck();

            _instance = new Instance();
            _instance.host = this;
            _instance.uiContext = new UIContext(_instance);
            _instance.moduleBase = new ModuleBase(_instance);
        }

        

        public void PluginRun()
        {
            while (gameState != GameState.Ingame || gameState == GameState.LoadingGameWorld) Utils.Sleep(50);

            Initialize();

            
            _instance.uiContext.LoadUI();

            try
            {
                while (!RequestShutdown) Utils.Sleep(10);
            }
            catch(ThreadAbortException)
            {
                // Skip
            }
            finally
            {
                _instance.uiContext.UnloadUI();
            }

        }
        
        public void PluginStop()
        {
            _instance.moduleBase.Stop();
            RequestShutdown = true;
        }



        public static string GetPluginAuthor()
        {
            return "Aeon";
        }

        public static string GetPluginDescription()
        {
            return "Ultimate Fighter";
        }

        public static string GetPluginVersion()
        {
            return "1.0.0";
        }
    }
}
