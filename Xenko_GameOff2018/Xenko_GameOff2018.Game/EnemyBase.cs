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
    public class EnemyBase : PO
    {
        public List<EnemyDrone> EnemyDroneAccess { get => DroneRefs; }

        SceneControl SceneRef;
        Player PlayerRef;
        List<Asteroid> AsteroidRefs;
        List<EnemyDrone> DroneRefs;
        EnemyBaseGun[] EnemyGuns = new EnemyBaseGun[8];
        Prefab DronePF;
        int OreCount;

        public override void Start()
        {
            base.Start();

            TheRadius = 50;
            DroneRefs = new List<EnemyDrone>();

            for (int i = 0; i < 8; i++)
            {
                string gun = "EnemyBaseTurret-" + 0 + (i + 1);
                EnemyGuns[i] = Entity.FindChild(gun).Get<EnemyBaseGun>();
                EnemyGuns[i].Setup(SceneRef);
            }

            int spawn = RandomGenerator.Next(3);
            Vector2 outterBuffer = new Vector2(400, 350);
            float innerBuffer = 200;

            switch (spawn)
            {
                case 0:
                    Position.Y = RandomMinMax(innerBuffer, Edge.Y - outterBuffer.Y);
                    break;
                case 1:
                    Position.X = RandomMinMax(innerBuffer, Edge.X - outterBuffer.X);
                    break;
                case 2:
                    Position.Y = RandomMinMax(-innerBuffer, -Edge.Y + outterBuffer.Y);
                    break;
                case 3:
                    Position.X = RandomMinMax(-innerBuffer, -Edge.X + outterBuffer.X);
                    break;
            }

            switch(spawn)
            {
                case 1:
                case 3:
                    Position.Y = RandomMinMax(-innerBuffer, innerBuffer);
                    break;

                case 0:
                case 2:
                    Position.X = RandomMinMax(-Edge.X + outterBuffer.X, Edge.X - outterBuffer.X);
                    break;
            }

            Position.Z = 60;
            IsActive = true;

            DronePF = Content.Load<Prefab>("Prefabs/EnemyDronePF");

            SpawnDrone();
        }

        public override void Update()
        {
            base.Update();

            CheckCollusion();
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
        }

        public void AddChunk(OreType type)
        {
            OreCount++;
        }

        public void CheckHit()
        {
            foreach(EnemyBaseGun gun in EnemyGuns)
            {
                if (gun.Active)
                {
                    foreach (PlayerShot shot in PlayerRef.ShotsRef)
                    {
                        if (shot.Active)
                        {
                            if (shot.CirclesIntersect(gun.Position + Position, gun.Radius))
                            {
                                gun.Disable();
                                shot.Disable();
                            }
                        }
                    }
                }
            }
        }

        void CheckCollusion()
        {
            foreach (PlayerShot shot in PlayerRef.ShotsRef)
            {
                if (CirclesIntersect(shot.Position, shot.Radius))
                {
                    CheckHit();
                }
            }
        }

        void SpawnDrone()
        {
            Entity droneE = SceneRef.SetupEntity(DronePF);
            EnemyDrone droneS = droneE.Get<EnemyDrone>();
            droneS.Setup(SceneRef, this);
            DroneRefs.Add(droneS);
        }
    }
}
