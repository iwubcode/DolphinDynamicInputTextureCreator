using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Design
{
    class DesignHostDevice : Data.HostDevice
    {
        public static DesignHostDevice Instance => new DesignHostDevice();

        public DesignHostDevice()
        {
            HostKeys.Add(new Data.HostKey
            {
                Name = "`Gamepad A`",
                TexturePath = "pack://application:,,,/Design/Images/a.png"
            });

            HostKeys.Add(new Data.HostKey
            {
                Name = "`Gamepad B`",
                TexturePath = "pack://application:,,,/Design/Images/b.png"
            });

            HostKeys.Add(new Data.HostKey
            {
                Name = "`Gamepad C`",
                TexturePath = "pack://application:,,,/Design/Images/c.png"
            });

            HostKeys.Add(new Data.HostKey
            {
                Name = "`Gamepad D`",
                TexturePath = "pack://application:,,,/Design/Images/d.png"
            });
        }
    }
}
