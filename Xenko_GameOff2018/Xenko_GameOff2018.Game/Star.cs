using System;
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
    public class Star : PO
    {

        public override void Start()
        {
            base.Start();

            float outterBuffer = 100;

            Position.X = RandomMinMax(-Edge.X + outterBuffer, Edge.X + outterBuffer);
            Position.Y = RandomMinMax(-Edge.Y + outterBuffer, Edge.Y + outterBuffer);
            Position.Z = -75;

            float amount = RandomMinMax(-5, 5);

            float x = RandomMinMax(-amount, amount);
            float y = RandomMinMax(-amount, amount);
            float z = RandomMinMax(-amount, amount);

            RotationVelocity = new Vector3(x, y, z);

            IsActive = true;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}