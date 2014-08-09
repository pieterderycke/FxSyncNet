using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Models
{
    [DataContract]
    public class Bookmark
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public BookmarkType Type { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string ParentName { get; set; }
        [DataMember(Name="bmkUri")]
        public Uri Uri { get; set; }
        [DataMember]
        public IEnumerable<string> Tags { get; set; }
        [DataMember]
        public string Keyword { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool LoadInSidebar { get; set; }
        [DataMember]
        public IEnumerable<string> Children { get; set; }
        [DataMember]
        public string ParentId { get; set; }
    }
}
