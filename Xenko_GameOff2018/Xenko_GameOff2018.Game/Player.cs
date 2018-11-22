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
    public class Player : PO
    {
        public List<PlayerShot> ShotsAccess { get => Shots; }

        ModelComponent FlameRModel;
        ModelComponent FlameLModel;
        Timer MineTimer;
        Timer UnloadTimer;
        Timer BlinkTimerR;
        Timer BlinkTimerL;
        List<Chunk> ChunkRefs;
        List<Asteroid> AsteroidRefs;
        Asteroid NearAsteroid;
        SceneControl SceneRef;
        Prefab ShotPF;
        List<PlayerShot> Shots;

        float ThrustAmount = 3.666f;
        bool ThrustOn;
        bool InDock;

        public Entity TheCamera;

        public override void Start()
        {
            base.Start();

            ShotPF = Content.Load<Prefab>("Prefabs/PlayerShotPF");

            TheRadius = 40;
            Position = new Vector3(0, 0, 0);

            Shots = new List<PlayerShot>();
            ChunkRefs = new List<Chunk>();

            Deceleration = 0.01f;
            MaxVelocity = 500;
            IsActive = true;

            Entity mineTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(mineTimerE);
            MineTimer = mineTimerE.Get<Timer>();
            MineTimer.Reset(2);

            Entity unloadTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(unloadTimerE);
            UnloadTimer = unloadTimerE.Get<Timer>();

            Entity blinkTimerRE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(blinkTimerRE);
            BlinkTimerR = blinkTimerRE.Get<Timer>();
            BlinkTimerR.Reset(0.1f);

            Entity blinkTimerLE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(blinkTimerLE);
            BlinkTimerL = blinkTimerLE.Get<Timer>();
            BlinkTimerL.Reset(0.1f);

            FlameRModel = this.Entity.FindChild("PlayerFlameR").Get<ModelComponent>();
            FlameRModel.Enabled = false;
            FlameLModel = this.Entity.FindChild("PlayerFlameL").Get<ModelComponent>();
            FlameLModel.Enabled = false;
        }

        public override void Update()
        {
            base.Update();

            if (HitEdge())
                MoveToOppisiteEdge();

            GetInput();

            if (SceneSystem.GraphicsCompositor.Cameras[0].Camera != null)
                TheCamera = SceneSystem.GraphicsCompositor.Cameras[0].Camera.Entity;

            if (TheCamera != null)
            {
                TheCamera.Transform.Position.X = Position.X;
                TheCamera.Transform.Position.Y = Position.Y;
            }

            CheckCollusion();
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            AsteroidRefs = scene.AsteroidRefAccess;
            RandomGenerator = SceneControl.RandomGenerator;
        }

        public void PickupOre(Chunk chunk)
        {
            if (ChunkRefs.Count > 4)
                return;

            ChunkRefs.Add(chunk);
            chunk.Disable();
        }

        public void Bump(PO other)
        {
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.5f) * -1;
            Velocity += other.Velocity * 0.95f;
            Velocity -= VelocityFromVectors(other.Position, 75);
        }

        void GetInput()
        {
            float turnSpeed = 4.75f;

            if (!InDock)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    RotationVelocity.Z = turnSpeed;

                    BlinkRFlame();
                    FlameLModel.Enabled = false;
                }
                else if (Input.IsKeyDown(Keys.Right))
                {
                    RotationVelocity.Z = -turnSpeed;

                    BlinkLFlame();
                    FlameRModel.Enabled = false;
                }
                else
                {
                    RotationVelocity.Z = 0;
                    FlamesOff();
                }
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                Thrust();

                if (InDock)
                {
                    if (UnloadTimer.Expired)
                    {
                        Position.X = SceneRef.PlayerBaseRefAccess.Radius + Radius + 1;
                        Position.Z = 0;
                        InDock = false;
                    }
                }
            }
            else
            {
                ThrustOff();
            }

            if (Input.IsKeyDown(Keys.Down))
            {
                //Shield(true);
            }
            else
            {
                //Shield(false);
            }

            if (Input.IsKeyPressed(Keys.LeftCtrl) || Input.IsKeyPressed(Keys.Space))
            {
                FireShot();
            }

            if (Input.IsKeyDown(Keys.LeftShift))
            {
                MineOre();
            }
        }

        void MineOre()
        {
            if (MineTimer.Expired)
            {
                MineTimer.Reset();

                NearAsteroid = null;

                foreach(Asteroid rock in AsteroidRefs)
                {
                    if (Vector3.Distance(Position, rock.Position) < 100)
                    {
                        NearAsteroid = rock;
                        break;
                    }
                }

                if (NearAsteroid != null)
                    NearAsteroid.MineAttempt();
            }
        }

        void FireShot()
        {
            bool found = false;
            PlayerShot theShot = null;
            float speed = 450;

            foreach (PlayerShot shot in Shots)
            {
                if (!shot.Active)
                {
                    theShot = shot;
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                theShot = SceneRef.SetupEntity(ShotPF).Get<PlayerShot>();
                Shots.Add(theShot);
                theShot.Setup(SceneRef);
            }

            theShot.Fire(Position + VelocityFromRadian(Radius - 20, Rotation.Z),
                VelocityFromRadian(speed, Rotation.Z) + Velocity * 0.25f, Rotation.Z);
            theShot.UpdatePR();

            //FireSI.Stop();
            //FireSI.Play();
        }

        void Thrust()
        {
            //if (!ThrustSI.IsLooping)
            //{
            //    ThrustSI.Play();
            //    ThrustSI.IsLooping = true;
            //}

            if (!InDock)
            {
                if (Accelerate(ThrustAmount))
                {
                    ThrustOn = true;
                }
                else
                {
                    ThrustOff();
                }
            }

            BlinkRFlame();
            BlinkLFlame();
        }

        void BlinkRFlame()
        {
            if (BlinkTimerR.Expired)
            {
                BlinkTimerR.Reset(RandomMinMax(0.015f, 0.075f));
                FlameRModel.Enabled = !FlameRModel.Enabled;
            }
        }

        void BlinkLFlame()
        {
            if (BlinkTimerL.Expired)
            {
                BlinkTimerL.Reset(RandomMinMax(0.005f, 0.075f));
                FlameLModel.Enabled = !FlameLModel.Enabled;
            }
        }

        void ThrustOff()
        {

            if (ThrustOn)
            {
                //ThrustSI.IsLooping = false;
                //ThrustSI.Stop();
                FlamesOff();
                ThrustOn = false;
            }
        }

        void FlamesOff()
        {
            FlameRModel.Enabled = false;
            FlameLModel.Enabled = false;
        }

        void Docked()
        {
            InDock = true;

            foreach (Chunk chunk in ChunkRefs)
            {
                SceneRef.PlayerBaseRefAccess.UnloadOre(chunk.ThisOreType);
                chunk.IsInTransit = false;
            }

            UnloadTimer.Reset(RandomMinMax(1 + (ChunkRefs.Count / 2), ChunkRefs.Count + 1));

            ChunkRefs.Clear();

            RotationVelocity.Z = 0;
            Rotation.Z = 0;
            Position = Vector3.Zero;
            Position.Z = 50;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
        }

        void CheckCollusion()
        {
            if (!InDock)
            {
                if (CirclesIntersect(SceneRef.PlayerBaseRefAccess))
                {
                    Docked();
                }
            }
        }
    }
}
