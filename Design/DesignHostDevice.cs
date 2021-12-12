using DolphinDynamicInputTexture.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Design
{
    class DesignHostDevice : HostDevice
    {
        public static DesignHostDevice Instance => new DesignHostDevice();

        public DesignHostDevice()
        {
            HostKeys.Add(new HostKey
            {
                Name = "`Gamepad A`",
                TexturePath = "pack://application:,,,/Design/Images/a.png"
            });

            HostKeys.Add(new HostKey
            {
                Name = "`Gamepad B`",
                TexturePath = "pack://application:,,,/Design/Images/b.png"
            });

            HostKeys.Add(new HostKey
            {
                Name = "`Gamepad C`",
                TexturePath = "pack://application:,,,/Design/Images/c.png"
            });

            HostKeys.Add(new HostKey
            {
                Name = "`Gamepad D`",
                TexturePath = "pack://application:,,,/Design/Images/d.png"
            });
        }
    }
}
