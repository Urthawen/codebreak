﻿using Codebreak.Service.World.Database.Structure;
using Codebreak.Service.World.Game.Entity;
using Codebreak.Service.World.Game.Stats;
using Codebreak.Service.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.ActionEffect
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AddLifeEffect : ActionEffectBase<AddLifeEffect>
    {
        const int EMOTE_EAT_BREAD = 17;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="item"></param>
        /// <param name="effect"></param>
        /// <param name="targetId"></param>
        /// <param name="targetCell"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override bool ProcessItem(EntityBase entity, InventoryItemDAO item, GenericStats.GenericEffect effect, long targetId, int targetCell)
        {            
            if(entity.Type == EntityTypeEnum.TYPE_CHARACTER)
                entity.Dispatch(WorldMessage.ACCOUNT_STATS((CharacterEntity)entity));

            switch((ItemTypeEnum)item.GetTemplate().Type)
            {
                case ItemTypeEnum.TYPE_PAIN:
                    entity.Map.Dispatch(WorldMessage.EMOTE_PLAY(entity.Id, EMOTE_EAT_BREAD));
                    break;
                    
                default:
                    break;
            }

            Process(entity, new Dictionary<string, string>() { { "life", effect.Items.ToString() } });

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parameters"></param>
        public override void Process(EntityBase entity, Dictionary<string, string> parameters)
        {
            var heal = int.Parse(parameters["life"]);

            if (entity.Life + heal > entity.MaxLife)
                heal = entity.MaxLife - entity.Life;

            entity.Life += heal;
        }
    }
}