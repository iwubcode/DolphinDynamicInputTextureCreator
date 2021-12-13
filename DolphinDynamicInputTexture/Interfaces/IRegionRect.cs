using DolphinDynamicInputTexture.Data;
using System;

namespace DolphinDynamicInputTexture.Interfaces
{
    public interface IRectRegion : ICloneable, IEquatable<IRectRegion>
    {
        DynamicInputTexture OwnedTexture { get; internal set; }
        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner.
        /// </summary>
        double X { get; set; }
        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner.
        /// </summary>
        double Y { get; set; }
        /// <summary>
        /// Gets or sets the Height.
        /// </summary>
        double Height { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        double Width { get; set; }
        /// <summary>
        /// Gets the x-coordinate, which is the sum of x and Width.
        /// </summary>
        double RightX { get; }
        /// <summary>
        /// Gets the y-coordinate, which is the sum of Y and Height.
        /// </summary>
        double BottomY { get; }


        /// <summary>
        /// Determines if this region intersects with another.
        /// </summary>
        /// <param name="other">another region</param>
        /// <returns>true if there is any intersection; otherwise, false.</returns>
        bool IntersectsWith(IRectRegion other);
        /// <summary>
        /// checks if this region is on a texture.
        /// </summary>
        /// <returns>true if this region is on a texture</returns>
        bool OnTexture();
        /// <summary>
        /// Determines if the region is completely contained in this region.
        /// </summary>
        /// <param name="other">another region to test</param>
        /// <returns>true if the another region is contained within this region</returns>
        bool Contains(IRectRegion other);
        /// <summary>
        /// Determines if the specified point is contained within this region.
        /// </summary>
        /// <param name="x">The x-coordinate of the point to test.</param>
        /// <param name="y">The y-coordinate of the point to test.</param>
        /// <returns>true if the point defined by x and y is contained within this region</returns>
        bool Contains(double x, double y);
        /// <summary>
        /// Indicates whether the current object is equal to x y width height.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        bool Equals(double x, double y, double width, double height);
    }
}