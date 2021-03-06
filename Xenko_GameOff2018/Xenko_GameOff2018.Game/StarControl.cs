﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;

namespace Xenko_GameOff2018
{
    public class StarControl : SyncScript
    {

        public override void Start()
        {
            Prefab StarPF = Content.Load<Prefab>("Prefabs/StarPF");

            for (int i = 0; i < 300; i++)
            {
                SceneSystem.SceneInstance.RootScene.Entities.Add(StarPF.Instantiate().First());
            }
        }

        public override void Update()
        {

        }
    }
}