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
            Radius = 5;

            float rAmount = RandomMinMax(0.1f, 1);
            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);

            RotationVelocity = new Vector3(rX, rY, 0);

            float vAmount = RandomMinMax(6, 14);
            float vX = 0;
            float vY = 0;

            switch (RandomGenerator.Next(0, 1))
            {
                case 0:
                    vX = RandomMinMax(vAmount, vAmount + 5);
                    vY = RandomMinMax(vAmount, vAmount + 5);
                    break;
                case 1:
                    vX = RandomMinMax(-vAmount - 5, -vAmount);
                    vY = RandomMinMax(-vAmount - 5, -vAmount);
                    break;
            }

            Velocity = new Vector3(vX, vY, 0);

            Active = true;

            TypeofOre = (OreType)RandomGenerator.Next(0, 6);
        }

        public override void Update()
        {

        }
    }
}
