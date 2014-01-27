﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using PMKS_Silverlight_App;
using PlanarMechanismSimulator;

namespace PMKS_Silverlight_App
{
    public class TimeToLinkParameterConverter : TimeToJointParameterConverter
    {
        protected int LinkIndex;
        public TimeToLinkParameterConverter(link l, joint j, StateVariableType linkState, Simulator pmks)
            : base(j, linkState, pmks)
        {
            LinkIndex = pmks.AllLinks.IndexOf(l);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result2 = (double[])base.Convert(value, targetType, parameter, culture);
            var currentTime = (double)value;

            double angleStateVar;
            switch (stateVariableTypeIndex)
            {
                case 0: angleStateVar = pmks.FindLinkPositionAtTime(currentTime, LinkIndex);
                    break;
                case 1: angleStateVar = pmks.FindLinkVelocityAtTime(currentTime, LinkIndex);
                    break;
                default: angleStateVar = pmks.FindLinkAccelerationAtTime(currentTime, LinkIndex);
                    break;
            }
            return new[] { result2[0], result2[1], angleStateVar };
        }
    }
}

