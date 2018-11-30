using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.UI;
using Xenko.UI.Controls;

namespace Xenko_GameOff2018
{
    public class HUD : SyncScript
    {
        public int Score { set => ScoreTB.Text = value.ToString(); }
        public int Ore { set => OreTB.Text = value.ToString(); }
        public int Level { set => LevelTB.Text = value.ToString(); }
        public int Exp { set => ExpTB.Text = value.ToString(); }
        public int BaseOre { set => BaseOreTB.Text = value.ToString(); }
        public int HP { set => HPTB.Text = value.ToString(); }

        TextBlock ScoreTB;
        TextBlock OreTB;
        TextBlock LevelTB;
        TextBlock ExpTB;
        TextBlock BaseOreTB;
        TextBlock HPTB;

        public override void Start()
        {
            UIPage page = Entity.Get<UIComponent>().Page;
            UIElement rootElement = page.RootElement;
            ScoreTB = rootElement.FindVisualChildOfType<TextBlock>("Score");
            OreTB = rootElement.FindVisualChildOfType<TextBlock>("Ore");
            LevelTB = rootElement.FindVisualChildOfType<TextBlock>("Level");
            ExpTB = rootElement.FindVisualChildOfType<TextBlock>("Exp");
            BaseOreTB = rootElement.FindVisualChildOfType<TextBlock>("BaseOre");
            HPTB = rootElement.FindVisualChildOfType<TextBlock>("HP");
        }

        public override void Update()
        {

        }
    }
}
