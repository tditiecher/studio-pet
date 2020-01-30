// Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

using System.ComponentModel.Composition;
using System.Diagnostics;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace StudioPet
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class ViewportAdornmentTextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        public SVsServiceProvider ServiceProvider { get; set; }

        [Export(typeof(AdornmentLayerDefinition))]
        [Name(ViewportAdornment.Name)]
        [Order(Before = PredefinedAdornmentLayers.Caret)]
        // ReSharper disable once UnassignedField.Local
        private AdornmentLayerDefinition _editorAdornmentLayer;

        private ViewportAdornment _adornment;

        // ReSharper disable once NotAccessedField.Local
        private BuildEvents _buildEvents;

        private bool? _buildSucceeded;


        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _adornment = new ViewportAdornment(textView);

            if (ServiceProvider.GetService(typeof(DTE)) is DTE dte)
            {
                _buildEvents = dte.Events.BuildEvents;
                dte.Events.BuildEvents.OnBuildBegin += BuildEvents_OnBuildBegin;
                dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
                dte.Events.BuildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
            }
        }

        private void BuildEvents_OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            Debug.WriteLine($"StudioPet: OnBuildBegin scope:{scope} action:{action}");
            
            _buildSucceeded = null;
        }

        private void BuildEvents_OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            Debug.WriteLine($"StudioPet: OnBuildDone scope:{scope} action:{action} buildSucceeded:{_buildSucceeded}.");

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_buildSucceeded.HasValue)
            {
                _adornment?.Shell.ExpressEmotion(_buildSucceeded.Value ? Emotion.Happy : Emotion.Sad);
            }
        }

        private void BuildEvents_OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            Debug.WriteLine($"StudioPet: OnBuildProjConfigDone project:{project} success:{success}");

            _buildSucceeded = success && (_buildSucceeded ?? true);
        }
    }
}

#pragma warning restore 649, 169
