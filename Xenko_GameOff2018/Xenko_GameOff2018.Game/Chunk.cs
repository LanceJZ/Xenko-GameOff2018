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
        public bool IsInTransit { get => InTransit; set => InTransit = value; }

        OreType TypeofOre;
        Player PlayerRef;

        bool InTransit;

        public override void Start()
        {
            base.Start();

            float rAmount = RandomMinMax(0.1f, 1);
            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);

            TheRadius = 5;
            RotationVelocity = new Vector3(rX, rY, 0);
            IsActive = true;
            TypeofOre = (OreType)RandomGenerator.Next(0, 6);
        }

        public override void Update()
        {
            base.Update();

            if (Active)
            {
                if (HitEdge())
                    MoveToOppisiteEdge();

                CheckCollusion();
            }
        }

        public void Setup(Player player)
        {
            PlayerRef = player;
            RandomGenerator = SceneControl.RandomGenerator;
        }

        public void Enable()
        {
            IsActive = true;
        }

        public void Disable()
        {
            IsActive = false;
        }

        void CheckCollusion()
        {
            if (PlayerRef.Active)
            {
                if (CirclesIntersect(PlayerRef))
                {
                    PlayerRef.PickupOre(this);
                    InTransit = true;
                }
            }
        }
    }
}
