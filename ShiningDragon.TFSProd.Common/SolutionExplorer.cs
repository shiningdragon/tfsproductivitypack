using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE80;
using EnvDTE;
using System.Collections;

namespace ShiningDragon.TFSProd.Common
{
    public class SolutionExplorer
    {
        public SolutionExplorer(DTE2 _dte)
        {
            dte = _dte;
        }

        public bool IsSolutionOpen
        {
            get
            {
                return dte.ToolWindows.SolutionExplorer.UIHierarchyItems.Count > 0;
            }
        }

        public List<string> GetSelectedItems()
        {
            List<string> result = new List<string>();
            string localPath = string.Empty;
            if (dte.SelectedItems != null && dte.SelectedItems.Count > 0)
            {
                foreach (SelectedItem item in dte.SelectedItems)
                {
                    localPath = GetLocalPath(item);
                    if (!String.IsNullOrWhiteSpace(localPath))
                    {
                        result.Add(localPath);
                    }
                }
            }
            return result;
        }

        public void FindInSolutionExplorer(string localPath)
        {

        }

        /// <summary>
        /// Select the current active document in the solution explorer, taken from
        /// http://social.msdn.microsoft.com/Forums/vstudio/en-US/1f9d7cb2-cfbc-44b7-88e5-6faf564cdc74/uihierarchyitem-from-a-projectitem
        /// </summary>
        public void FindCurrentActiveDocumentInSolutionExplorer()
        {
            ProjectItem projectItem = dte.ActiveDocument.ProjectItem;
            UIHierarchyItems solutionItems = dte.ToolWindows.SolutionExplorer.UIHierarchyItems;

            // check if we have a solution 
            if (solutionItems.Count != 1)
                return;

            // FindHierarchyItem expands nodes as well (it must do so, because children arent loaded until expanded) 
            UIHierarchyItem uiItem = FindHierarchyItem(solutionItems.Item(1).UIHierarchyItems, projectItem);

            if (uiItem != null)
            {
                dte.Windows.Item(Constants.vsWindowKindSolutionExplorer).Activate();
                uiItem.Select(vsUISelectionType.vsUISelectionTypeSelect);
            }
        }

        private UIHierarchyItem FindHierarchyItem(UIHierarchyItems items, object item)
        {
            // Enumerating children recursive would work, but it may be slow on large solution. 
            // This tries to be smarter and faster 
           
            Stack stack = new Stack();
            CreateItemsStack(stack, item);

            UIHierarchyItem last = null;
            while (stack.Count != 0)
            {
                if (!items.Expanded)
                {
                    items.Expanded = true;
                }
                if (!items.Expanded)
                {
                    // bug: expand doesn't always work... 
                    UIHierarchyItem parent = ((UIHierarchyItem)items.Parent);
                    parent.Select(vsUISelectionType.vsUISelectionTypeSelect);
                    dte.ToolWindows.SolutionExplorer.DoDefaultAction();
                }

                object o = stack.Pop();

                last = null;
                foreach (UIHierarchyItem child in items)
                {
                    // There is a bug here - we cant match some custom project types e.g. wix
                    if (child.Object == o)
                    {
                        last = child;
                        items = child.UIHierarchyItems;
                        break;
                    }
                }
            }

            return last;
        }

        private void CreateItemsStack(Stack s, object item)
        {
            if (item is ProjectItem)
            {
                ProjectItem pi = (ProjectItem)item;
                s.Push(pi);
                CreateItemsStack(s, pi.Collection.Parent);
            }
            else if (item is Project)
            {
                Project p = (Project)item;
                s.Push(p);
                if (p.ParentProjectItem != null)
                {
                    // top nodes dont have solution as parent, but is null 
                    CreateItemsStack(s, p.ParentProjectItem);
                }
            }
            else if (item is Solution)
            {
                // do nothing
            }
            else
            {
                throw new ApplicationException("unknown item");
            }
        }

        private string GetLocalPath(SelectedItem item)
        {
            string result = string.Empty;

            if (item.ProjectItem == null)
            {
                if (item.Project == null)
                {
                    // If there's no ProjectItem and no Project then it's (probably?) the solution
                    result = dte.Solution.FullName;
                }
                else
                {
                    // If there's no ProjectItem but there is a Project then the Project node is selected
                    result = item.Project.FullName;
                }
            }
            else
            {
                // Just selected a file
                // Regular items in a project seem to be zero-based
                // Items inside of solution folders seem to be one-based...
                try
                {
                    result = item.ProjectItem.get_FileNames(0);
                }
                catch (ArgumentException)
                {
                    result = item.ProjectItem.get_FileNames(1);
                }
            }
            return result;
        }

        private DTE2 dte;

    }
}
