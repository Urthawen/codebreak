﻿using Codebreak.Framework.Database;
using Codebreak.Service.World.Database.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Database.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CharacterWaypointRepository : Repository<CharacterWaypointRepository, CharacterWaypointDAO>
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<long, List<CharacterWaypointDAO>> m_waypointByCharacter;

        /// <summary>
        /// 
        /// </summary>
        public CharacterWaypointRepository()
        {
            m_waypointByCharacter = new Dictionary<long, List<CharacterWaypointDAO>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public override void OnObjectAdded(CharacterWaypointDAO waypointDAO)
        {
            if (!m_waypointByCharacter.ContainsKey(waypointDAO.CharacterId))
                m_waypointByCharacter.Add(waypointDAO.CharacterId, new List<CharacterWaypointDAO> ());
            m_waypointByCharacter[waypointDAO.CharacterId].Add(waypointDAO);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public List<CharacterWaypointDAO> GetByCharacterId(long characterId)
        {
            if (!m_waypointByCharacter.ContainsKey(characterId))
                m_waypointByCharacter.Add(characterId, new List<CharacterWaypointDAO>());
            return m_waypointByCharacter[characterId];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        public override void UpdateAll(MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
            // NO UPDATE
        }

        public void RemoveAll(long characterId)
        {
            base.Removed(GetByCharacterId(characterId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterId"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public CharacterWaypointDAO Create(long characterId, int mapId)
        {
            var instance = new CharacterWaypointDAO()
            {
                CharacterId = characterId,
                MapId = mapId,
            };
            base.Created(instance);
            return instance;
        }
    }
}
