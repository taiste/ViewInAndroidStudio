using System;
using MonoDevelop.Ide;
using System.Linq;
using System.Collections.Generic;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Tasks;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using System.IO;

namespace ViewInAndroidStudio
{
    public class NamingConventionChecker: CommandHandler
    {
        protected override void Run ()
        {
            IdeApp.ProjectOperations.StartBuild += (sender, e) => CheckProjectNamingConventions ();

            IdeApp.Workspace.FileRenamedInProject += IdeApp_Workspace_FileRenamedInProject;

            IdeApp.Workspace.FileAddedToProject += (sender, e) => CheckProjectFileNamingConventions (e.Select (ea => ea.ProjectFile));

            IdeApp.Workspace.FileRemovedFromProject += (sender, e) => ClearOwnFileErrors (e.Select (ea => ea.ProjectFile.FilePath));

            IdeApp.Workspace.SolutionLoaded += (sender, e) => CheckProjectNamingConventions ();
        }

        void IdeApp_Workspace_FileRenamedInProject (object sender, ProjectFileRenamedEventArgs e)
        {
            
            foreach (var ea in e) {
               
                if (ea.OldName.FileName == ea.NewName.FileName) {
                    //a directory filename changed
                    var oldDir = ea.OldName.ParentDirectory;
                    var newDir = ea.NewName.ParentDirectory;

                    if (!newDir.ToString ().Contains ("Resources")) {
                        return;
                    }

                    while (oldDir.FileName == newDir.FileName) {

                        if (oldDir.ParentDirectory == null) {
                            System.Diagnostics.Debug.WriteLine ("should not happen");
                            return;
                        }
                        oldDir = oldDir.ParentDirectory;
                        newDir = newDir.ParentDirectory;
                    }
                    ClearOwnFileErrors (oldDir);
                    CheckDirectoryNamingConvention (newDir);
                } else {
                    ClearOwnFileErrors (ea.OldName);
                    CheckProjectFileNamingConventions (ea.ProjectFile);
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



        protected override void Update (CommandInfo info)
        {
            base.Update (info);
        }

        private void CheckProjectFileNamingConventions (IEnumerable<ProjectFile> files)
        {
            foreach (var projectFile in files) {
                CheckProjectFileNamingConventions (projectFile);
            }
        }

        private void CheckProjectFileNamingConventions (ProjectFile file)
        {
            ClearOwnFileErrors (file.FilePath);
            if (ProjectFileUtils.IsResource (file)) {
                if (file.FilePath.FileName.Split ('.').Last ().ToLowerInvariant () == "axml") {
                    TaskService.Errors.Add (new Task (file.FilePath, "Layout file has axml extension", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
                }
                if (file.FilePath.FileName.Any (Char.IsUpper)) {
                    TaskService.Errors.Add (new Task (file.FilePath, "Resource file has captial letter in filename", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
                }
            }
        }

        private void CheckDirectoryNamingConvention (FilePath fp)
        {
            ClearOwnFileErrors (fp);
            if (fp.FileName.Any (Char.IsUpper)) {
                TaskService.Errors.Add (new Task (fp, "Resource folder has captial letter in filename", 0, 0, TaskSeverity.Warning, TaskPriority.Normal, null, this));
            }
        }

        private  void CheckProjectNamingConventions ()
        {
            var projects = IdeApp.Workspace.GetAllProjects ();
            var fileLists = projects.Select (p => p.Files);
            var files = fileLists.Aggregate (new List<ProjectFile> (), (acc, li) => {
                acc.AddRange (li);
                return acc;
            });

            TaskService.Errors.ClearByOwner (this);

            foreach (var file in files) {
                CheckProjectFileNamingConventions (file);
            }

            var directories = projects.Select (p => p.BaseDirectory);
            foreach (var baseDirectory in directories) {
                var resDirPath = FilePath.Build (baseDirectory.FullPath, "Resources").FullPath;
                if (Directory.Exists (resDirPath)) {
                    foreach (var dir in Directory.EnumerateDirectories (resDirPath)) {
                        var fp = new FilePath (dir);
                        CheckDirectoryNamingConvention (fp);
                    }
                }

            } 
        }
    }
}

