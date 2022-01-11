using Microsoft.AspNetCore.Components;

namespace ShowcaseClient.Components
{
    public partial class ScoreProgressBar
    {
        [Parameter] public int Score { get; set; }
        [Parameter] public int WidthInPixels { get; set; }

        private string _colorClass = "bg-success";
        private string _progressBarWidthStyle = "";
        private string _componentWidthStyle = "";

        protected override void OnParametersSet()
        {
            if (Score is > 333 and < 666)
            {
                _colorClass = "bg-warning";
            }

            if (Score >= 666)
            {
                _colorClass = "bg-danger";
            }

            _progressBarWidthStyle = $"width: {(Score / 10)}%";
            _componentWidthStyle = $"width: {WidthInPixels}px;";
        }
    }
}
