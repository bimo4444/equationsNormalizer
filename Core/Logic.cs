using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Extensions;
using System.Globalization;

namespace Core
{
    //all next to first letter char is a variable
    public class Logic
    {
        class Unit
        {
            public string Letters { get; set; }
            public decimal Digits { get; set; }
        }
        System.Globalization.NumberFormatInfo numberFormatInfo;
        public Logic()
        {
            System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.InstalledUICulture;
            numberFormatInfo = (System.Globalization.NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormatInfo.NumberDecimalSeparator = ".";
        }
        public string Go(string input)
        {
            input = input.Replace(" ", "");
            Check(input);
            string[] sides = input.Split('=');                                  //split
            var leftSide = Process(sides[0]);                                   //logic
            var rightSide = Process(sides[1]);
            foreach (var v in rightSide)
                v.Digits *= -1;                                                 //invert
            leftSide = leftSide.Union(rightSide).ToList();                      //union
            leftSide = GroupWithSum(leftSide);                                  //group
            leftSide = leftSide
                .Where(w => w.Digits != 0)
                .ToList();                                                      //delete zeros
            return MakeString(leftSide);
        }
        private void Check(string input)
        {
            input = input.Replace("'", "?").Replace(",", ".");                  //change ',' with '.' for parsing
            if (input.Length == 0)
                throw new Exception("empty string");
            if (input.IndexOfAny(new char[] { '\\', '/', ':', '*', '<', '>', '|', '#', '{', '}', '%', '~', '&', '"', '?', '!' }) != -1)
                throw new Exception("not supported char");
            if(!input.Contains("="))
                throw new Exception("input doesn't contain '='");
            if (input.StartsWith("=") || input.EndsWith("="))
                throw new Exception("wrong format");
            Regex regex = new Regex("[=]", RegexOptions.IgnoreCase);            
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 1)
                throw new Exception("'=' can't be more then once");             //if multiple '='
        }
        private List<Unit> Process(string s)
        {
            if (s.Contains("(") || s.Contains(")"))
            {
                var indexes1 = s.AllIndexesOf("(").ToList();
                var indexes2 = s.AllIndexesOf(")").ToList();
                indexes2.Reverse();
                if(!indexes1.Any() || !indexes2.Any())
                {
                    throw new Exception("wrong format");
                }
                int start = indexes1.Select(c => c).Last();
                int end = indexes2.Select(c => c).Last();
                if (indexes1.Count() != indexes2.Count() || start > end)
                {
                    throw new Exception("wrong format");
                }
                string left = "", center = "", right = "";
                bool minus = false;
                if (start > 0)                                                      //to grab +/- char
                {
                    char j = s.ToCharArray()[start - 1];
                    if (j == '+' || j == '-')
                    {
                        start--;
                    }
                }
                center = s.Substring(start, end - start + 1);                       //inside brackets
                if (center.StartsWith("-"))
                {
                    minus = true;
                    center = center.Substring(1);
                }
                center = center.Replace("(", "").Replace(")", "");                  //del brackets
                List<Unit> temp = Split(new List<Unit>(), center);                  //split first piece
                temp = GroupWithSum(temp);                                  
                if (minus)
                {
                    foreach (var v in temp)
                        v.Digits *= -1;                                             //invert
                }
                if (start != 0)
                {
                    left += s.Substring(0, start);                                  //left to brackets
                }
                end++;                                                              //to skip closing bracket
                if (end < s.Length)                                                 
                {
                    right += s.Substring(end, s.Length - end);                      //right to brackets
                }
                if (left != "" || right != "")                                      //split second piece
                {
                    foreach (var v in temp)
                        left += UnitToString(v);
                    left += right;
                    return Process(left);                                           //recursive
                }
                else
                {
                    return temp;                                                    //if nothing lasts to parse
                }
            }
            else
            {
                var result = Split(new List<Unit>(), s);                            //if doesn't contains brackets
                return GroupWithSum(result);
            }
        }
        //skips several +/-
        private List<Unit> Split(List<Unit> list, string s)
        {
            bool positive = true;
            while (s.StartsWith("+") || s.StartsWith("-"))
            {
                if(s.StartsWith("+"))
                {
                    s = s.Substring(1);
                }
                if(s.StartsWith("-"))
                {
                    positive = false;
                    s = s.Substring(1);
                }
            }
            if (s.Contains("+") || s.Contains("-"))
            {
                int index = s.IndexOfAny(new char[]{'+','-'});
                string left = s.Substring(0, index);
                string right = s.Substring(index);
                list.Add(ParseToUnit(left, positive));
                Split(list, right);                                                 //recursive
                return list;
            }
            else
            {
                list.Add(ParseToUnit(s, positive));
                return list;
            }
        }
        private Unit ParseToUnit(string s, bool positive = true)
        {
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (i < chars.Length - 1)
                {
                    if (Char.IsDigit(chars[i]))
                    {
                        if (Char.IsDigit(chars[i + 1]) || chars[i + 1].Equals('.'))     //if next is digit or point
                        {
                            if(chars[i + 1].Equals('.') && (i + 2 == chars.Length || !Char.IsDigit(chars[i + 2])))    
                            {
                                throw new Exception("wrong format");                    //if last char point
                            }
                            continue;                                                   //skip while next is digits
                        }
                        if (Char.IsLetter(chars[i + 1]))                                //next char is letter
                        {
                            string left = s.Substring(0, i + 1);
                            string right = s.Substring(i + 1);
                            decimal d = Decimal.Parse(left, numberFormatInfo);          //parsing digits
                            return new Unit { Digits = positive ? d : -d, Letters = right };
                        }
                    }
                    else if (Char.IsLetter(chars[i]))                                   //if letter char is the first
                    {
                        break;
                    }
                }
                else                                                                    //if case of a single char
                {
                    if (chars[i].Equals('.'))                                           //if last char point
                    {
                        throw new Exception("wrong format");
                    }
                    decimal d = Char.IsDigit(chars[i]) ? Decimal.Parse(s, numberFormatInfo) : 1;
                    return new Unit { Digits = positive ? d : -d, Letters = !Char.IsDigit(chars[i]) ? s : "" };
                }
            }
            return new Unit { Digits = positive ? 1 : -1, Letters = s };
        }
        private List<Unit> GroupWithSum(List<Unit> list)                                //group by variables
        {
            return list
                .GroupBy(g => g.Letters)
                .Select(c => new Unit
                {
                    Letters = c.Key,
                    Digits = c.Sum(ss => ss.Digits)
                })
                .ToList();
        }
        private string UnitToString(Unit unit)                                          //for result
        {
            if (unit.Digits == 0)
                return "";                                                              //empty
            if (unit.Digits == 1 && unit.Letters != "")
                return "+" + unit.Letters;
            if (unit.Digits == -1 && unit.Letters != "")                                //-1gg to -gg
                return "-" + unit.Letters;
            string result = unit.Digits.ToString("0.##############################", numberFormatInfo) + unit.Letters;
            return unit.Digits > 0 ? "+" + result : result;                             //must contain plus
        }
        private string MakeString(List<Unit> list)
        {
            string result = "";
            foreach (var v in list)
                result += UnitToString(v);
            if (!list.Any())                                                            //sum == 0
                return "0 = 0";
            if (result.StartsWith("+"))
                result = result.Substring(1);
            result = result
                .Replace("+", " + ")
                .Replace("-", " - ")
                .Trim();
            return result + " = 0";
        }
    }
}
