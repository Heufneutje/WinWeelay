using System.IO;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace WinWeelay
{
    public static class DockingManagerLayoutHelper
    {
        public const string LayoutPath = "docklayout.xml";

        public static void SaveLayout(DockingManager dockingManager)
        {
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            using (StreamWriter writer = new StreamWriter(LayoutPath))
                layoutSerializer.Serialize(writer);
        }

        public static void RestoreLayout(DockingManager dockingManager)
        {
            if (!File.Exists(LayoutPath))
                return;

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            layoutSerializer.LayoutSerializationCallback += LayoutSerializer_LayoutSerializationCallback;
            using (StreamReader reader = new StreamReader(LayoutPath))
                layoutSerializer.Deserialize(reader);
        }

        private static void LayoutSerializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (e.Model is LayoutDocument)
                e.Cancel = true;
        }
    }
}
