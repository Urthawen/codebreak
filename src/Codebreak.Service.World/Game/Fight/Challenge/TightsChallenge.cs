﻿using Codebreak.Service.World.Game.Map;
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
    public sealed class TightsChallenge : AbstractChallenge
    {
        /// <summary>
        /// 
        /// </summary>
        public TightsChallenge()
            : base(ChallengeTypeEnum.TIGHTS)
        {
            BasicDropBonus = 40;
            BasicXpBonus = 40;

            TeamDropBonus = 40;
            TeamXpBonus = 40;

            ShowTarget = false;
            TargetId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        public override void EndTurn(AbstractFighter fighter)
        {
            var nearestFighters = Pathfinding.GetFightersNear(fighter.Fight, fighter.Cell.Id);
            if(nearestFighters.Where(f => f.Team == fighter.Team).Count() == 0)            
                base.OnFailed(fighter.Name);            
        }
    }
}
