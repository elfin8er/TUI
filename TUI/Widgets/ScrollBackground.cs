﻿using System;
using System.Linq;
using TUI.Base;
using TUI.Base.Style;

namespace TUI.Widgets
{
    public class ScrollBackground : VisualObject
    {
        #region Data

        private Action<ScrollBackground, int> ScrollBackgroundCallback;
        public int BeginIndent { get; protected set; }
        public int Limit { get; protected set; }
        public bool AllowToPull { get; set; }
        public bool RememberTouchPosition { get; set; }

        #endregion

        #region Constructor

        public ScrollBackground(bool allowToPull = true, bool rememberTouchPosition = true, bool useMoving = true, Action<ScrollBackground, int> callback = null)
            : base(0, 0, 0, 0, new UIConfiguration() { UseMoving=useMoving, UseEnd=true, UseOutsideTouches=true })
        {
            AllowToPull = allowToPull;
            RememberTouchPosition = rememberTouchPosition;
            ScrollBackgroundCallback = callback;

            SetFullSize(FullSize.Both);
        }

        #endregion
        #region Invoke

        public override bool Invoke(Touch touch)
        {
            if (Parent?.Style?.Layout == null)
                throw new Exception("Scroll has no parent or parent doesn't have layout.");
            LayoutStyle layout = Parent.Style.Layout;
            int indent = layout.LayoutIndent;
            Limit = layout.IndentLimit;
            bool vertical = layout.Direction == Direction.Up || layout.Direction == Direction.Down;
            bool forward = layout.Direction == Direction.Right || layout.Direction == Direction.Down;
            if (touch.State == TouchState.Begin)
                BeginIndent = indent;
            if (touch.State == TouchState.End || (Configuration.UseMoving && touch.State == TouchState.Moving))
            {
                int newIndent;
                int indentDelta;
                if (RememberTouchPosition)
                {
                    indentDelta = vertical
                        ? touch.AbsoluteY - touch.Session.BeginTouch.AbsoluteY
                        : touch.AbsoluteX - touch.Session.BeginTouch.AbsoluteX;
                    newIndent = BeginIndent + (forward ? -indentDelta : indentDelta);
                }
                else
                {
                    indentDelta = vertical
                        ? touch.AbsoluteY - touch.Session.PreviousTouch.AbsoluteY
                        : touch.AbsoluteX - touch.Session.PreviousTouch.AbsoluteX;
                    newIndent = indent + (forward ? -indentDelta : indentDelta);
                }
                if (newIndent != indent || touch.State == TouchState.End)
                {
                    VisualObject first = layout.Objects.FirstOrDefault();
                    VisualObject last = layout.Objects.LastOrDefault();
                    if (first == null || last == null)
                        return true;
                    if (touch.State == TouchState.End || !AllowToPull)
                    {
                        if (newIndent < 0)
                            newIndent = 0;
                        else if (newIndent > Limit)
                            newIndent = Limit;
                    }
                    if (Parent.Style.Layout.LayoutIndent != newIndent)
                    {
                        Parent.LayoutIndent(newIndent);
                        Action<ScrollBackground, int> callback = ScrollBackgroundCallback;
                        if (callback != null)
                            callback.Invoke(this, newIndent);
                        else
                            Parent.Update().Apply().Draw();
                    }
                }
            }
            return true;
        }

        #endregion
        #region PulseThisNative

        protected override void PulseThisNative(PulseType type)
        {
            base.PulseThisNative(type);
            if (type == PulseType.Reset)
            {
                if (Parent.Style.Layout.LayoutIndent != 0)
                {
                    Parent.LayoutIndent(0);
                    Action<ScrollBackground, int> callback = ScrollBackgroundCallback;
                    if (callback != null)
                        callback.Invoke(this, 0);
                }
            }
        }

        #endregion
    }
}
