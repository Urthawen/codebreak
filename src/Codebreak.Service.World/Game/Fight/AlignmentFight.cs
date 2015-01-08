﻿using Codebreak.Service.World.Database.Repository;
using Codebreak.Service.World.Database.Structure;
using Codebreak.Service.World.Game.Entity;
using Codebreak.Service.World.Game.Map;
using Codebreak.Service.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Fight
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AlignmentFight : FightBase
    {
        /// <summary>
        /// 
        /// </summary>
        public const int KNGIHT_MONSTER_ID = 394;

        /// <summary>
        /// 
        /// </summary>
        public bool IsNeutralAgression
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CharacterEntity Aggressor
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public CharacterEntity Victim
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        private StringBuilder m_serializedFlag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggressor"></param>
        /// <param name="victim"></param>
        public AlignmentFight(MapInstance map, long id, CharacterEntity aggressor, CharacterEntity victim)
            : base(FightTypeEnum.TYPE_AGGRESSION, map, id, aggressor.Id, aggressor.CellId, victim.Id, victim.CellId, 30000, 30000)
        {
            Aggressor = aggressor;
            Victim = victim;
            IsNeutralAgression = Victim.CharacterAlignment.AlignmentId == (int)AlignmentTypeEnum.ALIGNMENT_NEUTRAL;
            
            JoinFight(Aggressor, Team0);
            JoinFight(Victim, Team1);
                 
            base.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnFightStart()
        {
            if(IsNeutralAgression)
            {
                var aggressors = Aggressor.Team.AliveFighters;
                var averageLevel = (int)aggressors.Average(aggressor => aggressor.Level);
                var knighLevel = 0;

                if (averageLevel < 50)
                    knighLevel = 0;
                else if (averageLevel < 80)
                    knighLevel = 1;
                else if (averageLevel < 110)
                    knighLevel = 2;
                else if (averageLevel < 140)
                    knighLevel = 3;
                else if (averageLevel < 170)
                    knighLevel = 4;
                else
                    knighLevel = 5;

                var knight = MonsterRepository.Instance.GetById(KNGIHT_MONSTER_ID);
                if (knight != null)
                {
                    if (knight.GetGrades().Count() > knighLevel)
                    {
                        SummonFighter(new MonsterEntity(base.NextFighterId, knight.GetGrades().ElementAt(knighLevel)), Victim.Team, Victim.Team.FreePlace.Id);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        public override void OnCharacterJoin(CharacterEntity character, FightTeam team)
        {
            if (!IsNeutralAgression)
                character.EnableAlignment();
            else            
                if (((CharacterEntity)team.Leader).CharacterAlignment.AlignmentId != (int)AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                    character.EnableAlignment();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public override bool CanJoin(CharacterEntity character)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fighter"></param>
        /// <param name="kick"></param>
        /// <returns></returns>
        public override FightActionResultEnum FightQuit(FighterBase fighter, bool kick = false)
        {
            if (LoopState == FightLoopStateEnum.STATE_WAIT_END || LoopState == FightLoopStateEnum.STATE_ENDED)
                return FightActionResultEnum.RESULT_NOTHING;

            switch (State)
            {
                case FightStateEnum.STATE_PLACEMENT:
                    if (fighter.IsLeader)
                    {
                        foreach (var teamFighter in fighter.Team.Fighters)
                        {
                            if (base.TryKillFighter(teamFighter, teamFighter.Id, true, true) == FightActionResultEnum.RESULT_END)
                            {
                                return FightActionResultEnum.RESULT_END;
                            }
                        }

                        return FightActionResultEnum.RESULT_END;
                    }
                    else
                    {
                        fighter.Fight.Dispatch(WorldMessage.FIGHT_FLAG_UPDATE(OperatorEnum.OPERATOR_REMOVE, fighter.Team.LeaderId, fighter));
                        fighter.Fight.Dispatch(WorldMessage.GAME_MAP_INFORMATIONS(OperatorEnum.OPERATOR_REMOVE, fighter));
                        fighter.LeaveFight();
                        fighter.Dispatch(WorldMessage.FIGHT_LEAVE());

                        return FightActionResultEnum.RESULT_NOTHING;
                    }

                case FightStateEnum.STATE_FIGHTING:
                    if (fighter.IsSpectating)
                    {
                        fighter.LeaveFight(kick);
                        fighter.Dispatch(WorldMessage.FIGHT_LEAVE());

                        return FightActionResultEnum.RESULT_NOTHING;
                    }

                    if (TryKillFighter(fighter, fighter.Id, true, true) != FightActionResultEnum.RESULT_END)
                    {
                        fighter.LeaveFight();
                        fighter.Dispatch(WorldMessage.FIGHT_LEAVE());

                        return FightActionResultEnum.RESULT_DEATH;
                    }

                    return FightActionResultEnum.RESULT_END;
            }

            return FightActionResultEnum.RESULT_NOTHING;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void InitEndCalculation()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ApplyEndCalculation()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void SerializeAs_FightList(StringBuilder message)
        {
            message.Append(Id.ToString()).Append(';');
            message.Append(UpdateTime).Append(';');
            message.Append("0,");
            message.Append(Aggressor.CharacterAlignment.AlignmentId).Append(",");
            message.Append(Team0.AliveFighters.Count()).Append(';');
            message.Append("0,");
            message.Append(Victim.CharacterAlignment.AlignmentId).Append(",");
            message.Append(Team1.AliveFighters.Count()).Append(';');
            message.Append('|');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void SerializeAs_FightFlag(StringBuilder message)
        {
            if (m_serializedFlag == null)
            {
                m_serializedFlag = new StringBuilder();
                m_serializedFlag.Append(Id).Append(';');
                m_serializedFlag.Append((int)Type).Append('|');
                m_serializedFlag.Append(Team0.LeaderId).Append(';');
                m_serializedFlag.Append(Team0.FlagCellId).Append(';');
                m_serializedFlag.Append('0').Append(';');
                m_serializedFlag.Append(Aggressor.CharacterAlignment.AlignmentId).Append('|');
                m_serializedFlag.Append(Team1.LeaderId).Append(';');
                m_serializedFlag.Append(Team1.FlagCellId).Append(';');
                m_serializedFlag.Append('0').Append(';');
                m_serializedFlag.Append(Victim.CharacterAlignment.AlignmentId);
            }
            message.Append(m_serializedFlag.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            Aggressor = null;
            Victim = null;

            m_serializedFlag.Clear();
            m_serializedFlag = null;

            base.Dispose();
        }
    }
}
