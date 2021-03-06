﻿using System;
using System.Text;
using System.Linq;
using Codebreak.Framework.Command;
using Codebreak.Service.World.Database.Structure;
using Codebreak.Service.World.Game;
using Codebreak.Service.World.Database.Repository;
using Codebreak.Service.World.Game.Entity;
using Codebreak.Service.World.Manager;
using Codebreak.Service.World.Network;
using Codebreak.Service.World.Game.Spell;
using Codebreak.Service.World.Game.Action;
using System.Collections.Generic;

namespace Codebreak.Service.World.Command
{
    public sealed class CharacterCommand : Command<WorldCommandContext>
    {
        private readonly string[] _aliases =
        {
            "character"
        };

        public override string[] Aliases => _aliases;

        public override string Description => "Character management commands.";

        protected override bool CanExecute(WorldCommandContext context)
        {
            return true;
        }

        protected override void Process(WorldCommandContext context)
        {
            context.Character.Dispatch(WorldMessage.BASIC_NO_OPERATION());
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class TitleCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "title"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Change your character title. Arguments : %titleId%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                int titleId = 0;
                if (Int32.TryParse(context.TextCommandArgument.NextWord(), out titleId))
                {
                    context.Character.TitleId = titleId;
                    context.Character.RefreshOnMap();
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character title %titleId%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class SizeCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "size"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Change your character skin. Arguments : %skinId%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                int size = 0;
                if (Int32.TryParse(context.TextCommandArgument.NextWord(), out size))
                {
                    context.Character.DatabaseRecord.SkinSize = size;
                    context.Character.RefreshOnMap();
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character size %size%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class EffectCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "effect"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Apply a specified effect to your character. Arguments : %effectId";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                int effectId = 0;
                if (!Int32.TryParse(context.TextCommandArgument.NextWord(), out effectId))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character effect %effectId%"));
                    return;
                }

                var parameters = new Dictionary<string, string>();
                foreach(var parameter in context.TextCommandArgument.NextWord().Split(','))
                {
                    if (parameter.Contains('='))
                    {
                        var data = parameter.Split('=');
                        parameters.Add(data[0], data[1]);
                    }
                }
                ActionEffectManager.Instance.ApplyEffect(context.Character, (EffectEnum)effectId, parameters);
            }
        }

        public sealed class MorphCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "skin"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Modify the player skin.";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                int skinId = 0;
                if(Int32.TryParse(context.TextCommandArgument.NextWord(), out skinId))
                {
                    context.Character.DatabaseRecord.Skin = skinId;
                    context.Character.RefreshOnMap();
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character morph %skinId%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class WarnCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "warn"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Warn a player. Arguments : %playerName%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                var characterName = context.TextCommandArgument.NextWord();
                var reason = context.TextCommandArgument.NextWord();

                WorldService.Instance.AddMessage(() =>
                {
                    var character = EntityManager.Instance.GetCharacterByName(characterName);
                    if (character == null)
                    {
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player not found."));
                        return;
                    }

                    character.SafeDispatch(WorldMessage.INFORMATION_MESSAGE(InformationTypeEnum.INFO, InformationEnum.INFO_BASIC_WARNING_BEFORE_SANCTION, reason));
                    context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player warned."));
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class EmoteCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "emote"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Play an emote on the map. Arguments %emoteId%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                var emoteId = -1;
                if(!Int32.TryParse(context.TextCommandArgument.NextWord(), out emoteId))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character emote %emoteId%"));
                    return;
                }

                foreach(var entity in context.Character.Map.Entities)
                    entity.AddMessage(() => entity.EmoteUse(emoteId));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class AlignmentResetCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "alignmentreset"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Reset your character alignment.";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                context.Character.ResetAlignment();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class AlignmentCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "alignment"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Set a new alignment to your character. Arguments : %alignmentId%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                var alignmentId = -1;
                if(!Int32.TryParse(context.TextCommandArgument.NextWord(), out alignmentId))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character alignment %alignementId%"));
                    return;
                }
                context.Character.SetAlignment(alignmentId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class AddHonorCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "addhonor"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Will honor your character. Arguments : %honorValue%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                var honorValue = -1;
                if(!Int32.TryParse(context.TextCommandArgument.NextWord(), out honorValue))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character addhonor %honorValue%"));
                    return;
                }

                context.Character.AddHonour(honorValue);                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class KickCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "kick"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Kick a player. Arguments : %playerName% %reason%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                var characterName = context.TextCommandArgument.NextWord();
                var reason = context.TextCommandArgument.NextWord();

                WorldService.Instance.AddMessage(() =>
                {
                    var character = EntityManager.Instance.GetCharacterByName(characterName);
                    if (character == null)
                    {
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player not found."));
                        return;
                    }

                    if (character.Account.Power >= context.Character.Account.Power)
                    {
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("This player is a god, god cannot be kicked. In addition, he will be noticed."));
                        character.SafeDispatch(WorldMessage.SERVER_ERROR_MESSAGE("Player " + context.Character.Name + " tried to kick you."));
                        return;
                    }

                    character.SafeKick(context.Character.Name, reason);
                    context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player kicked successfully."));
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class LifeCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "life"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Restore your life.";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                context.Character.Life = context.Character.MaxLife;
                context.Character.Dispatch(WorldMessage.ACCOUNT_STATS(context.Character));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class GuildCreateCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "guild"  
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Open a guild creation panel";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return base.CanExecute(context);
            }

            protected override void Process(WorldCommandContext context)
            {
                if (context.Character.CanGameAction(Game.Action.GameActionTypeEnum.GUILD_CREATE))
                {
                    context.Character.GuildCreationOpen();
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to start a guild creation in your actual state."));
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public sealed class TeleportMeCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "teleme"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Teleport a player to your location. Arguments : %playerName%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                //if (context.Character.Power < 1)
                //{
                //    context.Character.Dispatch(WorldMessage.SERVER_ERROR_MESSAGE("You're not admin, your attempt was registered"));
                //    return false;
                //}

                return true;
            }

            protected override void Process(WorldCommandContext context)
            {
                string characterName = context.TextCommandArgument.NextWord();
                WorldService.Instance.AddMessage(() =>
                {
                    var character = EntityManager.Instance.GetCharacterByName(characterName);
                    if (character == null)
                    {
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player not found."));
                        return;
                    }

                    var mapId = context.Character.MapId;
                    var cellId = context.Character.CellId;

                    character.AddMessage(() =>
                    {
                        if (!character.CanGameAction(Game.Action.GameActionTypeEnum.MAP_TELEPORT))
                        {
                            context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to teleport remote player due to his actual state."));
                            return;
                        }

                        character.Teleport(mapId, cellId);
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player teleported successfully."));
                    });
                });         
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class TeleportToCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "teleto"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Teleport yourself to a player location. Arguments : %playerName%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                //if (context.Character.Power < 1)
                //{
                //    context.Character.Dispatch(WorldMessage.SERVER_ERROR_MESSAGE("You're not admin, your attempt was registered"));
                //    return false;
                //}

                return true;
            }

            protected override void Process(WorldCommandContext context)
            {                
                string characterName = context.TextCommandArgument.NextWord();                
                WorldService.Instance.AddMessage(() => 
                    {
                        var character = EntityManager.Instance.GetCharacterByName(characterName);
                        if(character == null)
                        {
                            context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player not found."));
                            return;
                        }

                        var mapId = character.MapId;
                        var cellId = character.CellId;

                        context.Character.AddMessage(() =>
                            {
                                if (!context.Character.CanGameAction(Game.Action.GameActionTypeEnum.MAP_TELEPORT))
                                {
                                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to teleport yourself in your actual state."));
                                    return;
                                }

                                context.Character.Teleport(mapId, cellId);
                            });
                    });                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class ResetSpellCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "resetspell"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Reset the spells of a player. Arguments : %playerName%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                return true;
            }

            protected override void Process(WorldCommandContext context)
            {
                string characterName = context.TextCommandArgument.NextWord();
                WorldService.Instance.AddMessage(() =>
                {
                    var character = EntityManager.Instance.GetCharacterByName(characterName);
                    if (character == null)
                    {
                        context.Character.SafeDispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Player not found."));
                        return;
                    }

                    character.AddMessage(() =>
                    {
                        character.HardResetSpells();
                    });
                });
            }
        }
 
        /// <summary>
        /// 
        /// </summary>
        public sealed class TeleportCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases = 
            {
                "tele"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Teleport yourself at the desired location. Arguments : %mapId% %cellId%";

            protected override bool CanExecute(WorldCommandContext context)
            {
                //if (context.Character.Power < 1)
                //{
                //    context.Character.Dispatch(WorldMessage.SERVER_ERROR_MESSAGE("You're not admin, your attempt was registered"));
                //    return false;
                //}

                return true;
            }

            protected override void Process(WorldCommandContext context)
            {
                int mapId;
                if(Int32.TryParse(context.TextCommandArgument.NextWord(), out mapId))
                {
                    var map = MapManager.Instance.GetById(mapId);
                    if (map != null)
                    {
                        int cellId;
                        if (Int32.TryParse(context.TextCommandArgument.NextWord(), out cellId))
                        {
                            var cell = map.GetCell(cellId);
                            if (cell == null || !cell.Walkable)
                            {                                
                                context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Null cell or not walkable"));
                                return;
                            }

                            if (cellId != -1)
                            {
                                if (context.Character.CanGameAction(Game.Action.GameActionTypeEnum.MAP_TELEPORT))
                                {
                                    context.Character.Teleport(mapId, cellId);
                                }
                                else
                                {
                                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to teleport yourself in your actual state."));
                                }
                            }
                            else
                            {
                                context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("No cell available to be teleported on"));
                            }
                        }
                        else
                        {
                            context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character teleport %mapId% %cellId%"));
                        }
                    }
                    else
                    {
                        context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unknow mapId"));
                    }
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character teleport %mapId% %cellId%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class OnlineCharacterSubCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "online"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Check out whos online.";

            protected override void Process(WorldCommandContext context)
            {
                WorldService.Instance.AddMessage(() =>
                    {
                        var message = new StringBuilder("Online players " + ClientManager.Instance.Clients.Count() + " :\n");

                        int i = 1;
                        foreach(var client in ClientManager.Instance.Clients)
                        {
                            if(client.CurrentCharacter != null)
                            {
                                message.Append(i++ + " : account(" + client.Account.Name + ") " + client.CurrentCharacter.Name + " map(" + client.CurrentCharacter.MapId + ") ip(" + client.Ip + ")\n");
                            }
                        }
                        context.Character.AddMessage(() =>
                            {
                                context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE(message.ToString()));
                            });
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class LevelUpSubCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "level"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Level up your character. Arguments : %level%";

            protected override void Process(WorldCommandContext context)
            {
                int level;
                if (Int32.TryParse(context.TextCommandArgument.NextWord(), out level))
                {
                    if (level > context.Character.Level)
                    {
                        while (level > context.Character.Level)                        
                            context.Character.LevelUp();
                        
                        context.Character.Dispatch(WorldMessage.CHARACTER_NEW_LEVEL(context.Character.Level));
                        context.Character.Dispatch(WorldMessage.SPELLS_LIST(context.Character.SpellBook));
                        context.Character.Dispatch(WorldMessage.ACCOUNT_STATS(context.Character));
                        context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("You are now level " + level));
                    }
                    else
                    {
                        context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("New level should be higher than yours"));
                    }
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character levelup %level%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class WinFightCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "winfight"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Win the current fight in the favor of your team.";

            protected override void Process(WorldCommandContext context)
            {
                if(!context.Character.HasGameAction(GameActionTypeEnum.FIGHT))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to execute this command out of fight."));
                    return;
                }

                foreach (var fighter in context.Character.Team.OpponentTeam.AliveFighters)
                    fighter.Life = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class WinFightToCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "winfightto"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Win the current fight in the choosen player favor.";

            protected override void Process(WorldCommandContext context)
            {
                if (!context.Character.HasGameAction(GameActionTypeEnum.FIGHT))
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unable to execute this command out of fight."));
                    return;
                }

                var targetName = context.TextCommandArgument.NextWord().Trim();
                var target = context.Character.Fight.Fighters.FirstOrDefault(fighter => fighter.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
                if (target == null)
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unknow character name."));
                    return;
                }

                foreach (var fighter in target.Team.OpponentTeam.AliveFighters)
                    fighter.Life = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class KamasSubCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "kamas"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Add some kamas to your invotentory. Arguments : %kamas%";

            protected override void Process(WorldCommandContext context)
            {
                long kamas;
                if (long.TryParse(context.TextCommandArgument.NextWord(), out kamas))
                {
                    context.Character.Inventory.AddKamas(kamas);
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character kamas %kamas%"));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class ItemSubCommand : SubCommand<WorldCommandContext>
        {
            private readonly string[] _aliases =
            {
                "item"
            };

            public override string[] Aliases => _aliases;

            public override string Description => "Add an item in your inventory, with max jet. Arguments : %templateId%";

            protected override void Process(WorldCommandContext context)
            {
                int templateId;
                if (Int32.TryParse(context.TextCommandArgument.NextWord(), out templateId))
                {
                    var itemTemplate = ItemTemplateRepository.Instance.GetById(templateId);
                    if (itemTemplate != null)
                    {
                        int quantity = 1;
                        if (!int.TryParse(context.TextCommandArgument.NextWord(), out quantity) || quantity == templateId)                        
                            quantity = 1;                        

                        var instance = itemTemplate.Create(quantity, ItemSlotEnum.SLOT_INVENTORY, true);
                        if (instance != null)
                        {
                            context.Character.Inventory.AddItem(instance);
                            context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE(
                                String.Format("Item {0} added in your inventory", itemTemplate.Name)
                                ));
                        }
                    }
                    else
                    {
                        context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Unknow templateId"));
                    }
                }
                else
                {
                    context.Character.Dispatch(WorldMessage.BASIC_CONSOLE_MESSAGE("Command format : character item %templateId%"));
                }
            }
        }
    }
}
