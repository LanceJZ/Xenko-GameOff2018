using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Xenko_GameOff2018
{
    enum AIState
    { //0 = Evade player, 1 = Search for player, 2 = Wander aimlessly, 3 = Wait - Consider move, 4 = mine asteroid for mineral.
        Evade,
        Attack,
        Search,
        Wait,
        Mine,
        RetrieveOre,
        GoToWaypoint,
        ReturnToBase
    }

    public class EnemyDrone : PO
    {
        Timer MineTimer;
        Timer BumpTimer;
        AIState InState = new AIState();
        Player PlayerRef;
        EnemyBase FromBaseRef;
        Chunk ChunkRef;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;
        Asteroid NearAsteroid;
        Vector3 Waypoint = Vector3.Zero;
        bool WaypointReached;
        bool WasBumped;
        bool HasOre;

        float Thrust = 15.666f;

        public override void Start()
        {
            base.Start();

            Entity mineTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(mineTimerE);
            MineTimer = mineTimerE.Get<Timer>();
            MineTimer.Reset(2);

            Entity bumpTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(bumpTimerE);
            BumpTimer = bumpTimerE.Get<Timer>();

            InState = AIState.Search;
            TheRadius = 11;
            MaxVelocity = 0;
            Deceleration = 0.125f;
            SetModel();
        }

        public override void Update()
        {
            base.Update();

            if (Active)
            {
                CheckCollusion();

                if (HitEdge())
                    MoveToOppisiteEdge();

                if (WasBumped)
                {
                    RecoverFromBump();
                    return;
                }

                switch (InState)
                {
                    case AIState.Search:
                        Search();
                        break;
                    case AIState.Mine:
                        MineOre();
                        break;
                    case AIState.RetrieveOre:
                        RetrieveOre();
                        break;
                    case AIState.GoToWaypoint:
                        GotoWaypoint();
                        break;
                    case AIState.ReturnToBase:
                        ReturnToBase();
                        break;
                }

                Accelerate(Thrust);
            }
        }

        public void RecoverFromBump()
        {
            if (BumpTimer.Expired) //TODO: This is not always working.
            {
                Rotation.X = 0;
                Rotation.Y = 0;
                RotationVelocity.X = 0;
                RotationVelocity.Y = 0;
                Deceleration = 0.125f;
                WasBumped = false;
            }
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

        public void Setup(SceneControl scene, EnemyBase fromBase)
        {
            PlayerRef = scene.PlayerRefAccess;
            EnemyBaseRefs = scene.EnemyBaseRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
            FromBaseRef = fromBase;
        }

        public void Launch()
        {
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            Position.X = FromBaseRef.Position.X;
            Position.Y = FromBaseRef.Position.Y;
            UpdatePR();
            NearAsteroid = null;
            ChunkRef = null;
            IsActive = true;
            InState = AIState.Search;
            WasBumped = false;
        }

        void GotoWaypoint()
        {
            SetHeading(Waypoint);
            //RotationVelocity.Z = AimAtTarget(Waypoint, Rotation.Z, MathUtil.Pi);
            MaxVelocity = 20;

            if (Vector3.Distance(Waypoint, Position) < 40)
                 InState = AIState.ReturnToBase;
        }

        void ReturnToBase()
        {
            if (FromBaseRef.Active == false)
                FromBaseRef = FindNearbyBase();

            if (FromBaseRef == null)
                return;

            SetHeading(FromBaseRef);
            //RotationVelocity.Z = AimAtTarget(FromBaseRef.Position, Rotation.Z, MathUtil.Pi);

            if (Vector3.Distance(Position, FromBaseRef.Position) > 300)
                MaxVelocity = 200;
            else
                MaxVelocity = 50;

            if (Vector3.Distance(FromBaseRef.Position, Position) < 70)
            {
                if (ChunkRef != null && HasOre)
                {
                    FromBaseRef.AddChunk(ChunkRef.ThisOreType);
                    ChunkRef = null;
                }

                InState = AIState.Search;
            }
        }

        void RetrieveOre()
        {
            Thrust = 15.666f;

            if (ChunkRef != null && ChunkRef.Active)
            {
                SetHeading(ChunkRef);
                //RotationVelocity.Z = AimAtTarget(ChunkRef.Position, Rotation.Z, MathUtil.Pi);
                MaxVelocity = 5;
            }
            else
            {
                InState = AIState.Search;
                ChunkRef = null;
            }
        }

        void MineOre()
        {
            if (NearAsteroid == null || !NearAsteroid.Active)
            {
                InState = AIState.Search;
                return;
            }

            //if (!StayClose())
            //    return;

            SetHeading(NearAsteroid);
            Acceleration = Vector3.Zero;
            Velocity = NearAsteroid.Velocity;
            Deceleration = 0;
            Thrust = 0;

            if (MineTimer.Expired)
            {
                MineTimer.Reset();
                Chunk chunk = NearAsteroid.MineAttempt();

                if (chunk != null)
                {
                    ChunkRef = chunk;
                    InState = AIState.RetrieveOre;
                }
            }
        }

        void Search()
        {
            Thrust = 15.666f;

            if (NearAsteroid == null)
            {
                NearAsteroid = FindNearbyAsteroid();
                Waypoint = NearAsteroid.Position;
            }
            else
            {
                if (!WaypointReached)
                {
                    SetHeading(Waypoint);
                    //RotationVelocity.Z = AimAtTarget(Waypoint, Rotation.Z, MathUtil.Pi);

                    MaxVelocity = 150;

                    if (Vector3.Distance(Waypoint, Position) < 150)
                    {
                        WaypointReached = true;
                        MaxVelocity = 55;
                    }
                }
                else
                {
                    SetHeading(NearAsteroid);

                    if (!StayClose())
                        return;

                    if (Vector3.Distance(NearAsteroid.Position, Position) < 90)
                    {
                        Velocity = NearAsteroid.Velocity;
                        Acceleration = Vector3.Zero;
                        InState = AIState.Mine;
                    }
                    else
                    {
                        MaxVelocity = 35;
                        InState = AIState.Search;
                    }
                }
            }
        }

        void AimAtAsteroid()
        {
            SetHeading(NearAsteroid, Vector3.Normalize(NearAsteroid.Velocity) * 75);
            //RotationVelocity.Z = AimAtTarget((NearAsteroid.Position +
            //    ((Vector3.Normalize(NearAsteroid.Velocity) * 75)) * -1),
            //    Rotation.Z, MathUtil.Pi);
        }

        bool StayClose()
        {
            if (Vector3.Distance(NearAsteroid.Position, Position) > 200)
            {
                InState = AIState.Search;
                NearAsteroid = FindNearbyAsteroid();
                Waypoint = NearAsteroid.Position;
                return false;
            }

            return true;
        }

        void CheckCollusion()
        {
            foreach (Asteroid rock in AsteroidRefs)
            {
                if (CirclesIntersect(rock))
                {
                    Destroy();
                }

                if (!HasOre)
                {
                    foreach (Chunk chunk in rock.Chunks)
                    {
                        if (CirclesIntersect(chunk))
                        {
                            chunk.IsInTransit = true;
                            ChunkRef = chunk;
                            ChunkRef.Disable();
                            HasOre = true;
                            InState = AIState.GoToWaypoint;
                        }
                    }
                }
            }

            foreach (PlayerShot shot in PlayerRef.ShotsRef)
            {
                if (shot.Active)
                {
                    if (CirclesIntersect(shot))
                    {
                        Destroy();
                        shot.Disable();
                        break;
                    }
                }
            }

            if (CirclesIntersect(PlayerRef))
            {
                Destroy();
            }
        }

        void Destroy()
        {
            if (ChunkRef != null && HasOre)
            {
                ChunkRef.IsInTransit = false;
                ChunkRef.Enable();
                ChunkRef.Position = Position;
                ChunkRef.UpdatePR();
                ChunkRef = null;
                HasOre = false;
            }

            IsActive = false;
        }

        void SetHeading(Vector3 waypoint, Vector3 stayback)
        {
            RotationVelocity.Z = AimAtTarget(waypoint + (stayback * -1),
                Rotation.Z, MathUtil.Pi);
        }

        void SetHeading(PO target, Vector3 stayback)
        {
            SetHeading(target.Position, stayback);
        }

        void SetHeading(Vector3 waypoint)
        {
            SetHeading(waypoint, Vector3.Zero);
        }

        void SetHeading(PO target)
        {
            SetHeading(target.Position, Vector3.Zero);
        }

        Asteroid FindNearbyAsteroid()
        {
            MaxVelocity = 300;
            Asteroid nearRock = null;
            float distance = -1;

            foreach (Asteroid rock in AsteroidRefs)
            {
                float rockDist = Vector3.Distance(rock.Position, Position);

                if (rockDist < distance || distance < 0)
                {
                    distance = rockDist;
                    nearRock = rock;
                }
            }

            return nearRock;
        }

        EnemyBase FindNearbyBase()
        {
            float distance = -1;
            EnemyBase nearBase = null;

            foreach (EnemyBase enemyBase in EnemyBaseRefs)
            {
                float baseDist = Vector3.Distance(enemyBase.Position, Position);

                if (baseDist < distance || distance < 0)
                {
                    distance = baseDist;
                    nearBase = enemyBase;
                }
            }

            return nearBase;
        }
    }
}
