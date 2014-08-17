﻿using System;
using Codebreak.Framework.Network;
using Codebreak.Service.World.Game;
using Codebreak.Service.World.Game.Entity;

namespace Codebreak.Service.World.Frames
{
    public sealed class SpellFrame : FrameBase<SpellFrame, EntityBase, string>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override Action<EntityBase, string> GetHandler(string message)
        {
            if (message.Length < 2)
                return null;

            switch (message[0])
            {
                case 'S':
                    switch (message[1])
                    {
                        case 'M':
                            return SpellMove;

                        case 'B':
                            return SpellBoost;
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void SpellMove(EntityBase entity, string message)
        {
            var data = message.Substring(2).Split('|');
            if (data.Length != 2)
            {
                entity.Dispatch(WorldMessage.BASIC_NO_OPERATION());
                return;
            }

            var spellId = int.Parse(data[0]);
            var position = int.Parse(data[1]);

            if (entity.Spells == null)
            {
                entity.Dispatch(WorldMessage.BASIC_NO_OPERATION());
                return;
            }

            if (!entity.Spells.HasSpell(spellId))
            {
                entity.Dispatch(WorldMessage.BASIC_NO_OPERATION());
                return;
            }

            entity.AddMessage(() =>
                {
                    entity.Spells.MoveSpell(spellId, position);
                    entity.Dispatch(WorldMessage.BASIC_NO_OPERATION());
                });
        }

        private void SpellBoost(EntityBase entity, string message)
        {
            var spellId = -1;
            if (!int.TryParse(message.Substring(2), out spellId))
            {
                entity.Dispatch(WorldMessage.SPELL_UPGRADE_ERROR());
                return;
            }

            if (entity.Spells == null)
            {
                entity.Dispatch(WorldMessage.SPELL_UPGRADE_ERROR());
                return;
            }

            if (!entity.Spells.HasSpell(spellId))
            {
                entity.Dispatch(WorldMessage.SPELL_UPGRADE_ERROR());
                return;
            }
            
            if(entity.Type != EntityTypEnum.TYPE_CHARACTER)
            {
                entity.Dispatch(WorldMessage.SPELL_UPGRADE_ERROR());
                return;
            }

            entity.AddMessage(() =>
                {
                    var spell = entity.Spells.GetSpellLevel(spellId);
                    var characterEntity = (CharacterEntity)entity;

                    if (characterEntity.SpellPoint < spell.Level)
                    {
                        entity.Dispatch(WorldMessage.SPELL_UPGRADE_ERROR());
                        return;
                    }

                    entity.Spells.LevelUp(spellId);
                    characterEntity.SpellPoint -= spell.Level;

                    entity.Dispatch(WorldMessage.SPELL_UPGRADE_SUCCESS(spellId, spell.Level + 1));
                    entity.Dispatch(WorldMessage.ACCOUNT_STATS(characterEntity));
                });
        }
    }
}