using Android.Content;
using Android.Graphics;
using Android.Views;

namespace OdborkyApp.Components
{
    internal class CircleDrawView : View
    {
        private readonly Paint _paint;
        private readonly int _desiredWidth;
        private readonly int _desiredHeight;

        public CircleDrawView (Context context, int width = 250, int height = 250) : base(context)
        {     
            _paint = new Paint();
            _desiredHeight = height;
            _desiredWidth = width;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            SetMeasuredDimension(_desiredWidth, _desiredHeight);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            _paint.Color = new Color(33, 150, 243);
            canvas.DrawCircle(125, 125, 100, _paint);

            _paint.Color = Color.White;
            canvas.DrawRect(115, 75, 135, 175, _paint);
            canvas.DrawRect(75, 115, 175, 135, _paint);
        }
    }
}