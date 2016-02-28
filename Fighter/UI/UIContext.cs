using System;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;

namespace Fighter
{
    internal class UIContext
    {
        private Instance instance;

        // Main window
        internal Window _window;

        // Main window Thread
        private Thread windowThread;

        
        public UIContext(Instance instance) 
        {
            this.instance = instance; InitUI();
        }

        private void InitUI()
        {
            Application.EnableVisualStyles();

            _window = new Window(instance);
            _window.Load += Form_Load;
            _window.FormClosing += Form_Closing;
        }

        private void RunWindow()
        {
            Application.Run(_window);
        }

        public void LoadUI()
        {
            windowThread = new Thread(RunWindow);
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
        }

        public void UnloadUI()
        {
            try
            {
                if (_window != null && !_window.IsDisposed)
                {
                    _window.Invoke(new Action(() => _window.Close()));
                    _window.Invoke(new Action(() => _window.Dispose()));
                }
            }
            catch { }

            try
            {
                if (windowThread != null && windowThread.ThreadState == ThreadState.Running)
                {
                    windowThread.Abort();
                    windowThread.Join();
                }
            }
            catch { }
        }


        private void Form_Load(object sender, EventArgs e)
        {
            _window.FormLoaded();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            // Form closing
        }
    }
}
