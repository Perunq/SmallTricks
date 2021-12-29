using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmallTricks
{
    public class AdditiveUnitPart : OldStyleUnitPart
    {
        [JsonProperty]
        protected List<EntityFact> buffs = new List<EntityFact>();

        public virtual void AddBuff(EntityFact buff)
        {
            if (!buffs.Contains(buff))
            {
                buffs.Add(buff);
            }
        }

        public virtual void RemoveBuff(EntityFact buff)
        {
            buffs.Remove(buff);
        }
    }



    public class AdditiveUnitPartWithCheckLock : AdditiveUnitPart
    {
        Dictionary<EntityFact, bool> lock_map = new Dictionary<EntityFact, bool>();

        public override void AddBuff(EntityFact buff)
        {
            if (!buffs.Contains(buff))
            {
                buffs.Add(buff);
                lock_map[buff] = false;
            }
        }

        public override void RemoveBuff(EntityFact buff)
        {
            buffs.Remove(buff);
            lock_map.Remove(buff);
        }


        protected bool Check<T>(EntityFact buff, Predicate<T> pred) where T : BlueprintComponent
        {
            if (!buffs.Contains(buff))
            {
                return false;
            }
            if (lock_map[buff])
            {
                return false;
            }
            lock_map[buff] = true;

            bool res = false;
            buff.CallComponents<T>(c => res = pred(c));
            lock_map[buff] = false;
            return res;
        }
    }
}