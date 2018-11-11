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
        public List<PlayerShot> ShotsRef { get => Shots; }

        ModelComponent FlameRModel;
        ModelComponent FlameLModel;
        Timer BlinkTimerR;
        Timer BlinkTimerL;
        List<PlayerShot> Shots;

        float ThrustAmount = 3.666f;
        bool ThrustOn;
        public Entity TheCamera;

        public override void Start()
        {
            base.Start();

            Radius = 40;
            Position = new Vector3(0, 0, 0);

            Shots = new List<PlayerShot>();

            Deceleration = 0.01f;
            MaxVelocity = 500;
            Active = true;

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

            SetModel();
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
        }

        void GetInput()
        {
            float turnSpeed = 4.75f;

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

            if (Input.IsKeyDown(Keys.Up))
            {
                Thrust();
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
                //FireShot();
            }
        }

        void Thrust()
        {
            //if (!ThrustSI.IsLooping)
            //{
            //    ThrustSI.Play();
            //    ThrustSI.IsLooping = true;
            //}

            if (Accelerate(ThrustAmount))
            {
                ThrustOn = true;
            }
            else
            {
                ThrustOff();
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
            Acceleration = -Velocity * Deceleration;

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
    }
}
