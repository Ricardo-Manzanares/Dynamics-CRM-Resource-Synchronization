using DynamicsCRMResourceSynchronization.Properties;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DynamicsCRMResourceSynchronization.Extensions
{
    public static class Notification
    {
        public static void SendNotification(string heading, string text, string pathImageApp = null, string pathImageText = null)
        {
            try
            {
                if (Settings.Default.Notifications != "Off")
                {
                    ToastContentBuilder notification = new ToastContentBuilder();
                    notification.AddAttributionText("Dynamics CRM Resource Synchronization");
                    notification.AddAppLogoOverride(new Uri(ResourcePath.GetLocalPath("Dynamics-CRM-Resource-Synchronization_150x150.png")), ToastGenericAppLogoCrop.Default, "Dynamics CRM Resource Synchronization");

                    if (!string.IsNullOrEmpty(heading))
                        notification.AddText(heading);
                    if (!string.IsNullOrEmpty(text))
                        notification.AddText(text);

                    if (Settings.Default.Notifications == "On")
                    {
                        notification.AddAudio(new ToastAudio() { Silent = false });
                    }
                    else if (Settings.Default.Notifications == "Silent")
                    {
                        notification.AddAudio(new ToastAudio() { Silent = true });
                    }
                    notification.Show();
                    //notification.AddAttributionText("Via SMS");
                    //notification.AddCustomTimeStamp(new DateTime(2017, 04, 15, 19, 45, 00, DateTimeKind.Utc));
                    /*notification.AddVisualChild(new AdaptiveProgressBar()
                      {
                          Title = "Weekly playlist",
                          Value = 0.6,
                          ValueStringOverride = "15/26 songs",
                          Status = "Downloading..."
                      });*/
                    // Profile (app logo override) image
                    //if (!string.IsNullOrEmpty(pathImageApp))
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot send notification : '{0}'", ex.Message));
            }            
        }
    }
}
