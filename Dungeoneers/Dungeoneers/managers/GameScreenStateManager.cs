﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeoneers.managers
{
    static class GameScreenStateManager
    {
        public static ScreenStates CurrentState { get; set; }
    }

    public enum ScreenStates
    {
        Play = 1,
        MessageHistory = 2,
        Portrait_Msg = 3,
        Inspect = 4
    }
}
