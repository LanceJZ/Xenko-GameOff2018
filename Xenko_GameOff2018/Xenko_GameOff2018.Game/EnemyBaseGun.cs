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
    public class EnemyBaseGun : PO
    {
        Player PlayerRef;

        public override void Start()
        {
            base.Start();

            Radius = 5;
        }

        public override void Update()
        {
            base.Update();

            Rotation.Z = AngleFromVectors(Position, PlayerRef.Position);
        }

        public void Setup(SceneControl scene)
        {
            PlayerRef = scene.PlayerRefAccess;
            Active = true;
        }

        public bool CheckCollusion(Vector3 position, float radius)
        {
            return CirclesIntersect(position, radius);
        }
    }
}
