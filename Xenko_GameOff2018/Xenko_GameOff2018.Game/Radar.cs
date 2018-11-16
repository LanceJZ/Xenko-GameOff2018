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
        Prefab BlueCubePF;
        SceneControl SceneRef;
        Player PlayerRef;
        PlayerBase PlayerBaseRef;
        Entity PlayerDot;
        Vector3 HomePosition;
        List<Asteroid> AsteroidRefs;
        List<EnemyBase> EnemyBaseRefs;
        List<Entity> AsteroidCubeRefs;
        List<Entity> EnemyBaseCubeRefs;
        Entity PlayerBaseDot;

        public override void Start()
        {
            AsteroidCubeRefs = new List<Entity>();
            EnemyBaseCubeRefs = new List<Entity>();

            WhiteCubePF = Content.Load<Prefab>("Prefabs/WhiteCubePF");
            RedCubePF = Content.Load<Prefab>("Prefabs/RedCubePF");
            BlueCubePF = Content.Load<Prefab>("Prefabs/BlueCubePF");


            //TestCubes();
            HomePosition = new Vector3(336 - (336f/5) - 60, 269 - (269f/5) - 60, 0);
            PlayerDot = SceneRef.SetupEntity(WhiteCubePF);
            PlayerBaseDot = SceneRef.SetupEntity(BlueCubePF);
            PlayerDot.Transform.Scale = new Vector3(2, 2, 1);

            CreateAsteroidCubes();
            CreateEnemyBaseCubes();
        }

        public override void Update()
        {
            DisplayAsteroids();
            DisplayEnemyBases();

            PlayerDot.Transform.Position = HomePosition + PlayerRef.Position;
            PlayerBaseDot.Transform.Position = HomePosition + PlayerRef.Position;
            PlayerBaseDot.Transform.Position -= ((PlayerRef.Position / 10) / 5);
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
            foreach (Asteroid rock in AsteroidRefs)
            {
                Entity theRock = SceneRef.SetupEntity(WhiteCubePF);
                //theRock.Transform.Scale = new Vector3(2, 2, 1);
                AsteroidCubeRefs.Add(theRock);
            }
        }

        public void CreateEnemyBaseCubes()
        {
            foreach(EnemyBase enemy in EnemyBaseRefs)
            {
                Entity theEnemy = SceneRef.SetupEntity(RedCubePF);

                EnemyBaseCubeRefs.Add(theEnemy);
            }
        }

        void DisplayEnemyBases()
        {
            int enemyNumber = 0;

            foreach(Entity enemyDot in EnemyBaseCubeRefs)
            {
                Vector3 enemyPos = ((EnemyBaseRefs[enemyNumber].Position / 10) / 5 + HomePosition + PlayerRef.Position);
                enemyPos -= ((PlayerRef.Position / 10) / 5);
                enemyDot.Transform.Position = enemyPos;
                enemyNumber++;
            }
        }

        void DisplayAsteroids()
        {
            int rockNumber = 0;

            foreach(Entity rockDot in AsteroidCubeRefs)
            {
                Vector3 rockPos = ((AsteroidRefs[rockNumber].Position / 10) / 5) + HomePosition + PlayerRef.Position;
                rockPos -= ((PlayerRef.Position / 10) / 5);
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
