using System;
using System.Collections;
using System.Windows.Forms;
using ComponentPro.Net;
using ComponentPro.IO;

namespace SftpClient
{    
    /// <summary>
    /// Base comparer class for comparing two list view items.
    /// </summary>
    public abstract class ListViewItemComparerBase : IComparer
    {
        private readonly int _folderImageIndex;
        private readonly int _folderLinkImageIndex;
        protected SortOrder _sortOrder;

        /// <summary>
        /// Initializes a new instance of the ListViewItemComparerBase.
        /// </summary>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="folderImageIndex">The defined folder image index.</param>
        /// <param name="folderLinkImageIndex">The defined folder link image index.</param>
        protected ListViewItemComparerBase(SortOrder sortOrder, int folderImageIndex, int folderLinkImageIndex)
        {
            _sortOrder = sortOrder;

            _folderImageIndex = folderImageIndex;
            _folderLinkImageIndex = folderLinkImageIndex;
        }

        public int Compare(object xobject, object yobject)
        {
            ListViewItem x = (ListViewItem) xobject;
            ListViewItem y = (ListViewItem) yobject;

            // Compare file to file or folder to folder only.
            int xIsFolder = (x.ImageIndex == _folderImageIndex || x.ImageIndex == _folderLinkImageIndex) ? 1 : 0;
            int yIsFolder = (y.ImageIndex == _folderImageIndex || y.ImageIndex == _folderLinkImageIndex) ? 1 : 0;

            if (xIsFolder != yIsFolder)
                return _sortOrder == SortOrder.Ascending ? (yIsFolder - xIsFolder) : (xIsFolder - yIsFolder);
            return _sortOrder == SortOrder.Ascending ? OnCompare((AbstractFileInfo)x.Tag, (AbstractFileInfo)y.Tag, x.Text, y.Text) : OnCompare((AbstractFileInfo)y.Tag, (AbstractFileInfo)x.Tag, y.Text, x.Text);
        }

        public abstract int OnCompare(AbstractFileInfo x, AbstractFileInfo y, string xtext, string ytext);
    }

    /// <summary>
    /// Date time comparer.
    /// </summary>
    public class ListViewItemDateComparer : ListViewItemComparerBase
    {
        public ListViewItemDateComparer(SortOrder sortOrder, int folderImageIndex, int folderLinkImageIndex) : base(sortOrder, folderImageIndex, folderLinkImageIndex)
        {
        }

        public override int OnCompare(AbstractFileInfo x, AbstractFileInfo y, string xtext, string ytext)
        {
            return DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
        }
    }

    /// <summary>
    /// File permissions comparer.
    /// </summary>
    public class ListViewItemPermissionsComparer : ListViewItemComparerBase
    {
        public ListViewItemPermissionsComparer(SortOrder sortOrder, int folderImageIndex, int folderLinkImageIndex)
            : base(sortOrder, folderImageIndex, folderLinkImageIndex)
        {
        }

        public override int OnCompare(AbstractFileInfo x, AbstractFileInfo y, string xtext, string ytext)
        {
            return ((SftpFileInfo)x).Permissions - ((SftpFileInfo)y).Permissions;
        }
    }

    /// <summary>
    /// Name comparer.
    /// </summary>
    public class ListViewItemNameComparer : ListViewItemComparerBase
    {
        public ListViewItemNameComparer(SortOrder sortOrder, int folderImageIndex, int folderLinkImageIndex)
            : base(sortOrder, folderImageIndex, folderLinkImageIndex)
        {
        }

        public override int OnCompare(AbstractFileInfo x, AbstractFileInfo y, string xtext, string ytext)
        {
            return string.Compare(xtext, ytext, true);
        }
    }

    /// <summary>
    /// Size comparer.
    /// </summary>
    public class ListViewItemSizeComparer : ListViewItemComparerBase
    {
        public ListViewItemSizeComparer(SortOrder sortOrder, int folderImageIndex, int folderLinkImageIndex)
            : base(sortOrder, folderImageIndex, folderLinkImageIndex)
        {
        }

        public override int OnCompare(AbstractFileInfo x, AbstractFileInfo y, string xtext, string ytext)
        {
            if (x.Length == y.Length)
                return 0;
            return x.Length > y.Length ? 1 : -1;
        }
    }
}
