using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;

namespace Xenko_GameOff2018
{
    public class Radar : SyncScript
    {
        Prefab WhiteCubePF;
        Prefab RedCubePF;
        Prefab OrangeCubePF;
        SceneControl SceneRef;
        Player PlayerRef;
        PlayerBase PlayerBaseRef;
        Entity PlayerDot;
        Vector3 HomePosition;
        Vector3 PlayerPosition;
        List<Asteroid> AsteroidRefs;
        List<EnemyBase> EnemyBaseRefs;
        List<Vector3> BasePositions;
        List<EnemyBoss> BossRefs;
        List<Entity> AsteroidCubes;
        List<Entity> BaseCubes;
        List<Entity> BossCubes;
        Entity PlayerBaseDot;

        public override void Start()
        {
            AsteroidCubes = new List<Entity>();
            BaseCubes = new List<Entity>();
            BossCubes = new List<Entity>();
            BasePositions = new List<Vector3>();
            BossRefs = new List<EnemyBoss>();

            WhiteCubePF = Content.Load<Prefab>("Prefabs/WhiteCubePF");
            RedCubePF = Content.Load<Prefab>("Prefabs/RedCubePF");
            OrangeCubePF = Content.Load<Prefab>("Prefabs/OrangeCubePF");
            Prefab blueCubePF = Content.Load<Prefab>("Prefabs/BlueCubePF");
            Prefab lightBlueCubePF = Content.Load<Prefab>("Prefabs/LightBlueCubePF");


            //TestCubes();
            HomePosition = new Vector3(336 - (336f/5) - 60, 269 - (269f/5) - 60, 0);
            PlayerDot = SceneRef.SetupEntity(lightBlueCubePF);
            PlayerBaseDot = SceneRef.SetupEntity(blueCubePF);
            PlayerDot.Transform.Scale = new Vector3(1.5f, 1.5f, 1);

            CreateAsteroidCubes();
            CreateEnemyBaseCubes();
        }

        public override void Update()
        {
            PlayerPosition = PlayerRef.Position;
            PlayerDot.Transform.Position = HomePosition + PlayerPosition;
            PlayerBaseDot.Transform.Position = HomePosition + PlayerPosition;
            PlayerBaseDot.Transform.Position -= ((PlayerPosition / 10) / 5);

            DisplayAsteroids();
            DisplayEnemyBases();
            DisplayEnemyBosses();
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerRefAccess;
            PlayerBaseRef = scene.PlayerBaseRefAccess;
            AsteroidRefs = scene.AsteroidRefAccess;
            EnemyBaseRefs = scene.EnemyBaseRefAccess;
        }

        public void CreateAsteroidCubes()
        {
            AsteroidCubes.Clear();

            foreach (Asteroid rock in AsteroidRefs)
            {
                AsteroidCubes.Add(SceneRef.SetupEntity(WhiteCubePF));
            }
        }

        public void CreateEnemyBaseCubes()
        {
            BaseCubes.Clear();
            BasePositions.Clear();

            foreach(EnemyBase enemy in EnemyBaseRefs)
            {
                BaseCubes.Add(SceneRef.SetupEntity(RedCubePF));
                BasePositions.Add(enemy.Position);
            }
        }

        public void CreateEnemyBossCubes()
        {
            BossCubes.Clear();
            BossRefs.Clear();

            foreach (EnemyBase enemyBase in EnemyBaseRefs)
            {
                foreach (EnemyBoss boss in enemyBase.BossAccess)
                {
                    if (boss.Active)
                    {
                        BossCubes.Add(SceneRef.SetupEntity(OrangeCubePF));
                        BossRefs.Add(boss);
                    }
                }
            }
        }

        void DisplayEnemyBosses()
        {
            int bossNumber = 0;

            foreach (Entity bossDot in BossCubes)
            {
                Vector3 bossPos = ((BossRefs[bossNumber].Position / 10) / 5 + HomePosition + PlayerPosition);
                bossPos -= ((PlayerPosition / 10) / 5);
                bossDot.Transform.Position = bossPos;
                bossNumber++;
            }
        }

        void DisplayEnemyBases()
        {
            int enemyNumber = 0;

            foreach(Entity enemyDot in BaseCubes)
            {
                Vector3 enemyPos = ((BasePositions[enemyNumber] / 10) / 5 + HomePosition + PlayerPosition);
                enemyPos -= ((PlayerPosition / 10) / 5);
                enemyDot.Transform.Position = enemyPos;
                enemyNumber++;
            }
        }

        void DisplayAsteroids()
        {
            int rockNumber = 0;

            foreach(Entity rockDot in AsteroidCubes)
            {
                Vector3 rockPos = ((AsteroidRefs[rockNumber].Position / 10) / 5) + HomePosition + PlayerPosition;
                rockPos -= ((PlayerPosition / 10) / 5);
                rockDot.Transform.Position = rockPos;
                rockNumber++;
            }

        }

        void TestCubes()
        {
            Entity testTL = SceneRef.SetupEntity(WhiteCubePF);
            Entity testBR = SceneRef.SetupEntity(WhiteCubePF);

            float x = 336;
            float y = 269;
            testTL.Transform.Position = new Vector3(-x, y, 0);
            testBR.Transform.Position = new Vector3(x, -y, 0);
            testTL.Transform.Scale = new Vector3(4, 4, 1);
            testBR.Transform.Scale = new Vector3(4, 4, 1);
        }
    }
}
