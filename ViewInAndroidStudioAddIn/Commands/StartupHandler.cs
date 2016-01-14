using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Projects;
using Taiste.ViewInAndroidStudio.Util;

namespace Taiste.ViewInAndroidStudio.Commands
{
    public class StartupHandler: CommandHandler
    {
        protected override void Run ()
        {
            IdeApp.ProjectOperations.StartBuild += (sender, e) => CheckProjectNamingConventions ();

            IdeApp.Workspace.FileRenamedInProject += IdeApp_Workspace_FileRenamedInProject;

            IdeApp.Workspace.FileAddedToProject += (sender, e) => CheckProjectFileName (e.Select (ea => ea.ProjectFile));

            IdeApp.Workspace.FileRemovedFromProject += (sender, e) => ClearOwnFileErrors (e.Select (ea => ea.ProjectFile.FilePath));

            IdeApp.ProjectOperations.CurrentProjectChanged += (sender, e) => CheckProjectNamingConventions ();


        }

        void IdeApp_Workspace_FileRenamedInProject (object sender, ProjectFileRenamedEventArgs e)
        {
            
            foreach (var file in e) {
               
                if (file.OldName.FileName == file.NewName.FileName) {
                    //a directory filename changed
                    var oldDir = file.OldName.ParentDirectory;
                    var newDir = file.NewName.ParentDirectory;

                    if (!newDir.ToString ().Contains ("Resources")) {
                        return;
                    }

                    while (oldDir.FileName == newDir.FileName) {
                        oldDir = oldDir.ParentDirectory;
                        newDir = newDir.ParentDirectory;
                    }

                    ClearOwnFileErrors (oldDir);
                    CheckDirectoryName (newDir);
                } else {
                    ClearOwnFileErrors (file.OldName);
                    CheckProjectFileName (file.ProjectFile);
                }
            }
        }

        private void ClearOwnFileErrors (IEnumerable<FilePath> paths)
        {
            foreach (var path in paths) {
                ClearOwnFileErrors (path);
            }
        }


        private void ClearOwnFileErrors (FilePath path)
        {
            var errorsToClear = TaskService.Errors.Where (e => e.Owner == this && e.FileName == path).ToList ();
            foreach (var error in errorsToClear) {
                TaskService.Errors.Remove (error);
            }
        }

        private void CheckProjectFileName (IEnumerable<ProjectFile> files)
        {
            foreach (var projectFile in files) {
                CheckProjectFileName (projectFile);
            }
        }

        private void CheckProjectFileName (ProjectFile file)
        {
            ClearOwnFileErrors (file.FilePath);
            if (file.IsResource ()) {
                if (file.FilePath.FileName.Split ('.').Last ().ToLowerInvariant () == "axml") {
                    TaskService.Errors.Add (new Task (file.FilePath, "Layout file has axml extension", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
                }
                if (file.FilePath.FileName.Any (Char.IsUpper)) {
                    TaskService.Errors.Add (new Task (file.FilePath, "Resource file has captial letter in filename", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
                }
            }
        }

        private void CheckDirectoryName (FilePath fp)
        {
            ClearOwnFileErrors (fp);
            if (fp.FileName.Any (Char.IsUpper)) {
                TaskService.Errors.Add (new Task (fp, "Resource folder has capital letter in filename", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
            }
        }

        private void CheckProjectNamingConventions ()
        {
            IEnumerable<Project> projects = IdeApp.Workspace.GetAllProjects ().ToList ();

            projects = projects.Where (ProjectExtensions.IsAndroidProject);

            var files = 
                projects
                .Select (p => p.Files)
                .Aggregate (new List<ProjectFile> (), 
                    (acc, li) => {
                        acc.AddRange (li);
                        return acc;
                    }
                );

            TaskService.Errors.ClearByOwner (this);

            foreach (var file in files) {
                CheckProjectFileName (file);
            }

            var directories = projects.Select (p => p.BaseDirectory);
            foreach (var baseDirectory in directories) {
                var resDirPath = baseDirectory.Combine ("Resources");
                if (Directory.Exists (resDirPath)) {
                    foreach (var dir in Directory.EnumerateDirectories (resDirPath)) {
                        CheckDirectoryName (new FilePath (dir));
                    }
                }
            } 
        }
    }
}

