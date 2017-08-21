﻿using System;

namespace Doing.BDDExtensions
{
    public static class Catch
    {
        public static Exception Exception(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
    }
}