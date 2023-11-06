using GameProject5.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject5.StateManagement
{
    /// <summary>
    /// Defines an object that can create a screen when given its type.
    /// </summary>
    public interface IScreenFactory
    {
        GameScreen CreateScreen(Type screenType);
    }
}
