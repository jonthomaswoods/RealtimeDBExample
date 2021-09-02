using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RealtimeDBExample.Models
{
    /// <summary>
    /// A Firebase model for saving a Description and the current Date
    /// </summary>
    public class ItemsModel
    {
        public string Description { get; set; }
        public string Date { get; set; }
        [JsonIgnore]
        public string Key { get; set; }
    }
}
