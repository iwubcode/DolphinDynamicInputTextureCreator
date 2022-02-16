using DolphinDynamicInputTexture.Data;
using DolphinDynamicInputTexture.Interfaces;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace DolphinDynamicInputTextureCreator.ViewModels
{
    public static class Dialogs
    {
        #region MessageDialogs

        public static bool ImageNotExistMessage(IImage image, string details)
        {
            MessageBoxResult MessageResult;
            MessageResult = MessageBox.Show(string.Format("'{0}'\nThe image '{1}' could not be found!\nSearch for the picture?", image.TexturePath, details), "Image could not be found!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (MessageResult == MessageBoxResult.Yes)
            {
                return DialogForNewPath(image);
            }
            return false;
        }

        #endregion

        #region FileDialogs

        public static bool DialogForNewPath(IImage image)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = Path.GetFileName(image.TexturePath);
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

        public static void DialogAddTexture(in DynamicInputPackViewModel pack)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG Texture Files (tex1*.png)|tex1_*x*_*.png|PNG Files (*.png)|*.png";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                foreach (var filename in dialog.FileNames)
                {
                    pack.Textures.Add(new DynamicInputTexture
                    {
                        TextureHash = Path.GetFileName(filename),
                        TexturePath = filename
                    });
                }
            }
        }

        public static bool DialogSaveDIT(in DynamicInputPackViewModel pack, ref string savepath)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "DIT Files (*.dit)|*.dit|JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                string output = JsonConvert.SerializeObject(pack, Formatting.Indented);
                File.WriteAllText(dialog.FileName, output);
                savepath = dialog.FileName;
                return true;
            }
            return false;
        }

        public static DynamicInputPackViewModel DialogOpenDIT(ref string savepath)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DIT Files (*.dit)|*.dit|JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                string input = File.ReadAllText(dialog.FileName);
                var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
                try
                {
                    savepath = dialog.FileName;
                    return JsonConvert.DeserializeObject<DynamicInputPackViewModel>(input, settings);
                }
                catch (JsonReaderException JRE)
                {
                    MessageBox.Show(String.Format("JSON parse error reading file '{0}' was found on line '{1}'", dialog.FileName, JRE.LineNumber), "Open Error!");
                }
            }
            return null;
        }


        public static DynamicInputPackViewModel DialogImportFromLocation() => DialogImportFromLocation(new DynamicInputPackViewModel());
        public static DynamicInputPackViewModel DialogImportFromLocation(DynamicInputPackViewModel pack)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON Files (*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    pack.ImportFromLocation(dialog.FileName);
                    return pack;
                }
                catch (JsonReaderException JRE)
                {
                    MessageBox.Show(String.Format("JSON parse error reading file '{0}' was found on line '{1}'", dialog.FileName, JRE.LineNumber), "Import Error!");
                }
            }
            return null;
        }

        public static bool DialogExportToLocation(in DynamicInputPackViewModel pack)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pack.ExportToLocation(dialog.SelectedPath);
                // Updating the user interface in case the image file has changed.
                pack.Textures.Select(pack.Textures.Selected);
                return true;
            }
            return false;
        }

        #endregion

    }
}
