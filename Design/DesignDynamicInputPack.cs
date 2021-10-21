using DolphinDynamicInputTexture.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Design
{
    public class DesignDynamicInputPack : DynamicInputPack
    {
        public static DesignDynamicInputPack Instance => new DesignDynamicInputPack();

        public DesignDynamicInputPack()
        {
            Textures.Add(new DynamicInputTexture
            {
                TextureHash = "tex1_320x448_a43c76abe05e0a80_6.png",
                TexturePath = "pack://application:,,,/Design/Images/tex1_1024x1024_abcdef_1.png"
            });

            Textures.Add(new DynamicInputTexture
            {
                TextureHash = "tex1_512x128_4bad6aed6f886849_14.png",
                TexturePath = "pack://application:,,,/Design/Images/tex1_1024x1024_abcdef_1.png"
            });

            Textures.Add(new DynamicInputTexture
            {
                TextureHash = "tex1_32x32_33b61a99f534262b_14.png",
                TexturePath = "pack://application:,,,/Design/Images/tex1_1024x1024_abcdef_1.png"
            });
        }
    }
}
