﻿using Codebreak.Service.World.Game.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Map
{
    public enum FieldTypeEnum
    {
        TYPE_MAP,
        TYPE_FIGHT,
    }

    public interface IMovementHandler
    {
        bool CanAbortMovement
        {
            get;
        }
        FieldTypeEnum FieldType
        {
            get;
        }
        void Move(EntityBase entity, int cellId, string movementPath);
        void MovementFinish(EntityBase entity, MovementPath path, int cellId);
        void Dispatch(string message);
    }
}