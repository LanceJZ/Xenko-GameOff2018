using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Xenko_GameOff2018
{
    public class PlayerBase : PO
    {
        int OreCount = 0;

        public override void Start()
        {
            base.Start();

            TheRadius = 65;
            Position.Z = 70;
        }

        public override void Update()
        {
            base.Update();


        }

        public void UnloadOre(OreType oreType)
        {
            OreCount++;
        }
    }
}
