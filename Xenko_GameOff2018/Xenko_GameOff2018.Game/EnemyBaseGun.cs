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

namespace Xenko_GameOff2018
{
    public class EnemyBaseGun : PO
    {
        public float FireRate { set => CurrentFireRate = value; }

        Player PlayerRef;
        SceneControl SceneRef;
        Prefab MissilePF;
        List<Missile> Missiles;
        Vector3 WorldPos;
        Timer FireTimer;
        float CurrentFireRate;

        public override void Start()
        {
            base.Start();

            Entity lifeTimerE = new Entity { new Timer() };
            SceneSystem.SceneInstance.RootScene.Entities.Add(lifeTimerE);
            FireTimer = lifeTimerE.Get<Timer>();

            Missiles = new List<Missile>();
            TheRadius = 10;
            MissilePF = Content.Load<Prefab>("Prefabs/MissilePF");
        }

        public override void Update()
        {
            base.Update();

            if (Active)
            {
                WorldPos = Entity.Transform.Parent.Position + Position;

                Rotation.Z = AngleFromVectors(WorldPos, PlayerRef.Position);

                if (FireTimer.Expired && PlayerRef != null)
                {
                    if (Vector3.Distance(WorldPos, PlayerRef.Position) < 300)
                    {
                        FireMissile();
                    }

                    if (PlayerRef.LevelAccess < 9)
                        CurrentFireRate = 10 - PlayerRef.LevelAccess;

                    FireTimer.Reset(RandomMinMax(CurrentFireRate, CurrentFireRate + PlayerRef.LevelAccess));
                }
            }
        }

        public void Setup(SceneControl scene)
        {
            SceneRef = scene;
            PlayerRef = scene.PlayerAccess;
            RandomGenerator = SceneControl.RandomGenerator;
        }

        public void Enable()
        {
            IsActive = true;
        }

        public void Disable()
        {
            IsActive = false;
        }

        void FireMissile()
        {
            bool found = false;
            Missile thisMissile = null;

            foreach (Missile missile in Missiles)
            {
                if (!missile.Active)
                {
                    thisMissile = missile;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                thisMissile = SceneRef.SetupEntity(MissilePF).Get<Missile>();
                Missiles.Add(thisMissile);
                thisMissile.Setup(SceneRef);
            }

            thisMissile.Fire(WorldPos + VelocityFromRadian(Radius - 5, Rotation.Z), Rotation.Z);

            //FireSI.Stop();
            //FireSI.Play();
        }
    }
}
