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
        public int LevelAccess { get => Level; }
        public int MaxHPAccess { get => MaxHP; }
        public int HPAccess
        {
            get => HP;

            set
            {
                if (HP + value < MaxHP && HP + value > 0)
                {
                    HP += value;
                    SceneRef.HUDAccess.HP = HP;
                }

                if (HP + value <= 0)
                {
                    Destroyed();
                }
            }
        }

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

        SoundInstance FireSI;
        SoundInstance ThrustSI;
        SoundInstance HitSI;
        SoundInstance DestroyedSI;
        SoundInstance DockSI;
        SoundInstance UnDockSI;
        SoundInstance MiningSI;

        List<PlayerShot> Shots;

        float ThrustAmount = 3.666f;
        int ExP = 0;
        int Level = 1;
        int HP = 100;
        int MaxHP = 100;
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

            FireSI = Content.Load<Sound>("Sounds/PlayerShotSound").CreateInstance();
            ThrustSI = Content.Load<Sound>("Sounds/ThrustSound").CreateInstance();
            HitSI = Content.Load<Sound>("Sounds/PlayerHitSound").CreateInstance();
            DestroyedSI = Content.Load<Sound>("Sounds/PlayerDeadSound").CreateInstance();
            DockSI = Content.Load<Sound>("Sounds/DockSound").CreateInstance();
            UnDockSI = Content.Load<Sound>("Sounds/UnDockSound").CreateInstance();
            MiningSI = Content.Load<Sound>("Sounds/MiningSound").CreateInstance();
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
            AsteroidRefs = scene.AsteroidAccess;
            RandomGenerator = SceneControl.RandomGenerator;
        }

        public void Damaged()
        {
            HitSI.Play();
        }

        public void PickupOre(Chunk chunk)
        {
            if (ChunkRefs.Count > 4)
                return;

            chunk.PickupSound();
            ChunkRefs.Add(chunk);
            SceneRef.HUDAccess.Ore = ChunkRefs.Count;
            chunk.Disable();
        }

        public void ExpGain(int points)
        {
            ExP += points;
            SceneRef.HUDAccess.Exp = ExP;

            if (ExP > Level * 500)
            {
                Level++;
                SceneRef.HUDAccess.Level = Level;
            }
        }

        public void Bump(PO other)
        {
            SceneRef.BumpSound();
            Acceleration = Vector3.Zero;
            Velocity = (Velocity * 0.5f) * -1;
            Velocity += other.Velocity * 0.95f;
            Velocity -= VelocityFromVectors(other.Position, 75);
        }

        public void Reset()
        {
            Docked();
        }

        public void Destroyed()
        {
            DestroyedSI.Play();
            HP = 100;
            ExP = 0;
            Level = 1;
            ChunkRefs.Clear();
            SceneRef.HUDAccess.Ore = 0;
            SceneRef.HUDAccess.HP = HP;
            SceneRef.PlayerBaseAccess.Reset();
            Reset();
        }

        void GetInput()
        {
            float turnSpeed = 4.75f;

            if (!InDock)
            {
                if (Input.IsKeyDown(Keys.Left))
                {
                    RotationVelocity.Z = turnSpeed;
                    ThrustSI.Volume = 0.5f;
                    BlinkRFlame();
                    FlameLModel.Enabled = false;
                }
                else if (Input.IsKeyDown(Keys.Right))
                {
                    RotationVelocity.Z = -turnSpeed;
                    ThrustSI.Volume = 0.5f;
                    BlinkLFlame();
                    FlameRModel.Enabled = false;
                }
                else
                {
                    RotationVelocity.Z = 0;
                    FlamesOff();

                    if (!ThrustOn)
                    {
                        ThrustSoundOff();
                    }
                }
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                Thrust();

                if (InDock)
                {
                    if (UnloadTimer.Expired)
                    {
                        UnDockSI.Stop();
                        UnDockSI.Play();
                        Position.X = SceneRef.PlayerBaseAccess.Radius + Radius + 1;
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
            else
            {
                if (MiningSI.IsLooping)
                {
                    MiningSI.IsLooping = false;
                    MiningSI.Stop();
                }
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
                    if (Vector3.Distance(Position, rock.Position) < 150)
                    {
                        NearAsteroid = rock;
                        break;
                    }
                    else
                    {
                        NearAsteroid = null;
                    }
                }

                if (NearAsteroid != null)
                {
                    NearAsteroid.MineAttempt();
                }
            }

            if (NearAsteroid != null)
            {
                if (!MiningSI.IsLooping)
                {
                    MiningSI.Play();
                    MiningSI.IsLooping = true;
                }
            }
            else
            {
                if (MiningSI.IsLooping)
                {
                    ThrustSI.IsLooping = false;
                }
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

            FireSI.Stop();
            FireSI.Play();

            int damage = 30;

            if (Level < 20)
                damage = (int)(Level * 1.5f);

            theShot.Fire(Position + VelocityFromRadian(Radius - 20, Rotation.Z),
                VelocityFromRadian(speed, Rotation.Z) + Velocity * 0.25f, Rotation.Z, damage);
            theShot.UpdatePR();
        }

        void Thrust()
        {
            ThrustOn = true;
            ThrustSI.Volume = 1;

            if (!InDock)
            {
                if (!Accelerate(ThrustAmount))
                {
                    ThrustSI.Volume = 0.25f;
                }
            }

            BlinkRFlame();
            BlinkLFlame();
        }

        void ThrustSoundOn()
        {
            if (!ThrustSI.IsLooping)
            {
                ThrustSI.Play();
                ThrustSI.IsLooping = true;
            }
        }

        void ThrustOff()
        {
            if (ThrustOn)
            {
                ThrustSoundOff();
                FlamesOff();
                ThrustOn = false;
            }
        }

        void ThrustSoundOff()
        {
            if (ThrustSI.IsLooping)
            {
                ThrustSI.IsLooping = false;
                ThrustSI.Stop();
            }
        }

        void BlinkRFlame()
        {
            ThrustSoundOn();

            if (BlinkTimerR.Expired)
            {
                BlinkTimerR.Reset(RandomMinMax(0.015f, 0.075f));
                FlameRModel.Enabled = !FlameRModel.Enabled;
            }
        }

        void BlinkLFlame()
        {
            ThrustSoundOn();

            if (BlinkTimerL.Expired)
            {
                BlinkTimerL.Reset(RandomMinMax(0.005f, 0.075f));
                FlameLModel.Enabled = !FlameLModel.Enabled;
            }
        }

        void FlamesOff()
        {
            FlameRModel.Enabled = false;
            FlameLModel.Enabled = false;
        }

        void Docked()
        {
            DockSI.Stop();
            DockSI.Play();
            InDock = true;

            foreach (Chunk chunk in ChunkRefs)
            {
                ExpGain(20);
                SceneRef.PlayerBaseAccess.UnloadOre(chunk.ThisOreType);
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
            SceneRef.HUDAccess.Ore = 0;
        }

        void CheckCollusion()
        {
            if (!InDock)
            {
                if (CirclesIntersect(SceneRef.PlayerBaseAccess))
                {
                    Docked();
                    HP += SceneRef.PlayerBaseAccess.HealPlayer;
                    SceneRef.HUDAccess.HP = HP;
                }
            }
        }
    }
}
