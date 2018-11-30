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
        public List<Chunk> Chunks { get => ChunkRefs; }

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

            TheRadius = 49;
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

            IsActive = true;
        }

        public override void Update()
        {
            base.Update();

            if (Active)
            {
                if (HitEdge())
                    MoveToOppisiteEdge();

                CheckCollusion();
            }
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerAccess;
        }

        public Chunk MineAttempt()
        {
            if (RandomMinMax(0, MaxHardness) > Hardness)
            {
                bool found = false;
                Chunk theChunk = null;

                foreach (Chunk chunk in ChunkRefs)
                {
                    if (!chunk.Active && !chunk.IsInTransit)
                    {
                        theChunk = chunk;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Entity chunkE = SceneRef.SetupEntity(ChunkPF);
                    theChunk = chunkE.Get<Chunk>();
                    theChunk.Setup(PlayerRef);
                    ChunkRefs.Add(chunkE.Get<Chunk>());
                }

                theChunk.Position = (Position + ((Vector3.Normalize(Velocity) * 60)) * -1);
                theChunk.Velocity = Velocity * 0.25f;
                theChunk.UpdatePR();

                return theChunk;
            }

            return null;
        }

        void CheckCollusion()
        {
            if (CirclesIntersect(PlayerRef))
            {
                PlayerRef.Bump(this);
            }
        }
    }
}
