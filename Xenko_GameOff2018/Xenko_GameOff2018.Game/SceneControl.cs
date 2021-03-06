﻿using System;
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
using System.Diagnostics.Contracts;

namespace Xenko_GameOff2018
{
    public enum GameState
    {
        Over,
        InPlay,
        HighScore,
        Attract
    };

    public enum OreType
    {
        Iron,
        Nickel,
        Iridium,
        Palladium,
        Platinum,
        Gold,
        Magnesium,
    };

    public class SceneControl : SyncScript
    {
        public HUD HUDAccess { get => TheHUD; }
        public Radar RadarAccess { get => TheRadar; }
        public Player PlayerAccess { get => PlayerRef; }
        public PlayerBase PlayerBaseAccess { get => PlayerBaseRef; }
        public List<EnemyBase> EnemyBaseAccess { get => EnemyBaseRefs; }
        public List<Asteroid> AsteroidAccess { get => AsteroidRefs; }
        public GameState TheGameMode { get => GameMode; }
        public static Random RandomGenerator { get => RandomNumbers; }

        GameState GameMode = GameState.Over;
        Prefab EnemyBasePF;
        Prefab AsteroidPF;
        Player PlayerRef;
        PlayerBase PlayerBaseRef;
        Radar TheRadar;
        HUD TheHUD;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;
        static Random RandomNumbers;

        SoundInstance BumpSI;

        int Score = 0;

        public override void Start()
        {
            Game.Window.Title = "Dynastar";

            if (RandomNumbers == null)
                RandomNumbers = new Random(DateTime.UtcNow.Millisecond * 666);

            EnemyBaseRefs = new List<EnemyBase>();
            AsteroidRefs = new List<Asteroid>();

            Prefab playerPF = Content.Load<Prefab>("Prefabs/PlayerPF");
            Prefab playerBasePF = Content.Load<Prefab>("Prefabs/PlayerBasePF");
            EnemyBasePF = Content.Load<Prefab>("Prefabs/EnemyBasePF");
            AsteroidPF = Content.Load<Prefab>("Prefabs/AsteroidPF");
            Prefab hudPF = Content.Load<Prefab>("Prefabs/HudPF");

            BumpSI = Content.Load<Sound>("Sounds/BumpSound").CreateInstance();

            PlayerRef = SetupEntity(playerPF).Get<Player>();
            PlayerRef.Setup(this);
            PlayerBaseRef = SetupEntity(playerBasePF).Get<PlayerBase>();
            PlayerBaseRef.Setup(this);

            for (int i = 0; i < 4; i++)
            {
                SpawnAsteroid();
            }

            for (int i = 0; i < 4; i++)
            {
                SpawnBase(i);
            }

            Entity radarE = new Entity { new Radar() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(radarE);
            TheRadar = radarE.Get<Radar>();
            TheRadar.Setup(this);
            TheRadar.CreateEnemyBaseCubes();

            TheHUD = SetupEntity(hudPF).Get<HUD>();
        }

        public override void Update()
        {
            CheckBumps();


        }

        public void CheckEnemyBases()
        {
            int baseCount = 0;

            foreach(EnemyBase enemyBase in EnemyBaseRefs)
            {
                if (enemyBase.Active)
                    baseCount++;
            }

            if (baseCount == 0)
            {
                foreach (EnemyBase enemyBase in EnemyBaseRefs)
                {
                    enemyBase.Spawn();
                }

                PlayerRef.Reset();
            }

            TheRadar.CreateEnemyBaseCubes();
        }

        public Entity SetupEntity(Prefab prefab)
        {
            Contract.Ensures(Contract.Result<Entity>() != null);
            Entity entity = prefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
            return entity;
        }

        public void BumpSound()
        {
            BumpSI.Stop();
            BumpSI.Play();
        }

        void SpawnBase(int sector)
        {
            Entity enemyBaseE = SetupEntity(EnemyBasePF);
            enemyBaseE.Get<EnemyBase>().Setup(this, sector);
            EnemyBaseRefs.Add(enemyBaseE.Get<EnemyBase>());
        }

        void SpawnAsteroid()
        {
            Entity asteroidE = SetupEntity(AsteroidPF);
            asteroidE.Get<Asteroid>().Setup(this);
            AsteroidRefs.Add(asteroidE.Get<Asteroid>());
        }

        void CheckBumps()
        {
            foreach(EnemyBase theBaseA in EnemyBaseRefs)
            {
                if (theBaseA.DroneAccess == null)
                    return;

                foreach(EnemyDrone droneA in theBaseA.DroneAccess)
                {
                    foreach (EnemyBase theBaseB in EnemyBaseRefs)
                    {
                        if (theBaseB.DroneAccess == null)
                            return;

                        foreach (EnemyDrone droneB in theBaseB.DroneAccess)
                        {
                            if (droneA != droneB)
                            {
                                if (droneA.Active && droneB.Active)
                                {
                                    if (droneA.CirclesIntersect(droneB))
                                    {
                                        BumpSI.Stop();
                                        BumpSI.Play();

                                        droneA.Bumped(droneB);
                                        droneB.Bumped(droneA);
                                    }
                                }
                            }

                        }

                        if (theBaseA.BossAccess == null || theBaseB.BossAccess == null)
                            return;

                        foreach (EnemyBoss bossA in theBaseA.BossAccess)
                        {
                            foreach (EnemyBoss bossB in theBaseB.BossAccess)
                            {
                                if (bossA != bossB)
                                {
                                    if (bossA.Active && bossB.Active)
                                    {
                                        if (bossA.CirclesIntersect(bossB))
                                        {
                                            BumpSI.Stop();
                                            BumpSI.Play();
                                            bossA.Bumped(bossB);
                                            bossB.Bumped(bossA);
                                        }
                                    }
                                }
                            }

                            if (droneA.Active && bossA.Active)
                            {
                                if (droneA.CirclesIntersect(bossA))
                                {
                                    BumpSI.Stop();
                                    BumpSI.Play();
                                    droneA.Bumped(bossA);
                                    bossA.Bumped(droneA);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}