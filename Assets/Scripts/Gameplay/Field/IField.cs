using UnityEngine;

namespace Gameplay
{
    public interface IField
    {
        bool IsPositionInsideField(Vector2 Input);
    }
}
