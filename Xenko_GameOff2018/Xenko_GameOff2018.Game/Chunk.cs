using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;

namespace Xenko_GameOff2018
{
    public class Chunk : PO
    {
        public OreType ThisOreType { get => TypeofOre; }

        OreType TypeofOre;

        public override void Start()
        {
            base.Start();

            Radius = 5;

            float rAmount = RandomMinMax(0.1f, 1);
            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);

            RotationVelocity = new Vector3(rX, rY, 0);

            Active = true;

            TypeofOre = (OreType)RandomGenerator.Next(0, 6);
        }

        public override void Update()
        {
            base.Update();

        }
    }
}
