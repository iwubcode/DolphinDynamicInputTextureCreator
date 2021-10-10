using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public static class Dialogs
    {

        public static bool ImageNotExistMessage(Interfaces.IImage image, string details)
        {
            MessageBoxResult MessageResult;
            MessageResult = MessageBox.Show(string.Format("'{0}'\nThe image '{1}' could not be found!\nSearch for the picture?", image.TexturePath, details), "Image could not be found!", MessageBoxButton.YesNo);
            if (MessageResult == MessageBoxResult.Yes)
            {
                return DialogForNewPath(image);
            }
            return false;
        }

        #region Dialogs

        #endregion

        public static bool DialogForNewPath(Interfaces.IImage image)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = System.IO.Path.GetFileName(image.TexturePath);
            dialog.DefaultExt = ".png";
            dialog.Filter = "Original Name |" + dialog.FileName;
            dialog.Filter += "|PNG Files (*.png)|*.png";
            if (dialog.ShowDialog() == true)
            {
                image.TexturePath = dialog.FileName;
                return true;
            }
            return false;
        }


    }
}
