using System;
using System.Linq;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Xenko_GameOff2018
{
    public class PO : SyncScript
    {
        public bool Hit { get => hit; set => hit = value; }
        public bool Active { get => active; set => active = value; }
        public bool Moveable { get => moveable; set => moveable = value; }
        public bool Paused { get => pause; set => pause = value; }
        public bool GameOver { get => gameOver; set => gameOver = value; }
        public int Points { get => points; set => points = value; }
        public float Radius { get => radius; set => radius = value; }
        public float Deceleration { get => deceleration; set => deceleration = value; }
        public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
        public Random RandomGenerator { get => RandomNumbers; set => RandomNumbers = value; }

        bool hit;
        bool active;
        bool moveable = true;
        bool pause;
        bool gameOver;
        int points;
        float radius;
        float deceleration = 0;
        float maxVelocity;

        public ModelComponent Model;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 RotationVelocity = Vector3.Zero;
        public Vector3 RotationAcceleration = Vector3.Zero;

        protected static Vector2 Edge = new Vector2(1500, 1500);
        static Random RandomNumbers;

        public override void Start()
        {

            if (RandomNumbers == null)
                RandomNumbers = new Random(DateTime.UtcNow.Millisecond * 666);

            if (this.Entity.Transform.Position.Length() > 0 || this.Entity.Transform.Position.Length() < 0)
                Position = this.Entity.Transform.Position;
        }

        public override void Update()
        {
            if (!Paused)
            {
                if (Active && Moveable)
                {
                    //Calculate movement this frame according to velocity and acceleration.
                    float elapsed = (float)Game.UpdateTime.Elapsed.TotalSeconds;

                    if (Acceleration == Vector3.Zero)
                    {
                        Acceleration = -Velocity * Deceleration;
                    }

                    Velocity += Acceleration;
                    Position += Velocity * elapsed;
                    Rotation = Rotate(Rotation + RotationVelocity * elapsed);

                    UpdatePR();
                }

                UpdateActive(Active);
            }
        }

        public Vector3 Rotate(Vector3 rotate)
        {
            Vector3 framerotate = rotate;

            rotate.X = Rotate(framerotate.X);
            rotate.Y = Rotate(framerotate.Y);
            rotate.Z = Rotate(framerotate.Z);

            return rotate;
        }

        float Rotate(float rotate)
        {
            float framerotate = MathUtil.Clamp(rotate, -MathUtil.TwoPi, MathUtil.TwoPi * 2);

            if (framerotate < 0)
            {
                framerotate += MathUtil.TwoPi;
                return framerotate;
            }

            if (framerotate > MathUtil.TwoPi)
                framerotate -= MathUtil.TwoPi;

            return framerotate;
        }

        /// <summary>
        /// Returns a Vector3 direction of travel from angle and magnitude.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="magnitude"></param>
        /// <returns>Vector3</returns>
        public Vector3 SetVelocity(float angle, float magnitude)
        {
            Vector3 Vector = Vector3.Zero;
            Vector.Y = (float)(Math.Sin(angle) * magnitude);
            Vector.X = (float)(Math.Cos(angle) * magnitude);
            return Vector;
        }

        public void SetModel()
        {
            Model = this.Entity.Get<ModelComponent>();
            Model.Enabled = false;
        }

        public void SetModelChild()
        {
            Model = this.Entity.GetChild(0).Get<ModelComponent>();
            Model.Enabled = false;
        }

        public void UpdateActive(bool active)
        {
            Active = active;

            if (Model != null)
                Model.Enabled = Active;
        }

        public void UpdatePR()
        {
            this.Entity.Transform.Position = Position;
            this.Entity.Transform.RotationEulerXYZ = Rotation;
        }

        public void UpdateScale()
        {
            this.Entity.Transform.Scale = Scale;
        }

        public bool CirclesIntersect(Vector3 Target, float TargetRadius)
        {
            float dx = Target.X - Position.X;
            float dy = Target.Y - Position.Y;
            float rad = Radius + TargetRadius;

            if ((dx * dx) + (dy * dy) < rad * rad)
                return true;

            return false;
        }

        public bool CheckForEdge()
        {
            if (Position.X > Edge.X)
            {
                Position.X = -Edge.X;
                return true;
            }

            if (Position.X < -Edge.X)
            {
                Position.X = Edge.X;
                return true;
            }

            if (Position.Y > Edge.Y)
            {
                Position.Y = -Edge.Y;
                return true;
            }

            if (Position.Y < -Edge.Y)
            {
                Position.Y = Edge.Y;
                return true;
            }

            return false;
        }

        public void MoveToOppisiteEdge()
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

        public bool Accelerate(float amount)
        {
            if (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) < maxVelocity)
            {
                Acceleration = SetVelocity(Rotation.Z, amount);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Get a random float between min and max
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        public float RandomMinMax(float min, float max)
        {
            return min + ((float)RandomGenerator.NextDouble() * (max - min));
        }
        /// <summary>
        /// Returns random number from zero to Pi times two.
        /// </summary>
        /// <returns>float</returns>
        public float RandomRadian()
        {
            return RandomMinMax(0, MathUtil.TwoPi);
        }

        /// <summary>
        /// Returns the velocity by direction in radian.
        /// </summary>
        /// <param name="speed">The velocity of object.</param>
        /// <param name="radian">The direction of target object.</param>
        public Vector3 VelocityFromRadian(float speed, float radian)
        {
            return new Vector3((float)Math.Cos(radian) * speed, (float)Math.Sin(radian) * speed, 0);
        }

        /// <summary>
        /// Returns a velocity in a random direction with a random speed.
        /// </summary>
        /// <param name="speedMax">Maximum speed.</param>
        /// <param name="speedMin">Minimum speed.</param>
        public Vector3 RandomVelocity(float speedMin, float speedMax)
        {
            float rad = RandomRadian();
            float amt = RandomMinMax(speedMin, speedMax);
            return new Vector3((float)Math.Cos(rad) * amt, (float)Math.Sin(rad) * amt, 0);
        }

        /// <summary>
        /// Returns a velocity of magnitude in a random direction.
        /// </summary>
        /// <param name="magnitude">Units per second.</param>
        public Vector3 RandomVelocity(float magnitude)
        {
            float rad = RandomRadian();
            return new Vector3((float)Math.Cos(rad) * magnitude, (float)Math.Sin(rad) * magnitude, 0);
        }

        public float AngleFromVectors(Vector3 origin, Vector3 target)
        {
            return (float)(Math.Atan2(target.Y - origin.Y, target.X - origin.X));
        }

        /// <summary>
        /// Returns a velocity using rotation as direction, at magnitude.
        /// </summary>
        /// <param name="rotation">Direction in radians.</param>
        /// <param name="magnitude">Speed in units per second.</param>
        /// <returns></returns>
        public Vector3 VelocityFromAngle(float rotation, float magnitude)
        {
            return new Vector3((float)Math.Cos(rotation) * magnitude, (float)Math.Sin(rotation) * magnitude, 0);
        }

        public Vector3 VelocityFromAngle(float magnitude)
        {
            float ang = RandomRadian();
            return new Vector3((float)Math.Cos(ang) * magnitude, (float)Math.Sin(ang) * magnitude, 0);
        }

        public float RandomHeight()
        {
            return RandomMinMax(-Edge.Y * 0.9f, Edge.Y * 0.9f);
        }

        public float RandomWidth()
        {
            return RandomMinMax(-Edge.X * 0.9f, Edge.X * 0.9f);
        }

        public Vector3 RandomXEdge()
        {
            return new Vector3(Edge.X, RandomMinMax(-Edge.Y * 0.9f, Edge.Y * 0.9f), 0);
        }

        public Vector3 RandomYEdge()
        {
            return new Vector3(Edge.Y, RandomMinMax(-Edge.X * 0.9f, Edge.X * 0.9f), 0);
        }

        public float AimAtTarget(Vector3 target, float facingAngle, float magnitude)
        {
            float turnVelocity = 0;
            float targetAngle = AngleFromVectors(Position, target);
            float targetLessFacing = targetAngle - facingAngle;
            float facingLessTarget = facingAngle - targetAngle;

            if (Math.Abs(targetLessFacing) > Math.PI)
            {
                if (facingAngle > targetAngle)
                {
                    facingLessTarget = ((MathUtil.TwoPi - facingAngle) + targetAngle) * -1;
                }
                else
                {
                    facingLessTarget = (MathUtil.TwoPi - targetAngle) + facingAngle;
                }
            }

            if (facingLessTarget > 0)
            {
                turnVelocity = -magnitude;
            }
            else
            {
                turnVelocity = magnitude;
            }

            return turnVelocity;
        }
    }
}
