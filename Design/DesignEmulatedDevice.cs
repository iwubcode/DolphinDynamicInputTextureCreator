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
                    Name = "Buttons/A"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/B"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/C"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/D"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/E"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/F"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/G"
                }
            );

            EmulatedKeys.Add(
                new Data.EmulatedKey
                {
                    Name = "Buttons/Z"
                }
            );
        }
    }
}
