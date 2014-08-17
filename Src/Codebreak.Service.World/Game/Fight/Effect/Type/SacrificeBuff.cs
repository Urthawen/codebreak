﻿using Codebreak.Service.World.Game.Spell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Fight.Effect.Type
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SacrificeBuff : BuffBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="castInfos"></param>
        /// <param name="target"></param>
        public SacrificeBuff(CastInfos castInfos, FighterBase target)
            : base(castInfos, target, ActiveType.ACTIVE_ATTACKED_AFTER_JET, DecrementType.TYPE_ENDTURN)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DamageValue"></param>
        /// <param name="DamageInfos"></param>
        /// <returns></returns>
        public override FightActionResultEnum ApplyEffect(ref int DamageValue, CastInfos DamageInfos = null)
        {
            var targetTeleport = new CastInfos(EffectEnum.Teleport, CastInfos.SpellId, CastInfos.Caster.Cell.Id, 0, 0, 0, 0, 0, Target, null);
            var casterTeleport = new CastInfos(EffectEnum.Teleport, CastInfos.SpellId, Target.Cell.Id, 0, 0, 0, 0, 0, CastInfos.Caster, null);

            Caster.SetCell(null);
            Target.SetCell(null);

            if (TeleportEffect.ApplyTeleport(targetTeleport) == FightActionResultEnum.RESULT_END)
                return FightActionResultEnum.RESULT_END;

            if (TeleportEffect.ApplyTeleport(casterTeleport) == FightActionResultEnum.RESULT_END)
                return FightActionResultEnum.RESULT_END;

            if (DamageEffect.ApplyDamages(DamageInfos, CastInfos.Caster, ref DamageValue) == FightActionResultEnum.RESULT_END)
                return FightActionResultEnum.RESULT_END;

            DamageValue = 0;

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }
    }
}