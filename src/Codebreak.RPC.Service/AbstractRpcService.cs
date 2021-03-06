﻿using System;
using System.Collections.Generic;
using Codebreak.Framework.Network;

namespace Codebreak.RPC.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TServer"></typeparam>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TMessageBuilder"></typeparam>
    public abstract class AbstractRpcService<TServer, TClient, TMessageBuilder> : AbstractTcpServer<TServer, TClient>
        where TServer : AbstractRpcService<TServer, TClient, TMessageBuilder>, new()
        where TClient : AbstractRpcClient<TClient>, new()
        where TMessageBuilder : RpcMessageBuilder, new ()
    {
        /// <summary>
        /// 
        /// </summary>
        public RpcMessageBuilder MessageBuilder
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, Action<TClient, AbstractRcpMessage>> m_handlers;

        /// <summary>
        /// 
        /// </summary>
        protected AbstractRpcService()
        {
            m_handlers = new Dictionary<int, Action<TClient, AbstractRcpMessage>>();
            MessageBuilder = new TMessageBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(int messageId, Action<TClient, AbstractRcpMessage> handler)
        {
            if (m_handlers.ContainsKey(messageId))
                throw new InvalidOperationException(string.Format("RPCService::RegisterHandler already registered handler for messageId = {0}", messageId));
            else
                m_handlers.Add(messageId, handler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void HandleMessage(TClient client, AbstractRcpMessage message)
        {
            if (!m_handlers.ContainsKey(message.Id))
                Logger.Debug(string.Format("RPCService::HandlerMessage unregistered handler for messageId={0}", message.Id));
            else
                // execute in context
                AddMessage(() => m_handlers[message.Id](client, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected override void OnClientConnected(TClient client)
        {
            client.MessageBuilder = MessageBuilder;

            // execute in thread context
            AddMessage(() => OnRPCClientConnected(client));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected override void OnClientDisconnected(TClient client)
        {
            // execute in thread context
            AddMessage(() => OnRPCClientDisconnected(client));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        protected override void OnDataReceived(TClient client, byte[] buffer, int offset, int count)
        {
            foreach (var message in client.GetMessages(buffer, offset, count))
            {
                // execute in context
                AddMessage(() => OnMessageReceived(client, message));

                HandleMessage(client, message);  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected abstract void OnRPCClientConnected(TClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        protected abstract void OnRPCClientDisconnected(TClient client);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        protected abstract void OnMessageReceived(TClient client, AbstractRcpMessage message);
    }
}
