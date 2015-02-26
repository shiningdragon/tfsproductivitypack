using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using ShiningDragon.TFSProd.TFS.Connection;
using ShiningDragon.TFSProd.TFS.Builds;
using ShiningDragon.TFSProd.TFS.VersionControl;
using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.Common.Commands;
using ShiningDragon.TFSProd.Common.VSCT;
using ShiningDragon.TFSProd.Commands.Builds;
using ShiningDragon.TFSProd.Commands.SourceControlEx;
using ShiningDragon.TFSProd.Commands.SolutionEx;
using ShiningDragon.TFSProd.Commands.CodeWindow;


namespace ShiningDragon.TFSProd.Package
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.CodeWindow_string)]
    [Guid(GuidList.guidTFSProductivityPackPkgString)]
    public sealed class TFSProductivityPackage : Microsoft.VisualStudio.Shell.Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public TFSProductivityPackage()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            InitializeDTE();

            LogLevel logLevel = LogLevel.Info;
            if (Debugger.IsAttached)
            {
                logLevel = LogLevel.Verbose;
            }
            logger = new OutputWindowLogger(dte, "TFS Productivity Pack", logLevel);
            try
            {
                logger.Log("TFSProductivityPackage Initialize", LogLevel.Info);
                commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

                ITFSConnection tfsConnection = new TFSConnection(dte, logger);
                ITFSVersionControl tfsVersionControl = new TFSVersionControl(tfsConnection, dte, logger);
                ITFSBuildService tfsBuildService = new TFSBuildService(tfsConnection, this, logger);
                
                //showDeletedFilesCmd = new ToggleShowDeletedItemsCommand(
                //    commandService,
                //    logger,
                //    tfsVersionControl);

                quickCompareCmd = new TFSQuickCompareCommand(
                    commandService,
                    logger,
                    tfsVersionControl);

                findInSCEFromSolExpCmd = new FindInSCEFromSolExpCommand(
                    commandService,
                    logger,
                    dte,
                    tfsVersionControl);

                findInSCEFromCodeWindowCmd = new FindInSCEFromCodeWindowCommand(
                    commandService,
                    logger,
                    dte,
                    tfsVersionControl);

                findInSolExpFromCodeWindowCmd = new FindInSolExpFromCodeWindowCommand(
                    commandService,
                    logger,
                    dte);

                compareToBranchCommand = new CompareToBranchCommand(
                    commandService,
                    logger,
                    tfsVersionControl);

                branchBuildDefinitionCmd = new BranchBuildDefinitionCommand(
                    commandService,
                    logger,
                    tfsBuildService,
                    tfsVersionControl);

                CommandID comapreToBranchMenuID = new CommandID(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.menuIdCompareToBranchMenu);
                OleMenuCommand menuItem = new OleMenuCommand(null, comapreToBranchMenuID);
                menuItem.BeforeQueryStatus += compareToBranchCommand.ParentMenuQueryStatus;
                commandService.AddCommand(menuItem);
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error initializing TFsProductivityPackage\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        private void queryStatusSceContexMenu(object sender, EventArgs e) 
        {

        }
        private ILogger logger;
        private OleMenuCommandService commandService;
        private ICommand quickCompareCmd;
        //private ICommand showDeletedFilesCmd;
        private ICommand findInSCEFromSolExpCmd;
        private ICommand findInSCEFromCodeWindowCmd;
        private ICommand findInSolExpFromCodeWindowCmd;
        private ICommand branchBuildDefinitionCmd;
        private CompareToBranchCommand compareToBranchCommand;

        #region DTE

        private EnvDTE80.DTE2 dte;
        private DteInitializer dteInitializer;

        /// <summary>
        /// http://www.mztools.com/articles/2013/MZ2013029.aspx 
        /// </summary>
        private void InitializeDTE()
        {
            IVsShell shellService;

            this.dte = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as EnvDTE80.DTE2;

            if (this.dte == null) // The IDE is not yet fully initialized
            {
                shellService = this.GetService(typeof(SVsShell)) as IVsShell;
                this.dteInitializer = new DteInitializer(shellService, this.InitializeDTE);
            }
            else
            {
                this.dteInitializer = null;
            }
        }
        internal class DteInitializer : IVsShellPropertyEvents
        {
            private IVsShell shellService;
            private uint cookie;
            private Action callback;

            internal DteInitializer(IVsShell shellService, Action callback)
            {
                int hr;

                this.shellService = shellService;
                this.callback = callback;

                // Set an event handler to detect when the IDE is fully initialized
                hr = this.shellService.AdviseShellPropertyChanges(this, out this.cookie);

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }

            int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
            {
                int hr;
                bool isZombie;

                if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
                {
                    isZombie = (bool)var;

                    if (!isZombie)
                    {
                        // Release the event handler to detect when the IDE is fully initialized
                        hr = this.shellService.UnadviseShellPropertyChanges(this.cookie);

                        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

                        this.cookie = 0;

                        this.callback();
                    }
                }
                return VSConstants.S_OK;
            }
        }

        #endregion
    }
}

