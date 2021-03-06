﻿using System;
using System.Linq;
using TUI.Base;
using TUI.Base.Style;

namespace TUI.Widgets
{
    public class ConfirmWindow : VisualObject
    {
        #region Data

        public Label Label { get; set; }
        public Button YesButton { get; set; }
        public Button NoButton { get; set; }
        public Action<bool> ConfirmCallback { get; set; }
        private VisualContainer Container { get; set; }

        #endregion

        #region Constructor

        public ConfirmWindow(string text, Action<bool> callback, ContainerStyle style = null,
                ButtonStyle yesButtonStyle = null, ButtonStyle noButtonStyle = null)
            : base(0, 0, 0, 0)
        {
            ConfirmCallback = callback ?? throw new ArgumentNullException(nameof(callback));

            SetAlignmentInParent(Alignment.Center);
            SetFullSize(FullSize.Both);

            Container = Add(new VisualContainer(style ?? new ContainerStyle()
                { Wall = 165, WallColor = 27 })) as VisualContainer;
            Container.SetAlignmentInParent(Alignment.Center)
                .SetFullSize(FullSize.Horizontal)
                .SetupLayout(Alignment.Center, Direction.Down, childIndent: 0);

            int lines = (text?.Count(c => c == '\n') ?? 0) + 1;
            Label = Container.AddToLayout(new Label(0, 0, 0, 1 + lines * 3, text, null,
                new LabelStyle() { TextIndent = new Indent() { Horizontal = 1, Vertical = 1 } }))
                .SetFullSize(FullSize.Horizontal) as Label;

            VisualContainer yesno = Container.AddToLayout(new VisualContainer(0, 0, 24, 4)) as VisualContainer;

            yesButtonStyle = yesButtonStyle ?? new ButtonStyle()
            {
                WallColor = PaintID.DeepGreen,
                BlinkStyle = ButtonBlinkStyle.Full,
                BlinkColor = PaintID.White
            };
            yesButtonStyle.TriggerStyle = ButtonTriggerStyle.TouchEnd;
            YesButton = yesno.Add(new Button(0, 0, 12, 4, "yes", null, yesButtonStyle,
                ((self, touch) =>
                {
                    self.Root.HidePopUp();
                    callback.Invoke(true);
                }))) as Button;

            noButtonStyle = noButtonStyle ?? new ButtonStyle()
            {
                WallColor = PaintID.DeepRed,
                BlinkStyle = ButtonBlinkStyle.Full,
                BlinkColor = PaintID.White
            };
            noButtonStyle.TriggerStyle = ButtonTriggerStyle.TouchEnd;
            NoButton = yesno.Add(new Button(12, 0, 12, 4, "no", null, noButtonStyle,
                ((self, touch) =>
                {
                    self.Root.HidePopUp();
                    callback.Invoke(false);
                }))) as Button;

            Callback = CancelCallback;
            Container.SetWH(0, Label.Height + yesno.Height);
        }

        #endregion
        #region CancelCallback

        private void CancelCallback(VisualObject window, Touch touch)
        {
            (window as ConfirmWindow).ConfirmCallback.Invoke(false);
            window.Root.HidePopUp();
        }

        #endregion
        #region Copy

        public ConfirmWindow(ConfirmWindow confirmWindow)
            : this(confirmWindow.Label.GetText(), confirmWindow.ConfirmCallback.Clone() as Action<bool>,
                new ContainerStyle(confirmWindow.Container.ContainerStyle),
                new ButtonStyle(confirmWindow.YesButton.ButtonStyle),
                new ButtonStyle(confirmWindow.NoButton.ButtonStyle))
        {
        }

        #endregion
    }
}
