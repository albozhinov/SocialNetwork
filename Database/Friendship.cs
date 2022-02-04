using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public class Friendship
    {
        public int FromUserId { get; set; }

        public User FromUser { get; set; }

        public int ToUserId { get; set; }

        public User ToUser { get; set; }
    }
}
