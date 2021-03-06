﻿using System.Linq;
using TUI.Base;
using TUI.Base.Style;

namespace TUI.Widgets
{
    public class AlertWindow : VisualContainer
    {
        #region Data

        public Label Label { get; set; }
        public Button Button { get; set; }

        #endregion

        #region Constructor

        public AlertWindow(string text, ContainerStyle windowStyle = null, ButtonStyle buttonStyle = null)
            : base(0, 0, 0, 0, null, windowStyle ?? new ContainerStyle() { Wall = 165, WallColor = 27 })
        {
            SetAlignmentInParent(Alignment.Center);
            SetupLayout(Alignment.Center, Direction.Down, childIndent: 0);
            int lines = (text?.Count(c => c == '\n') ?? 0) + 1;
            Label = AddToLayout(new Label(0, 0, 0, 1 + lines * 3, text, null,
                new LabelStyle() { TextIndent = new Indent() { Horizontal = 1, Vertical = 1 } }))
                .SetFullSize(FullSize.Horizontal) as Label;
            buttonStyle = buttonStyle ?? new ButtonStyle()
            {
                WallColor = PaintID.DeepGreen,
                BlinkStyle = ButtonBlinkStyle.Full,
                BlinkColor = PaintID.White
            };
            buttonStyle.TriggerStyle = ButtonTriggerStyle.TouchEnd;
            Button = AddToLayout(new Button(0, 0, 14, 4, "ok", null, buttonStyle,
                ((self, touch) => self.Root.HidePopUp()))) as Button;
            SetWH(0, Label.Height + Button.Height);
            SetFullSize(FullSize.Horizontal);
        }

        #endregion
        #region Copy

        public AlertWindow(AlertWindow alertWindow)
            : this(alertWindow.Label.GetText(), new ContainerStyle(alertWindow.ContainerStyle),
                new ButtonStyle(alertWindow.Button.ButtonStyle))
        {
        }

        #endregion
    }
}
