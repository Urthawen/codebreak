﻿using Codebreak.Service.World.Game.Fight.Effect;
using Codebreak.Service.World.Game.Spell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Fight.Challenge
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AbnegationChallenge : AbstractChallenge
    {
        /// <summary>
        /// 
        /// </summary>
        public AbnegationChallenge()
            : base(ChallengeTypeEnum.ABNEGATION)
        {
            BasicDropBonus = 10;
            BasicXpBonus = 10;

            TeamDropBonus = 25;
            TeamXpBonus = 25;

            ShowTarget = false;
            TargetId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="castInfos"></param>
        public override void CheckSpell(AbstractFighter fighter, CastInfos castInfos)
        {
            if(castInfos.EffectType == EffectEnum.AddLife && castInfos.Target != null && castInfos.Target.Team == fighter.Team)            
                base.OnFailed(fighter.Name);            
        }
    }
}
