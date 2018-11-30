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
    public class Missile : PO
    {
        Timer LifeTimer;
        List<Asteroid> AsteroidRefs;
        Player PlayerRef;
        SceneControl SceneRef;

        SoundInstance HitSI;
        SoundInstance FireSI;

        float Thrust = 10;
        float TurnRate = 0.25f;

        public override void Start()
        {
            base.Start();

            MaxVelocity = 100;
            TheRadius = 5;
            RotationVelocity = new Vector3(3.666f, 0, 0);
            Deceleration = 0.01f;
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

            if (PlayerRef != null)
            {
                RotationVelocity.Z = AimAtTarget(PlayerRef.Position, Rotation.Z, MathUtil.PiOverTwo * TurnRate);
            }

            Accelerate(Thrust);

            CheckCollusion();
        }

        public void Fire(Vector3 position, float rotation)
        {
            FireSI.Stop();
            FireSI.Play();
            IsActive = true;
            Position = position;
            Position.Z = 0;
            Rotation.Z = rotation;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;

            if (LifeTimer == null)
            {
                Entity lifeTimerE = new Entity { new Timer() };
                SceneSystem.SceneInstance.RootScene.Entities.Add(lifeTimerE);
                LifeTimer = lifeTimerE.Get<Timer>();
            }

            LifeTimer.Reset(6);
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            AsteroidRefs = scene.AsteroidAccess;
            PlayerRef = scene.PlayerAccess;
            RandomGenerator = SceneControl.RandomGenerator;

            HitSI = Content.Load<Sound>("Sounds/MissileHitSound").CreateInstance();
            FireSI = Content.Load<Sound>("Sounds/MissileFiredSound").CreateInstance();
        }

        public void Disable()
        {
            HitSI.Stop();
            HitSI.Play();
            IsActive = false;
        }

        void CheckCollusion()
        {
            foreach(Asteroid rock in AsteroidRefs)
            {
                if (rock.Active)
                {
                    if (CirclesIntersect(rock))
                    {
                        Disable();
                    }
                }
            }

            if (PlayerRef != null)
            {
                if (CirclesIntersect(PlayerRef))
                {
                    Disable();
                    PlayerRef.Damaged();
                    PlayerRef.HPAccess = -10;
                    return;
                }

                foreach (PlayerShot shot in PlayerRef.ShotsAccess)
                {
                    if (shot.Active)
                    {
                        if (CirclesIntersect(shot))
                        {
                            SceneRef.PlayerScore(10);
                            Disable();
                            shot.Disable();
                            return;
                        }
                    }
                }
            }
        }
    }
}
