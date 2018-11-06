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
using Xenko.Audio;

namespace Xenko_GameOff2018
{
    public class Asteroid : PO
    {
        Player PlayerRef;
        List<EnemyBase> EnemyBaseRefs;
        SceneControl SceneRef;

        public override void Start()
        {
            base.Start();

            Position.X = RandomMinMax(-Edge.X, Edge.X);
            Position.Y = RandomMinMax(-Edge.Y, Edge.Y);
            Position.Z = 0;

            float rAmount = RandomMinMax(-1, 1);
            float vAmount = RandomMinMax(-20, 20);

            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);
            float rZ = RandomMinMax(-rAmount, rAmount);

            float vX = RandomMinMax(-vAmount, vAmount);
            float vY = RandomMinMax(-vAmount, vAmount);

            RotationVelocity = new Vector3(rX, rY, 0);
            Velocity = new Vector3(vX, vY, 0);

            Active = true;

        }

        public override void Update()
        {
            base.Update();

            if (CheckForEdge())
                MoveToOppisiteEdge();

        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
        }
    }
}
