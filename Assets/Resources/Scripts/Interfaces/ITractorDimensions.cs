using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts
{
  public interface ITractorDimensions
    {
        Transform GetRecipricolTransform();
        Vector2 GetBeamDirection();
        float GetWidth();
        float GetHeight();
    }
}
