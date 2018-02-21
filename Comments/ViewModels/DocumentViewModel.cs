using Comments.Models;
using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel(string title, IEnumerable<Location> locations)
        {
            Title = title;
            Locations = locations;
        }

        public string Title { get; private set; }

        public IEnumerable<Location> Locations{ get; private set; }
    }
}
