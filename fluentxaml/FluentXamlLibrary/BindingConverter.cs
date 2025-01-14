﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FluentXamlLibrary
{
    public sealed class BindingConverter : IValueConverter
    {
        readonly Func<object, Type, object> converter;
        readonly Func<object, Type, object> backConverter;

        public BindingConverter(
            Func<object, Type, object> converter,
            Func<object, Type, object> backConverter)
        {
            this.converter = converter;
            this.backConverter = backConverter;
        }

        public BindingConverter(Func<object, Type, object> converter) : this(converter, null) { }

        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return this.converter(values, targetType);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (this.backConverter == null)
                throw new NotSupportedException();
            else
                return this.backConverter(value, targetTypes);
        }
    }
}
