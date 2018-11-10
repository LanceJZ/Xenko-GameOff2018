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
    public class Asteroid : PO
    {
        Prefab ChunkPF;
        Player PlayerRef;
        List<EnemyBase> EnemyBaseRefs;
        List<Chunk> ChunkRefs;
        SceneControl SceneRef;
        float Hardness;
        float MaxHardness = 10;

        public override void Start()
        {
            base.Start();

            ChunkPF = Content.Load<Prefab>("Prefabs/ChunkBasePF");
            ChunkRefs = new List<Chunk>();

            Radius = 49;
            Position.X = RandomMinMax(-Edge.X, Edge.X);
            Position.Y = RandomMinMax(-Edge.Y, Edge.Y);
            Position.Z = 0;

            Hardness = RandomMinMax(1, MaxHardness + 1);

            float rAmount = RandomMinMax(0.1f, 1);

            float rX = RandomMinMax(-rAmount, rAmount);
            float rY = RandomMinMax(-rAmount, rAmount);

            RotationVelocity = new Vector3(rX, rY, 0);

            float vAmount = RandomMinMax(3, 7);
            float vX = 0;
            float vY = 0;

            switch (RandomGenerator.Next(0, 1))
            {
                case 0:
                    vX = RandomMinMax(vAmount, vAmount + 5);
                    vY = RandomMinMax(vAmount, vAmount + 5);
                    break;
                case 1:
                    vX = RandomMinMax(-vAmount - 5, -vAmount);
                    vY = RandomMinMax(-vAmount - 5, -vAmount);
                    break;
            }

            Velocity = new Vector3(vX, vY, 0);

            Active = true;

        }

        public override void Update()
        {
            base.Update();

            if (HitEdge())
                MoveToOppisiteEdge();

        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
        }

        public Chunk MineAttempt()
        {
            if (RandomMinMax(0, MaxHardness) > Hardness)
            {
                Entity chunkE = SceneRef.SetupEntity(ChunkPF);
                Chunk chunkS = chunkE.Get<Chunk>();
                chunkS.Position = (Position + ((Vector3.Normalize(Velocity) * 55)) * -1);
                chunkS.Velocity = Velocity * 0.25f;
                ChunkRefs.Add(chunkE.Get<Chunk>());
                return chunkS;
            }

            return null;
        }
    }
}
