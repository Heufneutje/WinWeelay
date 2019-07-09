using System;
using System.IO;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace WinWeelay
{
    public static class DockingManagerLayoutHelper
    {
        private static readonly string _layoutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "docklayout.xml");

        public static void SaveLayout(DockingManager dockingManager)
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            using (StreamWriter writer = new StreamWriter(_layoutPath))
                layoutSerializer.Serialize(writer);
        }

        public static void RestoreLayout(DockingManager dockingManager)
        {
            if (!File.Exists(_layoutPath))
                return;

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            layoutSerializer.LayoutSerializationCallback += LayoutSerializer_LayoutSerializationCallback;
            using (StreamReader reader = new StreamReader(_layoutPath))
                layoutSerializer.Deserialize(reader);
        }

        private static void LayoutSerializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (e.Model is LayoutDocument)
                e.Cancel = true;
        }
    }
}
