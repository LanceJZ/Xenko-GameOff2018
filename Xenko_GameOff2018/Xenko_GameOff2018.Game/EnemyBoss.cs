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
    public class EnemyBoss : PO
    {
        public List<Missile> MissilesAccess { get => Missiles; }

        SceneControl SceneRef;
        Player PlayerRef;
        List<Missile> Missiles;
        Prefab MissilePF;
        Vector3 CurrentHeading = Vector3.Zero;
        Timer ChaseTimer;
        Timer BumpTimer;
        Timer FireTimer;
        bool PlayerInRange;
        bool WasBumped;
        float Thrust = 5.666f;

        public override void Start()
        {
            base.Start();

            Missiles = new List<Missile>();

            MissilePF = Content.Load<Prefab>("Prefabs/MissilePF");
            SetModel();

            Entity chaseTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(chaseTimerE);
            ChaseTimer = chaseTimerE.Get<Timer>();

            Entity bumpTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(bumpTimerE);
            BumpTimer = bumpTimerE.Get<Timer>();

            Entity fireTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(fireTimerE);
            FireTimer = fireTimerE.Get<Timer>();

            TheRadius = 20;
            Deceleration = 0.025f;
            HeadingSpeed = MathUtil.PiOverTwo;
        }

        public override void Update()
        {
            base.Update();

            if (!Active)
                return;

            if (HitEdge())
                MoveToOppisiteEdge();

            if (WasBumped)
            {
                RecoverFromBump();
                return;
            }

            if (PlayerInRange)
            {
                MaxVelocity = 0;

                if (FireTimer.Expired)
                {
                    FireTimer.Reset(RandomMinMax(2, 5));
                    FireMissile();
                }
            }
            else
            {
                MaxVelocity = 50;
                Accelerate(Thrust);
            }

            if (ChaseTimer.Expired)
            {
                ChaseTimer.Reset(RandomMinMax(1, 3));
                SetHeading(PlayerRef);
            }

            CheckForPlayer();
        }

        public void Destroy()
        {
            SceneRef.RadarAccess.CreateEnemyBossCubes();
            IsActive = false;
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerRefAccess;
        }

        public void Launch(Vector3 position)
        {
            IsActive = true;
            Position = position;
            UpdatePR();
        }

        public void Bumped(Vector3 position, Vector3 velocity)
        {
            Deceleration = 0;
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += velocity * 0.95f;
            Velocity -= VelocityFromVectors(position, 15);
            RotationVelocity.Z = RandomMinMax(-MathUtil.PiOverFour, MathUtil.PiOverFour);
            RotationVelocity.X = RandomMinMax(-MathUtil.PiOverFour, MathUtil.PiOverFour);
            RotationVelocity.Y = RandomMinMax(-MathUtil.PiOverFour, MathUtil.PiOverFour);
            BumpTimer.Reset(RandomMinMax(2, 6));
            WasBumped = true;
        }

        void RecoverFromBump()
        {
            if (BumpTimer.Expired)
            {
                Rotation = new Vector3(0, 0, Rotation.Z);
                RotationVelocity = Vector3.Zero;
                Velocity = Vector3.Zero;
                Deceleration = 0.001f;
                WasBumped = false;
            }
        }

        void CheckCollusion()
        {

        }

        void CheckForPlayer()
        {
            if (Vector3.Distance(Position, PlayerRef.Position) > 300)
            {
                PlayerInRange = false;
            }
            else
            {
                PlayerInRange = true;
            }
        }

        void FireMissile()
        {
            bool found = false;
            Missile thisMissile = null;

            foreach (Missile missile in Missiles)
            {
                if (!missile.Active)
                {
                    thisMissile = missile;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                thisMissile = SceneRef.SetupEntity(MissilePF).Get<Missile>();
                Missiles.Add(thisMissile);
                thisMissile.Setup(SceneRef);
            }

            thisMissile.Fire(Position + VelocityFromRadian(Radius - 5, Rotation.Z), Rotation.Z);

            //FireSI.Stop();
            //FireSI.Play();
        }
    }
}
