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
        public Player PlayerRefAccess { get => PlayerRef; }
        public List<EnemyBase> EnemyBaseRefAccess { get => EnemyBaseRefs; }
        public List<Asteroid> AsteroidRefAccess { get => AsteroidRefs; }
        public GameState TheGameMode { get => GameMode; }

        GameState GameMode = GameState.Over;
        Prefab PlayerPF;
        Prefab EnemyBasePF;
        Prefab AsteroidPF;
        Player PlayerRef;
        List<EnemyBase> EnemyBaseRefs;
        List<Asteroid> AsteroidRefs;

        public override void Start()
        {
            EnemyBaseRefs = new List<EnemyBase>();
            AsteroidRefs = new List<Asteroid>();

            PlayerPF = Content.Load<Prefab>("Prefabs/PlayerPF");
            PlayerRef = SetupEntity(PlayerPF).Get<Player>();
            PlayerRef.Setup(this);

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
        }

        public override void Update()
        {

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
    }
}