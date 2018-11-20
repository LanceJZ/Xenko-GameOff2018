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
        public bool Hit { get => IsHit; }
        public bool Active { get => IsActive; }
        public bool Moveable { get => IsMoveable; }
        public float Radius { get => TheRadius; }
        public bool Paused { get => pause; set => pause = value; }
        public bool GameOver { get => gameOver; set => gameOver = value; }
        public int HitPoints { get => points; set => points = value; }
        public float Deceleration { get => deceleration; set => deceleration = value; }
        public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
        public static Random RandomGenerator { get => RandomNumbers; set => RandomNumbers = value; }

        protected bool IsHit;
        protected bool IsActive;
        protected bool IsMoveable = true;
        protected float TheRadius;
        protected float HeadingSpeed = MathUtil.Pi;
        bool pause;
        bool gameOver;
        int points;
        float deceleration = 0.001f;
        float maxVelocity;

        public ModelComponent Model;
        public Vector3 Position = Vector3.Zero;
        public Vector3 Velocity = Vector3.Zero;
        public Vector3 Acceleration = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 RotationVelocity = Vector3.Zero;
        public Vector3 RotationAcceleration = Vector3.Zero;

        protected static Vector2 Edge = new Vector2(3360, 2690);
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

                    Velocity += Acceleration;
                    Position += Velocity * elapsed;
                    Rotation += RotationVelocity * elapsed;
                    Acceleration = -Velocity * Deceleration;

                    UpdatePR();
                }

                UpdateActive(Active);
            }
        }

        public void UpdateActive(bool active)
        {
            IsActive = active;

            if (Model != null)
                Model.Enabled = IsActive;
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
        /// <summary>
        /// Circle collusion detection. Target circle will be compared to this class's.
        /// Will return true of they intersect.
        /// </summary>
        /// <param name="TargetPosition">Position of target.</param>
        /// <param name="TargetRadius">Radius of target.</param>
        /// <returns></returns>
		public bool CirclesIntersect(Vector3 TargetPosition, float TargetRadius)
        {
            float distanceX = TargetPosition.X - Position.X;
            float distanceY = TargetPosition.Y - Position.Y;
            float radius = Radius + TargetRadius;

            if ((distanceX * distanceX) + (distanceY * distanceY) < radius * radius)
                return true;

            return false;
        }
        /// <summary>
        /// Circle collusion detection. Target circle will be compared to this class's.
        /// Will return true of they intersect.
        /// </summary>
        /// <param name="Target">Target Positioned Object.</param>
        /// <returns></returns>
        public bool CirclesIntersect(PO Target)
        {
            float distanceX = Target.Position.X - Position.X;
            float distanceY = Target.Position.Y - Position.Y;
            float radius = Radius + Target.Radius;

            if ((distanceX * distanceX) + (distanceY * distanceY) < radius * radius)
                return true;

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
            float rotation = RandomRadian();
            float magnitute = RandomMinMax(speedMin, speedMax);
            return new Vector3((float)Math.Cos(rotation) * magnitute, (float)Math.Sin(rotation) * magnitute, 0);
        }//TODO: These functions can use the same function.
        /// <summary>
        /// Returns a velocity of magnitude in a random direction.
        /// </summary>
        /// <param name="magnitude">Units per second.</param>
        public Vector3 RandomVelocity(float magnitude)
        {
            float rotation = RandomRadian();
            return new Vector3((float)Math.Cos(rotation) * magnitude, (float)Math.Sin(rotation) * magnitude, 0);
        }
        /// <summary>
        /// Returns a float of the angle in radians derived from two Vector3 passed into it, using only the X and Y.
        /// </summary>
        /// <param name="origin">Vector3 of origin</param>
        /// <param name="target">Vector3 of target</param>
        /// <returns>Float</returns>
        public float AngleFromVectors(Vector3 origin, Vector3 target)
        {
            return (float)(Math.Atan2(target.Y - origin.Y, target.X - origin.X));
        }
        /// <summary>
        /// Returns a float of the angle in radians to target, using only the X and Y.
        /// </summary>
        /// <param name="target">Vector3 of target</param>
        /// <returns>Float</returns>
        public float AngleFromVectors(Vector3 target)
        {
            return AngleFromVectors(Position, target);
        }
        /// <summary>
        /// Returns Vector3 direction of travel from origin to target. Z is ignored.
        /// </summary>
        /// <param name="origin">Vector3 of origin</param>
        /// <param name="target">Vector3 of target</param>
        /// <param name="magnitude">float of speed of travel</param>
        /// <returns>Vector3</returns>
        public Vector3 VelocityFromVectors(Vector3 origin, Vector3 target, float magnitude)
        {
            return VelocityFromAngle(AngleFromVectors(origin, target), magnitude);
        }
        /// <summary>
        /// Returns Vector3 direction of travel to target. Z is ignored.
        /// </summary>
        /// <param name="target">Vector3 of target</param>
        /// <param name="magnitude">float of speed of travel</param>
        /// <returns>Vector3</returns>
        public Vector3 VelocityFromVectors(Vector3 target, float magnitude)
        {
            return VelocityFromVectors(Position, target, magnitude);
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
        /// <summary>
        /// Return a velocity using a random rotation as direction, at magnitude.
        /// </summary>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public Vector3 VelocityFromAngle(float magnitude)
        {
            float rotaiton = RandomRadian();
            return VelocityFromAngle(rotaiton, magnitude);
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

        protected bool HitEdge()
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

        protected float Rotate(float rotate)
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

        protected void SetModel()
        {
            Model = this.Entity.Get<ModelComponent>();
        }

        protected void SetModelChild()
        {
            Model = this.Entity.GetChild(0).Get<ModelComponent>();
        }

        protected void SetHeading(Vector3 waypoint, Vector3 stayback)
        {
            RotationVelocity.Z = AimAtTarget(waypoint + (stayback * -1),
                Rotation.Z, HeadingSpeed);
        }

        protected void SetHeading(PO target, Vector3 stayback)
        {
            SetHeading(target.Position, stayback);
        }

        protected void SetHeading(Vector3 waypoint)
        {
            SetHeading(waypoint, Vector3.Zero);
        }

        protected void SetHeading(PO target)
        {
            SetHeading(target.Position, Vector3.Zero);
        }

        protected void MoveToOppisiteEdge()
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

        protected bool Accelerate(float amount)
        {
            if (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) < maxVelocity)
            {
                Acceleration = SetVelocity(Rotation.Z, amount);
                return true;
            }

            return false;
        }
    }
}
