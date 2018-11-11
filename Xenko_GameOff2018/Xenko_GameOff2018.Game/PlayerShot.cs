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
    public class PlayerShot : PO
    {
        List<Asteroid> AsteroidRefs;
        List<EnemyBase> EnemyBaseRefs;
        Player PlayerRef;

        public override void Start()
        {
            base.Start();

            RotationVelocity = new Vector3(6.666f, 0, 0);
        }

        public override void Update()
        {
            base.Update();

        }

        public void Fire(Vector3 position, Vector3 velocity, float rotation)
        {
            Active = true;
            Position = position;
            Velocity = velocity;
            Rotation.Z = rotation;
        }

        public void Setup(SceneControl scene)
        {
            AsteroidRefs = scene.AsteroidRefAccess;
            EnemyBaseRefs = scene.EnemyBaseRefAccess;
            PlayerRef = scene.PlayerRefAccess;
        }

        void CheckCollision()
        {
            foreach (Asteroid rock in AsteroidRefs)
            {

            }
        }
    }
}
