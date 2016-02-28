using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArcheBuddy.Bot.Classes;
using ArcheBuddy.SQL;
using System.Diagnostics;

namespace Fighter
{
    internal class CombatModule : Extension
    {
        private Host host;
        private Settings prefs;
        private CancellationToken token;
        private Window UI;

        public CombatModule(Host host, Settings prefs, CancellationToken token, Window UI) : base(host)
        {
            this.host = host; this.prefs = prefs; this.token = token; this.UI = UI;

            Initialize();
        }

        

        private CombatState combatState = CombatState.Check;


        private CombatTemplate template;

        private Creature target;
        private int targetHash;

        private int sequence = 0;
        private List<string> rotation;

        private Swap temp;
        private bool swapActive = false;

        private List<int> toLoot = new List<int>();


        private void Initialize()
        {
            template = UI.FetchTemplate();
            rotation = template.skills.rotation;

            // Register events
            host.onLootAvailable += onLootAvailable;
        }




        private void CheckState()
        {
            combatState = CombatState.Search;
        }

        private void SearchState()
        {
            if(UnderAttack())
            {
                GetTarget(host.getAggroMobs());
            }
            else GetTarget();


            if (target != null)
            {
                combatState = (host.dist(target) > 30) ? CombatState.Move : CombatState.Fight;
            }
        }

        private void MoveState()
        {
            bool isMoveDone = false;

            Task.Run(() =>
            {
                while(!isMoveDone)
                {
                    IsCancelTask();

                    // checks
                    if (host.dist(target) <= 20) host.CancelMoveTo();


                    Utils.Delay(50, token);
                }
            }, token);

            isMoveDone = host.ComeTo(target);


            if(isMoveDone)
            {
                combatState = CombatState.Fight;
            }
            else
            {
                isMoveDone = true;
                combatState = CombatState.Search;
            }
        }
        
        private void FightState()
        {
            if (IsCasting()) return;


            if (target != null && IsAttackable(target))
            {
                if (host.me.target != target) host.SetTarget(target);

                
                if (!IsDisabled(host.me) && !IsSilenced(host.me))
                {
                    DoRotation();
                }
                else
                {
                    // Anti CC skills
                }

                return;
            }

            if (swapActive) RestoreSwap();
            

            
            if(prefs.lootEnabled && toLoot.Count > 0)
            {
                Utils.Delay(850, 1250, token);

                if (!UnderAttack())
                {
                    combatState = CombatState.Control;
                    return;
                }
            }

            combatState = CombatState.Check;
        }

        private void ControlSate()
        {
            var loot = host.getCreatures().Find(c => toLoot.Contains(c.GetHashCode()));
            
            if(loot != null)
            {
                host.ComeTo(loot);
                
                host.PickupAllDrop(loot);
                toLoot.Remove(loot.GetHashCode());
                

                return;
            }

            combatState = CombatState.Check;
        }




        public void DoRoutine()
        {
            try
            {
                switch (combatState)
                {
                    case CombatState.Check:
                        //Log("State: Check");
                        CheckState();
                        break;

                    case CombatState.Search:
                        //Log("State: Search");
                        SearchState();
                        break;

                    case CombatState.Move:
                        //Log("State: Move");
                        MoveState();
                        break;

                    case CombatState.Fight:
                        //Log("State: Fight");
                        FightState();
                        break;

                    case CombatState.Control:
                        //Log("State: Control");
                        ControlSate();
                        break;
                }
            }
            catch(Exception e)
            {
                // Exception
                Log(e.Message);
            }
        }


        private void Sequence()
        {
            if (sequence < (rotation.Count - 1))
            {
                sequence = (sequence + 1);
            }
            else
            {
                if (swapActive)
                {
                    RestoreSwap();
                    return;
                }

                sequence = 0;
            }
        }

        private void SwapRotation(List<string> nRotation)
        {
            temp = new Swap()
            {
                rotation = rotation, sequence = sequence
            };
            swapActive = true;

            rotation = nRotation;
            sequence = 0;
        }

        private void RestoreSwap()
        {
            rotation = temp.rotation;
            sequence = temp.sequence;

            swapActive = false;
            Sequence();
        }

        private void DoRotation()
        {
            Skill sk = host.getSkill(rotation[sequence]);

            if (sk == null)
            {
                Sequence(); return;
            }

            SqlSkill ssk = GetSQLSkill(sk.id);


            SkillBehavior sb = SkillGet.behavior(sk.id);

            int optimalWait = 375;


            if (sb != null)
            {
                if (sb.optimalWait != 0) optimalWait = sb.optimalWait;
            }



            if (!swapActive && ssk.cooldownTime != 0 && !CanCastSkill(sk))
            {
                Sequence(); return;
            }

            
            Combos combos = null;
            bool isComboExists = false;

            if (!swapActive)
            {
                combos = template.skills.combos.Find(c => (c.triggerName == sk.name));

                if (combos != null)
                {
                    if (combos.combo.Count > 0 && combos.combo.Where(c => (host.getSkill(c) == null) || (host.skillCooldown(c) > 0)).Count() == 0)
                    {
                        isComboExists = true;
                    }
                }
            }


            if (SkillGet.hasNoAutoCome(sk.id))
            {
                if(host.dist(target) > ssk.maxRange)
                {
                    ComeToTarget(ssk.maxRange, true);
                }
            }


            bool isCastSuccess = false;

            if (!SkillGet.hasLocationTarget(sk.id))
            {
                isCastSuccess = UseSkill(sk);
            }
            else
            {
                isCastSuccess = UseSkill(sk.id, target);
            }


            if (isCastSuccess)
            {
                Log("Used: " + sk.name);
                Utils.Delay(optimalWait, token);


                if (!swapActive && isComboExists)
                {
                    SwapRotation(combos.combo);
                    return;
                }
            }

            Sequence();
        }



        private void ComeToTarget(int radius, bool faceTarget)
        {
            if(host.ComeTo(target))
            {
                if (faceTarget) host.TurnDirectly(target);
            }
        }

        private void GetTarget(List<Creature> mobs = null)
        {
            if(mobs == null)
            {
                RoundZone zone = new RoundZone(host.me.X, host.me.Y, prefs.zoneRadius);

                mobs = host.getCreatures().Where(
                    c =>
                    zone.ObjInZone(c)
                    && IsAttackable(c)
                    && host.isAttackable(c)
                    && (!prefs.ignoredMobs.Contains(c.name))

                ).ToList();
            }

            int mobsNum = mobs.Count;


            switch(mobsNum)
            {
                case 0:
                    target = null;
                    return;

                case 1:
                    target = mobs[0];
                    targetHash = target.GetHashCode();
                    return;
            }


            mobs.Sort((m1, m2) => host.me.dist(m1.X, m1.Y).CompareTo(host.me.dist(m2.X, m2.Y)));

            target = mobs.First();
            targetHash = target.GetHashCode();
        }


        private void onLootAvailable(Creature obj)
        {
            if(obj.GetHashCode() == targetHash) toLoot.Add(obj.GetHashCode());
        }

        private void IsCancelTask()
        {
            token.ThrowIfCancellationRequested();
        }
    }
}
