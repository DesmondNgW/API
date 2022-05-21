using System;
using X.Util.Core;
using X.Util.Entities.Enum;

namespace X.Util.Other
{
    /// <summary>
    /// FormulaDeal
    /// </summary>
    public class FormulaDeal
    {
        private double CalculateExpress(string strExpression)
        {
            while (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                string strTempB;
                string strTwo;
                string strOne;
                string strTemp;
                double ReplaceValue;
                if (strExpression.IndexOf("*") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("*") + 1, strExpression.Length - strExpression.IndexOf("*") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("*"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);
                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = GetExpType(strOne).Convert2Double(default) * GetExpType(strTwo).Convert2Double(default);
                    strExpression = strExpression.Replace(strOne + "*" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("/") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("/") + 1, strExpression.Length - strExpression.IndexOf("/") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("/"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);
                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = GetExpType(strOne).Convert2Double(default) / GetExpType(strTwo).Convert2Double(default);
                    strExpression = strExpression.Replace(strOne + "/" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("+") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("+") + 1, strExpression.Length - strExpression.IndexOf("+") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("+"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);
                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = GetExpType(strOne).Convert2Double(default) + GetExpType(strTwo).Convert2Double(default);
                    strExpression = strExpression.Replace(strOne + "+" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("-") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("-") + 1, strExpression.Length - strExpression.IndexOf("-") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("-"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);
                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = GetExpType(strOne).Convert2Double(default) - GetExpType(strTwo).Convert2Double(default);
                    strExpression = strExpression.Replace(strOne + "-" + strTwo, ReplaceValue.ToString());
                }
            }
            return strExpression.Convert2Double(default);
        }

        private double CalculateExExpress(string strExpression, EnumFormula ExpressType)
        {
            double retValue = 0;
            switch (ExpressType)
            {
                case EnumFormula.Sin:
                    retValue = Math.Sin(strExpression.Convert2Double(default));
                    break;
                case EnumFormula.Cos:
                    retValue = Math.Cos(strExpression.Convert2Double(default));
                    break;
                case EnumFormula.Tan:
                    retValue = Math.Tan(strExpression.Convert2Double(default));
                    break;
                case EnumFormula.ATan:
                    retValue = Math.Atan(strExpression.Convert2Double(default));
                    break;
                case EnumFormula.Sqrt:
                    retValue = Math.Sqrt(strExpression.Convert2Double(default));
                    break;
                case EnumFormula.Pow:
                    retValue = Math.Pow(strExpression.Convert2Double(default), 2);
                    break;
            }
            if (retValue == 0) return strExpression.Convert2Double(default);
            return retValue;
        }


        private int GetNextPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.IndexOf("+");
            ExpPos[1] = strExpression.IndexOf("-");
            ExpPos[2] = strExpression.IndexOf("*");
            ExpPos[3] = strExpression.IndexOf("/");
            int tmpMin = strExpression.Length;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMin > ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMin = ExpPos[count - 1];
                }
            }
            return tmpMin;
        }


        private int GetPrivorPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.LastIndexOf("+");
            ExpPos[1] = strExpression.LastIndexOf("-");
            ExpPos[2] = strExpression.LastIndexOf("*");
            ExpPos[3] = strExpression.LastIndexOf("/");
            int tmpMax = -1;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMax < ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMax = ExpPos[count - 1];
                }
            }
            return tmpMax;

        }
        public string SpiltExpression(string strExpression)
        {
            while (strExpression.IndexOf("(") != -1)
            {
                string strTemp = strExpression.Substring(strExpression.LastIndexOf("(") + 1, strExpression.Length - strExpression.LastIndexOf("(") - 1);
                string strExp = strTemp.Substring(0, strTemp.IndexOf(")"));
                strExpression = strExpression.Replace("(" + strExp + ")", CalculateExpress(strExp).ToString());
            }
            if (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                strExpression = CalculateExpress(strExpression).ToString();
            }
            return strExpression;
        }

        private string GetExpType(string strExpression)
        {
            strExpression = strExpression.ToUpper();
            if (strExpression.IndexOf("SIN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Sin).ToString();
            }
            if (strExpression.IndexOf("COS") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("S") + 1, strExpression.Length - 1 - strExpression.IndexOf("S")), EnumFormula.Cos).ToString();
            }
            if (strExpression.IndexOf("TAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Tan).ToString();
            }
            if (strExpression.IndexOf("ATAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.ATan).ToString();
            }
            if (strExpression.IndexOf("SQRT") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("T") + 1, strExpression.Length - 1 - strExpression.IndexOf("T")), EnumFormula.Sqrt).ToString();
            }
            if (strExpression.IndexOf("POW") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("W") + 1, strExpression.Length - 1 - strExpression.IndexOf("W")), EnumFormula.Pow).ToString();
            }
            return strExpression;
        }
    }
}
