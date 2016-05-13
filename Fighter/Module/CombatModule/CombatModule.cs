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

        private List<string> buffs;

        private List<int> toLoot = new List<int>();


        private void Initialize()
        {
            template = UI.FetchTemplate();

            // Build rotation
            rotation = template.buffs.combat;
            rotation = rotation.Concat(template.skills.rotation).ToList();

            // GroupUp buffs
            buffs = template.buffs.combat;
            buffs = buffs.Concat(template.buffs.preCombat).ToList();


            // Register events
            host.onLootAvailable += onLootAvailable;
        }




        private void Check()
        {
            if (template.skills.rotation.Count < 1) return;

            combatState = CombatState.Search;
        }

        private void Search()
        {
            bool uAttack = false;

            if((uAttack = UnderAttack()))
            {
                GetTarget(host.getAggroMobs());
            }
            else GetTarget();


            if (target != null && host.isExists(target))
            {
                if(uAttack)
                {
                    combatState = CombatState.Fight;
                }
                else
                {
                    combatState = (host.dist(target) > (20 - 1)) ? CombatState.Move : CombatState.Ready;
                }
            }
        }

        private void Move()
        {
            if(ComeToTarget(20 - 1))
            {
                combatState = CombatState.Ready;
            }
            else
            {
                combatState = CombatState.Search;
            }
        }

        private void Ready()
        {
            if(template.buffs.preCombat.Count > 0)
            {
                SwapRotation(template.buffs.preCombat);
            }

            combatState = CombatState.Fight;
        }

        private void Fight()
        {
            if (IsCasting()) return;


            bool isMeTarget = true;

            if(UnderAttack(target))
            {
                isMeTarget = (target.target == host.me);
            }


            if (target != null && isMeTarget && IsAttackable(target))
            {
                if (host.me.target != target)
                {
                    host.SetTarget(target);
                }

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

            if (swapActive) RestoreSwap(true);
            sequence = 0;

            combatState = CombatState.Analyze;
        }

        private void Analyze()
        {
            if(UnderAttack())
            {
                combatState = CombatState.Check;
                return;
            }

            if (prefs.lootEnabled && toLoot.Count > 0)
            {
                Utils.Delay(850, 1250, token);

                var loot = host.getCreatures().Find(c => toLoot.Contains(c.GetHashCode()));

                if (loot != null)
                {
                    host.ComeTo(loot);

                    host.PickupAllDrop(loot);
                    toLoot.Remove(loot.GetHashCode());

                    return;
                }

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
                        Check();
                        break;

                    case CombatState.Search:
                        //Log("State: Search");
                        Search();
                        break;

                    case CombatState.Move:
                        //Log("State: Move");
                        Move();
                        break;

                    case CombatState.Ready:
                        //Log("State: Ready");
                        Ready();
                        break;

                    case CombatState.Fight:
                        //Log("State: Fight");
                        Fight();
                        break;

                    case CombatState.Analyze:
                        //Log("State: Analyze");
                        Analyze();
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

        private void RestoreSwap(bool skipSeq = false)
        {
            rotation = temp.rotation;
            sequence = temp.sequence;

            swapActive = false;
            if(!skipSeq) Sequence();
        }

        private void DoRotation()
        {
            Skill sk = host.getSkill(rotation[sequence]);

            if (sk == null)
            {
                Sequence(); return;
            }

            bool isBuff = buffs.Contains(sk.name);


            int optimalWait = 375;

            SkillBehavior sb = SkillGet.behavior(sk.id);

            if (sb != null)
            {
                if (sb.optimalWait != 0) optimalWait = sb.optimalWait;
            }


            if (!swapActive && sk.db.cooldownTime != 0 && !CanCastSkill(sk))
            {
                Sequence(); return;
            }

            
            Combos combos = null;
            bool isComboExists = false;

            if (!swapActive && !isBuff)
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


            IsCancelTask();

            bool isCastSuccess = false;

            if (!SkillGet.hasLocationTarget(sk.id))
            {
                isCastSuccess = UseSkill(sk, false, isBuff, !isBuff);
            }
            else
            {
                isCastSuccess = UseSkill(sk, true);
            }


            if (isCastSuccess)
            {
                Log("Used: " + sk.name); Utils.Delay(optimalWait, token);

                if (!swapActive && isComboExists)
                {
                    SwapRotation(combos.combo);
                    return;
                }
            }

            Sequence();
        }

        private void DoReadyBuffs()
        {
            
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
                    && host.getAggroMobsCount(c) == 0
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

        private bool ComeToTarget(double distance)
        {
            bool isMoveDone = false;

            Task.Run(() =>
            {
                while (!isMoveDone)
                {
                    IsCancelTask();

                    // Conditions
                    if (IsDisabled(host.me) || UnderAttack()) break;


                    Utils.Delay(50, token);
                }

                host.CancelMoveTo();

            }, token);


            isMoveDone = host.ComeTo(target, distance);
            

            if (isMoveDone)
            {
                return true;
            }
            else
            {
                isMoveDone = true;
                return false;
            }
        }

        public bool UseSkill(Skill skill, bool isTarget = false, bool selfTarget = false, bool autoCome = true)
        {
            if (autoCome && (host.dist(target) > skill.db.maxRange))
            {
                double dist = host.me.calcSkillMaxRange(skill.id) - 1;

                ComeToTarget(dist);
            }


            if (!isTarget)
            {
                return skill.UseSkill(false, selfTarget);
            }
            else
            {
                return host.UseSkill(skill.id, target.X, target.Y, target.Z, false);
            }
        }




        private void onLootAvailable(Creature obj)
        {
            if(obj.GetHashCode() == targetHash) toLoot.Add(obj.GetHashCode());
        }


        public void Flush()
        {
            host.onLootAvailable -= onLootAvailable;
        }

        private void IsCancelTask()
        {
            token.ThrowIfCancellationRequested();
        }
    }
}
