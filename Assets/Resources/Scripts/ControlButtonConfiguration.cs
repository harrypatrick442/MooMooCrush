using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class ControlButtonConfiguration : MonoBehaviour
        {
            public ControlButton.Types Type;
            public Sprite SpriteUp;
            public Sprite SpriteDown;
            public Sprite SpriteActiveUp;
            public Sprite SpriteActiveDown;
            public Action _GoingDown;
            public Action _GoingUp;
            public Action _GoneDown;
            public Action _GoneUp;
    }
}
