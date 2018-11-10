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
        ReturnToBase
    }

    public class EnemyDrone : PO
    {
        Timer MineTimer;
        AIState InState = new AIState();
        Player PlayerRef;
        EnemyBase FromBaseRef;
        Chunk ChunkRef;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;
        Asteroid NearAsteroid;
        Vector3 Waypoint = Vector3.Zero;
        bool WaypointReached = false;

        float Thrust = 15.666f;

        public override void Start()
        {
            base.Start();

            Entity mineTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(mineTimerE);
            MineTimer = mineTimerE.Get<Timer>();
            MineTimer.Reset(2);

            InState = AIState.Search;
            Radius = 11;
            MaxVelocity = 0;
            Deceleration = 0.125f;
            Active = true;
        }

        public override void Update()
        {
            base.Update();

            switch (InState)
            {
                case AIState.Search:
                    Search();
                    break;
                case AIState.Mine:
                    Mine();
                    break;
                case AIState.RetrieveOre:
                    RetrieveOre();
                    break;
                case AIState.ReturnToBase:
                    ReturnToBase();
                    break;
            }

            if (!Accelerate(Thrust))
                Acceleration = -Velocity * Deceleration;

            if (HitEdge())
                MoveToOppisiteEdge();

            Collusion();
        }

        public void Setup(SceneControl scene, EnemyBase fromBase)
        {
            PlayerRef = scene.PlayerRefAccess;
            EnemyBaseRefs = scene.EnemyBaseRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
            FromBaseRef = fromBase;
            Launch();
        }

        void ReturnToBase()
        {
            RotationVelocity.Z = AimAtTarget(FromBaseRef.Position, Rotation.Z, MathUtil.Pi);

            if (Vector3.Distance(Position, FromBaseRef.Position) > 300)
                MaxVelocity = 300;
            else
                MaxVelocity = 50;

            if (Vector3.Distance(FromBaseRef.Position, Position) < 30)
            {
                if (ChunkRef != null)
                {
                    FromBaseRef.AddChunk(ChunkRef.ThisOreType);
                    ChunkRef = null;
                    Launch();
                }
            }
        }

        void RetrieveOre()
        {
            RotationVelocity.Z = AimAtTarget(ChunkRef.Position, Rotation.Z, MathUtil.PiOverFour);
            MaxVelocity = 75;
        }

        void Mine()
        {
            if (NearAsteroid == null)
            {
                InState = AIState.Search;
                return;
            }

            AimAtAsteroid();

            if (!StayClose())
                return;

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
            if (NearAsteroid == null)
            {
                NearAsteroid = FindNearbyAsteroid();
                Waypoint = NearAsteroid.Position;
            }
            else
            {
                if (!WaypointReached)
                {
                    RotationVelocity.Z = AimAtTarget(Waypoint, Rotation.Z, MathUtil.Pi);

                    MaxVelocity = 150;

                    if (Vector3.Distance(Waypoint, Position) < 150)
                    {
                        WaypointReached = true;
                        MaxVelocity = 55;
                    }
                }
                else
                {
                    AimAtAsteroid();

                    if (!StayClose())
                        return;

                    if (Vector3.Distance(NearAsteroid.Position, Position) < 90)
                    {
                        Velocity = NearAsteroid.Velocity;
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
            RotationVelocity.Z = AimAtTarget((NearAsteroid.Position +
                ((Vector3.Normalize(NearAsteroid.Velocity) * 75)) * -1),
                Rotation.Z, MathUtil.Pi);
        }

        bool StayClose()
        {
            if (Vector3.Distance(NearAsteroid.Position, Position) > 500)
            {
                MaxVelocity = 200;
                InState = AIState.Search;
                NearAsteroid = FindNearbyAsteroid();
                Waypoint = NearAsteroid.Position;
                return false;
            }

            return true;
        }

        void Collusion()
        {
            foreach (Asteroid rock in AsteroidRefs)
            {
                if (CirclesIntersect(rock.Position, rock.Radius))
                {
                    if (ChunkRef != null)
                    {
                        ChunkRef.Active = true;
                    }

                    Launch();
                }
            }

            if (ChunkRef != null)
            {
                if (CirclesIntersect(ChunkRef.Position, ChunkRef.Radius))
                {
                    ChunkRef.Active = false;
                    InState = AIState.ReturnToBase;
                }
            }
        }

        void Launch()
        {
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            Position.X = FromBaseRef.Position.X;
            Position.Y = FromBaseRef.Position.Y;
            NearAsteroid = null;
        }

        void SetHeading(Vector3 waypoint, Vector3 stayback)
        {
            RotationVelocity.Z = AimAtTarget(waypoint + (stayback * -1),
                Rotation.Z, MathUtil.PiOverFour);
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
    }
}
