﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPS.SSO.Freshdesk.Login.Models
{
    public abstract class TimeProvider
    {
        private static TimeProvider current =
            DefaultTimeProvider.Instance;

        public static TimeProvider Current
        {
            get { return TimeProvider.current; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                TimeProvider.current = value;
            }
        }

        public abstract DateTime UtcNow { get; }

        public static void ResetToDefault()
        {
            TimeProvider.current = DefaultTimeProvider.Instance;
        }
    }
}