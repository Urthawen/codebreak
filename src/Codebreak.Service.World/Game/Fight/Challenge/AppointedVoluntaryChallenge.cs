﻿using Codebreak.Service.World.Network;
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
    public sealed class AppointedVoluntaryChallenge : AbstractChallenge
    {
        /// <summary>
        /// 
        /// </summary>
        public AppointedVoluntaryChallenge()
            : base(ChallengeTypeEnum.APPOINTED_VOLUNTARY)
        {
            BasicDropBonus = 30;
            BasicXpBonus = 30;

            TeamDropBonus = 60;
            TeamXpBonus = 60;

            ShowTarget = true;
            TargetId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="team"></param>
        public override void StartFight(FightTeam team)
        {
            if (team.OpponentTeam.HasSomeoneAlive)
            {
                var randomIndex = Util.Next(0, team.OpponentTeam.AliveFighters.Count());
                var target = team.OpponentTeam.AliveFighters.ElementAt(randomIndex);

                TargetId = target.Id;
                base.FlagCell(target.Cell.Id, TargetId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        public override void CheckDeath(AbstractFighter fighter)
        {
            if (fighter.Id == TargetId)
            {
                base.OnSuccess();
            }
            else
            {
                base.OnFailed();
            }
        }
    }
}
