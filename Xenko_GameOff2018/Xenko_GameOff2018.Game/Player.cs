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
        float ThrustAmount = 1.666f;
        bool ThrustOn;
        public Entity TheCamera;

        public override void Start()
        {
            base.Start();


            Position = new Vector3(0, 0, 0);

            Deceleration = 0.01f;
            MaxVelocity = 500;
            Active = true;
            SetModel();
        }

        public override void Update()
        {
            base.Update();

            if (CheckForEdge())
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
            }
            else if (Input.IsKeyDown(Keys.Right))
            {
                RotationVelocity.Z = -turnSpeed;
            }
            else
                RotationVelocity.Z = 0;

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

            //if (FlameTimer.Expired)
            //{
            //    FlameTimer.Reset();
            //    FlameM.Enabled = !FlameM.Enabled;
            //}
        }

        void ThrustOff()
        {
            Acceleration = -Velocity * Deceleration;

            if (ThrustOn)
            {
                //ThrustSI.IsLooping = false;
                //FlameM.Enabled = false;
                //ThrustSI.Stop();
                ThrustOn = false;
            }
        }
    }
}
