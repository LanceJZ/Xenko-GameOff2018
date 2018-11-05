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

namespace Xenko_GameOff2018
{
    public class SceneControl : SyncScript
    {
        Prefab PlayerPF;
        Prefab EnemyBasePF;
        Player PlayerRef;

        public override void Start()
        {
            PlayerPF = Content.Load<Prefab>("Prefabs/PlayerPF");
            Entity playerE = PlayerPF.Instantiate().First();
            PlayerRef = playerE.Get<Player>();
            SceneSystem.SceneInstance.RootScene.Entities.Add(playerE);

            EnemyBasePF = Content.Load<Prefab>("Prefabs/EnemyBasePF");

            for (int i = 0; i < 8; i++)
            {
                Entity enemyBaseE = EnemyBasePF.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(enemyBaseE);
            }

            Prefab StarPF = Content.Load<Prefab>("Prefabs/StarPF");

            for (int i = 0; i < 300; i++)
            {
                Entity starE = StarPF.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(starE);
            }

        }

        public override void Update()
        {

        }
    }
}