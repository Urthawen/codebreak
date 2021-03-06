﻿using Codebreak.RPC.Protocol;
using Codebreak.RPC.Service;
using System.Collections.Generic;

namespace Codebreak.Service.Auth.RPC
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AuthRPCServiceClient : AbstractRpcClient<AuthRPCServiceClient>
    {
        /// <summary>
        /// 
        /// </summary>
        public GameStateEnum GameState
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public AuthStateEnum AuthState
        {
            get;
            set;
        }

        public string RemoteIp
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int GameId
        {
            get;
            set;
        }
                
        /// <summary>
        /// 
        /// </summary>
        public List<long> Players
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public AuthRPCServiceClient()
        {
            GameState = GameStateEnum.OFFLINE;
            AuthState = AuthStateEnum.NEGOTIATING;
            Players = new List<long>();
            GameId = -1;
        }
    }
}
