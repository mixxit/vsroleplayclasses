using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace vsroleplayclasses.src.Items
{
    public class AbilityScrollItem : Item
    {
        public override void OnLoaded(ICoreAPI api)
        {
            if (api.Side != EnumAppSide.Client)
                return;

            ICoreClientAPI capi = api as ICoreClientAPI;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            if (IsAbilityScribed(inSlot.Itemstack))
            {
                dsc.AppendLine(Lang.Get("Ability: {0}", GetScribedAbilityName(inSlot.Itemstack)));
                var s_spellEffectIndex = GetScribedAbilityRunicComponent<SpellEffectIndex>(inSlot.Itemstack);
                var s_spellEffectType = GetScribedAbilityRunicComponent<SpellEffectType>(inSlot.Itemstack);
                var s_targetType = GetScribedAbilityRunicComponent<TargetType>(inSlot.Itemstack);
                var s_resistType = GetScribedAbilityRunicComponent<ResistType>(inSlot.Itemstack);
                var s_powerLevel = GetScribedAbilityRunicComponent<PowerLevel>(inSlot.Itemstack);
                var s_adventureClass = GetScribedAbilityRunicComponent<AdventureClass>(inSlot.Itemstack);

                if (s_spellEffectIndex != SpellEffectIndex.None)
                    dsc.AppendLine(Lang.Get("Spell Effect Index: {0} ({1})", s_spellEffectIndex, RunicTools.GetSpellEffectIndexMagicWord(s_spellEffectIndex)));
                if (s_spellEffectType != SpellEffectType.None)
                    dsc.AppendLine(Lang.Get("Spell Effect Type: {0} ({1})", s_spellEffectType, RunicTools.GetSpellEffectTypeMagicWord(s_spellEffectType)));
                if (s_targetType != TargetType.None)
                    dsc.AppendLine(Lang.Get("Target Type: {0} ({1})", s_targetType, RunicTools.GetTargetTypeMagicWord(s_targetType)));
                if (s_resistType != ResistType.None)
                    dsc.AppendLine(Lang.Get("Resist Type: {0} ({1})", s_resistType, RunicTools.GetResistTypeMagicWord(s_resistType)));
                if (s_powerLevel != PowerLevel.None)
                    dsc.AppendLine(Lang.Get("Power Level: {0} ({1})", s_powerLevel, RunicTools.GetPowerLevelMagicWord(s_powerLevel)));
                if (s_adventureClass != AdventureClass.None)
                    dsc.AppendLine(Lang.Get("Adventure Class: {0} ({1})", s_adventureClass, RunicTools.GetAdventureClassMagicWord(s_adventureClass)));
                return;
            }
            else
            {
                if (HasSpareRuneSlot(inSlot.Itemstack))
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll is incomplete"));
                    var spellEffectIndex = GetWordOfPower<SpellEffectIndex>(inSlot.Itemstack);
                    var spellEffectType = GetWordOfPower<SpellEffectType>(inSlot.Itemstack);
                    var targetType = GetWordOfPower<TargetType>(inSlot.Itemstack);
                    var resistType = GetWordOfPower<ResistType>(inSlot.Itemstack);
                    var powerLevel = GetWordOfPower<PowerLevel>(inSlot.Itemstack);
                    var adventureClass = GetWordOfPower<AdventureClass>(inSlot.Itemstack);

                    if (spellEffectIndex != SpellEffectIndex.None)
                        dsc.AppendLine(Lang.Get("Spell Effect Index: {0} ({1})", spellEffectIndex, RunicTools.GetSpellEffectIndexMagicWord(spellEffectIndex)));
                    if (spellEffectType != SpellEffectType.None)
                        dsc.AppendLine(Lang.Get("Spell Effect Type: {0} ({1})", spellEffectType, RunicTools.GetSpellEffectTypeMagicWord(spellEffectType)));
                    if (targetType != TargetType.None)
                        dsc.AppendLine(Lang.Get("Target Type: {0} ({1})", targetType, RunicTools.GetTargetTypeMagicWord(targetType)));
                    if (resistType != ResistType.None)
                        dsc.AppendLine(Lang.Get("Resist Type: {0} ({1})", resistType, RunicTools.GetResistTypeMagicWord(resistType)));
                    if (powerLevel != PowerLevel.None)
                        dsc.AppendLine(Lang.Get("Power Level: {0} ({1})", powerLevel, RunicTools.GetPowerLevelMagicWord(powerLevel)));
                    if (adventureClass != AdventureClass.None)
                        dsc.AppendLine(Lang.Get("Adventure Class: {0} ({1})", adventureClass, RunicTools.GetAdventureClassMagicWord(adventureClass)));
                    return;
                }
                else
                {
                    dsc.AppendLine(Lang.Get("Ability: {0}", "This scroll appears useless"));
                    var s_spellEffectIndex = GetScribedAbilityRunicComponent<SpellEffectIndex>(inSlot.Itemstack);
                    var s_spellEffectType = GetScribedAbilityRunicComponent<SpellEffectType>(inSlot.Itemstack);
                    var s_targetType = GetScribedAbilityRunicComponent<TargetType>(inSlot.Itemstack);
                    var s_resistType = GetScribedAbilityRunicComponent<ResistType>(inSlot.Itemstack);
                    var s_powerLevel = GetScribedAbilityRunicComponent<PowerLevel>(inSlot.Itemstack);
                    var s_adventureClass = GetScribedAbilityRunicComponent<AdventureClass>(inSlot.Itemstack);

                    if (s_spellEffectIndex != SpellEffectIndex.None)
                        dsc.AppendLine(Lang.Get("Spell Effect Index: {0} ({1})", s_spellEffectIndex, RunicTools.GetSpellEffectIndexMagicWord(s_spellEffectIndex)));
                    if (s_spellEffectType != SpellEffectType.None)
                        dsc.AppendLine(Lang.Get("Spell Effect Type: {0} ({1})", s_spellEffectType, RunicTools.GetSpellEffectTypeMagicWord(s_spellEffectType)));
                    if (s_targetType != TargetType.None)
                        dsc.AppendLine(Lang.Get("Target Type: {0} ({1})", s_targetType, RunicTools.GetTargetTypeMagicWord(s_targetType)));
                    if (s_resistType != ResistType.None)
                        dsc.AppendLine(Lang.Get("Resist Type: {0} ({1})", s_resistType, RunicTools.GetResistTypeMagicWord(s_resistType)));
                    if (s_powerLevel != PowerLevel.None)
                        dsc.AppendLine(Lang.Get("Power Level: {0} ({1})", s_powerLevel, RunicTools.GetPowerLevelMagicWord(s_powerLevel)));
                    if (s_adventureClass != AdventureClass.None)
                        dsc.AppendLine(Lang.Get("Adventure Class: {0} ({1})", s_adventureClass, RunicTools.GetAdventureClassMagicWord(s_adventureClass)));
                    return;
                }
            }
        }

        public bool IsAbilityScribed(ItemStack itemStack)
        {
            return (GetScribedAbilityId(itemStack) > 0);
        }

        // Seed so client and server can match
        public void SetScribedAbility(ItemStack itemStack, Ability ability)
        {
            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetLong("scribedAbility", ability.Id);
                itemStack.Attributes.SetString("scribedAbilityName", ability.Name);
                itemStack.Attributes.SetLong("scribedAbilitySpellEffectIndex", (int)ability.SpellEffectIndex);
                itemStack.Attributes.SetLong("scribedAbilitySpellEffectType", (int)ability.SpellEffect);
                itemStack.Attributes.SetLong("scribedAbilityTargetType", (int)ability.TargetType);
                itemStack.Attributes.SetLong("scribedAbilityResistType", (int)ability.ResistType);
                itemStack.Attributes.SetLong("scribedAbilityPowerLevel", (int)ability.PowerLevel);
                itemStack.Attributes.SetLong("scribedAbilityAdventureClass", (int)ability.AdventureClass);
                if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                    throw new Exception("This should not happen");
                if (!itemStack.Attributes.HasAttribute("scribedAbilityName"))
                    throw new Exception("This should not happen");
            }
        }

        public void SetWordOfPower<T>(ItemStack itemStack, T magicPower) where T : Enum
        {
            MagicPowerSlot magicPowerSlot = (MagicPowerSlot)Enum.Parse(typeof(MagicPowerSlot), typeof(T).Name);

            if (magicPowerSlot == MagicPowerSlot.None)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                itemStack.Attributes.SetInt("wordOfPower_"+ magicPowerSlot, (int)(object)magicPower);
                if (!itemStack.Attributes.HasAttribute("wordOfPower_" + magicPowerSlot))
                    throw new Exception("This should not happen");
            }
        }

        internal T GetWordOfPower<T>(ItemStack itemStack) where T : Enum
        {
            MagicPowerSlot magicPowerSlot = (MagicPowerSlot)Enum.Parse(typeof(MagicPowerSlot), typeof(T).Name);

            if (magicPowerSlot == MagicPowerSlot.None)
                throw new Exception("Invalid slot");

            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("wordOfPower_"+ magicPowerSlot))
                        return default(T);

                    var wordOfPower = itemStack.Attributes.TryGetInt("wordOfPower_" + magicPowerSlot);
                    if (wordOfPower == null)
                        return default(T);


                    return (T)Enum.ToObject(typeof(T), wordOfPower);
                }
                catch (InvalidCastException)
                {

                    return default(T);
                }
            }
            return default(T);
        }
        
        internal long GetScribedAbilityId(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedAbility"))
                        return -1;

                    return itemStack.Attributes.GetLong("scribedAbility", -1); // when deserialized json item it will default to long over int
                }
                catch (InvalidCastException)
                {

                    return -1;
                }
            }
            return -1;
        }

        internal string GetScribedAbilityName(ItemStack itemStack)
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute("scribedAbilityName"))
                        return "Unknown Ability";

                    return itemStack.Attributes.GetString("scribedAbilityName", "Unknown Ability"); 
                }
                catch (InvalidCastException)
                {

                    return "Unknown Ability";
                }
            }
            return "Unknown Ability";
        }

        internal T GetScribedAbilityRunicComponent<T>(ItemStack itemStack) where T : Enum
        {
            if (itemStack.Attributes != null)
            {
                try
                {
                    if (!itemStack.Attributes.HasAttribute($"scribedAbility{typeof(T).Name}"))
                        return default(T);

                    return (T)Enum.ToObject(typeof(T), itemStack.Attributes.GetLong($"scribedAbility{typeof(T).Name}", 0));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
            return default(T);
        }

        public bool HasSpareRuneSlot(ItemStack itemstack)
        {
            if (!HasRunePower<SpellEffectIndex>(itemstack))
                return true;
            if (!HasRunePower<SpellEffectType>(itemstack))
                return true;
            if (!HasRunePower<TargetType>(itemstack))
                return true;
            if (!HasRunePower<ResistType>(itemstack))
                return true;
            if (!HasRunePower<PowerLevel>(itemstack))
                return true;
            if (!HasRunePower<AdventureClass>(itemstack))
                return true;

            return false;
        }


        internal bool HasRunePower<T>(ItemStack itemstack) where T : Enum
        {
            return !Object.Equals(GetWordOfPower<T>(itemstack), default(T));
        }
    }
}