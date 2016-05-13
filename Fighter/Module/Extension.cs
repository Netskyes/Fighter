using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArcheBuddy.Bot.Classes;
using ArcheBuddy.SQL;

namespace Fighter
{
    internal class Extension
    {
        private Host host;

        public Extension(Host host)
        {
            this.host = host;
        }

        public void Log(string text)
        {
            host.Log(text);
        }

        public bool CanCastSkill(Skill skill)
        {
            return (skill != null && (host.skillCooldown(skill) == 0));
        }

        public bool CanCastSkill(string skillName)
        {
            var skill = host.getSkill(skillName); return CanCastSkill(skill);
        }

        public bool CanUseItem(Item item)
        {
            return (item != null && (host.itemCooldown(item) == 0));
        }

        public bool CanUseItem(string itemName)
        {
            var item = host.getInvItem(itemName); return CanUseItem(item);
        }

        public bool UnderAttack()
        {
            return (host.getAggroMobsCount() > 0);
        }

        public bool UnderAttack(Creature obj)
        {
            return (host.getAggroMobsCount(obj) > 0);
        }

        public bool IsCasting()
        {
            return host.isCasting() || host.me.isGlobalCooldown;
        }

        public bool IsAttackable(Creature obj)
        {
            return (obj.isAlive() && !IsAttackImmune(obj) && !IsBuffExists(obj, 550));
        }

        public bool IsAttackImmune(Creature obj)
        {
            return (host.isMeleeImmune(obj) || host.isRangedImmune(obj) || host.isSpellImmune(obj));
        }

        public bool IsDisabled(Creature obj)
        {
            return (host.isKnockedDown(obj) || host.isSleep(obj) || host.isStun(obj));
        }

        public bool IsSlowDown(Creature obj)
        {
            return (host.isCrippled(obj) || host.isRoot(obj));
        }

        public bool IsSilenced(Creature obj)
        {
            return (host.isSilense(obj));
        }

        public bool IsBuffExists(uint buffId)
        {
            return (host.getBuff(buffId) != null);
        }

        public bool IsBuffExists(Creature obj, uint buffId)
        {
            return (host.getBuff(obj, buffId) != null);
        }

        public bool AnyBuffExists(IEnumerable<uint> buffs)
        {
            return host.me.getBuffs().Exists(b => buffs.Contains(b.id));
        }
    }
}
