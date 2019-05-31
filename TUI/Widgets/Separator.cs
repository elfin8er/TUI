﻿using TUI.Base;
using TUI.Base.Style;

namespace TUI.Widgets
{
    public class Separator : VisualObject
    {
        public Separator(int size, UIStyle style = null)
            : base(0, 0, size, size, null, style)
        {
        }

        public Separator(int width, int height, UIStyle style = null)
            : base(0, 0, width, height, null, style)
        {
        }

        public override VisualObject Pulse(PulseType type) => this;
        public override VisualObject Update() => UpdateThis();
        public override VisualObject Apply() => ApplyTiles();
    }
}