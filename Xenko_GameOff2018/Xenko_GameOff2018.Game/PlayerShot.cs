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
        Timer LifeTimer;

        public override void Start()
        {
            base.Start();

            RotationVelocity = new Vector3(13.666f, 0, 0);
        }

        public override void Update()
        {
            base.Update();

            if (!IsActive)
                return;

            if (HitEdge())
                MoveToOppisiteEdge();

            if (LifeTimer.Expired)
            {
                Disable();
            }

            CheckCollision();
        }

        public void Fire(Vector3 position, Vector3 velocity, float rotation)
        {
            IsActive = true;
            Position = position;
            Velocity = velocity;
            Rotation.Z = rotation;

            if (LifeTimer == null)
            {
                Entity lifeTimerE = new Entity { new Timer() };
                SceneSystem.SceneInstance.RootScene.Entities.Add(lifeTimerE);
                LifeTimer = lifeTimerE.Get<Timer>();
            }

            LifeTimer.Reset(2);
        }

        public void Setup(SceneControl scene)
        {
            AsteroidRefs = scene.AsteroidAccess;
            EnemyBaseRefs = scene.EnemyBaseAccess;
            PlayerRef = scene.PlayerAccess;
            RandomGenerator = SceneControl.RandomGenerator;
        }

        public void Disable()
        {
            IsActive = false;
        }

        void CheckCollision()
        {
            foreach (Asteroid rock in AsteroidRefs)
            {
                if (rock.Active)
                {
                    if (CirclesIntersect(rock))
                    {
                        Disable();
                    }
                }
            }
        }
    }
}
