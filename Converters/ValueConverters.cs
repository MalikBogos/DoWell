using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DoWell.ViewModels;

namespace DoWell.Converters
{
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? FontWeights.Bold : FontWeights.Normal;
            }
            catch
            {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FontWeights.Bold.Equals(value);
        }
    }

    public class BoolToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? FontStyles.Italic : FontStyles.Normal;
            }
            catch
            {
                return FontStyles.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FontStyles.Italic.Equals(value);
        }
    }

    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? TextDecorations.Underline : null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && ((TextDecorationCollection)value).Count > 0;
        }
    }

    public class SimpleRowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rowIndex)
            {
                return (rowIndex + 1).ToString();
            }
            return "1";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is System.Collections.ObjectModel.ObservableCollection<CellViewModel> row && row.Count > 0)
                {
                    return (row[0].Row + 1).ToString();
                }
                return "1";
            }
            catch
            {
                return "1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}