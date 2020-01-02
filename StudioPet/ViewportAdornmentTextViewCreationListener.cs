// Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169

using System.ComponentModel.Composition;
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

        public void TextViewCreated(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _adornment = new ViewportAdornment(textView);

            if (ServiceProvider.GetService(typeof(DTE)) is DTE dte)
            {
                _buildEvents = dte.Events.BuildEvents;
                dte.Events.BuildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
            }
        }

        private void BuildEvents_OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            _adornment?.Shell.ExpressEmotion(success ? Emotion.Happy : Emotion.Sad);
        }
    }
}

#pragma warning restore 649, 169
