using CRMResourceSynchronization.Core.Dynamics;
using CRMResourceSynchronization.Extensions;
using CRMResourceSynchronization.Properties;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using static CRMResourceSynchronization.Core.Dynamics.CRMClient;

namespace CRMResourceSynchronization
{
    /// <summary>
    /// Interaction logic for LoginWindowControl.
    /// </summary>
    public partial class PathsWindowControl : UserControl
    {
        private string pathOfProyect = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="PathsWindowControl"/> class.
        /// </summary>
        public PathsWindowControl()
        {
            this.InitializeComponent();
            PathToResources();
        }       

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The paths of the saved resource types will be updated by those configured again, this operation cannot be undone. You're sure?", "Save paths", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SaveSettings();
                var window = Window.GetWindow(this);
                window.Close();
            }                
        }

        private void SaveSettings()
        {
            //Settings.Default.CRMUrl = CRMUrl.Text;
            if (!string.IsNullOrEmpty(PathHtml.Text))
                Settings.Default.PathHTML = PathHtml.Text;
            if (!string.IsNullOrEmpty(PathCSS.Text))
                Settings.Default.PathCSS = PathCSS.Text;
            if (!string.IsNullOrEmpty(PathJS.Text))
                Settings.Default.PathJS = PathJS.Text;
            if (!string.IsNullOrEmpty(PathXML.Text))
                Settings.Default.PathXML = PathXML.Text;
            if (!string.IsNullOrEmpty(PathPNG.Text))
                Settings.Default.PathPNG = PathPNG.Text;
            if (!string.IsNullOrEmpty(PathJPG.Text))
                Settings.Default.PathJPG = PathJPG.Text;
            if (!string.IsNullOrEmpty(PathGIF.Text))
                Settings.Default.PathGIF = PathGIF.Text;
            if (!string.IsNullOrEmpty(PathXAP.Text))
                Settings.Default.PathXAP = PathXAP.Text;
            if (!string.IsNullOrEmpty(PathXSL.Text))
                Settings.Default.PathXSL = PathXSL.Text;
            if (!string.IsNullOrEmpty(PathICO.Text))
                Settings.Default.PathICO = PathICO.Text;
            if (!string.IsNullOrEmpty(PathSVG.Text))
                Settings.Default.PathSVG = PathSVG.Text;
            if (!string.IsNullOrEmpty(PathRESX.Text))
                Settings.Default.PathRESX = PathRESX.Text;

            Settings.Default.Save();
            Settings.Default.Reload();
        }

        private void PathToResources()
        {
            PathHtml.Text = Settings.Default.PathHTML;
            PathCSS.Text = Settings.Default.PathCSS;
            PathJS.Text = Settings.Default.PathJS;
            PathXML.Text = Settings.Default.PathXML;
            PathPNG.Text = Settings.Default.PathPNG;
            PathJPG.Text = Settings.Default.PathJPG;
            PathGIF.Text = Settings.Default.PathGIF;
            PathXAP.Text = Settings.Default.PathXAP;
            PathXSL.Text = Settings.Default.PathXSL;
            PathICO.Text = Settings.Default.PathICO;
            PathSVG.Text = Settings.Default.PathSVG;
            PathRESX.Text = Settings.Default.PathRESX;

            INFOPATHTrue.Visibility = Visibility.Hidden;
            INFOPATHFalse.Visibility = Visibility.Hidden;
            pathOfProyect = "";

            ThreadHelper.ThrowIfNotOnUIThread();

            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelect, out selectionContainerPointer);

            if (hierarchyPointer != null && hierarchyPointer.ToInt32() > 0)
            {
                IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;

                if (selectedHierarchy == null)
                {
                    INFO.Text = "INFO - Cannot find an open solution in Visual Studio";
                }
                else
                {
                    selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject);
                    if (selectedObject == null)
                    {
                        INFO.Text = "INFO - You do not have any active projects in the solution";
                    }
                    else
                    {
                        EnvDTE.Project selectedProject = selectedObject as EnvDTE.Project;
                        if (selectedProject == null)
                        {
                            INFO.Text = "INFO - You do not have any active projects in the solution";
                        }
                        else
                        {

                            INFO.Text = "INFO - Set project path as initial path of resource types. Path of proyect : " + selectedProject.FullName;
                            pathOfProyect = selectedProject.FullName.Substring(0, selectedProject.FullName.IndexOf(selectedProject.FullName.Split('\\').Last()));
                            INFOPATHTrue.Visibility = Visibility.Visible;
                            INFOPATHFalse.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                INFO.Text = "INFO - Cannot find an open solution in Visual Studio";
            }
        }

        private void INFOPATH_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Name == "INFOPATHTrue" && rb.IsChecked != null && rb.IsChecked.Value)
            {
                PathHtml.Text = pathOfProyect;
                PathCSS.Text = pathOfProyect;
                PathJS.Text = pathOfProyect;
                PathXML.Text = pathOfProyect;
                PathPNG.Text = pathOfProyect;
                PathJPG.Text = pathOfProyect;
                PathGIF.Text = pathOfProyect;
                PathXAP.Text = pathOfProyect;
                PathXSL.Text = pathOfProyect;
                PathICO.Text = pathOfProyect;
                PathSVG.Text = pathOfProyect;
                PathRESX.Text = pathOfProyect;
            }
            else
            {
                PathToResources();
            }
        }
    }
}
