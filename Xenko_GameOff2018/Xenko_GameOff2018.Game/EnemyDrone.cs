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
        ReturnToBase
    }

    public class EnemyDrone : PO
    {
        Player PlayerRef;
        EnemyBase FromBaseRef;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;
        Asteroid NearbyAsteroid;

        public override void Start()
        {
            base.Start();

            Active = true;
        }

        public override void Update()
        {
            base.Update();

            if (NearbyAsteroid == null)
                NearbyAsteroid = FindNearbyAsteroid();
            else
                SetHeading();

            Velocity = VelocityFromAngle(Rotation.Z, 100);
        }

        public void Setup(SceneControl scene, EnemyBase fromBase)
        {
            PlayerRef = scene.PlayerRefAccess;
            EnemyBaseRefs = scene.EnemyBaseRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
            FromBaseRef = fromBase;
            Position.X = fromBase.Position.X;
            Position.Y = fromBase.Position.Y;
        }

        void SetHeading()
        {
            RotationVelocity.Z = AimAtTarget(NearbyAsteroid.Position, Rotation.Z, MathUtil.PiOverFour);
        }

        Asteroid FindNearbyAsteroid()
        {
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
