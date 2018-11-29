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
        public int Score { set => TheScore = value.ToString(); }

        String TheScore;
        TextBlock ScoreTB;

        public override void Start()
        {
            UIPage page = Entity.Get<UIComponent>().Page;
            UIElement rootElement = page.RootElement;
            ScoreTB = rootElement.FindVisualChildOfType<TextBlock>("Score");
        }

        public override void Update()
        {
            ScoreTB.Text = TheScore;
        }
    }
}
