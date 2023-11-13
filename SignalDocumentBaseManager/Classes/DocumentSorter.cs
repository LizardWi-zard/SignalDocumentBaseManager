using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SignalDocumentBaseManager.Classes
{
    static public class DocumentSorter
    {
        public enum SortType
        {
            ByRelease = 1,
            ByEntry,
            ByType
        }

        static public List<DocumentFile> Sort(List<DocumentFile> toSort, SortType type)
        {
            switch (type)   
            {
                case SortType.ByRelease:
                    return DateSort(toSort, type);
                case SortType.ByEntry:
                    return DateSort(toSort, type);
                case SortType.ByType:
                    return TypeSort(toSort);
                default:
                    return null;
            }

        }

        private static List<DocumentFile> TypeSort(List<DocumentFile> toSort)
        {
            int listCount = toSort.Count;
            for (int lastSortedDocumen = 0; lastSortedDocumen < listCount - 1; lastSortedDocumen++)// indexOfLastDocumentInSortedPart is index of last document, which is in sorted part of outputList
            {
                for (int currentDocument = 0; currentDocument < listCount - lastSortedDocumen - 1; currentDocument++)
                {
                    int nextDocument = currentDocument + 1;
                    if (toSort[currentDocument].Type.CompareTo(toSort[nextDocument].Type) > 0)
                    {
                        var docCopy = toSort[currentDocument];
                        toSort[currentDocument] = toSort[nextDocument];
                        toSort[nextDocument] = docCopy;
                    }
                }
            }

            return toSort;
        }

        private static List<DocumentFile> DateSort(List<DocumentFile> toSort, SortType type)
        {
            int listCount = toSort.Count;
            
            DateTime currentDate = DateTime.Now;
            DateTime nextDate = DateTime.Now;

            for (int lastSortedDocumen = 0; lastSortedDocumen < listCount - 1; lastSortedDocumen++)
            {
                for (int currentDocument = 0; currentDocument < listCount - lastSortedDocumen - 1; currentDocument++)
                {
                    int nextDocument = currentDocument + 1;

                    switch (type)
                    {
                        case SortType.ByRelease:
                            currentDate = DateTime.Parse(toSort[currentDocument].ReleaseDate);
                            nextDate = DateTime.Parse(toSort[nextDocument].ReleaseDate);
                            break;
                        case SortType.ByEntry:
                            currentDate = DateTime.Parse(toSort[currentDocument].EntryDate);
                            nextDate = DateTime.Parse(toSort[nextDocument].EntryDate);
                            break;
                    }

                    if (currentDate > nextDate)
                    {
                        var docCopy = toSort[currentDocument];
                        toSort[currentDocument] = toSort[nextDocument];
                        toSort[nextDocument] = docCopy;
                    }
                }
            }

            return toSort;
        }
    }
}
