// A set of C# classes for spelling Russian numerics 
// Copyright (c) 2002 RSDN Group

using System.Text;
using System.Collections.Specialized;

namespace Logic.Services
{
    public class RusNumber
    {
        private static readonly string[] hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        private static readonly string[] tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        public static string Str(int val, bool male, string one, string two, string five)
        {
            string[] frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

            int num = val % 1000;
            if (0 == num) return "";
            if (num < 0) throw new ArgumentOutOfRangeException(nameof(val), "Параметр не может быть отрицательным");
            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            StringBuilder r = new(hunds[num / 100]);

            if (num % 100 < 20)
            {
                r.Append(frac20[num % 100]);
            }
            else
            {
                r.Append(tens[num % 100 / 10]);
                r.Append(frac20[num % 10]);
            }

            r.Append(Case(num, one, two, five));

            if (r.Length != 0) r.Append(' ');
            return r.ToString();
        }

        public static string Case(int val, string one, string two, string five)
        {
            int t = val % 100 > 20 ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return one;
                case 2: case 3: case 4: return two;
                default: return five;
            }
        }
    };

    struct CurrencyInfo
    {
        public bool male;
        public string seniorOne, seniorTwo, seniorFive;
        public string juniorOne, juniorTwo, juniorFive;
    };

    public class RusCurrency
    {
        private static readonly HybridDictionary currencies = new();

        static RusCurrency()
        {
            Register("RUR", true, "рубль", "рубля", "рублей", "копейка", "копейки", "копеек");
            Register("EUR", true, "евро", "евро", "евро", "евроцент", "евроцента", "евроцентов");
            Register("USD", true, "доллар", "доллара", "долларов", "цент", "цента", "центов");
        }

        public static void Register(string currency, bool male,
            string seniorOne, string seniorTwo, string seniorFive,
            string juniorOne, string juniorTwo, string juniorFive)
        {
            CurrencyInfo info;
            info.male = male;
            info.seniorOne = seniorOne; info.seniorTwo = seniorTwo; info.seniorFive = seniorFive;
            info.juniorOne = juniorOne; info.juniorTwo = juniorTwo; info.juniorFive = juniorFive;
            currencies.Add(currency, info);
        }

        public static string Str(double val)
        {
            return Str(val, "RUR");
        }

        public static string Str(double val, string currency)
        {
            CurrencyInfo info = currencies[currency] is CurrencyInfo dictValue ? dictValue : throw new ArgumentOutOfRangeException(nameof(currency), $"Валюта {currency} не зарегистрирована");
            return Str(val, info.male,
                info.seniorOne, info.seniorTwo, info.seniorFive,
                info.juniorOne, info.juniorTwo, info.juniorFive);
        }

        public static string Str(double val, bool male,
            string seniorOne, string seniorTwo, string seniorFive,
            string juniorOne, string juniorTwo, string juniorFive)
        {
            bool minus = false;
            if (val < 0) { val = -val; minus = true; }

            int n = (int)val;
            int remainder = (int)((val - n + 0.005) * 100);

            StringBuilder r = new();

            if (0 == n) r.Append("0 ");
            if (n % 1000 != 0)
                r.Append(RusNumber.Str(n, male, seniorOne, seniorTwo, seniorFive));
            else
                r.Append(seniorFive);

            n /= 1000;

            r.Insert(0, RusNumber.Str(n, false, "тысяча", "тысячи", "тысяч"));
            n /= 1000;

            r.Insert(0, RusNumber.Str(n, true, "миллион", "миллиона", "миллионов"));
            n /= 1000;

            r.Insert(0, RusNumber.Str(n, true, "миллиард", "миллиарда", "миллиардов"));
            n /= 1000;

            r.Insert(0, RusNumber.Str(n, true, "триллион", "триллиона", "триллионов"));
            n /= 1000;

            r.Insert(0, RusNumber.Str(n, true, "триллиард", "триллиарда", "триллиардов"));
            if (minus) r.Insert(0, "минус ");

            r.Append(remainder.ToString("00 "));
            r.Append(RusNumber.Case(remainder, juniorOne, juniorTwo, juniorFive));

            //Делаем первую букву заглавной
            r[0] = char.ToUpper(r[0]);

            return r.ToString();
        }
    };
};