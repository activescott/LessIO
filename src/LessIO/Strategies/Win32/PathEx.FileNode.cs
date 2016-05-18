using System;
using System.Collections.Generic;
using System.Diagnostics;
using BadPath = System.IO.Path;

namespace LessIO.Strategies.Win32
{
    internal class FileNode
    {
        public FileNode(Path path)
        {
            this.Path = path;
            this.Children = new List<FileNode>();
        }

        public Path Path { get; private set; }
        public List<FileNode> Children { get; private set; }

        public bool IsDescendentPath(Path path)
        {
            if (path.IsEmpty)
                throw new ArgumentNullException("pathName");
            // HACK: This isn't safely looking at path separtors, but since all of our paths are from the same source it's probably safe (famous last words)
            return path.FullName.Length > this.Path.FullName.Length
                && path.FullName.StartsWith(this.Path.FullName, StringComparison.CurrentCultureIgnoreCase);
        }

        public FileNode FindChild(Path path)
        {
            return Children.Find(f => path.Equals(f));
        }

        public FileNode FindDescendent(Path path)
        {
            var ancestor = this.FindClosestAncestor(path);
            if (ancestor != null)
            {
                return ancestor.FindChild(path);
            }
            return null;
        }

        /// <summary>
        /// Returns the FileNode that is the closest ancestor of the specified childPathName.
        /// If the current node and no child nodes are ancestors of the specified path, then null is returned.
        /// </summary>
        /// <param name="childPath">The path for which an ancestor is sought.</param>
        public FileNode FindClosestAncestor(Path childPath)
        {
            return FindClosestAncestor(this, childPath);
        }

        /// <summary>
        /// Returns the FileNode that is the closest ancestor of the specified childPathName
        /// </summary>
        /// <param name="tree">The tree to search.</param>
        /// <param name="childPath">The path for which an ancestor is sought.</param>
        /// <returns>FileNode or null if the tree does not contain an ancestor.</returns>
        public static FileNode FindClosestAncestor(FileNode tree, Path childPath)
        {
            FileNode candidate = null;
            if (tree.IsDescendentPath(childPath))
            {
                candidate = tree;
                var ancestor = tree.Children.Find(f => f.IsDescendentPath(childPath));
                if (ancestor != null)
                {
                    candidate = ancestor;
                    // is there a closer ancestor?
                    var closer = FindClosestAncestor(ancestor, childPath);
                    if (closer != null)
                        candidate = closer;
                }
            }
            return candidate;
        }

        internal void Insert(FileNode newNode)
        {
            var parent = this.FindClosestAncestor(newNode.Path);
            Debug.Assert(parent.FindChild(newNode.Path) == null, "Node already exists!");
            Debug.Assert(!parent.Equals(newNode.Path), "adding node as a child of hisself!");
            parent.Children.Add(newNode);
        }

        /// <summary>
        /// Deletes the file or directory associated with this node and all children.
        /// </summary>
        public void RemoveFileOrDirectoryRecursively()
        {
            this.Children.ForEach(f => f.RemoveFileOrDirectoryRecursively());
            if (FileSystem.IsDirectory(Path))
                FileSystem.RemoveDirectory(this.Path, false);
            else
                FileSystem.RemoveFile(this.Path, true);
        }
    }
}
