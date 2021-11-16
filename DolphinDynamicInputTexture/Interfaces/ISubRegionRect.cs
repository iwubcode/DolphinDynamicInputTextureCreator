using DolphinDynamicInputTexture.Data;
using System;

namespace DolphinDynamicInputTexture.Interfaces
{
    public interface ISubRectRegion : IRectRegion
    {
        IRectRegion MainRegion { get; internal set; }
    }
}