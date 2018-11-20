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
        public Radar RadarAccess { get => TheRadar; }
        public Player PlayerRefAccess { get => PlayerRef; }
        public PlayerBase PlayerBaseRefAccess { get => PlayerBaseRef; }
        public List<EnemyBase> EnemyBaseRefAccess { get => EnemyBaseRefs; }
        public List<Asteroid> AsteroidRefAccess { get => AsteroidRefs; }
        public GameState TheGameMode { get => GameMode; }

        GameState GameMode = GameState.Over;
        Prefab PlayerPF;
        Prefab EnemyBasePF;
        Prefab AsteroidPF;
        Player PlayerRef;
        PlayerBase PlayerBaseRef;
        Radar TheRadar;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;

        public override void Start()
        {
            EnemyBaseRefs = new List<EnemyBase>();
            AsteroidRefs = new List<Asteroid>();

            PlayerPF = Content.Load<Prefab>("Prefabs/PlayerPF");
            PlayerRef = SetupEntity(PlayerPF).Get<Player>();

            Prefab playerBasePF = Content.Load<Prefab>("Prefabs/PlayerBasePF");
            PlayerBaseRef = SetupEntity(playerBasePF).Get<PlayerBase>();

            EnemyBasePF = Content.Load<Prefab>("Prefabs/EnemyBasePF");

            for (int i = 0; i < 4; i++)
            {
                SpawnBase();
            }

            AsteroidPF = Content.Load<Prefab>("Prefabs/AsteroidPF");

            for (int i = 0; i < 4; i++)
            {
                SpawnAsteroid();
            }

            PlayerRef.Setup(this);

            Entity radarE = new Entity { new Radar() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(radarE);
            TheRadar = radarE.Get<Radar>();
            TheRadar.Setup(this);
        }

        public override void Update()
        {
            CheckDroneBump();
        }

        public Entity SetupEntity(Prefab prefab)
        {
            Contract.Ensures(Contract.Result<Entity>() != null);
            Entity entity = prefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(entity);
            return entity;
        }

        void SpawnBase()
        {
            Entity enemyBaseE = SetupEntity(EnemyBasePF);
            enemyBaseE.Get<EnemyBase>().Setup(this);
            EnemyBaseRefs.Add(enemyBaseE.Get<EnemyBase>());
        }

        void SpawnAsteroid()
        {
            Entity asteroidE = SetupEntity(AsteroidPF);
            asteroidE.Get<Asteroid>().Setup(this);
            AsteroidRefs.Add(asteroidE.Get<Asteroid>());
        }

        void CheckDroneBump()
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
                                        droneA.Bumped(droneB.Position, droneB.Velocity);
                                        droneB.Bumped(droneA.Position, droneA.Velocity);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}