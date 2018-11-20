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
        public List<EnemyDrone> DroneAccess { get => Drones; }
        public List<EnemyBoss> BossAccess { get => Bosses; }

        SceneControl SceneRef;
        Player PlayerRef;
        List<Asteroid> AsteroidRefs;
        List<EnemyDrone> Drones;
        List<EnemyBoss> Bosses;
        EnemyBaseGun[] EnemyGuns = new EnemyBaseGun[8];
        Prefab DronePF;
        Prefab BossPF;
        int OreCount = 0;
        Timer LaunchTimer;
        bool LaunchNewDrone;

        public override void Start()
        {
            base.Start();

            Drones = new List<EnemyDrone>();
            Bosses = new List<EnemyBoss>();

            DronePF = Content.Load<Prefab>("Prefabs/EnemyDronePF");
            BossPF = Content.Load<Prefab>("Prefabs/EnemyBossPF");

            Entity launchTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(launchTimerE);
            LaunchTimer = launchTimerE.Get<Timer>();

            Position.Z = 60;
            IsActive = true;
            TheRadius = 50;
            int spawn = RandomGenerator.Next(3);
            Vector2 outterBuffer = new Vector2(400, 350);
            float innerBuffer = 400;

            for (int i = 0; i < 8; i++)
            {
                string gun = "EnemyBaseTurret-" + 0 + (i + 1);
                EnemyGuns[i] = Entity.FindChild(gun).Get<EnemyBaseGun>();
                EnemyGuns[i].Setup(SceneRef);
            }

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
        }

        public override void Update()
        {
            base.Update();

            if (Active)
            {
                CheckCollusion();

                if (LaunchTimer.Expired)
                {
                    float lTimer = 35 - OreCount;
                    LaunchTimer.Reset(RandomMinMax(lTimer / 3, lTimer));
                    int activeDrones = 0;

                    foreach (EnemyDrone drone in Drones)
                    {
                        if (drone.Active)
                            activeDrones++;
                    }

                    if (OreCount > 1)
                    {
                        SpawnBoss();
                        OreCount = 0;
                        return;
                    }

                    if (activeDrones > 1 + OreCount)
                        return;

                    SpawnDrone();
                }
            }
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
                if (CirclesIntersect(shot))
                {
                    CheckHit();
                }
            }
        }

        void SpawnBoss()
        {
            bool found = false;
            EnemyBoss thisBoss = null;

            foreach (EnemyBoss boss in Bosses)
            {
                if (!boss.Active)
                {
                    thisBoss = boss;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                thisBoss = SceneRef.SetupEntity(BossPF).Get<EnemyBoss>();
                Bosses.Add(thisBoss);
                thisBoss.Setup(SceneRef);
            }

            thisBoss.Launch(Position);
            SceneRef.RadarAccess.CreateEnemyBossCubes();
        }

        void SpawnDrone()
        {
            bool found = false;
            EnemyDrone thisDrone = null;

            foreach (EnemyDrone drone in Drones)
            {
                if (!drone.Active)
                {
                    thisDrone = drone;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                thisDrone = SceneRef.SetupEntity(DronePF).Get<EnemyDrone>();
                Drones.Add(thisDrone);
                thisDrone.Setup(SceneRef, this);
            }

            thisDrone.Launch();
        }
    }
}
