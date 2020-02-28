using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Design
{
    public class DesignEmulatedDevice : Data.EmulatedDevice
    {
        public static DesignEmulatedDevice Instance => new DesignEmulatedDevice();

        public DesignEmulatedDevice()
        {
            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/A",
                    RegionColor = Colors.Purple
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/B",
                    RegionColor = Colors.Red
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/C",
                    RegionColor = Colors.Orange
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/D",
                    RegionColor = Colors.Blue
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/E",
                    RegionColor = Colors.DarkCyan
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/F",
                    RegionColor = Colors.CornflowerBlue
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/G",
                    RegionColor = Colors.Chocolate
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/Z",
                    RegionColor = Colors.Yellow
                }
            );
        }
    }
}
