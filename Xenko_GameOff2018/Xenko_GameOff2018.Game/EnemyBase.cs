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
        EnemyBaseGun[] Guns = new EnemyBaseGun[8];
        Prefab DronePF;
        Prefab BossPF;
        int Sector;
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

                    if (OreCount > 5)
                    {
                        SpawnBoss();
                        OreCount = 0;

                        foreach(EnemyBaseGun gun in Guns)
                        {
                            gun.FireRate = 10;
                        }

                        return;
                    }

                    if (activeDrones > 1 + OreCount)
                        return;

                    SpawnDrone();
                }
            }
        }

        public void Setup(SceneControl scene, int sector)
        {
            SceneRef = scene;
            Sector = sector;
            PlayerRef = scene.PlayerRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
            RandomGenerator = SceneControl.RandomGenerator;

            for (int i = 0; i < 8; i++)
            {
                string gun = "EnemyBaseTurret-" + 0 + (i + 1);
                Guns[i] = Entity.FindChild(gun).Get<EnemyBaseGun>();
                Guns[i].Setup(SceneRef);
            }

            Spawn();
        }

        public void AddChunk(OreType type)
        {
            OreCount++;

            foreach(EnemyBaseGun gun in Guns)
            {
                if (OreCount > 9)
                    return;

                gun.FireRate = 10 - OreCount;
            }
        }

        public void Disable()
        {
            IsActive = false;
        }

        public void Spawn()
        {
            IsActive = true;
            OreCount = 0;
            HitPoints = 100;

            Vector2 outterBuffer = new Vector2(400, 350);
            Vector2 innerBuffer = new Vector2(450, 400);

            switch (Sector)
            {
                case 0:
                    Position.X = RandomMinMax(-innerBuffer.X, Edge.X - outterBuffer.X);
                    Position.Y = RandomMinMax(innerBuffer.Y, Edge.Y - outterBuffer.Y);
                    break;
                case 1:
                    Position.X = RandomMinMax(innerBuffer.X, Edge.X - outterBuffer.X);
                    Position.Y = RandomMinMax(-Edge.Y + outterBuffer.Y, innerBuffer.Y);
                    break;
                case 2:
                    Position.X = RandomMinMax(-Edge.X + outterBuffer.X, innerBuffer.X);
                    Position.Y = RandomMinMax(-Edge.Y + outterBuffer.Y, -innerBuffer.Y);
                    break;
                case 3:
                    Position.X = RandomMinMax(-Edge.X - outterBuffer.X, -innerBuffer.X);
                    Position.Y = RandomMinMax(-innerBuffer.Y, Edge.Y - outterBuffer.Y);
                    break;
            }

            foreach (EnemyBaseGun gun in Guns)
            {
                gun.Enable();
            }
        }

        void Destroyed()
        {
            Disable();

            foreach (EnemyDrone drone in Drones)
            {
                drone.Destroyed();
            }

            SceneRef.CheckEnemyBases();
         }

        void CheckHit(PlayerShot shot)
        {
            int gunCount = 0;

            foreach(EnemyBaseGun gun in Guns)
            {
                if (gun.Active)
                {
                    gunCount++;

                    if (shot.CirclesIntersect(gun.Position + Position, gun.Radius))
                    {
                        gun.Disable();
                        shot.Disable();
                    }
                }
            }

            if (gunCount == 0)
            {
                shot.Disable();
                HitPoints -= 10;

                if (HitPoints < 0)
                {
                    Destroyed();
                }
            }
        }

        void CheckCollusion()
        {
            foreach (PlayerShot shot in PlayerRef.ShotsAccess)
            {
                if (shot.Active)
                {
                    if (CirclesIntersect(shot))
                    {
                        CheckHit(shot);
                    }
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
