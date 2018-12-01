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
    public class PlayerBase : PO
    {
        public int HealPlayer
        {
            get
            {
                if (OreCount > 0)
                {
                    if (PlayerRef.HPAccess < PlayerRef.MaxHPAccess)
                    {
                        int repairPointsNeeded = PlayerRef.MaxHPAccess - PlayerRef.HPAccess;
                        int repairOre = OreCount * 10;
                        int possableRepairs = repairPointsNeeded - repairOre;

                        if (repairPointsNeeded > repairOre)
                        {
                            OreCount = 0;
                            SceneRef.HUDAccess.BaseOre = OreCount;
                            return possableRepairs;
                        }
                        else
                        {
                            OreCount -= (repairPointsNeeded / 10);
                            SceneRef.HUDAccess.BaseOre = OreCount;
                            return repairPointsNeeded;
                        }
                    }
                }

                return 0;
            }
        }

        SceneControl SceneRef;
        Player PlayerRef;
        int OreCount = 0;

        public override void Start()
        {
            base.Start();

            TheRadius = 65;
            Position.Z = 70;
            RandomGenerator = SceneControl.RandomGenerator;
            IsActive = true;
        }

        public override void Update()
        {
            base.Update();


        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerAccess;
        }

        public void UnloadOre(OreType oreType)
        {
            OreCount++;
            SceneRef.HUDAccess.BaseOre = OreCount;
        }

        public void Reset()
        {
            OreCount = 0;
            SceneRef.HUDAccess.BaseOre = OreCount;
        }
    }
}
