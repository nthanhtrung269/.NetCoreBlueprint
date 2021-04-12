using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.SharedModels
{
    public class ImageResizeCriteriaValidator
    {
        private readonly AssetSettings _setting;
        public ImageResizeCriteriaValidator(AssetSettings setting)
        {
            _setting = setting;
        }

        private IEnumerable<(int width, int height)> SupportedRatios => _setting.RatioRange.Select(ImageResizeCriteria.RatioRangeToValue);

        public bool IsTypeSupported(string type) => _setting.FileTypes.Contains(type);

        public bool SupportedByConfig(ImageResizeCriteria query)
        {
            if (!query.Valid)
                return false;

            return query.Empty
                || (query.HasBothDimension && IsRatioSupported((query.Width, query.Height)))
                || (IsMaxHeightSupported(query.Maxheight ?? 0))
                || IsMaxWidthSupported(query.MaxWidth ?? 0)
                || IsHeightSupported(query.Height ?? 0)
                || IsWidthSupported(query.Width ?? 0);
        }

        public int HeightFloor(int value)
        {
            return _setting.HeightRange.OrderByDescending(height => height).FirstOrDefault(height => height < value);
        }

        public int WidthFloor(int value)
        {
            return _setting.WidthRange.OrderByDescending(width => width).FirstOrDefault(width => width < value);
        }

        private bool IsRatioSupported((int? width, int? height) ratioToValidate)
        {
            var supportedRatio = SupportedRatios;
            bool stillInRange = supportedRatio.Any(ratio => ratio.width == ratioToValidate.width
                                    && ratio.height == ratioToValidate.height);
            return stillInRange;
        }

        private bool IsHeightSupported(int height)
        {
            return _setting.HeightRange.Contains(height);
        }

        private bool IsWidthSupported(int width)
        {
            return _setting.WidthRange.Contains(width);
        }

        private bool IsMaxHeightSupported(int maxHeight)
        {
            return _setting.HeightRange.Any(h => h <= maxHeight);
        }

        private bool IsMaxWidthSupported(int maxWidth)
        {
            return _setting.WidthRange.Any(w => w <= maxWidth);
        }

        public bool ValidCriteria(ImageResizeCriteria criteria)
        {
            //validate request valid
            Guard.AgainstInvalidArgumentWithMessage("Request not supported", criteria.Valid);

            //support by config?
            bool supportedByConfig = SupportedByConfig(criteria);
            Guard.AgainstInvalidArgumentWithMessage("Configuration not supported", supportedByConfig);

            return true;
        }

        public (int? width, int? height) GetCriteria(ImageResizeCriteria criteria)
        {
            Guard.AgainstInvalidOperationWithMessage("Config not supported", SupportedByConfig(criteria));

            Func<int?, bool> hasLength = ImageResizeCriteria.HasLength;

            if (criteria.HasBothDimension)
                return (criteria.Width, criteria.Height);
            if (hasLength(criteria.Width))
                return (criteria.Width, null);
            if (hasLength(criteria.Height))
                return (null, criteria.Height);
            if (hasLength(criteria.MaxWidth))
                return (WidthFloor(criteria.MaxWidth.Value), null);
            if (hasLength(criteria.Maxheight))
                return (null, HeightFloor(criteria.Maxheight.Value));
            return (null, null);
        }
    }
}
