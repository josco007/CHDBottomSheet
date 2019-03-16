using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Xamarin.Forms;

namespace CHD
{
    public class BottomSheet : ContentView
    {


        public enum Modes
        {
            Full,
            Default,
            Moving
        }

        public enum PanDirections
        {
            Up,
            Down
        }

        public Action<object, PanUpdatedEventArgs> OnPanUpdatedEvent;
        public Action<Modes> OnModeChanged;
        public Action<PanDirections, double> OnPanDirection;

        public double ySize;

        private RelativeLayout _relativeLayout;
        private double _currentY;
        private double _currentHeight = 200;
        private double _totalY = 0;
        private double _accomulatedY = 0;
        private double _oldTotalY;

        public double MaxDragToChangeMode { get; set; } = 50;
        public double SpeedToChangeMode { get; set; } = 30;
        public double DefaultHeight { get; set; } = 200;
        public Modes Mode { get; set; } = Modes.Default;
        public bool IsResizeEnabled { get; set; } = true;
        public double MarginTop { get; set; } = 0;

        public BottomSheet()
        {
            // Set PanGestureRecognizer.TouchPoints to control the 
            // number of touch points needed to pan
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

        }

        public void SetRelativeLayout(RelativeLayout relativeLayout)
        {
            _relativeLayout = relativeLayout;

            SetY(0, Modes.Default);
        }

        public void ChangeToDefaultMode()
        {
            SetY(0, Modes.Default);
            OnModeChanged?.Invoke(Modes.Default);
        }

        public void ChangeToFullMode()
        {
            SetY(0, Modes.Full);
            OnModeChanged?.Invoke(Modes.Full);
        }

        private void SetY(double y, Modes mode)
        {


            if (!IsResizeEnabled)
            {
                return;
            }

            Mode = mode;

            OnModeChanged?.Invoke(mode);
            var fwd = true;
            _relativeLayout.Animate("bounce",
                (delta) => {

                    switch (mode)
                    {
                        case Modes.Default:
                            _currentY = _relativeLayout.Height - DefaultHeight;
                            break;
                        case Modes.Full:
                            _currentY = 0 + MarginTop;
                            break;
                        case Modes.Moving:
                            _currentY = _relativeLayout.Height - (DefaultHeight + Math.Abs(y));
                            break;
                    }
                    _currentHeight = _relativeLayout.Height - _currentY;

                    //Console.WriteLine("currenty " + _currentY+ " y = "+y + "He " + _currentHeight);

                    var c = BoundsConstraint.FromExpression((Expression<Func<Rectangle>>)(() => new Rectangle(0, _currentY, _relativeLayout.Width, _currentHeight)), new View[0]);
                    RelativeLayout.SetBoundsConstraint(this, c);
                    _relativeLayout.ForceLayout();
                }, 16, 800, Easing.SinInOut, (f, b) => {
                    // reset direction
                    fwd = !fwd;
                }, () => {
                    // keep bouncing
                    return false;
                });
        }


        private double GetSpeed(double totalY, double currentY, double scrollableSize)
        {
            return Math.Max(Math.Min(0, currentY - scrollableSize + totalY), -Math.Abs(scrollableSize * -1));
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {


            switch (e.StatusType)
            {

                case GestureStatus.Running:
                    double yWithSpeed = GetSpeed(e.TotalY, _currentY, _relativeLayout.Height - DefaultHeight);


                    if (_oldTotalY > e.TotalY)
                    {
                        OnPanDirection?.Invoke(PanDirections.Up, e.TotalY);
                    }
                    else
                    {
                        OnPanDirection?.Invoke(PanDirections.Down, e.TotalY);
                    }
                    _oldTotalY = e.TotalY;

                    SetY(yWithSpeed, Modes.Moving);
                    _totalY = e.TotalY;
                    _accomulatedY += e.TotalY;

                    //Debug.WriteLine("transaliton x =  ySize "+ ySize + "totaly"+ e.TotalY+ "yWithSpeed "+ yWithSpeed);
                    break;

                case GestureStatus.Completed:

                    if (_totalY < (SpeedToChangeMode * -1) || _accomulatedY < MaxDragToChangeMode)
                    {
                        SetY(0, Modes.Full);
                    }
                    else if (_totalY > SpeedToChangeMode || _accomulatedY > MaxDragToChangeMode)
                    {
                        _currentY = _relativeLayout.Height - DefaultHeight;
                        SetY(0, Modes.Default);
                    }

                    _totalY = 0;
                    _accomulatedY = 0;
                    _oldTotalY = 0;

                    //Debug.WriteLine(" complete transaliton x = " + Content.TranslationX + " transaliton y = " + Content.TranslationY + " ySize " + ySize);
                    break;
            }

            OnPanUpdatedEvent?.Invoke(sender, e);
        }
    }
}
