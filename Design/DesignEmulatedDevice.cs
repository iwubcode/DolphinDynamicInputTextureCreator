using DolphinDynamicInputTexture.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DolphinDynamicInputTextureCreator.Design
{
    public class DesignEmulatedDevice : EmulatedDevice
    {
        public static DesignEmulatedDevice Instance => new DesignEmulatedDevice();

        public DesignEmulatedDevice()
        {
            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/A"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/B"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/C"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/D"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/E"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/F"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/G"
                }
            );

            EmulatedKeys.Add(
                new EmulatedKey
                {
                    Name = "Buttons/Z"
                }
            );
        }
    }
}
