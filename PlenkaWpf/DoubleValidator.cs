using System;
using System.Globalization;
using System.Windows.Controls;


namespace PlenkaWpf
{
    /// <summary>
    ///     Валидатор для double значений
    /// </summary>
    internal class DoubleValidator : ValidationRule
    {
        /// <summary>
        ///     Минимальное значение
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        ///     Максимальное значение
        /// </summary>
        public double? Max { get; set; }

        public bool IncludingMinValue { get; set; } = false;
        public bool IncludingMaxValue { get; set; } = true;

        private string LeftBound
        {
            get
            {
                var boundBracket = "";
                var boundValue = "";

                if (IncludingMinValue)
                {
                    boundBracket = "[";
                }
                else
                {
                    boundBracket = "(";
                }

                if (Min == null)
                {
                    boundBracket = "(";
                    boundValue = "-∞";
                }
                else
                {
                    boundValue = Min.ToString();
                }

                return $"{boundBracket}{boundValue}";
            }
        }

        private string RightBound
        {
            get
            {
                var boundBracket = "";
                var boundValue = "";

                if (IncludingMaxValue)
                {
                    boundBracket = "]";
                }
                else
                {
                    boundBracket = ")";
                }

                if (Max == null)
                {
                    boundBracket = ")";
                    boundValue = "∞";
                }
                else
                {
                    boundValue = Max.ToString();
                }

                return $"{boundValue}{boundBracket}";
            }
        }

        private string Range
        {
            get
            {
                return $"{LeftBound};{RightBound}";
            }
        }

        private string EnterValueInRange
        {
            get
            {
                return $"Введите значение в диапазоне: {Range}";
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double val = 0;

            if (value == null)
            {
                return new ValidationResult(false, "Значение не может быть пустым");
            }

            try
            {
                if (((string) value).Length > 0)
                {
                    val = double.Parse((string) value, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Введено недопустимое значение {e.Message}");
            }

            if (Min != null || Max != null)
            {
                if (Min == null && Max != null)
                {
                    if (IncludingMaxValue)
                    {
                        if (val > Max)
                        {
                            return new ValidationResult(false, EnterValueInRange);
                        }
                    }
                    else
                    {
                        if (val >= Max)
                        {
                            return new ValidationResult(false, EnterValueInRange);
                        }
                    }
                }

                if (Min != null && Max == null)
                {
                    if (IncludingMinValue)
                    {
                        if (val < Min)
                        {
                            return new ValidationResult(false, $"Введите значение большее чем {Min}.");
                        }
                    }
                    else
                    {
                        if (val <= Min)
                        {
                            return new ValidationResult(false, $"Введите значение большее чем {Min}.");
                        }
                    }
                }

                if (IncludingMinValue && IncludingMaxValue)
                {
                    if (val < Min || val > Max)
                    {
                        return new ValidationResult(false,
                                                    EnterValueInRange);
                    }
                }

                if (!IncludingMinValue && !IncludingMaxValue)
                {
                    if (val <= Min || val >= Max)
                    {
                        return new ValidationResult(false,
                                                    EnterValueInRange);
                    }
                }

                if (!IncludingMinValue)
                {
                    if (val <= Min || val > Max)
                    {
                        return new ValidationResult(false,
                                                    EnterValueInRange);
                    }
                }

                if (!IncludingMaxValue)
                {
                    if (val < Min || val >= Max)
                    {
                        return new ValidationResult(false,
                                                    EnterValueInRange);
                    }
                }
            }

            return ValidationResult.ValidResult;
        }
    }
}
