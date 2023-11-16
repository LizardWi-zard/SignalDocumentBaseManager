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
            ByName = 1,
            ByType,
            ByNumber,
            ByRelease,
            ByEntry,
        }

        static public List<DocumentFile> Sort(List<DocumentFile> toSort, SortType type)
        {
            switch (type)   
            {
                case SortType.ByName:
                    return StringSort(toSort, type);

                case SortType.ByType:
                    return StringSort(toSort, type);

                case SortType.ByNumber:
                    return NumberSort(toSort);

                case SortType.ByRelease:
                    return DateSort(toSort, type);

                case SortType.ByEntry:
                    return DateSort(toSort, type);

                default:
                    return toSort;
            }
        }

        private static List<DocumentFile> StringSort(List<DocumentFile> toSort, SortType type)
        {
            int listCount = toSort.Count;
            bool needToChange = false;

            for (int lastSortedDocument = 0; lastSortedDocument < listCount - 1; lastSortedDocument++)
            {
                for (int currentDocument = 0; currentDocument < listCount - lastSortedDocument - 1; currentDocument++)
                {
                    int nextDocument = currentDocument + 1;

                    switch (type)
                    {
                        case SortType.ByName:
                            needToChange = (toSort[currentDocument].Name.CompareTo(toSort[nextDocument].Name) > 0);
                            break;
                        case SortType.ByType:
                            needToChange = (toSort[currentDocument].Type.CompareTo(toSort[nextDocument].Type) > 0);
                            break;
                    }

                    if (needToChange)
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

            for (int lastSortedDocument = 0; lastSortedDocument < listCount - 1; lastSortedDocument++)
            {
                for (int currentDocument = 0; currentDocument < listCount - lastSortedDocument - 1; currentDocument++)
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

        private static List<DocumentFile> NumberSort(List<DocumentFile> toSort)
        {
            var outputList = toSort.Where(x => !(x.Number.Contains('.') || x.Number.Contains('-')));

            outputList = outputList.OrderBy(x => Convert.ToInt32(x.Number));

            var numbersWithSymbols = toSort.Where(x => (x.Number.Contains('.') || x.Number.Contains('-')));

            numbersWithSymbols = numbersWithSymbols.OrderBy(x => x.Number.Length);   

            return outputList.Concat(numbersWithSymbols).ToList();
        }
    }
}
