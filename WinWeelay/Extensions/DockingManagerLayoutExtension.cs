﻿using System;
using System.IO;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;

namespace WinWeelay
{
    /// <summary>
    /// Extension for saving and loading the layout of the UI docks.
    /// </summary>
    public static class DockingManagerLayoutExtension
    {
        private static readonly string _layoutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "docklayout.xml");

        /// <summary>
        /// Save the current layout of the UI docks.
        /// </summary>
        /// <param name="dockingManager">The element which managers the UI docks.</param>
        public static void SaveLayout(this DockingManager dockingManager)
        {
            XmlLayoutSerializer layoutSerializer = new(dockingManager);
            using (StreamWriter writer = new(_layoutPath))
                layoutSerializer.Serialize(writer);
        }

        /// <summary>
        /// Load current layout of the UI docks from file.
        /// </summary>
        /// <param name="dockingManager">The element which managers the UI docks.</param>
        public static void RestoreLayout(this DockingManager dockingManager)
        {
            if (!File.Exists(_layoutPath))
                return;

            try
            {
                XmlLayoutSerializer layoutSerializer = new(dockingManager);
                layoutSerializer.LayoutSerializationCallback += LayoutSerializer_LayoutSerializationCallback;
                using (StreamReader reader = new(_layoutPath))
                    layoutSerializer.Deserialize(reader);
            }
            catch (InvalidOperationException)
            {
                // If the file is not a valid XML for some reason, assume it was corrupted and remove it.
                File.Delete(_layoutPath);
            }
        }

        private static void LayoutSerializer_LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            if (e.Model is LayoutDocument)
                e.Cancel = true;
        }
    }
}
