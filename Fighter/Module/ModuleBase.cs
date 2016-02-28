using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;


namespace Fighter
{
    internal class ModuleBase
    {
        private Host host;
        private Instance instance;

        public ModuleBase(Instance instance)
        {
            this.instance = instance; host = instance.host;
        }

        private Window UI
        {
            get
            {
                return instance.uiContext._window;
            }
        }



        private Settings prefs;

        private CancellationTokenSource ts;
        private CancellationToken token;

        // Modules
        private CombatModule combatModule;
        private PartyModule partyModule;


        // Main loop
        private void Pulse()
        {
            while(!token.IsCancellationRequested)
            {
                IsCancelTask();
                combatModule.DoRoutine(); Utils.Delay(20, token);
            }
        }

        private void Initialize()
        {
            host.ClearLogs();

            ts = new CancellationTokenSource();
            token = ts.Token;


            prefs = UI.FetchSettings();
            Serializer.Save(prefs, Paths.SettingsFolder + host.serverName() + "[" + host.accountId + "].xml");


            // Instance modules
            combatModule = new CombatModule(host, prefs, token, UI);

            partyModule = new PartyModule();
        }

        public void Start()
        {
            Initialize();

            Task.Run(() => Pulse(), token);

            UI.ButtonSwitch = true;
            UI.UpdateButtonState("Stop");
        }

        public void Stop()
        {
            ts.Cancel();
            CancelActions();

            UI.ButtonSwitch = false;
            UI.UpdateButtonState("Start");
        }

        public void CancelActions()
        {
            host.CancelMoveTo(); host.CancelSkill();
        }

        private void IsCancelTask()
        {
            token.ThrowIfCancellationRequested();
        }
    }
}
