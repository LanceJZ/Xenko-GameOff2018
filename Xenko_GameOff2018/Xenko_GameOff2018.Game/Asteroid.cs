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
    public class Asteroid : PO
    {


        public override void Start()
        {
            base.Start();

            Position.X = RandomMinMax(-Edge.X, Edge.X);
            Position.Y = RandomMinMax(-Edge.Y, Edge.Y);
            Position.Z = -50;

            float rAmount = RandomMinMax(-5, 5);
            float vAmount = RandomMinMax(-20, 20);

            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);
            float rZ = RandomMinMax(-rAmount, rAmount);

            float vX = RandomMinMax(-vAmount, vAmount);
            float vY = RandomMinMax(-vAmount, vAmount);
            float vZ = RandomMinMax(-vAmount, vAmount);

            RotationVelocity = new Vector3(rX, rY, rZ);
            Velocity = new Vector3(vX, vY, vZ);

            Active = true;

        }

        public override void Update()
        {

        }
    }
}
