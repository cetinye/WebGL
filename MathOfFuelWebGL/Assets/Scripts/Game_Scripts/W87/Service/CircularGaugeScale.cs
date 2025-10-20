using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Witmina_MathOfFuel
{
    public class CircularGaugeScale : GaugeScale
    {
        [SerializeField] private LineRenderer _lineBig;
        [SerializeField] private LineRenderer _lineSmall;
        [SerializeField] private float _radius = 0.2f;
        [SerializeField] private float angle = 60f;
        [SerializeField] private Color _lineColor = Color.black;

        public override void SetScale(int denominator)
        {
            base.SetScale(denominator);
            
            //Determine the big denominator for long lines from gcd
            var bigDenom = denominator;
            foreach (var bd in BigDenoms)
            {
                if (bd < denominator && denominator % bd == 0)
                {
                    bigDenom = bd;
                    break;
                }
            }
            
            //Generate lines
            for (int i = 0; i <= denominator; i++)
            {
                var big = i % bigDenom == 0;
                
                var line = Instantiate(big ? _lineBig : _lineSmall, transform);
                var alignment = Quaternion.AngleAxis((angle / 2f) - i * (angle / denominator), Vector3.forward)
                                * (_radius * Vector3.up);
                line.transform.localPosition = alignment;
                line.transform.rotation = Quaternion.LookRotation(Vector3.forward, alignment);
                line.startColor = line.endColor = _lineColor;
                Lines.Add(line);
            }
        }
    }
}

