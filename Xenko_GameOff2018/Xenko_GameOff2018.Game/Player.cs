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
        bool ThrustOn;
        public Entity TheCamera;

        public override void Start()
        {
            base.Start();


            Position = new Vector3(0, 0, -50);

            Deceleration = 0.01f;
            Active = true;
        }

        public override void Update()
        {
            base.Update();

            if (CheckForEdge())
                DriftedOffEdge();

            GetInput();

            if (SceneSystem.GraphicsCompositor.Cameras[0].Camera != null)
                TheCamera = SceneSystem.GraphicsCompositor.Cameras[0].Camera.Entity;

            if (TheCamera != null)
            {
                TheCamera.Transform.Position.X = Position.X;
                TheCamera.Transform.Position.Y = Position.Y;
            }
        }

        void DriftedOffEdge()
        {
            if (Position.X > Edge.X)
                Position.X = -Edge.X;

            if (Position.X < -Edge.X)
                Position.X = Edge.X;

            if (Position.Y > Edge.Y)
                Position.Y = -Edge.Y;

            if (Position.Y < -Edge.Y)
                Position.Y = Edge.Y;
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

            float maxPerSecond = 500;
            float thrustAmount = 0.375f;

            if (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) < maxPerSecond)
            {
                Acceleration = SetVelocity(Rotation.Z, thrustAmount);
            }
            else
            {
                ThrustOff();
            }

            ThrustOn = true;

            //if (FlameTimer.Expired)
            //{
            //    FlameTimer.Reset();
            //    FlameM.Enabled = !FlameM.Enabled;
            //}
        }

        void ThrustOff()
        {
            float Deceration = 0.025f;
            Acceleration = -Velocity * Deceration;

            //FlameM.Enabled = false;
            //ThrustSI.IsLooping = false;

            if (ThrustOn)
            {
                //ThrustSI.IsLooping = false;
                //ThrustSI.Stop();
                ThrustOn = false;
            }
        }
    }
}
