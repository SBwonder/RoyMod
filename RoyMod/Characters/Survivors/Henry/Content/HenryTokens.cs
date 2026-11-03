using System;
using HenryMod.Modules;
using HenryMod.Survivors.Henry.Achievements;

namespace HenryMod.Survivors.Henry
{
    public static class HenryTokens
    {
        public static void Init()
        {
            AddHenryTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddHenryTokens()
        {
            string prefix = HenrySurvivor.HENRY_PREFIX;

            string desc = "Roy is the young lord of Pherae, a swordsman who fights up close and grows stronger the longer he keeps the flames of his blade alive.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Strike with the base of the blade: enemies hit up close take extra damage and are more likely to be crit." + Environment.NewLine + Environment.NewLine
             + "< ! > Double-Edge Dance, perfect dodges and counters build the flame meter. Keep it above half to set every swing ablaze." + Environment.NewLine + Environment.NewLine
             + "< ! > Use Double-Edge Dance in the air, or jump out of it, to carry enemies upward; use it right after a dash slash to drag them in and pin them down." + Environment.NewLine + Environment.NewLine
             + "< ! > After an air dodge, primary and secondary turn into aerial follow-ups. Finish with Blazer, and hold forward to send it across instead of up." + Environment.NewLine + Environment.NewLine
             + "< ! > A counter timed the instant you're hit rushes Roy straight to the attacker, wherever they are." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, the Sword of Seals quiet at last.";
            string outroFailure = "..and so he vanished, his flame gone out far from Pherae.";

            string lore = "The young lord of Pherae was never meant to lead an army. He was meant to study, to learn the duties of a marquess, and to inherit a quiet land in the east of Lycia.\n\n"
             + "War decided otherwise. With his father ill, Roy rode out at the head of the Lycian League, and by its end he carried the Sword of Seals, a blade that answers the will of its wielder with fire.\n\n"
             + "Now that blade has brought him somewhere no map of Elibe describes. The creatures here do not parley and do not retreat. Roy fights the way he always has: close enough to see his enemy's eyes, trusting that every comrade he cannot reach is still worth fighting for.";

            Language.Add(prefix + "NAME", "Roy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Young Lion");
            Language.Add(prefix + "LORE", lore);
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Awakening");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Flame Meter");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Double-Edge Dance, Blazer, perfect dodges and counters build Roy's <style=cIsDamage>flame meter</style>, raising his damage by up to <style=cIsDamage>50%</style>. At half full his blade is <style=cIsDamage>enflamed</style> and every hit <style=cIsDamage>ignites</style>. Sword hits up close deal <style=cIsDamage>25% more damage</style> and have <style=cIsDamage>+25% crit chance</style>.");
            Language.Add(prefix + "PRIMARY_NAME", "Sword of Seals");
            Language.Add(prefix + "PRIMARY_DESCRIPTION", "Click for a <style=cIsDamage>knee strike</style>, then a <style=cIsDamage>jab</style> and a <style=cIsDamage>forward smash</style>; hold to keep slashing. Sprinting attacks with a <style=cIsDamage>dash slash</style>, and in the air with a <style=cIsDamage>two-hit spinning slash</style>.");
            Language.Add(prefix + "SECONDARY_NAME", "Double-Edge Dance");
            Language.Add(prefix + "SECONDARY_DESCRIPTION", "A string of up to <style=cIsDamage>four slashes</style>; press again to continue. In the air, or by jumping out of it, the string <style=cIsUtility>rises into an air juggle</style>. Out of a dash slash it <style=cIsDamage>pulls enemies in and finishes with a flurry of stabs</style>. Builds the flame meter.");
            Language.Add(prefix + "UTILITY_NAME", "Evade");
            Language.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsUtility>Dodge</style> the way you're moving: a spot dodge, a roll, or an air dodge. Roll twice to break into a sprint. After an air dodge, primary <style=cIsDamage>slashes upward</style> and secondary <style=cIsDamage>stabs upward</style> or unleashes <style=cIsDamage>Blazer</style>. Dodge just as you're hit for a <style=cIsDamage>perfect dodge</style> that strikes back and <style=cIsDamage>engulfs the attacker in a pillar of fire</style>.");
            Language.Add(prefix + "SPECIAL_NAME", "Counter");
            Language.Add(prefix + "SPECIAL_DESCRIPTION", "Take a <style=cIsUtility>counter stance</style>. Blocking a hit strikes back for <style=cIsDamage>at least 500% damage</style> and sets the attacker in a <style=cIsDamage>pillar of fire</style>. Countering the instant you're hit <style=cIsUtility>rushes Roy to the attacker</style> for <style=cIsDamage>1400% damage</style> and leaves a <style=cIsDamage>fire tornado</style> behind. Roy is <style=cIsUtility>invulnerable</style> while he strikes back.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(HenryMasteryAchievement.identifier), "Roy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(HenryMasteryAchievement.identifier), "As Roy, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
