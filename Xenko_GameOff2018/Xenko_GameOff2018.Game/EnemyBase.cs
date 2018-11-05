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
    public class EnemyBase : PO
    {


        public override void Start()
        {
            base.Start();

            int spawn = RandomGenerator.Next(3);
            Vector2 outsideBuffer = new Vector2(400, 350);
            float innerBuffer = 200;

            switch (spawn)
            {
                case 0:
                    Position.X = RandomMinMax(-Edge.X + outsideBuffer.X, Edge.X - outsideBuffer.X);
                    Position.Y = RandomMinMax(innerBuffer, Edge.Y - outsideBuffer.Y);
                    break;
                case 1:
                    Position.X = RandomMinMax(innerBuffer, Edge.X - outsideBuffer.X);
                    Position.Y = RandomMinMax(-innerBuffer, innerBuffer);
                    break;
                case 2:
                    Position.X = RandomMinMax(-Edge.X + outsideBuffer.X, Edge.X - outsideBuffer.X);
                    Position.Y = RandomMinMax(-innerBuffer, -Edge.Y + outsideBuffer.Y);
                    break;
                case 3:
                    Position.X = RandomMinMax(-innerBuffer, -Edge.X + outsideBuffer.X);
                    Position.Y = RandomMinMax(-innerBuffer, innerBuffer);
                    break;
            }

            Position.Z = 0;
            Active = true;
        }

        public override void Update()
        {
            base.Update();


        }
    }
}
