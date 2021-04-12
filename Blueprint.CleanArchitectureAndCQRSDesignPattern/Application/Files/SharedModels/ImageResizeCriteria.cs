using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.SharedModels
{
    public class ImageResizeCriteria
    {
        public const string RatioRangeSeparator = "x";

        public const string widthQuery = @"^width_(\d+)$";
        public const string heightQuery = @"^height_(\d+)$";

        public const string maxWidthQuery = @"^maxwidth_(\d+)$";
        public const string maxHeightQuery = @"^maxheight_(\d+)$";

        public const string bothSideQuery = @"^(\d+)x(\d+)$";

        public ImageResizeCriteria()
        {
        }

        public ImageResizeCriteria(long imageId, string queryString)
        {
            ImageId = imageId;

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                //if any query match, execute call back and stop
                var patternCallbackPairs = new Dictionary<string, Action<Match>>();
                patternCallbackPairs.Add(widthQuery, match => { this.Width = int.Parse(match.Groups[1].Value); });
                patternCallbackPairs.Add(heightQuery, match => { this.Height = int.Parse(match.Groups[1].Value); });
                patternCallbackPairs.Add(maxWidthQuery, match => { this.MaxWidth = int.Parse(match.Groups[1].Value); });
                patternCallbackPairs.Add(maxHeightQuery, match => { this.Maxheight = int.Parse(match.Groups[1].Value); });
                patternCallbackPairs.Add(bothSideQuery, match =>
                {
                    this.Width = int.Parse(match.Groups[1].Value);
                    this.Height = int.Parse(match.Groups[2].Value);
                });

                foreach (var key in patternCallbackPairs.Keys)
                {
                    bool patternMatched = ValidateQueryString(queryString, key, patternCallbackPairs[key]);
                    if (patternMatched)
                        break;
                }
            }
        }

        public int? Width { set; get; }
        public int? Height { set; get; }
        public int? MaxWidth { set; get; }
        public int? Maxheight { set; get; }
        public long ImageId { set; get; }
        public bool HasBothDimension => HasLength(Height) && HasLength(Width);
        public bool HasBothMaxDimension => HasLength(Maxheight) && HasLength(MaxWidth);

        //valid param with given business? width and height or width or height or maxwidth or maxheight
        public bool Valid =>
            ((HasLength(MaxWidth) || HasLength(Maxheight)) && !HasBothMaxDimension && (!HasLength(Height) && !HasLength(Width)))
            || ((HasLength(Height) || HasLength(Width)) && (!HasLength(Maxheight) && !HasLength(MaxWidth)))
            || Empty;

        //this request has param?
        public bool Empty => (!Height.HasValue && !Width.HasValue && !Maxheight.HasValue && !MaxWidth.HasValue);
        public static (int width, int height) RatioRangeToValue(string ratioRange)
        {
            var ratioRangeSplitted = ratioRange.Split(RatioRangeSeparator, StringSplitOptions.RemoveEmptyEntries);
            int width = int.Parse(ratioRangeSplitted[0]);
            int height = int.Parse(ratioRangeSplitted[1]);
            return (width, height);
        }

        public static bool HasLength(int? @long)
        {
            return @long.HasValue && @long.Value > 0;
        }

        private bool ValidateQueryString(string queryString, string pattern, Action<Match> callback)
        {
            var match = Regex.Match(queryString, pattern);
            if (match.Success)
            {
                callback(match);

                return true;
            }

            return false;
        }
    }
}
