using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fighter
{
    internal class SkillGet
    {
        public static SkillBehavior behavior(uint skillId)
        {
            var dataSet = GetDataSet();

            return (dataSet.ContainsKey(skillId)) ? dataSet[skillId] : null;
        }

        public static bool hasNoAutoCome(uint skillId)
        {
            return getNoAutoComes().Contains(skillId);
        }

        public static bool hasLocationTarget(uint skillId)
        {
            return getLocationTargets().Contains(skillId);
        }

        

        private static uint[] getNoAutoComes()
        {
            return new uint[] 
            {
                Battlerage.WhirlwindSlash, Battlerage.SunderEarth
            };
        }

        private static  uint[] getLocationTargets()
        {
            return new uint[] 
            {
                Sorcery.MeteorStrike, Sorcery.GodsWhip, Battlerage.BehindEnemyLines, Defense.OllosHammer, Archery.MissileRain
            };
        }


        private static Dictionary<uint, SkillBehavior> GetDataSet()
        {
            var behaviors = new Dictionary<uint, SkillBehavior>();

            // Skill behavior properties
            behaviors.Add(Sorcery.Flamebolt, new SkillBehavior()
            {
                optimalWait = 50
            });

            behaviors.Add(Battlerage.WhirlwindSlash, new SkillBehavior()
            {
                optimalWait = 425
            });


            return behaviors;
        }

        internal struct Battlerage
        {
            public const uint TripleSlash = 18132;
            public const uint Charge = 11918;
            public const uint WhirlwindSlash = 13282;
            public const uint SunderEarth = 10644;
            public const uint Lasso = 12039;
            public const uint TerrifyingRoar = 18308;
            public const uint Bondbreaker = 12034;
            public const uint PrecisionStrike = 12026;
            public const uint Frenzy = 10455;
            public const uint BattleFocus = 10377;
            public const uint TigerStrike = 13315;
            public const uint BehindEnemyLines = 23587;
        }

        internal struct Shadowplay
        {
            public const uint RapidStrike = 18125;
            public const uint Overwhelm = 10648;
            public const uint DropBack = 12049;
            public const uint Wallop = 12029;
            public const uint StalkersMark = 12139;
            public const uint Stealth = 10082;
            public const uint ToxicShot = 10481;
            public const uint PinDown = 13344;
            public const uint Shadowsmite = 10496;
            public const uint Freerunner = 10189;
            public const uint ShadowStep = 12075;
            public const uint ThrowDagger = 23594;
        }

        internal struct Auramancy
        {
            public const uint Thwart = 16486;
            public const uint CometsBoon = 18222;
            public const uint ConversionShield = 11869;
            public const uint ViciousImplosion = 10710;
            public const uint Teleportation = 10152;
            public const uint HealthLift = 11991;
            public const uint Meditate = 11989;
            public const uint ShrugItOff = 11429;
            public const uint Liberation = 11380;
            public const uint Leech = 10104;
            public const uint ProtectiveWings = 10714;
            public const uint MirrorWarp = 23934;
        }

        internal struct Sorcery
        {
            public const uint Flamebolt = 10752;
            public const uint FreezingArrow = 10667;
            public const uint InsulatingLens = 10153;
            public const uint ArcLightning = 10670;
            public const uint FreezingEarth = 10151;
            public const uint SearingRain = 11939;
            public const uint FrigidTracks = 11314;
            public const uint MagicCircle = 12796;
            public const uint ChainLightning = 11967;
            public const uint FlameBarrier = 14774;
            public const uint MeteorStrike = 10664;
            public const uint GodsWhip = 23593;
        }

        internal struct Defense
        {
            public const uint ShieldSlam = 10399;
            public const uint Refreshment = 10645;
            public const uint BullRush = 10501;
            public const uint BoastfulRoar = 12048;
            public const uint Toughen = 11365;
            public const uint RevitalizingCheer = 12046;
            public const uint Redoubt = 10375;
            public const uint OllosHammer = 18757;
            public const uint MockingHowl = 10436;
            public const uint Imprison = 14529;
            public const uint Invincibility = 10372;
            public const uint Fortress = 23589;
        }

        internal struct Witchcraft
        {
            public const uint EarthenGrip = 14376;
            public const uint Enervate = 10159;
            public const uint BubbleTrap = 10154;
            public const uint InsidiousWhisper = 10409;
            public const uint Purge = 10712;
            public const uint PlayDead = 10488;
            public const uint CourageousAction = 11424;
            public const uint Lassitude = 10134;
            public const uint BansheeWail = 12001;
            public const uint FocalConcussion = 11353;
            public const uint DahutasBreath = 11443;
            public const uint FiendsKnell = 23588;
        }

        internal struct Archery
        {
            public const uint ChargedBolt = 16210;
            public const uint PiercingShot = 13564;
            public const uint EndlessArrows = 14835;
            public const uint DoubleRecurve = 11368;
            public const uint Deadeye = 15073;
            public const uint Snare = 12133;
            public const uint Float = 10694;
            public const uint Boneyard = 14760;
            public const uint ConcussiveArrow = 11933;
            public const uint MissileRain = 13281;
            public const uint Intensity = 10708;
            public const uint Snipe = 23592;
        }

        internal struct Vitalism
        {
            public const uint Antithesis = 10534;
            public const uint MirrorLight = 11379;
            public const uint Resurgence = 10547;
            public const uint Revive = 10546;
            public const uint Skewer = 13286;
            public const uint Mend = 10720;
            public const uint Infuse = 16783;
            public const uint AranzebsBoon = 16004;
            public const uint Renewal = 17412;
            public const uint FerventHealing = 14929;
            public const uint Twilight = 10721;
            public const uint WhirlwindsBlessing = 23596;
        }

        internal struct Songcraft
        {
            public const uint CriticalDiscord = 11973;
            public const uint StartlingStrain = 11934;
            public const uint Quickstep = 10723;
            public const uint Dissonance = 11943;
            public const uint HealingHymn = 17413;
            public const uint HummingbirdDitty = 11377;
            public const uint OdeToRecovery = 10724;
            public const uint RhythmicRenewal = 11948;
            public const uint BulwarkBallad = 11396;
            public const uint BloodyChantey = 10727;
            public const uint AlarmCall = 11961;
            public const uint GriefsCadence = 23595;
        }

        internal struct Occultism
        {
            public const uint ManaStars = 14810;
            public const uint ManaForce = 12759;
            public const uint HellSpear = 10135;
            public const uint AbsorbLifeforce = 11441;
            public const uint SummonCrows = 11395;
            public const uint CripplingMire = 10201;
            public const uint Telekinesis = 11504;
            public const uint Retribution = 10655;
            public const uint Stillness = 10665;
            public const uint Urgency = 11442;
            public const uint SummonWraith = 10434;
            public const uint DeathsVengeance = 23591;
        }
    }
}
