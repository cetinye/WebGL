using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public sealed class Fraction
    {
        public int Nominator;
        public int Denominator;

        public Fraction(int nominator, int denominator)
        {
            Nominator = nominator;
            Denominator = denominator;
        }

        public Fraction Simplified()
        {
            var gcd = GCD(Nominator, Denominator);
            if (gcd == Denominator)
                gcd = 1;
            
            return new Fraction(Nominator / gcd, Denominator / gcd);
        }

        public float Float()
        {
            return (float)Nominator / Denominator;
        }

        public static Fraction operator +(Fraction a, Fraction b)
        {
            var denom = LCM(a.Denominator, b.Denominator);
            var nom1 = a.Nominator * (denom / a.Denominator);
            var nom2 = b.Nominator * (denom / b.Denominator);
            return new Fraction(nom1 + nom2, denom);
        }
        
        public static Fraction operator -(Fraction a, Fraction b)
        {
            var denom = LCM(a.Denominator, b.Denominator);
            var nom1 = a.Nominator * (denom / a.Denominator);
            var nom2 = b.Nominator * (denom / b.Denominator);
            return new Fraction(nom1 - nom2, denom);
        }
        
        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.Nominator * b.Nominator, a.Denominator * b.Denominator);
        }
        
        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new Fraction(a.Nominator * b.Denominator, b.Nominator * a.Denominator);
        }

        public static int GCD(int a, int b)
        {
            int Remainder;
 
            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }
 
            return a;
        }
        
        public static int LCM(int a, int b)
        {
            return (a / GCD(a, b)) * b;
        }
        
        public static List<int> GetDividers(int number)
        {
            var dividers = new List<int>();
            var limit = Mathf.Sqrt(number);
            for (int i = 1; i <= limit; i++)
            {
                if (number % i == 0)
                {
                    dividers.Add(i);
                    var other = number / i;
                    if(other != i)
                        dividers.Add(other);
                }
            }
            return dividers;
        }
    }
}