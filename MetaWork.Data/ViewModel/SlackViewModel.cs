using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
   public class ConversationsListViewModel
    {
        public List<ChannelViewModel> channels { get; set; }
    }
    public class ChannelViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool is_channel     { get; set; }
        public bool is_group { get; set; }
        public bool is_im { get; set; }
        public int created { get; set; }
        public string creator { get; set; }
        public string name_normalized { get; set; }
        public int num_members { get; set; }
        public Guid NguoiTaoId { get; set; }
   
       
    }
    public class CreateChannelViewModel
    {
        public string name { get; set; }
    }
}
