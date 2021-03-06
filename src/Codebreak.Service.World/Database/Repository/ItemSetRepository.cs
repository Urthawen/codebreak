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
    public sealed class ItemSetRepository : Repository<ItemSetRepository, ItemSetDAO>
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, ItemSetDAO> m_setById;

        /// <summary>
        /// 
        /// </summary>
        public ItemSetRepository()
        {
            m_setById = new Dictionary<int, ItemSetDAO>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public override void OnObjectAdded(ItemSetDAO set)
        {
            m_setById.Add(set.Id, set);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public override void OnObjectRemoved(ItemSetDAO set)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemSetDAO GetSetById(int id)
        {
            if(m_setById.ContainsKey(id))
                return m_setById[id];
            return null;
        }


        public override void UpdateAll(MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
        }

        public override void DeleteAll(MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
        }

        public override void InsertAll(MySql.Data.MySqlClient.MySqlConnection connection, MySql.Data.MySqlClient.MySqlTransaction transaction)
        {
        }
    }
}
