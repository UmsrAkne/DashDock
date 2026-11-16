using System.Windows;
using System.Windows.Controls;
using DashDock.ViewModels;

namespace DashDock.Views.Controls
{
    public class VerticalFillPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var visibleChildren = InternalChildren.OfType<FrameworkElement>()
                .Where(c => c.Visibility != Visibility.Collapsed)
                .ToList();

            if (visibleChildren.Count == 0)
            {
                return new Size(0, 0);
            }

            var fillTarget = visibleChildren.LastOrDefault(c =>
                c.DataContext is NoteAreaViewModel or ImageGalleryAreaViewModel);

            var totalHeight = 0.0;
            var maxWidth = 0.0;

            // 1. Fill対象以外の要素を先に計測して高さを確定させる
            foreach (var child in visibleChildren)
            {
                if (child == fillTarget)
                {
                    continue;
                }

                child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
                totalHeight += child.DesiredSize.Height;
                maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            }

            // 2. Fill対象の要素には「残りの高さ」を上限として渡す
            if (fillTarget != null)
            {
                var remainingHeight = double.IsPositiveInfinity(availableSize.Height)
                    ? double.PositiveInfinity
                    : Math.Max(0.0, availableSize.Height - totalHeight);

                fillTarget.Measure(new Size(availableSize.Width, remainingHeight));
                totalHeight += fillTarget.DesiredSize.Height;
                maxWidth = Math.Max(maxWidth, fillTarget.DesiredSize.Width);
            }

            var resultWidth = double.IsPositiveInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
            var resultHeight = double.IsPositiveInfinity(availableSize.Height) ? totalHeight : availableSize.Height;

            return new Size(resultWidth, resultHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var visibleChildren = InternalChildren.OfType<FrameworkElement>()
                .Where(c => c.Visibility != Visibility.Collapsed)
                .ToList();

            if (visibleChildren.Count == 0)
            {
                return finalSize;
            }

            var fillTarget = visibleChildren.LastOrDefault(c =>
                c.DataContext is NoteAreaViewModel or ImageGalleryAreaViewModel);

            var currentY = 0.0;

            foreach (var child in visibleChildren)
            {
                double childHeight;

                if (child == fillTarget)
                {
                    // 残りの利用可能領域すべてを割り当てる（Math.Max で DesiredSize に縛られないようにする）
                    var afterHeight = NonFillHeightAfter(visibleChildren, child);
                    childHeight = Math.Max(0.0, finalSize.Height - currentY - afterHeight);
                }
                else
                {
                    childHeight = child.DesiredSize.Height;
                }

                child.Arrange(new Rect(0, currentY, finalSize.Width, childHeight));
                currentY += childHeight;
            }

            return finalSize;
        }

        private static double NonFillHeightAfter(List<FrameworkElement> children, FrameworkElement current)
        {
            var index = children.IndexOf(current);
            if (index < 0)
            {
                return 0.0;
            }

            return children.Skip(index + 1).Sum(c => c.DesiredSize.Height);
        }
    }
}